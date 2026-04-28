using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    private static EventManager em { get => EventManager.Instance; }
    private List<Tween> timer = new List<Tween>();
    public GameObject arrowObj;
    public Canvas fixedUICanvas;
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI tipShadow;
    public GameObject x_axis;
    public GameObject y_axis;
    public int chapter = 0;
    public int level = 0;
    public int tutorialTemp = 0;

    public bool inTutorial = false;

    //码的 一堆屎山 迟早换了你们
    public bool needingShootingShooterBullet = false;
    public bool needingClickingTheBlueBar = false;
    public bool needingUsingPlusCard = false;
    public bool needingBulletHitThePlusEntity = false;
    public bool needingWaitingProblemByKilled = false;
    public bool needingWaitingSCBroken = false;
    public bool needingWaitingWin = false;

    public bool needingAddCell13LWTo100000 = false;
    public bool needingAddCell13LWDTo100000 = false;
    public bool needingExportAnyCellNum = false;
    public bool needingImportAnyCellNum = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (inTutorial)
        {
            if (GameManager.Instance.levelName == "*REVIVE*")
            {
                if(Utils.findAllEntities(EntityGroup.Enemy).Count == 0 && needingWaitingWin)
                {
                    needingWaitingWin = false;
                    tutorialTemp++;
                    beginnerTutorial();
                }
            }
        }
    }
    #region 教程大全
    public void beginnerTutorial()
    {
        inTutorial = true;
        var cm = CameraManager.Instance;
        var blur = cm.mainCamera.GetComponent<BlurLayer>();
        var gm = GameManager.Instance;
        var gum = GamingUIManager.Instance;
        switch (tutorialTemp)
        {
            case 0:
                lockTheMouse();
                gm.inGame = true;
                gum.cardArea.SetActive(false);
                gm.summoningProblem = false;
                showTipsAndContinue("欢迎来到PVZ-MATH.",2);
                break;
            case 1:
                showTipsAndContinue("不要感到陌生.",1);
                break;
            case 2:
                showTipsAndContinue("这里的一切只是你第一印象的具现化.",3);
                break;
            case 3:
                blur.noBlur();
                showTipsAndContinue("请容许我对这里的一切做简单的介绍.",3);
                break;
            case 4:
                gum.mainUi.SetActive(true);
                gum.cardArea.SetActive(true);
                gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                showTipsAndContinue("这是你的脑力(BP).", 3);
                showArrow(gum.brainPointBar.gameObject.GetUIWorldPosition(), Vector3.zero);
                break;
            case 5:
                showTipsAndContinue("BP值会随着时间回复,没有上限.", 3);
                break;
            case 6:
                showTipsAndContinue("你也可通过使用其他方式快速回复BP.", 3);
                break;
            case 7:
                hideArrow();
                showTipsAndContinue("它的作用很多,之后会一一介绍.", 3);
                break;
            case 8:
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("相信你已经注意到这个背包了.", 2);
                break;
            case 9:
                showTipsAndContinue("背包存储着你的卡牌和道具,左键使用卡牌放置想法(T),要消耗相应的脑力(BP).\n你可以右键卡牌/道具给其设置快捷键.", 7);
                break;
            case 10:
                hideArrow();
                showTipsAndContinue("我这里给你预设了一套快捷键\n若用不惯你可以修改.", 4);
                break;
            case 11:
                showTipsAndContinue("左边的6个物体是解题中枢(SC)\n同时是我们的保护目标.", 3);
                break;
            case 12:
                hideArrow();
                showTipsAndContinue("SC与特殊的卡牌配合\n能够帮助我们更改地块(L)权重(LW)", 3);
                break;
            case 13:
                Action a13 = () =>
                {
                    showArrow(new Vector3(3.22f, 8.2f, 0), new Vector3(0, 0, -90));
                    showTipsAndContinue("这些格子就是我们的地块(L)", 4, true, true, 0, -428.8f,2);
                };
                gum.cardArea.SetActive(false);
                cm.changeSize(9, 2, a13);
                break;
            case 14:
                hideArrow();
                var s1 = Utils.putThinkEntityDirectly(EntityType.Plus, 26);
                var s2 = Utils.putThinkEntityDirectly(EntityType.Substract, 27);
                var s3 = Utils.putThinkEntityDirectly(EntityType.Multiply, 28);
                var s4 = Utils.putThinkEntityDirectly(EntityType.Divide, 29);
                Action a14 = () =>
                {
                    s1.entityDie();
                    s2.entityDie();
                    s3.entityDie();
                    s4.entityDie();
                };
                showTipsAndContinue("想法(T)能够放置在地块(L)上", 3,true,false,0,0,2,a14);
                break;
            case 15:
                showAllCellsNumNoReason();
                showTipsAndContinue("每个地块都有自己的权重(LW),地块的初始LW为0.", 4);
                break;
            case 16:
                Action a16 = () =>
                {
                    showAllCellsNumNeedReason();
                    showTipsAndContinue("通常情况下,LW = 0的地块不会显示其LW.", 3);
                };
                cm.changeSize(6, 2, a16);
                break;
            case 17:
                showArrow(new Vector3(0, 8.18f, 0), new Vector3(0, 0, 90));
                showTipsAndContinue("如何让解题中枢(SC)发射子弹?", 3,true,true);
                break;
            case 18:
                showTipsAndContinue("只需点击对应的SC,消耗1点BP即可发射.", 3);
                break;
            case 19:
                needingShootingShooterBullet = true;
                unlockTheMouse();
                showTipsAndContinue("现在,尝试一下让第一个解题中枢发射子弹吧.", 4,false);
                break;
            case 20:
                lockTheMouse();
                hideArrow();
                showTipsAndContinue("你或许注意到,子弹上面有个数字(BN = 1).", 3);
                break;
            case 21:
                showArrow(new Vector3(0.8f, 8.18f, 0), new Vector3(0, 0, 90));
                showTipsAndContinue("注意看,SC的后方有一个蓝条.", 2);
                break;
            case 22:
                unlockTheMouse();
                needingClickingTheBlueBar = true;
                showTipsAndContinue("蓝条内有一个白杠,则代表1,请左键点击一下.", 5, false);
                break;
            case 23:
                hideArrow();
                needingShootingShooterBullet = true;
                showTipsAndContinue("蓝条的白杠增加了,请让第一个解题中枢发射子弹.", 5, false);
                break;
            case 24:
                lockTheMouse();
                showTipsAndContinue("观察子弹上的数字(BN),相信你发现了规律.", 3);
                break;
            case 25:
                showTipsAndContinue("白条的最小数量为1,最大为5.", 3);
                break;
            case 26:
                showTipsAndContinue("也就是说你可以发射五种数字的子弹.\n(BN E {1,2,3,4,5})\n左键BN+1,右键BN-1.", 5);
                break;
            case 27:
                needingUsingPlusCard = true;
                unlockTheMouse();
                gum.cardArea.SetActive(true);
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("现在,请尝试放置第一张带加号的卡牌.", 7,false);
                break;
            case 28:
                needingBulletHitThePlusEntity = true;
                hideArrow();
                gum.cardArea.SetActive(false);
                showTipsAndContinue("点击同行的SC,发射子弹.", 7,false);
                break;
            case 29:
                lockTheMouse();
                showTipsAndContinue("仔细看(加号)所在的格子的权重,发生了改变.", 4);
                break;
            case 30:
                showTipsAndContinue("权重改变的公式:\nLW_NEW = BN [+/-/*/÷] LW \n符号为格子上放置的符号想法(ST)", 6);
                break;
            case 31:
                needingWaitingProblemByKilled = true;
                summonAThreeCellAndAThreeEqualingProblemEntity();
                showTipsAndContinue("请仔细观察第一行......", 7,false);
                break;
            case 32:
                showTipsAndContinue("这个形似鬼魂的物体是难题(P)\n也就是你需要消灭的敌人.", 5);
                break;
            case 33:
                showTipsAndContinue("P身上的(=3)\n说明需要权重为3的格子来杀死它.", 5);
                break;
            case 34:
                showTipsAndContinue("你没有看错\n只要碰到权重为3的格子\n它就会被消灭.", 5);
                break;
            case 35:
                blur.blur();
                showTipsAndContinue("难题的种类有许多\n你需要更改合适的格子权重来消灭它们.", 5);
                break;
            case 36:
                showTipsAndContinue("难题(P)可被想法(T)阻挡,但P会攻击T,导致T消失.", 7);
                break;
            case 37:
                showTipsAndContinue("同样地,P也会攻击SC,导致其损坏.", 3);
                break;
            case 38:
                blur.noBlur();
                needingWaitingSCBroken = true;
                summonACantBeatProblemEntityAndASymbolEntity();
                showTipsAndContinue(" ", 7,false);
                break;
            case 39:
                removeAllEntitiesExceptSC();
                showTipsAndContinue("看来第一行的SC被摧毁了.", 4);
                break;
            case 40: 
                showTipsAndContinue("不必担心,这不会导致游戏失败.", 4);
                break;
            case 41:
                blur.blur();
                showTipsAndContinue("被摧毁的SC将无法使用\n这一行将不会出现新的难题(P).", 5);
                break;
            case 42:
                showTipsAndContinue("但是,原本这行要出现的P会被其他行分担.", 4);
                break;
            case 43:
                showTipsAndContinue("因此,我们要避免损失任何SC.", 4);
                break;
            case 44:
                showTipsAndContinue("一旦全部SC被摧毁,游戏将失败.", 4);
                break;
            case 45:
                blur.noBlur();
                gm.spawnProblemDuration = 5;
                gm.summoningProblem = true;
                gm.gameStart();
                showTipsAndContinue("你需要抵抗到来的所有难题(P)\n消灭他们即可获得胜利.", 6);
                break;
            case 46:
                gum.cardArea.SetActive(true);
                showTipsAndContinue("忘记提醒你了.\n右上角显示的是当前难度\n它由两部分组成", 5f);
                break;
            case 47:
                showArrow(gum.fakeEraser.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("这是橡皮擦\n如果你有不想要的想法(T)\n可使用橡皮擦将其擦除.", 7);
                break;
            case 48:
                hideArrow();
                unlockTheMouse();
                showTipsAndContinue("让我们将知识组合到一起\n实战一下吧.", 5,false);
                GameInfoLoader.loadStartAction()?.Invoke();
                break;
            case 49:
                blur.blur();
                showTipsAndContinue("恭喜你通过了最简单的新手教程.", 4);
                break;
            case 50:
                showTipsAndContinue("未来还有更多挑战等着你!", 3.5f);
                break;
            case 51:
                inTutorial = false;
                gm.backToMainMenu();
                DataManager.Instance.data.completedNewPlayerTutorial = true;
                DataManager.Instance.savePlayerData();
                break;
        }
    }
    public void danceWithTFFO()
    {
        inTutorial = true;
        var cm = CameraManager.Instance;
        var blur = cm.mainCamera.GetComponent<BlurLayer>();
        var gum = GamingUIManager.Instance;
        var cell13 = Utils.findCellById(13);
        switch (tutorialTemp)
        {
            case 0:
                hideArrow();
                blur.noBlur();
                showTipsAndContinue("接下来,你需要使用四则运算的T来完成这关.", 4);
                break;
            case 1:
                gum.cardArea.SetActive(true);
                gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("你可能会遇见一些特殊情况...\n我在你背包新增了三个有用的T.", 6);
                break;
            case 2:
                showTipsAndContinue("之后你可以试用一下.", 2);
                break;
            case 3:
                cell13.setNum(100000);
                needingAddCell13LWTo100000 = true;
                showArrow(cell13.GetComponent<BoxCollider2D>().bounds.center, Vector3.zero);
                showTipsAndContinue("现在,请增加该地块的LW.", 4f,false);
                break;
            case 4:
                hideArrow();
                showTipsAndContinue("奇怪?为什么LW不增加呢?", 3f);
                break;
            case 5:
                showTipsAndContinue("系统已将LW限制在一定范围内.\nLW E [-100000,100000]", 5f);
                break;
            case 6:
                showTipsAndContinue("这为了你的安全着想,也可以避免无意义的运算.", 4f);
                break;
            case 7:
                showTipsAndContinue("之前的教程里,或许你感到疑惑\n为什么LW没有小数呢?", 5f);
                break;
            case 8:
                showTipsAndContinue("现在,我们将引入分数的LW.", 3f);
                break;
            case 9:
                showTipsAndContinue("同样地,系统也为LW的显示设置了一定限制.", 4f);
                break;
            case 10:
                showTipsAndContinue("小数LW不会直接显示\n它们往往会被L转化为一些好看的分数.", 6f);
                break;
            case 11:
                showArrow(cell13.GetComponent<BoxCollider2D>().bounds.center, Vector3.zero);
                cell13.numerator = 2f;
                cell13.denominator = 3f;
                showTipsAndContinue("例如,我令这个格子的LW = 0.667\n它LW便被转化为了(2 / 3,二分之三);", 6f);
                break;
            case 12:
                hideArrow();
                showTipsAndContinue("不用担心,系统会避免小数条件的P生成\n你将不会为它们做丑陋的低级运算.", 6f);
                break;
            case 13:
                showArrow(cell13.GetComponent<BoxCollider2D>().bounds.center, Vector3.zero);
                cell13.numerator = 123f;
                cell13.denominator = 100000f;
                needingAddCell13LWDTo100000 = true;
                showTipsAndContinue("现在,请使用除法增加该地块的LW的分母(d).", 3f, false);
                break;
            case 14:
                hideArrow();
                showTipsAndContinue("嗯?LW居然归0了.", 3f);
                break;
            case 15:
                showTipsAndContinue("我们规定,LW的分子(n)或分母(d)越过界限\nn,d  E [-100000,100000]\nLW便会发生改变.", 6f);
                break;
            case 16:
                showTipsAndContinue("新的LW值等于n/d四舍五入后得到的整数.", 4f);
                break;
            case 17:
                showTipsAndContinue("似乎可以利用这个特性,来达成某些目的?", 4f);
                break;
            case 18:
                showTipsAndContinue("还有个事情你必须知道.", 3f);
                break;
            case 19:
                showTipsAndContinue("那就是LW是可以*导出*和*导入*的.", 4f);
                break;
            case 20:
                showTipsAndContinue("只需*右键*你想操作的L即可.", 3f);
                break;
            case 21:
                needingExportAnyCellNum = true;
                foreach(var cell in Utils.findAllCells())
                {
                    cell.setNum(1f);
                }
                showTipsAndContinue("请尝试右键导出任意L的LW.", 5f,false);
                break;
            case 22:
                showTipsAndContinue("看来,你成功导出了.", 2f);
                break;
            case 23:
                needingImportAnyCellNum = true;
                showTipsAndContinue("现在,请右键*含任意符号想法(ST)*的任意L,然后点击导入.", 6f, false);
                break;
            case 24:
                showTipsAndContinue("仔细看,被导出L的LW似乎被转移了.", 5f);
                break;
            case 25:
                foreach (var cell in Utils.findAllCells())
                {
                    cell.setNum(0);
                }
                showTipsAndContinue("公式:\nLW1_NEW = LW1_OLD [+/-/x/÷] LW2_导出\nLW1和LW2可相等", 6f);
                break;
            case 26:
                showTipsAndContinue("有了这个新工具\n相信你一定能很快获得一些特殊数字.", 5f);
                break;
            case 27:
                showTipsAndContinue("新的一波难题即将到来,请准备战斗.", 4f);
                stopTutorialAndGameStart();
                break;
        }
    }
    public void theWayOfPV()
    {
        inTutorial = true;
        var cm = CameraManager.Instance;
        var blur = cm.mainCamera.GetComponent<BlurLayer>();
        var gum = GamingUIManager.Instance;
        switch (tutorialTemp)
        {
            case 0:
                hideArrow();
                blur.noBlur();
                gum.cardArea.SetActive(true);
                gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                showTipsAndContinue("是时候来一点几何了.", 2);
                break;
            case 1:
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("我在你的背包新增了一张Point想法", 3);
                break;
            case 2:
                showArrow(gum.fakePencil.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("战场中,你可以使用铅笔将两个Point连接.", 4);
                break;
            case 3:
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("现在,请放置两个Point想法,然后用铅笔将它们链接吧!\n选中铅笔,依次点击你要连接的两个点.", 6, false);
                em.drawSegmentEvent += () =>
                {
                    tutorialTemp++;
                    theWayOfPV();
                    em.drawSegmentEvent = null;
                };
                break;
            case 4:
                showTipsAndContinue("很好,你已经会画线段了.", 2);
                break;
            case 5:
                hideArrow();
                removeAllEntitiesExceptSC();
                drawTwoSegmentsAndASP();
                showTipsAndContinue("接下来请仔细观战.", 5,false);
                em.segmentProblemDieEvent += () =>
                {
                    tutorialTemp++;
                    theWayOfPV();
                    em.segmentProblemDieEvent = null;
                };
                break;
            case 6:
                showTipsAndContinue("只见胸口写着[V] = 1的P碰到线段后就死亡了.", 5);
                break;
            case 7:
                showTipsAndContinue("稍加观察便会发现:\n它碰到的线段在场上\n有一个与它垂直的线段,\n因此这条线段的V = 1.", 8);
                break;
            case 8:
                showTipsAndContinue("同理,当场上有n条线段形成的直线与它平行时,\n这条线段的P = n.\n这里我就不做演示了.", 7);
                break;
            case 9:
                showTipsAndContinue("我们称这种P为线段关系难题(SRP).", 4);
                break;
            case 10:
                showTipsAndContinue("杀死SRP的方式就是让其碰到满足其胸前条件[P/V值]的线段.", 6);
                break;
            case 11:
                showTipsAndContinue("恭喜你学会了这个玩法,准备战斗吧.\n * ^_^ *", 3);
                stopTutorialAndGameStart();
                break;
        }
    }
    public void lostX()
    {
        inTutorial = true;
        var cm = CameraManager.Instance;
        var blur = cm.mainCamera.GetComponent<BlurLayer>();
        var gum = GamingUIManager.Instance;
        switch (tutorialTemp)
        {
            case 0:
                hideArrow();
                blur.noBlur();
                gum.cardArea.SetActive(false);
                showTipsAndContinue("其实,在这个屏幕里,存在着一套坐标轴.", 4);
                break;
            case 1:
                Action a1 = () =>
                {
                    x_axis.SetActive(true);
                    y_axis.SetActive(true);
                    showTipsAndContinue("你现在看到的是X轴和Y轴.", 3, true, true, 0, -428.8f, 2);
                };
                cm.changeSize(9, 2, a1);
                break;
            case 2:
                showTipsAndContinue("请注意两轴的朝向.", 3);
                break;
            case 3:
                showTipsAndContinue("每个地块(L)可以看做是一个点\n每个L都有属于自己的坐标(X,Y)", 5);
                break;
            case 4:
                showTipsAndContinue("L的X坐标值从左向右增加.\n我帮你标出了部分格子的X轴坐标.", 5);
                foreach (var cell in Utils.findAllCells())
                {
                    if (((float)cell.cellId).numInRange(56, 66))
                    {
                        cell.setNum(cell.cellId - 55);
                    }
                }
                break;
            case 5:
                showTipsAndContinue("目前的战斗只发生在第一象限\n未来的战斗或许会扩展到多个象限\n甚至提升维度.", 7);
                break;
            case 6:
                y_axis.SetActive(false);
                showTipsAndContinue("不必害怕,我们先从X开始.\n这关我们暂时用不到Y轴,所以我隐藏了Y轴.", 6);
                break;
            case 7:
                foreach (var cell in Utils.findAllCells())
                {
                    if (((float)cell.cellId).numInRange(56, 66))
                    {
                        cell.setNum(0);
                    }
                }
                showTipsAndContinue("需要注意的是,L的坐标和任意坐标轴不会显示\n因此你要学会自己判断.", 5);
                break;
            case 8:
                x_axis.SetActive(false);
                cm.changeSize();
                showTipsAndContinue("接下来你将面对新的难题(P)\nn次幂函数难题(NFP).", 5,true, true);
                break;
            case 9:
                gum.cardArea.SetActive(true);
                gum.cardArea.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
                showArrow(gum.bag.gameObject.GetUIWorldPosition(), Vector3.zero);
                showTipsAndContinue("考虑到计算比较麻烦\n我在你背包新增了一张导数想法(T).", 5);
                break;
            case 10:
                hideArrow();
                showTipsAndContinue("它能将触碰到的NFP进行n阶导数,n = |[LW]|.", 5);
                break;
            case 11:
                showTipsAndContinue("利用你学到的知识,准备战斗吧.\n * ^_^ *", 3);
                stopTutorialAndGameStart();
                break;
        }
    }
    #endregion
    public void showTipsAndContinue(string txt, float showTime = 3,bool isContinue = true, bool move = false,float X = 0, float Y = 0, float duration = 2,Action onHide = null)
    {
        for (var i = 0; i < this.timer.Count; i++)
        {
            var time = this.timer[i];
            if (time == null) continue;
            time.Kill();
            this.timer.Remove(time);
        }
        tipText.text = txt;
        tipShadow.text = txt;
        var tipF = tipText.GetComponent<UI_FadeInFadeOut>();
        tipF.UI_FadeIn_Event();
        if (move) tipText.GetComponent<RectTransform>().DOAnchorPos(new Vector2(X, Y), duration);
        var timer = DOVirtual.DelayedCall(showTime, () =>
        {
            tipF.UI_FadeOut_Event();
            var timer2 = DOVirtual.DelayedCall(2f, () =>
            {
                onHide?.Invoke();
                if (isContinue)
                {
                    tutorialTemp++;
                    nextTip();
                }
            });
            this.timer.Add(timer2);
        });
        this.timer.Add(timer);
    }
    private void showArrow(Vector3 aimPos,Vector3 eulerAngle)
    {
        arrowObj.transform.position = aimPos;
        arrowObj.transform.localEulerAngles = eulerAngle;
        arrowObj.SetActive(true);
        arrowObj.GetComponent<Animator>().SetBool("Appear",true);
    }
    private void hideArrow()
    {
        arrowObj.SetActive(false);
    }
    private void removeAllEntitiesExceptSC()
    {
        foreach(var e in Utils.findAllEntities())
        {
            if(e == null || e.entityType == EntityType.Shooter) continue;
            e.entityDie();
        }
    }
    private void showAllCellsNumNoReason()
    {
        foreach(var cell in Utils.findAllCells())
        {
            cell.showingTextNoReason = true;
        }
    }
    private void showAllCellsNumNeedReason()
    {
        foreach (var cell in Utils.findAllCells())
        {
            cell.showingTextNoReason = false;
        }
    }
    private void summonAThreeCellAndAThreeEqualingProblemEntity()
    {
        var cell = Utils.findCellById(8);
        cell.setNum(3);
        var gm = GameManager.Instance;
        var problem = Utils.putProblemEntityDirectly(EntityType.Problem, gm.difficultF, gm.difficultB, 6,1);
        problem.num = 3;
        problem.problemType = ProblemType.Equals;
    }
    private void summonACantBeatProblemEntityAndASymbolEntity()
    {
        var gm = GameManager.Instance;
        var problem = Utils.putProblemEntityDirectly(EntityType.Problem, gm.difficultF, gm.difficultB, 6, 1);
        problem.num = 9999;
        problem.problemType = ProblemType.Equals;
        problem.moveSpeedX = 2;
        problem.damage *= 3;
        Utils.putThinkEntityDirectly(EntityType.Substract, 2);
    }
    private void drawTwoSegmentsAndASP()
    {
        var gm = GameManager.Instance;

        var p1 = Utils.putThinkEntityDirectly(EntityType.PointThink, 10).GetComponent<PointEntity>();
        var p2 = Utils.putThinkEntityDirectly(EntityType.PointThink, 65).GetComponent<PointEntity>();

        var p3 = Utils.putThinkEntityDirectly(EntityType.PointThink, 23).GetComponent<PointEntity>();
        var p4 = Utils.putThinkEntityDirectly(EntityType.PointThink, 33).GetComponent<PointEntity>();

        var s1 = Utils.spawnASegment(p1, p2);
        var s2 = Utils.spawnASegment(p3, p4);

        var sp = Utils.putProblemEntityDirectly(EntityType.SegmentRelationshipProblem, gm.difficultF, gm.difficultB, 2, 1).GetComponent<SegmentRelationshipProblemEntity>();
        sp.problemType = ProblemType.Equals;
        sp.verticalSegmentCount = 1;
        sp.parallelSegmentCount = 0;
    }
    private void lockTheMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        HandManager.Instance.trueMouse.SetActive(false);
        if (GlobalUIManager.Instance.bagIsOpened)
        {
            GlobalUIManager.Instance.closeBag();
        }
    }
    private void unlockTheMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        HandManager.Instance.trueMouse.SetActive(true);
    }
    private void nextTip()
    {
        var gm = GameManager.Instance;
        switch (gm.levelName)
        {
            case "*REVIVE*":
                beginnerTutorial();
                break;
            case "与四则共舞":
                danceWithTFFO();
                break;
            case "走丢的X":
                lostX();
                break;
            case "“垂平”之道":
                theWayOfPV();
                break;
        }
    }
    private void stopTutorialAndGameStart()
    {
        var gm = GameManager.Instance;
        var dat = DataManager.Instance.data;
        inTutorial = false;
        if(gm.levelName != "闯关模式")
        {
            dat.SBlevelTutorialWatched[gm.levelTemp] = true;
        }
        gm.gameStart();
    }
}
