using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance{ get; private set; }
    public Segment segmentPrefab;
    public List<string> difficults;//EAZY : 0 MIDDLE : 1 HARD : 2
    public List<Shooter> shooters;//当局全部SC
    public List<int> rowNumUp = new List<int> { 0,0,0,0,0,0};//每一行射手即将发射的子弹的数字 一二象限
    public List<int> rowNumDown = new List<int> { 0, 0, 0, 0, 0, 0 };//每一行射手即将发射的子弹的数字 三四象限
    public List<Effect> effectsInGame = new List<Effect>();//当局全部特效
    public List<GameObject> objsInGame = new List<GameObject>();

    public float spawnProblemDuration;
    public bool summoningProblem = false;
    private float spawnProblemTimer;
    public string difficultF;
    public int difficultB;
    public string levelName;
    public int levelTemp;
    public List<CardType> levelRewards;
    public List<EntityType> problemTypes;

    public int summonedProblemCount;//单局生成敌人数
    public int solvedProblemCount;//单局杀敌数
    public bool canWin = false;//允许胜利
    public bool pauseAble = true;
    public int winProblemCount;//胜利需杀敌数
    public Action levelOperatorUpdate = null;

    public int quardrantInTotal = 1;//总象限


    public BackgroundType currentBg;
    public bool inGame;
    public bool inPause;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (inGame)
        {
            levelOperatorUpdate?.Invoke();
        }
    }
    
    public void setCells()
    {
        for(var q = 1;q < quardrantInTotal + 1; q++)
        {
            for (var i = 0; i < 66; i++)
            {
                var cellNum = Instantiate(ResourceManager.Instance.miscs[0]).GetComponent<TextMeshPro>();
                var cell = Utils.findCellById(i + 1,q);
                if (cell == null)
                {
                    print("cant find cell");
                    continue;
                }
                cell.numText = cellNum;
                cellNum.transform.SetParent(cell.transform);
                cell.numText.GetComponent<RectTransform>().localPosition = Vector3.zero;
                var meshRenderer = cellNum.GetComponent<MeshRenderer>();
                meshRenderer.sortingLayerName = "EffectB";
            }
        }
    }
    public void setShooters()
    {
        shooters.Clear();
        //生成顶部SC
        rowNumUp = new List<int>{ 1, 1, 1, 1, 1, 1};
        var fakeRow = 6;
        //象限大于2,生成底部SC
        if (quardrantInTotal > 2)
        {
            rowNumDown = new List<int> { 1, 1, 1, 1, 1, 1 };
            fakeRow = 12;
        }
        else
        {
            rowNumDown = new List<int> { 0, 0, 0, 0, 0, 0 };
        }
        for (var row = 1; row < fakeRow + 1; row++)
        {
            var shooterPrefab = Utils.findEntityPrefabByType(EntityType.Shooter).GetComponent<Shooter>();

            Vector3 accuratePos = new Vector3(0, 8.2f - (row - 1) * 1.31f, 0);
            if (row > 6)
            {
                accuratePos.y -= 2.15f;
            }

            var shooter = Instantiate(shooterPrefab, accuratePos, Quaternion.identity);
            if (shooter != null)
            {
                shooter.entityState = EntityState.Enable;
                shooter.entityGroup = EntityGroup.Own;
                shooters.Add(shooter);
                shooter.row = row > 6 ? 6 - row : 7 - row;
            }
        }
    }
    public void setCamera()
    {
        var fm = Camera.main.GetComponent<FollowMouseTool>();
        var multiQuardant = quardrantInTotal > 1;

        fm.minOrthoSize = multiQuardant ? 4 : 2;
        fm.maxOrthoSize = multiQuardant ? 12 : 6;

        fm.minPerspDistance = multiQuardant ? 10 : 5;
        fm.maxPerspDistance = multiQuardant ? 20 : 10;

        var posLD = new Vector3(-8, -5, -10);
        var posRU = new Vector3(8, 5, -10);
        posLD.x = multiQuardant ? posLD.x : 0;
        posLD.y = multiQuardant ? posLD.y : 0;
        fm.posLD = posLD;
        fm.posRU = posRU;

        //目前只有二维 没写三维的
        Camera.main.transform.localPosition = multiQuardant ? new Vector3(3.96f, 2.35f, -10) : new Vector3(7.92f, 6.13f, -10);
        Camera.main.orthographicSize = fm.maxOrthoSize;
        Camera.main.GetComponent<FollowMouseTool>().enableTool = multiQuardant;
        Camera.main.GetComponent<FollowMouseTool>().cameraMoveSpeed = 30;
        GamingUIManager.Instance.miniMap.SetActive(multiQuardant);
        GamingUIManager.Instance.magnifier.SetActive(multiQuardant);
        GamingUIManager.Instance.magnifierText.text = "看不清难题?\n尝试点击一下它!";

        GameInfoLoader.loadExtraCameraSettings();
    }
    public void loadLevel(string levelName,string difficultF,int difficultB, int levelTemp,bool tutorial = false)
    {
        var dm = GlobalUIManager.Instance;
        var gum = GamingUIManager.Instance;
        pauseAble = false;
        //更新当前关卡信息
        this.levelName = levelName;
        this.difficultF = difficultF;
        this.difficultB = difficultB;
        this.levelTemp = levelTemp;
        //加载游戏内容
        dm.黑色转场.gameObject.SetActive(true);
        dm.黑色转场.UI_FadeIn_Event();
        dm.黑色转场.onFadeInOrFadeOut += () =>
        {
            gum.mainUi.SetActive(levelName != "*REVIVE*");
            gum.levelInfoShadow.text = "";
            gum.levelInfo.text = "";
            gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
            setShooters();
            playMusic();
            Utils.changeBackground(BackgroundType.MathWorld);
            problemTypes = GameInfoLoader.loadProblemTypes();
            inGame = true;
            CardManager.Instance.refreshCardOnTrial(GameInfoLoader.loadCardsInTrial());
            BrainPointManager.Instance.brainPoint = 5;
            if(DataManager.Instance.data != null)
            {
                for (int i = 0; i < DataManager.Instance.data.otherShortKey.Count; i++)
                {
                    if (i + 1 > gum.itemKeys.Count) continue;
                    gum.itemKeys[i].text = DataManager.Instance.data.otherShortKey[i + 2].ToString();
                }
            }
            pauseAble = true;
            CardManager.Instance.onGameStart();
        };
        //初始化游戏
        levelOperatorUpdate = null;
        canWin = false;
        solvedProblemCount = 0;
        summonedProblemCount = 0;
        winProblemCount = 0;
        quardrantInTotal = GameInfoLoader.loadQuardantCount();

        //初始化摄像机
        setCamera();

        //准备开始游戏/开始教程
        DOVirtual.DelayedCall(3.5f, () =>
        {
            dm.黑色转场.UI_FadeOut_Event();
            dm.黑色转场.onFadeInOrFadeOut += () =>
            {
                dm.黑色转场.gameObject.SetActive(false);
                setCells();

                if(levelName == "闯关模式")
                {
                    gameStart();
                }
                else
                {
                    if (DataManager.Instance.data == null)
                    {
                        if (tutorial)
                        {
                            loadTutorial();
                        }
                        else gameStart();
                    }
                    else
                    {
                        if ((levelTemp > DataManager.Instance.data.SBlevelTutorialWatched.Count - 1 ? true : !DataManager.Instance.data.SBlevelTutorialWatched[levelTemp]) || tutorial)
                        {
                            loadTutorial();
                        }
                        else
                        {
                            gameStart();
                        }
                    }
                }

            };
        });
    }
    public void gameStart()
    {
        cleanAllGamingEffects();
        Camera.main.GetComponent<FollowMouseTool>().enabled = true;
        GamingUIManager.Instance.cardArea.SetActive(true);
        GamingUIManager.Instance.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        summoningProblem = true;
        GameInfoLoader.loadStartAction()?.Invoke();
        levelOperatorUpdate += GameInfoLoader.loadOperatorUpdateAction();
        TutorialManager.Instance.showTipsAndContinue("/*检测到未知物体正在靠近*/", 3f, false);
    }
    public void restart()
    {
        if (!inGame) return;
        inGame = false;
        levelOperatorUpdate = null;
        var dm = GlobalUIManager.Instance;
        var mm = MainMenuUIManager.Instance;
        var gum = GamingUIManager.Instance;
        BrainPointManager.Instance.brainPoint = 100;
        gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        gum.gameContinue();
        dm.黑色转场.gameObject.SetActive(true);
        dm.黑色转场.UI_FadeIn_Event();
        cleanAllGamingEffects();
        SoundsManager.stopMusic();
        foreach(var en in Utils.findAllEntities())
        {
            Destroy(en.gameObject);
        }
        loadLevel(levelName, difficultF, difficultB, levelTemp);
    }
    public void gameOver(bool isWin = false)
    {
        SoundsManager.stopMusic();
        levelOperatorUpdate = null;
        GamingUIManager.Instance.cardArea.SetActive(false);
        GamingUIManager.Instance.gameContinue();
        GamingUIManager.Instance.pauseUI.SetActive(false);
        CardManager.Instance.cleanCardsOnTrial();
        summoningProblem = false;
        spawnProblemDuration = 999f;

        Camera.main.GetComponent<FollowMouseTool>().enabled = false;
        Camera.main.transform.DOPath(new Vector3[] { new Vector3(7.92f, 6.13f, -10) }, 1.5f);
        Camera.main.transform.DORotate(Vector3.zero, 1.5f);
        Camera.main.GetComponent<FollowMouseTool>().cameraMoveSpeed = 30;
        DOTween.To(() => Camera.main.orthographicSize,x => Camera.main.orthographicSize = x, 6, 1.5f).OnComplete(() =>
        {

            var dt = DataManager.Instance.data;
            var gameOverE = Utils.summonEffectDirectly(EffectType.GameOverEffect, new Vector3(8.21f,5.85f,0)).GetComponent<GameOverEffect>();
            //胜利
            if (isWin)
            {
                gameOverE.cleared();
                Sounds.胜利.play();
                //是否为闯关模式
                if (levelName == "闯关模式")
                {
                    var chapter = int.Parse(difficultF);
                    //第一次通关,解锁下一关且获得奖励.
                    if (dt.currentChapter < chapter)
                    {
                        dt.currentChapter = chapter;
                        dt.currentLevel = 1;
                    }
                    else if (dt.currentLevel <= difficultB)
                    {
                        dt.currentLevel++;
                    }

                    if (levelRewards.Count != 0)
                    {
                        CardManager.Instance.obtainNewCards(levelRewards);
                    }
                    //默认通关次数 + 1
                    dt.barrierFinishedTime[chapter * 5 + difficultB - 1]++;
                }
                else
                {
                    //第一次通关当前难度,获得奖励.
                    if (dt.SBlevelFinishTime[levelTemp] == 0)
                    {
                        if (levelRewards.Count != 0)
                        {
                            CardManager.Instance.obtainNewCards(levelRewards);
                        }
                    }
                    //第一次通关这个难度 解锁下一个难度.
                    if (dt.SBlevelProgess[levelTemp] == difficults.IndexOf(difficultF))
                    {
                        dt.SBlevelProgess[levelTemp]++;
                    }
                    //默认通关次数 + 1
                    dt.SBlevelFinishTime[levelTemp]++;
                }
                DataManager.Instance.savePlayerData();
            }
            //失败
            else
            {
                Sounds.失败.play();
                gameOverE.failed();
            }
            DOVirtual.DelayedCall(3, () =>
            {
                backToMainMenu();
            });
        });
    }
    public void backToMainMenu()
    {
        if (!inGame) return;
        print("back to menu");
        var dm = GlobalUIManager.Instance;
        var mm = MainMenuUIManager.Instance;
        var gum = GamingUIManager.Instance;
        SoundsManager.stopMusic();
        HandManager.Instance.trueMouse.SetActive(true);
        CardManager.Instance.cleanCardsOnTrial();
        inGame = false;
        levelOperatorUpdate = null;
        gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        gum.gameContinue();
        dm.黑色转场.gameObject.SetActive(true);
        dm.黑色转场.UI_FadeIn_Event();
        dm.黑色转场.onFadeInOrFadeOut += () =>
        {
            Camera.main.GetComponent<BlurLayer>().noBlur();
            Camera.main.orthographicSize = 5;
            Camera.main.orthographic = true;
            Camera.main.transform.position = new Vector3(0, 0, -10);
            dm.黑色转场.UI_FadeOut_Event();
            Utils.removeAllBackgrounds();
            Musics.开心地解题.play(true);
            mm.mainUi.SetActive(true);
            if (mm.checkName())
            {
                mm.mainUi.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                mm.systemUI.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                mm.updateDaveSystemNameText();
            }
            dm.黑色转场.onFadeInOrFadeOut += () =>
            {
                gum.mainUi.SetActive(false);
            };
        };
        cleanAllGamingEffects();
        foreach (var l in mm.levelList)
        {
            if (l != null && DataManager.Instance.data != null)l.loadLevelInformations();
        }
        foreach (var en in Utils.findAllEntities())
        {
            Destroy(en.gameObject);
        }
    }
    public void cleanAllGamingEffects()
    {
        foreach (var e in effectsInGame)
        {
            if (e == null) continue;
            Destroy(e.gameObject);
        }
        foreach (var o in objsInGame)
        {
            if (o == null) continue;
            Destroy(o);
        }
        effectsInGame.Clear();
        objsInGame.Clear();
    }
    internal void spawningProblemUpdate()
    {
        if (!summoningProblem || !inGame) return;
        spawnProblemTimer += Time.deltaTime;
        
        if (spawnProblemTimer >= spawnProblemDuration)
        {
            spawnProblemTimer = 0;
            var place = findAvailableSpawningPlace();
            var row = place[0];
            int randomQua = place[1];
            var randomProblemType = problemTypes[UnityEngine.Random.Range(0, problemTypes.Count)];
            spawnRandomProblemByDifficult(difficultF, difficultB, randomProblemType, row, randomQua);
        }
    }
    public ProblemEntity spawnRandomProblemByDifficult(string difficultF,int difficultB,EntityType problemType = EntityType.Problem,int row = 1,int quadrant = 1)
    {
        var problem = Utils.putProblemEntityDirectly(problemType, difficultF,difficultB,row, quadrant);
        //特殊P设置
        switch (problemType)
        {
            case EntityType.NTimeFunctionProblem:
                var f = problem.GetComponent<NTimeFunctionProblemEntity>();
                problem.problemType = ProblemType.Equals;
                if (levelName == "闯关模式")
                {
                    f.functionLens = 2;
                }
                else
                {
                    f.functionLens = (difficults.IndexOf(difficultF) != 0 ? 1 : 0) + 2;
                }
                if(difficults.IndexOf(difficultF) >= 2) f.moveSpeedX = 0.45f;
                break;
            case EntityType.CompositionProblem:
                var c = problem.GetComponent<CompositionProblemEntity>();
                if(levelName == "闯关模式")
                {
                    c.childNumCount = 2;
                }
                else
                {
                    c.childNumCount = (difficults.IndexOf(difficultF) != 0 ? 1 : 0) + 2;
                }
                problem.problemType = ProblemType.Equals;
                if (difficults.IndexOf(difficultF) >= 2) c.moveSpeedX = 0.45f;
                break;
        }
        if (summonedProblemCount >= winProblemCount && levelName != "*REVIVE*")
        {
            canWin = true;
            summoningProblem = false;
        }
        problem.setRandomProblem(difficultF, difficultB,levelName == "闯关模式");
        return problem;
    }
    /// <summary>
    /// 得到随机的生成位置 [0]:行数 [1]:象限 象限>2时 行数为负数形式
    /// 一旦选中的行解题中枢死亡,将会重新选择一行
    /// </summary>
    /// <returns></returns>
    public List<int> findAvailableSpawningPlace()
    {
        List<int> place = new List<int>() { 1,1};

        if (allSCDie()) return place;

        //生成随机行
        var minRowCount = quardrantInTotal > 1 ? -6 : 1;

        int row = UnityEngine.Random.Range(minRowCount, 7);
        if (row == 0)
        {
            return findAvailableSpawningPlace();
        }

        if(rowNumUp[6 - Mathf.Abs(row)] == 0 && row > 0)
        {
            return findAvailableSpawningPlace();
        }
        if(row < 0 && rowNumDown[Mathf.Abs(row) - 1] == 0)
        {
            return findAvailableSpawningPlace();
        }
        place[0] = row;

        //根据行数生成随机象限
        int quadrant = 1;
        if(row > 0 && quardrantInTotal >= 2)
        {
            quadrant = UnityEngine.Random.Range(1,3);
        }
        else if(row < 0 && quardrantInTotal > 2)
        {
            quadrant = UnityEngine.Random.Range(3, quardrantInTotal + 1);
        }
        place[1] = quadrant;

        return place;
    }
    internal bool allSCDie()
    {
        var allZero = true;
        foreach (var i in rowNumDown)
        {
            if (i != 0) allZero = false;
        }
        foreach (var i in rowNumUp)
        {
            if (i != 0) allZero = false;
        }
        return allZero;
    }
    private void loadTutorial()
    {
        GameInfoLoader.loadTutorial()?.Invoke();
    }
    public void playMusic()
    {
        var music = GameInfoLoader.loadMusic();
        if(music != Musics.None) music.play(true);
        else SoundsManager.stopMusic();
    }
    public void onProblemDie(bool isParticipatingInCounting = true)
    {
        var tm = TutorialManager.Instance;
        if (levelName != "*REVIVE*")
        {
            if (isParticipatingInCounting) solvedProblemCount++;
            if (canWin && solvedProblemCount == winProblemCount)
            {
                DOVirtual.DelayedCall(3f, () =>
                {
                    gameOver(true);
                });
            }
        }
        if (tm.needingWaitingProblemByKilled)
        {
            tm.tutorialTemp++;
            tm.needingWaitingProblemByKilled = false;
            switch (tm.chapter)
            {
                case 0:
                    tm.beginnerTutorial();
                    break;
            }
        }
    }
}
public static class GameInfoLoader
{
    public static GameManager gm { get => GameManager.Instance; }
    private static readonly int[] chapterQuardant = new int[]
    {
        1,4
    };
    private static readonly EntityType[][][] barrierProblems = new EntityType[][][]
    {
        new EntityType[][]
        {
            new EntityType[]{ EntityType.Problem },
            new EntityType[]{ EntityType.CompositionProblem,EntityType.Problem },
            new EntityType[]{ EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.Problem},
            new EntityType[]{ EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.Problem,EntityType.SegmentRelationshipProblem},
            new EntityType[]{ EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.Problem,EntityType.SegmentRelationshipProblem},
        },
        new EntityType[][]
        {
            new EntityType[]{ EntityType.Problem },
            new EntityType[]{ EntityType.CompositionProblem,EntityType.Problem },
            new EntityType[]{ EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.Problem},
            new EntityType[]{ EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.Problem,EntityType.SegmentRelationshipProblem},
            new EntityType[]{ EntityType.BossZCube},
        },
    };
    public static List<EntityType> loadProblemTypes()
    {
        List<EntityType> result = new List<EntityType>() { EntityType.Problem};
        var levelName = gm.levelName;

        //专项训练
        switch (levelName)
        {
            case "*REVIVE*":
                result = new List<EntityType>() { EntityType.Problem };
                break;
            case "与四则共舞":
                result = new List<EntityType>() { EntityType.CompositionProblem };
                break;
            case "走丢的X":
                result = new List<EntityType>() { EntityType.NTimeFunctionProblem };
                break;
            case "“垂平”之道":
                result = new List<EntityType>() { EntityType.SegmentRelationshipProblem };
                break;
        }
        if(levelName == "闯关模式")
        {
            var chapter = int.Parse(gm.difficultF);
            var level = gm.difficultB;
            result = barrierProblems[chapter][level - 1].ToList();
        }
        return result;
    }
    public static Action loadTutorial()
    {
        var tm = TutorialManager.Instance;
        Action a = null;
        tm.tutorialTemp = 0;
        var levelName = gm.levelName;
        switch (levelName)
        {
            case "*REVIVE*":
                a += () => tm.beginnerTutorial();
                break;
            case "与四则共舞":
                a += () => tm.danceWithTFFO();
                break;
            case "走丢的X":
                a += () => tm.lostX();
                break;
            case "“垂平”之道":
                a += () => tm.theWayOfPV();
                break;
        }
        return a;
    }
    public static Action loadStartAction()
    {
        Action a = null;
        var levelName = gm.levelName;
        a += () =>
        {
            //专项训练
            switch (levelName)
            {
                case "*REVIVE*":
                    DOVirtual.DelayedCall(15f, () =>
                    {
                        gm.difficultB = 1;
                    });
                    DOVirtual.DelayedCall(30f, () =>
                    {
                        gm.difficultB = 2;
                        gm.spawnProblemDuration = 4;
                    });
                    DOVirtual.DelayedCall(45f, () =>
                    {
                        gm.difficultB = 3;
                        gm.spawnProblemDuration = 3;
                    });
                    DOVirtual.DelayedCall(60f, () =>
                    {
                        TutorialManager.Instance.needingWaitingWin = true;
                        gm.summoningProblem = false;
                    });
                    break;
                case "与四则共舞":
                    gm.spawnProblemDuration = 15f;
                    gm.winProblemCount = 20;
                    DOVirtual.DelayedCall(30f, () =>
                    {
                        gm.spawnProblemDuration = 13f;
                    });
                    DOVirtual.DelayedCall(60f, () =>
                    {
                        gm.spawnProblemDuration = 12f;
                    });
                    break;
                case "走丢的X":
                    gm.winProblemCount = 20;
                    gm.spawnProblemDuration = 7f;
                    DOVirtual.DelayedCall(35f, () =>
                    {
                        gm.spawnProblemDuration = 6f;
                    });
                    DOVirtual.DelayedCall(45f, () =>
                    {
                        gm.spawnProblemDuration = 5f;
                    });
                    break;
                case "“垂平”之道":
                    gm.winProblemCount = 20;
                    gm.spawnProblemDuration = 15f;
                    DOVirtual.DelayedCall(35f, () =>
                    {
                        gm.spawnProblemDuration = 13f;
                    });
                    DOVirtual.DelayedCall(75f, () =>
                    {
                        gm.spawnProblemDuration = 12f;
                    });
                    break;
            }
            if(levelName == "闯关模式")
            {
                var chapter = int.Parse(gm.difficultF);
                var level = gm.difficultB;

                gm.spawnProblemDuration = 20f - level;
                gm.winProblemCount = 10 + level * 2 ;

                if(isBossBarrier())
                {
                    gm.winProblemCount = 1;
                    gm.spawnProblemDuration = 2;
                }
                else
                {
                    DOVirtual.DelayedCall(30f, () =>
                    {
                        gm.spawnProblemDuration -= 2;
                    });
                    DOVirtual.DelayedCall(60f, () =>
                    {
                        gm.spawnProblemDuration -= 2;
                    });
                }
            }
        };
        return a;
    }
    public static Action loadOperatorUpdateAction()
    {
        Action a = null;
        a += () =>
        {
            gm.spawningProblemUpdate();
            if (gm.allSCDie())
            {
                //全部SC损坏,游戏失败
                gm.gameOver(false);
            }
        };
        return a;
    }
    public static Musics loadMusic()
    {
        var accurateMusic = Musics.REVIVE;
        var levelName = gm.levelName;
        switch (levelName)
        {
            case "与四则共舞":
                accurateMusic = Musics.与四则共舞;
                break;
            case "走丢的X":
                accurateMusic = Musics.LoseX;
                break;
            case "*REVIVE*":
                accurateMusic = Musics.REVIVE;
                break;
            case "“垂平”之道":
                accurateMusic = Musics.逃离几何;
                break;
            case "闯关模式":
                accurateMusic = isBossBarrier() ? Musics.None : Musics.闯关模式;
                break;
        }
        
        return accurateMusic;
    }
    public static int loadQuardantCount()
    {
        if (gm.levelName == "闯关模式")
        {
            var chapter = int.Parse(gm.difficultF);
            return chapterQuardant[chapter];
        }
        return 1;
    }
    public static List<CardType> loadCardsInTrial()
    {
        var levelName = gm.levelName;
        var result = new List<CardType>();
        //专项训练
        switch (levelName)
        {
            case "*REVIVE*":
                result = new List<CardType>() { CardType.PlusCard, CardType.SubstractCard, CardType.MultiplyCard,CardType.DevisionCard };
                break;
            case "与四则共舞":
                result = new List<CardType>() { CardType.OppositeNumberCard ,CardType.ReciprocalCard,CardType.RoundDownCard};
                break;
            case "走丢的X":
                result = new List<CardType>() { CardType.DerivativesCard };
                break;
            case "“垂平”之道":
                result = new List<CardType>() { CardType.PointCard };
                break;
        }
        return result;
    }
    public static void loadExtraCameraSettings()
    {
        if(gm.levelName == "闯关模式")
        {
            var chapter = int.Parse(gm.difficultF);
            var level = gm.difficultB;
            if (chapter == 1 && level == 5)
            {
                Camera.main.orthographic = false;
                Camera.main.fieldOfView = 60;
                Camera.main.transform.position = new Vector3(0, -4.5f, -20);
                Camera.main.GetComponent<FollowMouseTool>().enabled = false;
                Camera.main.GetComponent<FollowMouseTool>().cameraMoveSpeed = 50;
            }
        }
    }
    private static bool isBossBarrier()
    {
        var chapter = int.Parse(gm.difficultF);
        var level = gm.difficultB;

        return chapter == 1 && level == 5;
    }
}
