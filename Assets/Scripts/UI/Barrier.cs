using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Barrier : MonoBehaviour
{
    public TextMeshProUGUI barrierText;

    public int chapter;
    public int level;
    public string info;
    public List<CardType> suggestedCards;
    public List<CardType> rewardCards;

    private List<Card> spawnedCardObjs = new List<Card>();
    public void refreshBarrierInfo(int chapter , int level,string info = "")
    {
        var dat = DataManager.Instance.data;

            //设置信息
        this.chapter = chapter;
        this.level = level;
        this.info = info;
        name = "- < " + chapter + "-" + level + " > -";
        barrierText.text = "- < " + chapter + " - " + level + " > -";

        //是否BOSS关
        if (chapter > 0 && level == 5)
        {
            var lightRed = new Color(1, 88f / 255f, 88f / 255f);
            GetComponent<Image>().color = lightRed;
            barrierText.color = lightRed;
        }

        //设置位置
        GetComponent<RectTransform>().localPosition = new Vector3(-312, 330 - 170 * (level - 1), 0);

        //添加建议卡牌
        suggestedCards = BarrierTools.getBarrierSuggestedCards(chapter,level).ToList();

        //添加奖励卡牌
        rewardCards = BarrierTools.getBarrierCardRewards(chapter, level).ToList();

        //检测是否解锁关卡

        if(dat.currentChapter < chapter)
        {
            Destroy(gameObject);
        }

        if(dat.currentLevel < level && dat.currentChapter == chapter)
        {
            Destroy(gameObject);
        }
    }
    public void loadBarrier()
    {
        var mmum = MainMenuUIManager.Instance;
        mmum.OpenBarrierInfo(chapter, level, BarrierTools.getBarrierInfo(chapter,level));
        mmum.barrierStartButton.GetComponent<Button>().onClick.AddListener(() => gameStart());
        loadingSuggestedAndRewardingCards();
    }
    private void loadingSuggestedAndRewardingCards()
    {
        var mmum = MainMenuUIManager.Instance;
        for(int q = 0; q < mmum.summonedInfoCards.Count; q++)
        {
            var c = mmum.summonedInfoCards[q];
            if (c == null) continue;
            Destroy(c.gameObject);
        }
        spawnedCardObjs.Clear();
        var i = 0;
        foreach (var type in suggestedCards)
        {
            var accuratePos = new Vector3(-351 + i * 80, -119, 0);
            var card = Instantiate(Utils.findCardPrefabByType(type));
            card.transform.SetParent(mmum.barrierInfoMenu.transform);
            card.GetComponent<RectTransform>().localPosition = accuratePos;
            card.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
            card.enabled = false;
            spawnedCardObjs.Add(card);
            mmum.summonedInfoCards.Add(card);
            i++;
        }
        var b = 0;
        foreach (var type in rewardCards)
        {
            var accuratePos = new Vector3(-351 + b * 80, -13, 0);
            var card = Instantiate(Utils.findCardPrefabByType(type));
            card.transform.SetParent(mmum.barrierInfoMenu.transform);
            card.GetComponent<RectTransform>().localPosition = accuratePos;
            card.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
            card.enabled = false;
            spawnedCardObjs.Add(card);
            mmum.summonedInfoCards.Add(card);
            b++;
        }
        mmum.suggestedCardText.gameObject.SetActive(suggestedCards.Count != 0);
        mmum.rewardingText.gameObject.SetActive(rewardCards.Count != 0);
    }
    public void gameStart()
    {
        var gm = GameManager.Instance;
        var mmum = MainMenuUIManager.Instance;
        mmum.mainUi.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        gm.loadLevel("闯关模式", chapter.ToString(), level, level);
        gm.levelRewards = rewardCards;
        mmum.barrierStartButton.GetComponent<Button>().onClick.RemoveListener(() => gameStart());
        print("游戏开始");
    }
}
public static class BarrierTools
{
    private static readonly string[][] BarrierInfo = new string[][]
    {
        new string[]
        {
            "大胆地算吧!",
            "建议完成专项突破：\n与四则共舞.",
            "建议完成专项突破：\n走丢的X.",
            "建议完成专项突破：\n“垂平”之道.",
            "新的战场正在被发现."
        },
        new string[]
        {
            "温馨提示:右键SC可以将其翻转!",
            "让思维翻转吧！",
            "战场似乎开始混乱了.",
            "摄像机有被打歪的风险.",
            "“Hi,我打歪了你的摄像机 :)”"
        }
    };
    private static readonly CardType[][][] cardRewards = new CardType[][][]
    {
        new CardType[][]
        {
            new CardType[]{CardType.ZeroCard},
            new CardType[]{},
            new CardType[]{CardType.SquareCard},
            new CardType[]{},
            new CardType[]{CardType.BrainFlowerCard},
        },
        new CardType[][]
        {
            new CardType[]{CardType.ShieldFlowerCard},
            new CardType[]{},
            new CardType[]{CardType.CalculatorCard},
            new CardType[]{},
            new CardType[]{CardType.CounterclockwiseCard},
        },
    };
    private static readonly CardType[][][] suggestedCards = new CardType[][][]
{
        new CardType[][]
        {
            new CardType[]{},
            new CardType[]{},
            new CardType[]{CardType.DerivativesCard},
            new CardType[]{CardType.DerivativesCard,CardType.PointCard},
            new CardType[]{CardType.DerivativesCard,CardType.PointCard},
        },
        new CardType[][]
        {
            new CardType[]{},
            new CardType[]{},
            new CardType[]{CardType.DerivativesCard},
            new CardType[]{CardType.DerivativesCard,CardType.PointCard},
            new CardType[]{CardType.DerivativesCard,CardType.PointCard},
        },
};
    public static string getBarrierInfo(int chapter, int level)
    {
        if (chapter >= 0 && chapter < BarrierInfo.Length &&
            level >= 0 && level <= BarrierInfo[chapter].Length)
        {
            return BarrierInfo[chapter][level - 1];
        }
        Debug.LogError($"无效的关卡索引: 章节 {chapter}, 关卡 {level}");
        return "未知关卡";
    }
    public static CardType[] getBarrierCardRewards(int chapter, int level)
    {
        if (chapter >= 0 && chapter < cardRewards.Length &&
            level >= 0 && level <= cardRewards[chapter].Length)
        {
            return cardRewards[chapter][level - 1];
        }
        return new CardType[] {};
    }
    public static CardType[] getBarrierSuggestedCards(int chapter, int level)
    {
        if (chapter >= 0 && chapter < suggestedCards.Length &&
            level >= 0 && level <= suggestedCards[chapter].Length)
        {
            return suggestedCards[chapter][level - 1];
        }
        return new CardType[] { };
    }
}
