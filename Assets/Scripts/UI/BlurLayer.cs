using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurLayer : MonoBehaviour
{
    [Range(0, 20)]
    public float blurSize = 2.0f;

    [Range(1, 4)]
    public int blurIterations = 3;

    [Range(0, 1)]
    public float frostedGlassIntensity = 0.5f;

    public Shader blurShader;
    private Material blurMaterial;
    public UI_FadeInFadeOut blurBlackFade;

    public bool isBlur = false;
    public float blurSpeed = 4;
    private float terationTimer;
    void OnEnable()
    {
        if (blurShader == null)
            blurShader = Shader.Find("Unlit/GaussianBlur");

        blurMaterial = new Material(blurShader);
        blurMaterial.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        float widthMod = 1.0f / (1.0f * (1 << blurIterations));
        blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0, 0));
        blurMaterial.SetFloat("_FrostedGlassIntensity", frostedGlassIntensity);

        RenderTexture buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
        RenderTexture buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);
        Graphics.Blit(source, buffer, blurMaterial, 0);
        Graphics.Blit(buffer, buffer2, blurMaterial, 1);
        for (int i = 1; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            blurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0, 0));

            Graphics.Blit(buffer2, buffer, blurMaterial, 0);
            Graphics.Blit(buffer, buffer2, blurMaterial, 1);
        }
        blurMaterial.SetTexture("_BlurTex", buffer2);
        Graphics.Blit(source, destination, blurMaterial, 2);
        RenderTexture.ReleaseTemporary(buffer);
        RenderTexture.ReleaseTemporary(buffer2);
    }
    private void Update()
    {
        if (isBlur)
        {
            terationTimer += Time.deltaTime;
            blurSize += Time.deltaTime * blurSpeed;
            frostedGlassIntensity += Time.deltaTime * blurSpeed;
            if(terationTimer >= 1 / blurSpeed)
            {
                terationTimer = 0;
                blurIterations += 1;
            }
        }
        else
        {
            terationTimer += Time.deltaTime;
            blurSize -= Time.deltaTime * blurSpeed;
            frostedGlassIntensity -= Time.deltaTime * blurSpeed;
            if (terationTimer >= 1 / blurSpeed)
            {
                terationTimer = 0;
                blurIterations -= 1;
            }
        }
        fixValue();
    }
    private void fixValue()
    {
        if (blurSize < 0) blurSize = 0;
        else if(blurSize > 20) blurSize = 20;
        if(blurIterations < 1) blurIterations = 1;
        else if(blurIterations > 4) blurIterations = 4;
        if (frostedGlassIntensity < 0) frostedGlassIntensity = 0;
        else if (frostedGlassIntensity > 1) frostedGlassIntensity = 1;
    }
    public void blur()
    {
        isBlur = true;
        blurBlackFade.UI_FadeIn_Event();
    }
    public void noBlur()
    {
        isBlur = false;
        blurBlackFade.UI_FadeOut_Event();
    }
}