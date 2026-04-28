using UnityEngine;
using UnityEngine.UI;

public class UIMouseShake : MonoBehaviour
{
    [SerializeField] private float shakeIntensity = 5f; // 晃动强度
    [SerializeField] private float smoothTime = 0.1f; // 平滑时间
    private RectTransform targetRect; // 目标UI的RectTransform
    [SerializeField] private bool randomShake = false;

    private Vector2 currentVelocity;
    private Vector2 originalPosition;

    private void Start()
    {
        if (targetRect == null)
            targetRect = GetComponent<RectTransform>();
        if (randomShake)
        {
            shakeIntensity = Random.Range(3f, 5f);
            smoothTime = Random.Range(0.1f, 0.3f);
        }
        originalPosition = targetRect.anchoredPosition;
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 normalizedMousePos = new Vector2(
            (mousePos.x / Screen.width) * 2 - 1,
            (mousePos.y / Screen.height) * 2 - 1
        );
        Vector2 shakeOffset = new Vector2(
            normalizedMousePos.x * shakeIntensity,
            normalizedMousePos.y * shakeIntensity
        );
        Vector2 targetPosition = originalPosition + shakeOffset;
        targetRect.anchoredPosition = Vector2.SmoothDamp(
            targetRect.anchoredPosition,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }
}