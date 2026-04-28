using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCamera : MonoBehaviour
{
    public Shader flipShader;
    private Material flipMaterial;
    public bool flipHorizontal = false;
    public bool flipVertical = false;

    void Start()
    {
        if (flipShader != null)
        {
            flipMaterial = new Material(flipShader);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (flipMaterial != null)
        {
            flipMaterial.SetInt("_FlipX", flipHorizontal ? 1 : 0);
            flipMaterial.SetInt("_FlipY", flipVertical ? 1 : 0);

            Graphics.Blit(source, destination, flipMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
