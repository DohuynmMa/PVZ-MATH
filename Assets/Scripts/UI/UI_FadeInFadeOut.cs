using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI_FadeInFadeOut : MonoBehaviour
{
    private float UI_Alpha = 1;
    public float alphaSpeed = 2f;
    public Action onFadeInOrFadeOut;
    private CanvasGroup canvasGroup { get => GetComponent<CanvasGroup>(); }

    void Update()
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (UI_Alpha != canvasGroup.alpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, UI_Alpha, alphaSpeed * Time.deltaTime);
            if (Mathf.Abs(UI_Alpha - canvasGroup.alpha) <= 0.01f)
            {
                if(onFadeInOrFadeOut != null)
                {
                    var a = onFadeInOrFadeOut;
                    onFadeInOrFadeOut = null;
                    a?.Invoke();
                    canvasGroup.alpha = UI_Alpha;
                }
            }
        }
    }

    public void UI_FadeIn_Event()
    {
        if (canvasGroup == null)
        {
            print("null canvasgroup" + gameObject.name);
            return;
        }
        UI_Alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void UI_FadeOut_Event()
    {
        if (canvasGroup == null)
        {
            print("null canvasgroup" + gameObject.name);
            return;
        }
        UI_Alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public float gertUI_Alpha()
    {
        return UI_Alpha;
    }


}