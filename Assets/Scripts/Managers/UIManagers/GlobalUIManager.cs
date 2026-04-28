using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public UI_FadeInFadeOut 黑色转场;

    public TextMeshProUGUI shortKeyTEXTPrefab;
    public List<TextMeshProUGUI> summonedOtherShortKeyText;
    public List<BagItem> items;

    public UI_FadeInFadeOut bagUI;
    public UI_FadeInFadeOut bagUIMain;
    public TextMeshProUGUI bagInfoTop;

    public UI_FadeInFadeOut bagUIInfo;
    public TextMeshProUGUI infoU;
    public TextMeshProUGUI infoD;

    public Card currentCard;
    public BagItem currentItem;
    public bool bagIsOpened = false;
    public void openBag()
    {
        bagIsOpened = true;
        bagUI.gameObject.SetActive(true);
        bagUI.UI_FadeIn_Event();
        bagUIMain.UI_FadeIn_Event();
        bagUIInfo.UI_FadeOut_Event();
        if (GameManager.Instance.inGame)
        {
            bagInfoTop.text = "<- [左键]使用卡牌 / [右键]设置快捷键 ->";
        }
        else bagInfoTop.text = "<- [左键]查看信息 / [右键]设置快捷键 ->";
    }
    public void closeBag()
    {
        bagIsOpened = false;
        bagUI.UI_FadeOut_Event();
        var sm = ShortKeyManager.Instance;
        if (sm.isChangingKey && sm.changingKeyTemp != -1)
        {
            sm.changeShortKey(sm.changingKeyTemp, KeyCode.None);
        }
    }
    public void closeInfo()
    {
        bagUIMain.UI_FadeIn_Event();
        bagUIInfo.UI_FadeOut_Event();
    }
    public void loadCardInfo(CardType type)
    {
        Destroy(currentCard?.gameObject);
        currentCard = null;
        Destroy(currentItem?.gameObject);
        currentItem = null;

        var card = Utils.findCardPrefabByType(type);
        if (card == null) return;
        bagUIMain.UI_FadeOut_Event();
        bagUIInfo.UI_FadeIn_Event();
        currentCard = Instantiate(card, Vector3.zero, Quaternion.identity, bagUIInfo.transform);
        currentCard.GetComponent<RectTransform>().localPosition = new Vector3(-300, 130, 0);
        currentCard.GetComponent<RectTransform>().localScale = Vector3.one * 2;
        currentCard.enabled = false;
        var hitPoint = Utils.findEntityPrefabByType(card.entityType).hitpoint;
        infoU.text = $"名称：{card.cardName}\n消耗BP：{card.costBrainPoint}\n冷却时间：{card.cooldown}s\n生命值：{hitPoint}";
        infoD.text = $"{card.cardInfo}";
    }
    public void loadItemInfo(BagItem item)
    {
        var info = item.info;
        var name = item.itemName;
        if (info == "") return;

        Destroy(currentCard?.gameObject);
        currentCard = null;
        Destroy(currentItem?.gameObject);
        currentItem = null;

        currentItem = Instantiate(item, Vector3.zero, Quaternion.identity, bagUIInfo.transform);
        currentItem.enabled = false;
        Destroy(currentItem.transform.GetChild(0).gameObject);
        currentItem.GetComponent<RectTransform>().localPosition = new Vector3(-300, 130, 0);
        currentItem.GetComponent<RectTransform>().localScale = Vector3.one * 2;

        bagUIMain.UI_FadeOut_Event();
        bagUIInfo.UI_FadeIn_Event();
        infoU.text = $"名称：{name}";
        infoD.text = $"{info}";
    }
    public void refreshBagItem()
    {
        var data = DataManager.Instance.data;
        //清理之前的快捷键文字
        foreach (var text in summonedOtherShortKeyText)
        {
            Destroy(text.gameObject);
        }
        summonedOtherShortKeyText.Clear();

        for (int i = 0; i < data.otherShortKey.Count; i++)
        {
            var item = items[i];
            var t = Instantiate(shortKeyTEXTPrefab, Vector3.zero, Quaternion.identity, item.transform);
            t.GetComponent<RectTransform>().localPosition = new Vector3(0, -60,0);
            t.text = data.otherShortKey[i].ToString();
            summonedOtherShortKeyText.Add(t);
        }
    }
}
