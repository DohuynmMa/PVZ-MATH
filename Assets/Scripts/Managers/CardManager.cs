using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    public List<Card> summonedCards = new List<Card>();
    public List<Card> summonedCardsOnTrial = new List<Card>();
    public List<KeyCode> cardShortKeyOnTrial = new List<KeyCode>();
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 添加玩家存档的卡牌到玩家背包
    /// </summary>
    /// <param name="cardTypeList"></param>
    public void refreshCard(List<CardType> cardTypeList)
    {
        //清理之前的卡牌
        cleanCardsOnTrial();
        foreach (var card in summonedCards)
        {
            Destroy(card.gameObject);
        }
        summonedCards.Clear();

        var i = 0;
        var gum = GlobalUIManager.Instance;
        var data = DataManager.Instance.data;

        foreach(var type in cardTypeList)
        {
            var accuratePos = new Vector3(-420 + (i % 8) * 125, 255 - Mathf.Round(i / 8) * 200, 0);
            var card = Instantiate(Utils.findCardPrefabByType(type),transform.position, Quaternion.identity);
            summonedCards.Add(card);
            card.transform.SetParent(gum.bagUIMain.transform);
            var key = Instantiate(gum.shortKeyTEXTPrefab,Vector3.zero,Quaternion.identity,card.transform);
            card.key = key;
            key.GetComponent<RectTransform>().localPosition = new Vector3(0, 85, 0);
            key.text = data.cardShortKey[data.cardTypes.IndexOf(type)].ToString();
            card.GetComponent<RectTransform>().localPosition = accuratePos;
            card.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            i++;
        }
        ShortKeyManager.Instance.loadShortKey();
    }

    /// <summary>
    /// 添加试用卡牌到玩家背包
    /// </summary>
    /// <param name="cardTypeList"></param>
    public void refreshCardOnTrial(List<CardType> cardTypeList)
    {
        //清理之前的卡牌
        cleanCardsOnTrial();

        var i = summonedCards.Count;
        var gum = GlobalUIManager.Instance;
        var data = DataManager.Instance.data;
        foreach (var type in cardTypeList)
        {
            //如果已经拥有试用卡牌 则放弃生成
            bool continueOuter = true;
            foreach (var cardNotInTrial in summonedCards)
            {
                if(cardNotInTrial.cardType == type)
                {
                    continueOuter = false;
                }
            }
            if (!continueOuter) continue;

            var accuratePos = new Vector3(-420 + (i % 8) * 125, 255 - Mathf.Round(i / 8) * 200, 0);
            var card = Instantiate(Utils.findCardPrefabByType(type), transform.position, Quaternion.identity);
            summonedCardsOnTrial.Add(card);
            card.transform.SetParent(gum.bagUIMain.transform);
            var key = Instantiate(gum.shortKeyTEXTPrefab, Vector3.zero, Quaternion.identity, card.transform);
            card.key = key;
            key.GetComponent<RectTransform>().localPosition = new Vector3(0, 85, 0);
            key.text = "试用";
            card.onTrial = true;
            card.GetComponent<RectTransform>().localPosition = accuratePos;
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            i++;
        }
        cardShortKeyOnTrial = Enumerable.Repeat(KeyCode.None, summonedCardsOnTrial.Count).ToList();
        ShortKeyManager.Instance.loadShortKey();
    } 

    /// <summary>
    /// 清除玩家背包内的试用卡牌
    /// </summary>
    public void cleanCardsOnTrial()
    {
        foreach (var card in summonedCardsOnTrial)
        {
            Destroy(card.gameObject);
        }
        summonedCardsOnTrial.Clear();
        ShortKeyManager.Instance.loadShortKey();
    }

    /// <summary>
    /// 将新卡牌添加到玩家存档,无保存,需调用后手动保存
    /// </summary>
    /// <param name="cards"></param>
    public void obtainNewCards(List<CardType> cards)
    {
        var dt = DataManager.Instance.data;
        foreach (var c in cards)
        {
            //当前版本不支持重复卡牌
            if(!dt.cardTypes.Contains(c)) dt.cardTypes.Add(c);
        }
        dt.loadCardShortKey();
        refreshCard(dt.cardTypes);
    }

    /// <summary>
    /// 根据Type得到已经生成的卡牌
    /// </summary>
    /// <param name="cardType"></param>
    /// <returns></returns>
    public Card findSummonedCardByType(CardType cardType)
    {
        foreach (var card in summonedCards)
        {
            if (card == null) continue;
            if (card.cardType == cardType) return card;
        }
        foreach (var card in summonedCardsOnTrial)
        {
            if (card == null) continue;
            if (card.cardType == cardType) return card;
        }
        return null;
    }
    /// <summary>
    /// 游戏开始初始化卡牌信息
    /// </summary>
    public void onGameStart()
    {
        foreach(var card in summonedCardsOnTrial)
        {
            card.cooldownTimer = card.cooldown * 0.5f;
        }
        foreach(var card in summonedCards)
        {
            card.cooldownTimer = card.cooldown * 0.5f;
        }
    }
}
