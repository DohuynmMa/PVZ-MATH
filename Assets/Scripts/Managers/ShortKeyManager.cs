using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;
using TMPro;
using UnityEngine.SocialPlatforms;

public class ShortKeyManager : MonoBehaviour
{
    public static ShortKeyManager Instance;
    private static GameManager gm { get => GameManager.Instance; }
    private static GlobalUIManager guim { get => GlobalUIManager.Instance; }
    private static CardManager cm { get => CardManager.Instance; }
    public bool isChangingKey = false;
    public bool onTrialKey = false;
    public Card cardOnTrial = null;
    public int changingKeyTemp = -1;
    public KeyCode gottenKeyCodeWhenChangingKey = KeyCode.None;

    public List<KeyCode> allShortKey;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        getShortKeyDown();
    }
    private void getShortKeyDown()
    {
        var data = DataManager.Instance.data;
        var hm = HandManager.Instance;
        var gum = GamingUIManager.Instance;

        if (data == null || Cursor.lockState == CursorLockMode.Locked) return;

        if (isChangingKey && Input.anyKeyDown)
        {
            gottenKeyCodeWhenChangingKey = GetPressedKey();
            if (IsMouseButton(gottenKeyCodeWhenChangingKey)) return;
            //Esc默认消除快捷键
            if (onTrialKey && cardOnTrial != null)
            {
                changeShortKeyOnTrial(cardOnTrial, gottenKeyCodeWhenChangingKey == KeyCode.Escape ? KeyCode.None : gottenKeyCodeWhenChangingKey);
            }
            else
            {
                changeShortKey(changingKeyTemp, gottenKeyCodeWhenChangingKey == KeyCode.Escape ? KeyCode.None : gottenKeyCodeWhenChangingKey);
            }
            return;
        }

        //--游戏中的快捷键--

        if (gm.inGame)
        {
            //暂停
            if (pressedNotConflictingShortKey(data.otherShortKey[0]) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (!guim.bagIsOpened && gm.levelName != "*REVIVE*" && gm.pauseAble)
                {
                    if (!gm.inPause)
                    {
                        GamingUIManager.Instance.pause();
                    }
                    else
                    {
                        GamingUIManager.Instance.gameContinue();
                    }
                }
            }

            //使用橡皮擦
            if (pressedNotConflictingShortKey(data.otherShortKey[2]))
            {
                if (guim.bagIsOpened || gm.inPause) return;
                if (hm.isImportingOrExportingCellNum())
                {
                    gum.rightClickMenu.SetActive(false);
                    hm.hidePlacePoint();
                }
                hm.useOrCancelEraser();
            }

            //使用铅笔
            if (pressedNotConflictingShortKey(data.otherShortKey[3]))
            {
                if (guim.bagIsOpened || gm.inPause) return;
                if (hm.isImportingOrExportingCellNum())
                {
                    gum.rightClickMenu.SetActive(false);
                    hm.hidePlacePoint();
                }
                hm.useOrCancelPencil();
            }

            //使用卡牌
            for (int i = 0; i < data.cardShortKey.Count; i++)
            {
                if (gm.inPause) return;
                var key = data.cardShortKey[i];
                if (pressedNotConflictingShortKey(key) && !guim.bagIsOpened)
                {
                    var cardType = data.cardTypes[i];
                    var card = cm.findSummonedCardByType(cardType);
                    card.useCard();
                }
            }
            for (int i = 0; i < cm.cardShortKeyOnTrial.Count; i++)
            {
                if (gm.inPause) return;
                var key = cm.cardShortKeyOnTrial[i];
                if (pressedNotConflictingShortKey(key) && !guim.bagIsOpened)
                {
                    var cardType = cm.summonedCardsOnTrial[i].cardType;
                    var card = cm.findSummonedCardByType(cardType);
                    card.useCard();
                }
            }
        }

        //--全局快捷键--

        //全屏
        if (pressedNotConflictingShortKey(data.otherShortKey[1]))
        {
            ScreenManager.Instance.ToggleFullScreen();
        }

        //打开背包
        if (pressedNotConflictingShortKey(data.otherShortKey[4]))
        {
            if (guim.bagIsOpened)
            {
                guim.closeBag();
            }
            else
            {
                guim.openBag();
            }
        }
        //ESC可关闭背包
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (guim.bagIsOpened)
            {
                guim.closeBag();
            }
        }
    }
    public void loadShortKey()
    {
        var data = DataManager.Instance.data;
        if(data == null) return;
        allShortKey = new List<KeyCode>();
        allShortKey.AddRange(data.cardShortKey);
        allShortKey.AddRange(data.otherShortKey);
        allShortKey.AddRange(cm.cardShortKeyOnTrial);
    }
    public void startToSetShortKey(int temp,bool onTrial = false)
    {
        var data = DataManager.Instance.data;
        changingKeyTemp = temp;
        isChangingKey = true;
        onTrialKey = false;
        if (temp + 1 <= data.cardShortKey.Count)
        {
            cm.findSummonedCardByType(data.cardTypes[changingKeyTemp]).key.text = "请输入";
        }
        else
        {
            guim.summonedOtherShortKeyText[temp - (data.cardShortKey.Count)].text = "请输入";
        }
    }
    public void startToSetShortKey(Card summonedCard)
    {
        var data = DataManager.Instance.data;
        changingKeyTemp = -1;
        isChangingKey = true;
        onTrialKey = true;
        cardOnTrial = summonedCard;
        summonedCard.key.text = "请输入";
    }
    public void changeShortKey(int changingTemp,KeyCode newCode)
    {
        var data = DataManager.Instance.data;
        allShortKey[changingTemp] = newCode;
        bool isCard = changingTemp + 1 <= data.cardShortKey.Count;

        //非卡牌快捷键
        if (!isCard)
        {
            var otherTemp = changingTemp - (data.cardShortKey.Count);
            data.otherShortKey[otherTemp] = newCode;
            guim.summonedOtherShortKeyText[otherTemp].text = newCode.ToString();
            if (gm.inGame)
            {
                GamingUIManager.Instance.itemKeys[otherTemp - 2].text = newCode.ToString();
            }
        }
        //卡牌快捷键
        else
        {
            data.cardShortKey[changingTemp] = newCode;
            cm.findSummonedCardByType(data.cardTypes[changingTemp]).key.text = newCode.ToString();
        }

        //检测这个按键是否冲突
        if (hasConflictingShortKey(newCode))
        {
            changeShortKey(changingTemp, KeyCode.None);
            if (isCard)
            {
                cm.findSummonedCardByType(data.cardTypes[changingTemp]).key.text = "按键冲突";
            }
            else
            {
                var otherTemp = changingTemp - (data.cardShortKey.Count);
                guim.summonedOtherShortKeyText[otherTemp].text = "按键冲突";
            }
            return;
        }

        isChangingKey = false;
        gottenKeyCodeWhenChangingKey = KeyCode.None;
        changingKeyTemp = -1;
        DataManager.Instance.savePlayerData();
    }
    public void changeShortKeyOnTrial(Card summonedCard,KeyCode newCode)
    {
        if (summonedCard == null) return;
        var temp = cm.summonedCardsOnTrial.IndexOf(summonedCard);
        cm.cardShortKeyOnTrial[temp] = newCode;
        summonedCard.key.text = newCode.ToString();

        //检测这个按键是否冲突
        if (hasConflictingShortKey(newCode))
        {
            changeShortKeyOnTrial(summonedCard, KeyCode.None);
            summonedCard.key.text = "按键冲突";
            return;
        }

        isChangingKey = false;
        gottenKeyCodeWhenChangingKey = KeyCode.None;
        cardOnTrial = null;
        changingKeyTemp = -1;
    }
    private KeyCode GetPressedKey()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode;
            }
        }
        return KeyCode.None;
    }
    private bool pressedNotConflictingShortKey(KeyCode keyCode)
    {
        return !hasConflictingShortKey(keyCode) && Input.GetKeyDown(keyCode);
    }

    private bool hasConflictingShortKey(KeyCode keyCode)
    {
        return keyCode != 0 && allShortKey.FindAll(x => x == keyCode).Count > 1;
    }
    private bool IsMouseButton(KeyCode keyCode)
    {
        return keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6;
    }
}
