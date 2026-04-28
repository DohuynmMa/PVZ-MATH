using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonResize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float intensity = 1.15f;
    public bool playSounds = true;
    private void Start()
    {
        originalScale = GetComponent<RectTransform>().localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(playSounds)Sounds.selected.playWithPitch();
        GetComponent<RectTransform>().DOScale(originalScale * intensity, 0.3f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RectTransform>().DOScale(originalScale, 0.3f);
    }
}