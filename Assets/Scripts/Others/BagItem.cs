using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagItem : MonoBehaviour, IPointerClickHandler
{
    public int itemTemp = 0;
    public string itemName = "";
    [TextArea]
    public string info = "";
    public void OnPointerClick(PointerEventData eventData)
    {
        var sm = ShortKeyManager.Instance;
        var data = DataManager.Instance.data;
        // 检测右键点击 查看说明
        if (eventData.button == PointerEventData.InputButton.Left && !GameManager.Instance.inGame)
        {
            GlobalUIManager.Instance.loadItemInfo(this);
        }
        // 检测右键点击 设置快捷键
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (sm.isChangingKey) return;
            sm.startToSetShortKey(itemTemp + data.cardShortKey.Count);
        }
    }
}
