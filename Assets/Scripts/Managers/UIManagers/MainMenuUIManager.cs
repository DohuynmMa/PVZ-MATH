using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum MainMenuUIType
{
    System,
    Start,
    Setting,
    About
}
public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private static readonly Color lightBlue = new Color(160f / 255f,1,1);
    public GameObject mainUi;
    public MainMenuUIType currentUIType;

    public GameObject mainMenuParticleEffect;

    public GameObject systemUI;
    public GameObject startUI;
    public GameObject settingUI;
    public GameObject aboutUi;

    public GameObject startButton;
    public GameObject settingButton;
    public GameObject aboutButton;

    public GameObject mainStartMenu;
    public GameObject SBStartMenu;
    public GameObject barrierStartMainMenu;
    public GameObject barrierStartMenuC0;
    public GameObject barrierStartMenuC1;
    public GameObject barrierC1Button;
    public int currentChapter = -1;

    public Barrier barrierPrefab;
    public GameObject barrierInfoMenu;
    public TextMeshProUGUI barrierName;
    public TextMeshProUGUI barrierInfo;
    public TextMeshProUGUI suggestedCardText;
    public Button barrierStartButton;
    public TextMeshProUGUI rewardingText;
    public TextMeshProUGUI startButtonText;
    public Image barrierInfoBg;
    public List<Card> summonedInfoCards;

    public UI_FadeInFadeOut backToTheMainStartMenuButton;

    public TextMeshProUGUI newsText;
    public TextMeshProUGUI daveSystemnameText;

    public Scrollbar soundVolumeBar;
    public Scrollbar musicVolumeBar;
    public Scrollbar voiceVolumeBar;

    public List<GameLevel> levelList;//专项突破
    public int levelPageTemp = 0;
    public List<GameObject> levelPages;
    public GameObject nextLevelPageButton;
    public GameObject lastLevelPageButton;

    public List<Barrier> levelListBarrier = new List<Barrier>();//闯关模式

    public GameObject setNameUI;
    public TextMeshProUGUI inputNameArea;
    public TextMeshProUGUI inputTips;

    private Dictionary<MainMenuUIType, GameObject> uiList = new Dictionary<MainMenuUIType, GameObject>();
    private List<Tween> tweens = new List<Tween>();
    [TextArea]
    public List<string> news; 
    private void Start()
    {
        GameManager.Instance.currentBg = BackgroundType.MathWorld;

        DOVirtual.DelayedCall(0.6f, () =>
        {
            var d = DataManager.Instance.data;
            if(d != null)
            {
                if (d.playerName == "" && d.completedNewPlayerTutorial)
                {
                    d.playerName = "无名氏";
                    DataManager.Instance.savePlayerData();
                }
            }
        });
        uiList.Add(MainMenuUIType.System, systemUI);
        uiList.Add(MainMenuUIType.Start, startUI);
        uiList.Add(MainMenuUIType.Setting, settingUI);
        uiList.Add(MainMenuUIType.About, aboutUi);
        print("已注册UI");
    }
    private void Update()
    {
        updateMainMenuParticleEffect();
    }
    public bool checkName()
    {
        if (DataManager.Instance.data.playerName == "")
        {
            setNameUI.SetActive(true);
            setNameUI.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
            return false;
        }
        else
        {
            setNameUI.SetActive(false);
            return true;
        }
    }
    /// <summary>
    /// 确认名字
    /// </summary>
    public void confirmName()
    {
        if (inputNameArea.text.Length > 30)
        {
            return;
        }
        if (inputNameArea.text.Length < 2)
        {
            inputTips.text = "名称太短";
            return;
        }
        DataManager.Instance.data.playerName = inputNameArea.text;
        DataManager.Instance.savePlayerData();
        updateDaveSystemNameText();
        setNameUI.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
    }
    public void updateDaveSystemNameText()
    {
        daveSystemnameText.text = "你好！" + DataManager.Instance.data.playerName;
    }
    public void openUrl(string url)
    {
        Application.OpenURL(url);
    }
    private void updateUI()
    {
        tweens.Clear();
        DataManager.Instance.savePlayerData();
        foreach (var ui in uiList)
        {
            if(ui.Key != currentUIType)
            {
                ui.Value.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
                var tw = DOVirtual.DelayedCall(0.6f, () =>
                {
                    ui.Value.SetActive(false);
                });
                tweens.Add(tw);
            }
            else
            {
                var tw = DOVirtual.DelayedCall(0.6f, () =>
                {
                    ui.Value.SetActive(true);
                    ui.Value.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                });
                tweens.Add(tw);
            }
        }
    }
    public void openStartUI()
    {
        if (currentUIType == MainMenuUIType.Start)
        {
            closeUI();
            startButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            currentUIType = MainMenuUIType.Start;
            allButtonWhite();
            startButton.GetComponent<Image>().color = lightBlue;
            updateUI();

        }
    }
    public void OpenSBLevelStartMenu()
    {
        SBStartMenu.SetActive(true);
        SBStartMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        mainStartMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        backToTheMainStartMenuButton.UI_FadeIn_Event();
        foreach (var l in levelList)
        {
            l.loadLevelInformations();
        }
    }
    public void nextSBLevelPage()
    {
        levelPages[levelPageTemp].SetActive(false);
        levelPageTemp++;
        if (levelPageTemp + 1 > levelPages.Count) levelPageTemp = 0;
        levelPages[levelPageTemp].SetActive(true);
    }
    public void lastSBLevelPage()
    {
        levelPages[levelPageTemp].SetActive(false);
        levelPageTemp--;
        if (levelPageTemp < 0) levelPageTemp = levelPages.Count - 1;
        levelPages[levelPageTemp].SetActive(true);
    }
    public void OpenBarrierStartMenu()
    {
        barrierC1Button.SetActive(DataManager.Instance.data.currentChapter >= 1);
        barrierStartMainMenu.SetActive(true);
        barrierStartMainMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        barrierStartMenuC0.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        barrierStartMenuC1.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        mainStartMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        backToTheMainStartMenuButton.UI_FadeIn_Event();
        currentChapter = -1;
        foreach (var l in levelList)
        {
            l.loadLevelInformations();
        }
    }
    public void OpenBarrierC0Menu()
    {
        barrierStartMenuC0.SetActive(true);
        currentChapter = 0;
        barrierStartMenuC0.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        barrierStartMainMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        spawnBarrierButton(0);
    }
    public void OpenBarrierC1Menu()
    {
        var dat = DataManager.Instance.data;
        if(dat.currentChapter < 1)
        {
            return;
        }
        SoundsManager.playSounds(9);
        barrierStartMenuC1.SetActive(true);
        currentChapter = 1;
        barrierStartMenuC1.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        barrierStartMainMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        spawnBarrierButton(1);
    }
    public void returnToCurrentChapterBarrierMenu()
    {
        barrierInfoMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        switch (currentChapter)
        {
            case 0:
                OpenBarrierC0Menu();
                break;
            case 1:
                OpenBarrierC1Menu();
                break;
        }
    }
    private void spawnBarrierButton(int chapter)
    {
        foreach (var b in levelListBarrier)
        {
            if (b != null)
            {
                Destroy(b.gameObject);
            }
        }
        levelListBarrier.Clear();
        for (int i = 0; i < 5; i++)
        {
            var b = Instantiate(barrierPrefab, Vector3.zero, Quaternion.identity, (chapter == 0 ? barrierStartMenuC0 : barrierStartMenuC1).transform);
            b.refreshBarrierInfo(chapter, i + 1);
            b.GetComponent<Button>().onClick.AddListener(() => SoundsManager.playSounds(9));
            levelListBarrier.Add(b);
        }
    }
    public void OpenBarrierInfo(int chapter,int level,string info = "")
    {
        barrierStartMenuC1.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        barrierStartMenuC0.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();

        barrierInfoMenu.SetActive(true);
        barrierInfoMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();

        //加载关卡信息
        barrierName.text = "闯关模式\n- < " + chapter + " - " + level + " > -";
        barrierInfo.text = info;

        //加载关卡的UI颜色
        var accurateColor = chapter > 0 && level == 5 ? new Color(1, 88f / 255f, 88f / 255f) : Color.white;
        barrierName.color = accurateColor;
        barrierInfo.color = accurateColor;
        rewardingText.color = accurateColor;
        suggestedCardText.color = accurateColor;
        barrierInfoBg.color = accurateColor;
        startButtonText.color = accurateColor;
        barrierStartButton.GetComponent<Image>().color = accurateColor;
    }
    public void returnToMainStartMenu()
    {
        mainStartMenu.SetActive(true);
        mainStartMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        SBStartMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        barrierStartMainMenu.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        updateDaveSystemNameText();
    }
    public void openSettingUI()
    {
        if (currentUIType == MainMenuUIType.Setting)
        {
            closeUI();
            settingButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            currentUIType = MainMenuUIType.Setting;
            musicVolumeBar.value = DataManager.Instance.data.musicVolume;
            soundVolumeBar.value = DataManager.Instance.data.soundVolume;
            voiceVolumeBar.value = DataManager.Instance.data.voiceVolume;
            allButtonWhite();
            settingButton.GetComponent<Image>().color = lightBlue;
            updateUI();
        }
    }
    public void openAboutUI()
    {
        if (currentUIType == MainMenuUIType.About)
        {
            closeUI();
            aboutButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            currentUIType = MainMenuUIType.About;
            allButtonWhite();
            aboutButton.GetComponent<Image>().color = lightBlue;
            updateUI();
        }
    }
    public void closeUI()
    {
        currentUIType = MainMenuUIType.System;
        updateUI();
    }
    private void allButtonWhite()
    {
        settingButton.GetComponent<Image>().color = Color.white;
        aboutButton.GetComponent<Image>().color = Color.white;
        startButton.GetComponent<Image>().color = Color.white;
    }
    public void setMusicVolume()
    {
        float volume = musicVolumeBar.value;
        SoundsManager.setMusicVolume(volume);
    }
    public void setSoundVolume()
    {
        float volume = soundVolumeBar.value;
        SoundsManager.setSoundVolume(volume);
    }
    public void setVoiceVolume()
    {
        float volume = voiceVolumeBar.value;
        SoundsManager.setVoiceVolume(volume);
    }
    public void refreshNews()
    {
        var orgNews = newsText.text;
        string news = orgNews;
        while (news == orgNews)
        {
            news = this.news[Random.Range(0, this.news.Count)];
        }
        newsText.text = news;
    }
    private void updateMainMenuParticleEffect()
    {
        if (mainUi.GetComponent<CanvasGroup>().alpha > 0.9f)
        {
            mainMenuParticleEffect.SetActive(true);
        }
        else
        {
            mainMenuParticleEffect.SetActive(false);
        }
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
