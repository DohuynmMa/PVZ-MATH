using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//TODO GAMEMODE
public class GameLevel : MonoBehaviour
{
    private List<string> difficults;
    public string gameDifficultF = "EAZY";
    public string levelName;
    [TextArea]
    public string levelInfo;
    public int gameDifficultB = 1;
    public int levelTemp = 0;
    public List<GameObject> ticks;
    public bool isAboutToStart = false;
    public List<CardType> cardRewards;
    [TextArea]
    public List<string> difficultAdditions;

    private Vector2 origPos;
    private UI_FadeInFadeOut levelNameText;
    private UI_FadeInFadeOut aboutToStartButton;
    private UI_FadeInFadeOut info;
    private UI_FadeInFadeOut progressParent;
    private UI_FadeInFadeOut setDifficultButton;
    private TextMeshProUGUI reward;
    private Image leaveButton;
    private TextMeshProUGUI difficultText;
    private static readonly Color mediumColor = new Color(196f / 255f, 1, 198f / 255f);
    private static readonly Color hardColor = new Color(156f / 255f, 223f/255f, 1);
    private List<Card> spawnedCardObjs = new List<Card>();
    private void Start()
    {
        difficults = new List<string>(GameManager.Instance.difficults);
        difficults.RemoveAt(difficults.Count - 1);
    }
    private void findObj()
    {
        info = transform.Find("Info").GetComponent<UI_FadeInFadeOut>();
        aboutToStartButton = transform.Find("StartButton").GetComponent<UI_FadeInFadeOut>();
        setDifficultButton = transform.Find("SetDifficultButton").GetComponent<UI_FadeInFadeOut>();
        levelNameText = transform.Find("Name").GetComponent<UI_FadeInFadeOut>();
        progressParent = transform.Find("Progress").GetComponent<UI_FadeInFadeOut>();
        difficultText = transform.Find("SetDifficultButton").Find("DifficultText").GetComponent<TextMeshProUGUI>();
        leaveButton = info.transform.Find("Back").GetComponent<Image>();
        reward = info.transform.Find("Reward").GetComponent<TextMeshProUGUI>();
    }
    public void loadLevelInformations()
    {
        findObj();
        info.UI_FadeOut_Event();
        info.GetComponent<TextMeshProUGUI>().text = $"<{levelName}>" + "\n\n" + levelInfo;
        setDifficultButton.UI_FadeOut_Event();
        setAllUIColor(Color.white);
        updateLevelProgess();
        gameDifficultF = "EAZY";
        difficultText.text = gameDifficultF;
        levelNameText.GetComponent<TextMeshProUGUI>().text = levelName;
        setDifficultButton.gameObject.SetActive(false);
        info.gameObject.SetActive(false);
        origPos = GetComponent<RectTransform>().localPosition;
    }

    public void aboutToStart()
    {
        if (!isAboutToStart)
        {
            setDifficultButton.gameObject.SetActive(true);
            info.gameObject.SetActive(true);
            isAboutToStart = true;
            levelNameText.GetComponent<TextMeshProUGUI>().text = "再次点击图标即可开始";
            info.UI_FadeIn_Event();
            loadingCardRewards();
            setDifficultButton.UI_FadeIn_Event();
            progressParent.UI_FadeOut_Event();
            GetComponent<RectTransform>().DOAnchorPos(new Vector2(-329, 250), 0.5f);
            var mmum = MainMenuUIManager.Instance;
            mmum.nextLevelPageButton.SetActive(false);
            mmum.lastLevelPageButton.SetActive(false);
            mmum.backToTheMainStartMenuButton.UI_FadeOut_Event();
            foreach (var l in mmum.levelList)
            {
                if (l != this)
                {
                    l.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
                }
            }
        }
        else
        {
            SoundsManager.stopMusic();
            returnToChooseLevel();
            gameStart();
            MainMenuUIManager.Instance.mainUi.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        }
    }
    public void returnToChooseLevel()
    {
        GetComponent<RectTransform>().DOAnchorPos(origPos, 0.5f);
        levelNameText.GetComponent<TextMeshProUGUI>().text = levelName;
        info.UI_FadeOut_Event();
        setDifficultButton.UI_FadeOut_Event();
        progressParent.UI_FadeIn_Event();
        var mmum = MainMenuUIManager.Instance;
        mmum.nextLevelPageButton.SetActive(true);
        mmum.lastLevelPageButton.SetActive(true);
        mmum.backToTheMainStartMenuButton.UI_FadeIn_Event();
        foreach (var l in mmum.levelList)
        {
            if (l != this)
            {
                l.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
            }
        }
        foreach(var card in spawnedCardObjs)
        {
            Destroy(card.gameObject);
        }
        spawnedCardObjs.Clear();
        isAboutToStart = false;
    }
    public void gameStart()
    {
        var gm = GameManager.Instance;
        MainMenuUIManager.Instance.mainUi.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        gm.loadLevel(levelName,gameDifficultF,gameDifficultB,levelTemp);
        gm.levelRewards = cardRewards;
        print("游戏开始");
    }
    private void updateLevelProgess()
    {
        var data = DataManager.Instance.data;
        int i = 1;
        foreach (var t in ticks)
        {
            if(i <= data.SBlevelProgess[levelTemp]) t.SetActive(true);
            else t.SetActive(false);
            i++;
        }
    }
    public void setDifficultF()
    {
        var temp = difficults.IndexOf(gameDifficultF);
        temp++;
        temp = temp % difficults.Count;
        if(temp > DataManager.Instance.data.SBlevelProgess[levelTemp])
        {
            info.GetComponent<TextMeshProUGUI>().text = $"<{levelName}>" + "\n\n" + levelInfo + "\n\n需完成至少一次当前难度\n才可解锁下一难度";
            temp = 0;
        }
        else info.GetComponent<TextMeshProUGUI>().text = $"<{levelName}>" + "\n\n" + levelInfo + "\n\n" + difficultAdditions[temp];
        gameDifficultF = difficults[temp];
        difficultText.text = gameDifficultF;
        var color = Color.white;
        switch (temp)
        {
            case 1:
                color = mediumColor;
                break;
            case 2:
                color = hardColor;
                break;
        }
        setAllUIColor(color);
    }
    private void setAllUIColor(Color color)
    {
        aboutToStartButton.GetComponent<Image>().DOColor(color, 0.5f);
        setDifficultButton.GetComponent<Image>().DOColor(color, 0.5f);
        info.GetComponent<TextMeshProUGUI>().DOColor(color, 0.5f);
        levelNameText.GetComponent<TextMeshProUGUI>().DOColor(color, 0.5f);
        leaveButton.DOColor(color, 0.5f);
        reward.DOColor(color, 0.5f);
        difficultText.DOColor(color == Color.white || color == mediumColor ? Color.black : Color.white, 0.5f);
    }
    private void loadingCardRewards()
    {
        spawnedCardObjs.Clear();
        var i = 0;
        foreach (var type in cardRewards)
        {
            var accuratePos = new Vector3(-130 + i * 80, -696, 0);
            var card = Instantiate(Utils.findCardPrefabByType(type), transform.position, Quaternion.identity);
            card.transform.SetParent(transform);
            card.GetComponent<RectTransform>().localPosition = accuratePos;
            card.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
            card.enabled = false;
            spawnedCardObjs.Add(card);
            i++;
        }
    }
}
