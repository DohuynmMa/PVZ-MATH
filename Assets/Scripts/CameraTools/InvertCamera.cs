using UnityEngine;

[RequireComponent(typeof(Camera))]
public class InvertCamera : MonoBehaviour
{
    public Shader invertShader;
    private Material invertMaterial;

    [Header("反相设置")]
    [Tooltip("是否水平翻转画面")]
    public bool flipHorizontal = false;

    [Tooltip("是否垂直翻转画面")]
    public bool flipVertical = false;

    [Tooltip("是否反相颜色")]
    public bool invertColors = true;

    void Start()
    {
        // 检查着色器是否可用
        if (invertShader == null || !invertShader.isSupported)
        {
            Debug.LogError("着色器不可用或未指定！");
            enabled = false;
        }
        else
        {
            invertMaterial = new Material(invertShader);
            invertMaterial.hideFlags = HideFlags.DontSave;
        }
    }

    void Update()
    {
        // 如果材质存在，更新参数
        if (invertMaterial != null)
        {
            invertMaterial.SetFloat("_FlipX", flipHorizontal ? 1 : 0);
            invertMaterial.SetFloat("_FlipY", flipVertical ? 1 : 0);
            invertMaterial.SetFloat("_InvertColors", invertColors ? 1 : 0);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (invertMaterial != null)
        {
            Graphics.Blit(source, destination, invertMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void OnDestroy()
    {
        // 清理材质
        if (invertMaterial != null)
        {
            Destroy(invertMaterial);
        }
    }
}
