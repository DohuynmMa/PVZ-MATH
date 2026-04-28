using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamingUIManager : MonoBehaviour
{
    public static GamingUIManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public GameObject mainUi;
    public Image brainPointBar;
    public TextMeshProUGUI brainPointText;
    public GameObject cardArea;
    public GameObject rightClickMenu;
    public TextMeshProUGUI levelInfoShadow;
    public TextMeshProUGUI levelInfo;
    public GameObject fakeEraser;
    public GameObject fakePencil;
    public GameObject bag;
    public GameObject miniMap;
    public UI_FadeInFadeOut dangerousBound;
    public GameObject magnifier;
    public TextMeshProUGUI magnifierText;

    public List<TextMeshProUGUI> itemKeys;

    public GameObject pauseUI;
    private static readonly Color mediumColor = new Color(196f / 255f, 1, 198f / 255f);
    private static readonly Color hardColor = new Color(156f / 255f, 223f / 255f, 1);
    private float updateTimer = 0;
    private void Update()
    {
        updateLevelInfoText();
    }
    private void updateLevelInfoText()
    {
        updateTimer += Time.deltaTime;
        if(updateTimer >= 1.5f)
        {
            updateTimer = 0;
            var gm = GameManager.Instance;
            if (gm.inGame)
            {
                string cleared = gm.winProblemCount == 0 ? "" : "\nCLEARED-" + ((float)((float)gm.solvedProblemCount / (float)gm.winProblemCount) * 100f).ToString("F1") + "%";
                levelInfoShadow.text = gm.levelName + "\n" + gm.difficultF + "-" + gm.difficultB + cleared;
                levelInfo.text = levelInfoShadow.text;
                var color = Color.white;
                switch (gm.difficultF)
                {
                    case "MIDDLE":
                        color = mediumColor;
                        break;
                    case "HARD":
                        color = hardColor;
                        break;
                    default:
                        color = Color.white;
                        break;
                }
                levelInfo.color = color;
            }
            else
            {
                levelInfoShadow.text = "";
                levelInfo.text = "";
            }
        }
    }
    public void pause()
    {
        pauseUI.SetActive(true);
        GameManager.Instance.inPause = true;
        pauseUI.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
    }
    public void gameContinue()
    {
        GameManager.Instance.inPause = false;
        pauseUI.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
    }
}
