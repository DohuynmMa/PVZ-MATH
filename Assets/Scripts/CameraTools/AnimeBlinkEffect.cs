using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ComicFlashEffect : MonoBehaviour
{
    [Tooltip("勾选开启效果，方便调试")]
    public bool enableEffect = false;

    [Range(0, 1)] public float intensity = 1f;
    [Range(0.1f, 0.9f)] public float blackWhiteThreshold = 0.5f;
    [Range(0, 0.01f)] public float lineThickness = 0.005f;

    // 手动指定shader，在Inspector中赋值
    public Shader comicFlashShader;

    private Material effectMat;
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int ThresholdId = Shader.PropertyToID("_Threshold");
    private static readonly int LineThicknessId = Shader.PropertyToID("_LineThickness");

    void OnEnable()
    {
        // 尝试查找shader
        if (comicFlashShader == null)
        {
            comicFlashShader = Shader.Find("Hidden/ComicFlash");
        }

        // 如果找到shader则创建材质
        if (comicFlashShader != null && comicFlashShader.isSupported)
        {
            effectMat = new Material(comicFlashShader);
        }
        else
        {
            Debug.LogError("找不到ComicFlash shader，请确保shader已正确导入项目");
            enabled = false; // 禁用组件
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!enableEffect || effectMat == null || !comicFlashShader.isSupported)
        {
            Graphics.Blit(source, destination);
            return;
        }

        effectMat.SetTexture(MainTexId, source);
        effectMat.SetFloat(IntensityId, intensity);
        effectMat.SetFloat(ThresholdId, blackWhiteThreshold);
        effectMat.SetFloat(LineThicknessId, lineThickness);

        Graphics.Blit(source, destination, effectMat);
    }

    void OnDisable()
    {
        if (effectMat != null)
        {
            DestroyImmediate(effectMat);
        }
    }
}
