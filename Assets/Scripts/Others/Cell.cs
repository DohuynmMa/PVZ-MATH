using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts.Utils;
using DG.Tweening;
public class Cell : MonoBehaviour
{
    public int cellId;
    public float num;
    public float numerator;
    public float denominator;
    public TextMeshPro numText;
    public Entity currentEntity;
    public bool showingTextNoReason = false;
    public int quadrant = 1;
    public List<Segment> passedThroughSegments = new List<Segment>();
    private void Update()
    {
        updateNumText();
        updateFractionInfo();
        clickUpdate();
    }
    private void clickUpdate()
    {
        var hm = HandManager.Instance;
        if (RightClicked())
        {
            if (hm.isImportingOrExportingCellNum() || hm.currentEntity != null || hm.usingEraser || hm.usingPencil || GlobalUIManager.Instance.bagIsOpened)
            {
                hm.hidePlacePoint();
                GamingUIManager.Instance.rightClickMenu.SetActive(false);
                return;
            }
            hm.showPlacePoint(this);
            GamingUIManager.Instance.rightClickMenu.SetActive(true);
            GamingUIManager.Instance.rightClickMenu.transform.position = GetComponent<BoxCollider2D>().bounds.center;
            hm.selectedCell = this;
        }
        if (LeftClicked())
        {
            if (hm.currentEntity != null)
            {
                hm.useCard(this);
            }
        }
    }
    public void setNum(float num)
    {
        addNum(num - this.num);
    }
    public void addNum(float num)
    {
        numerator += denominator * num;
    }
    public void updateNumText()
    {
        if (numText == null) return;
        fixTheNumAndshowTheAccurateNumText();
        if(showingTextNoReason) numText.text = num.ToString();
        var accuratePos = Vector3.zero;
        if (currentEntity == null)
        {
            numText.fontSizeMax = 10;
            numText.alignment = TextAlignmentOptions.Center;
            numText.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        }
        else
        {
            numText.fontSizeMax = 4;
            numText.alignment = TextAlignmentOptions.Right;
            if (isFractionLWL())
            {
                accuratePos.x += 0.34f;
                accuratePos.y -= 0.425f;
                numText.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0.5f);
            }
            else
            {
                accuratePos.x -= 0.03f;
                accuratePos.y -= 0.4f;
                numText.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
            }
        }
        numText.GetComponent<RectTransform>().localPosition = accuratePos;
    }
    private void updateFractionInfo()
    {
        num = numerator / denominator;
        if (isFractionLWL())
        {
            var fs = Utils.simplifyFraction(numerator, denominator);
            numerator = fs[0];
            denominator = fs[1];
            return;
        }
        var f = num.floatToFraction();
        numerator = f[0];
        denominator = f[1];
    }
    private void fixTheNumAndshowTheAccurateNumText()
    {
        var tm = TutorialManager.Instance;
            //--开始校正LW--

        //防止越界
        if (!num.numInRange(-100000, 100000))
        {
            num = num.whatsTheNumNear(-100000, 100000);
            if (tm.needingAddCell13LWTo100000 && cellId == 13)
            {
                tm.needingAddCell13LWTo100000 = false;
                tm.tutorialTemp++;
                tm.danceWithTFFO();
            }
        }
        //防止出现小数,自动转换为临近的分数
        if (num.numIsFloat())
        {
            updateFractionInfo();
            //防止分数数字太丑,太丑的话就直接化为整数
            var fs = numerator / denominator;
            if (!denominator.numInRange(-100000, 100000) || !numerator.numInRange(-100000, 100000))
            {
                if (tm.needingAddCell13LWDTo100000 && cellId == 13 && !denominator.numInRange(-100000, 100000))
                {
                    tm.needingAddCell13LWDTo100000 = false;
                    tm.tutorialTemp++;
                    tm.danceWithTFFO();
                }
                num = Mathf.Round(fs);
                numerator = num;
                denominator = 1;
            }
        }
        
            //--开始改变LW文本--

        //是否为分数显示模式
        if (isFractionLWL())
        {
            numText.text = Utils.floatToFractionStringDirectly(numerator, denominator);
        }
        else
        {
            var s = num.ToString().Split(".");
            var n = s[0];
            numText.text = num != 0 ? n : "";
        }
    }
    public bool isFractionLWL()
    {
        return denominator != 0 && denominator != 1;
    }

    private bool IsClicked(int button)
    {
        if (Input.GetMouseButtonDown(button))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float planeZ = 0f;
            float t = (planeZ - ray.origin.z) / ray.direction.z;
            Vector3 worldPoint = ray.origin + ray.direction * t;
            Vector2 point = new Vector2(worldPoint.x, worldPoint.y);
            Collider2D hitCollider = Physics2D.OverlapPoint(point);
            return hitCollider != null && hitCollider.gameObject == gameObject;
        }
        return false;
    }

    // 右键点击检测
    private bool RightClicked()
    {
        return IsClicked(1); // 1表示右键
    }

    // 左键点击检测
    private bool LeftClicked()
    {
        return IsClicked(0); // 0表示左键
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //检测是否被线段穿过
        if (collision.gameObject.GetComponent<Segment>() != null)
        {
            var segment = collision.gameObject.GetComponent<Segment>();
            if (!passedThroughSegments.Contains(segment))
            {
                passedThroughSegments.Add(segment);
                for (int i = 0; i < passedThroughSegments.Count; i++)
                {
                    var seg = passedThroughSegments[i];
                    if(seg == null)
                    {
                        passedThroughSegments.RemoveAt(i);
                    }
                }
            }
        }
    }
}
