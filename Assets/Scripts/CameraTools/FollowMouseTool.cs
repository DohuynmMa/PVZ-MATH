using Assets.Scripts.Utils;
using UnityEngine;

public class FollowMouseTool : MonoBehaviour
{
    public bool enableTool = false;

    public bool reverseHorizontal = false;
    public bool reverseVertical = true;
    public Vector3 posLD;
    public Vector3 posRU;

    // 正交模式
    public float minOrthoSize = 4;
    public float maxOrthoSize = 12;
    public float orthoZoomSpeed = 25f;

    // 透视模式
    public float minPerspDistance = 10;   // 最近距离（缩放最大）
    public float maxPerspDistance = 20;  // 最远距离（缩放最小，看到全场）

    public float perspZoomSpeed = 10f;   // 透视速度（实测与250正交速度匹配）

    public float fovChangeSpeed = 10f;   // 降低平滑速度，避免突破限制
    public float cameraMoveSpeed = 1f;
    public float boundarySmoothing = 5f;
    public float boundaryBuffer = 0.1f;

    private bool isFollowing = false;
    private Vector3 oldMousePos;
    private Vector3 targetPosition;
    private bool isFixingPosition = false;
    [SerializeField] private Vector3 cposLD;
    [SerializeField] private Vector3 cposRU;
    private float mv;

    private void Start()
    {
        targetPosition = Camera.main.transform.position;
        oldMousePos = Input.mousePosition;
    }

    private void Update()
    {
        if (!GameManager.Instance.inGame || GameManager.Instance.inPause || !enableTool) return;
        // 缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            var camera = Camera.main;
            if (!camera.orthographic)
            {
                // 透视模式：直接基于距离计算，确保限制生效
                float currentDistance = -camera.transform.localPosition.z;
                float newDistance = currentDistance - scroll * perspZoomSpeed * Time.deltaTime * 60;
                newDistance = Mathf.Clamp(newDistance, minPerspDistance, maxPerspDistance);
                camera.transform.localPosition = new Vector3(
                    camera.transform.localPosition.x,
                    camera.transform.localPosition.y,
                    -newDistance
                );
            }
            else
            {
                // 正交模式
                var newSize = camera.orthographicSize - scroll * orthoZoomSpeed * Time.deltaTime * 60;
                newSize = Mathf.Clamp(newSize, minOrthoSize, maxOrthoSize);
                camera.orthographicSize = newSize;
            }
        }

        // 移动逻辑
        if (Input.GetMouseButtonDown(1))
        {
            oldMousePos = Input.mousePosition;
            mv = 0.05f;
        }

        if (Input.GetMouseButton(1))
        {
            isFollowing = true;
            isFixingPosition = false;

            Camera camera = Camera.main;
            Vector3 mouseDelta = Input.mousePosition - oldMousePos;

            float hDirection = reverseHorizontal ? -1f : 1f;
            float vDirection = reverseVertical ? -1f : 1f;

            float sizeFactor = camera.orthographic ?
                (5f / camera.orthographicSize) :
                (5f / (-camera.transform.localPosition.z));
            float moveSpeed = cameraMoveSpeed * Time.deltaTime * mv * sizeFactor;
            Vector3 moveDirection = hDirection * -camera.transform.right * mouseDelta.x +
                                   vDirection * camera.transform.up * mouseDelta.y;
            moveDirection *= moveSpeed;

            camera.transform.Translate(moveDirection, Space.World);
            camera.transform.localPosition = new Vector3(
                camera.transform.localPosition.x,
                camera.transform.localPosition.y,
                camera.transform.localPosition.z
            );

            oldMousePos = Input.mousePosition;
            mv += mv >= 0.4f ? 0 : Time.deltaTime * 0.2f;
        }
        // 边界检查
        else
        {
            isFollowing = false;
            mv = 0.05f;

            if (!isFixingPosition)
            {
                targetPosition = GetFixedPosition();
                isFixingPosition = true;
            }

            Camera camera = Camera.main;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                new Vector3(targetPosition.x, targetPosition.y, camera.transform.position.z),
                Time.deltaTime * boundarySmoothing
            );

            if (Vector3.Distance(camera.transform.position, targetPosition) < 0.01f)
            {
                isFixingPosition = false;
            }
        }
    }


    private Vector3 GetFixedPosition()
    {
        Camera camera = Camera.main;
        Vector3 pos = camera.transform.localPosition;
        Vector3 fixedPos = pos;

        if (camera.orthographic)
        {
            cposLD = posLD * (10f / camera.orthographicSize);
            cposRU = posRU * (10f / camera.orthographicSize);
        }
        else
        {
            float distance = -camera.transform.localPosition.z;
            float verticalFOV = camera.fieldOfView * Mathf.Deg2Rad;
            float verticalScale = 2 * Mathf.Tan(verticalFOV / 2) * distance;
            float horizontalScale = verticalScale * camera.aspect;

            cposLD = new Vector3(posLD.x * (horizontalScale / 10f), posLD.y * (verticalScale / 10f), posLD.z);
            cposRU = new Vector3(posRU.x * (horizontalScale / 10f), posRU.y * (verticalScale / 10f), posRU.z);
        }

        float minX = cposLD.x + boundaryBuffer;
        float maxX = cposRU.x - boundaryBuffer;
        float minY = cposLD.y + boundaryBuffer;
        float maxY = cposRU.y - boundaryBuffer;

        if (pos.x < minX) fixedPos.x = minX;
        else if (pos.x > maxX) fixedPos.x = maxX;

        if (pos.y < minY) fixedPos.y = minY;
        else if (pos.y > maxY) fixedPos.y = maxY;

        fixedPos.z = pos.z;

        return fixedPos;
    }
}