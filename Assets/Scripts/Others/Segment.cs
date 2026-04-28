using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Segment : MonoBehaviour
{
    public static List<Segment> segments = new List<Segment>();
    private EdgeCollider2D col;
    private LineRenderer lr;
    public bool enableSegment = false;
    public PointEntity p1;
    public PointEntity p2;
    public float k;
    public List<Segment> parallelSegments = new List<Segment>();
    public List<Segment> verticalSegments = new List<Segment>();
    public int parallelSegmentCount = 0;
    public int verticalSegmentCount = 0;
    private void Awake()
    {
        col = GetComponent<EdgeCollider2D>();
        lr = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        segments.Add(this);
        EventManager.Instance.drawSegmentEvent?.Invoke();
        k = (float)(p1.fakeY - p2.fakeY) / (float)(p1.fakeX - p2.fakeX);
    }
    private void Update()
    {
        if(!enableSegment) return;

        //检查端点是否正常 若不正常,销毁线段.
        if (p1 == null || p2 == null)
        {
            segments.Remove(this);
            Destroy(gameObject);
            return;
        }

        //绘制线段
        drawSegment();

        //更新该线段作为的直线的k值
        k = (float)(p1.fakeY - p2.fakeY) / (float)(p1.fakeX - p2.fakeX);

        //检测与其他线段的关系
        foreach(var segment in segments)
        {
            if (!segment.enableSegment || segment == this) continue;
            //是否平行
            var isParallel = ((float.IsInfinity(k) && float.IsInfinity(segment.k)) || (!float.IsInfinity(k) && !float.IsInfinity(segment.k)) && segment.k == k) && (segment.p1.fakeX != p1.fakeX || segment.p1.fakeY != p1.fakeY);
            if (isParallel)
            {
                if(!parallelSegments.Contains(segment)) parallelSegments.Add(segment);
                parallelSegmentCount = parallelSegments.Count;
            }
            //是否垂直
            else if ((segment.k * k == -1 && !float.IsInfinity(segment.k) && !float.IsInfinity(k)) || (float.IsInfinity(segment.k) && k == 0) || (float.IsInfinity(k) && segment.k == 0))
            {
                if (!verticalSegments.Contains(segment))
                {
                    verticalSegments.Add(segment);
                }
                verticalSegmentCount = verticalSegments.Count;
            }
        }

        //检查与自己有关系线段的状态
        for (int i = 0; i < verticalSegments.Count; i++)
        {
            var v = verticalSegments[i];
            if(v == null) { 
                verticalSegments.RemoveAt(i);
                parallelSegmentCount = parallelSegments.Count;
            };
        }
        for (int i2 = 0; i2 < parallelSegments.Count; i2++)
        {
            var p = parallelSegments[i2];
            if (p == null)
            {
                parallelSegments.RemoveAt(i2);
                verticalSegmentCount = verticalSegments.Count;
            };
        }
    }
    private void drawSegment()
    {
        lr.SetPosition(0, p1.currentCell.GetComponent<BoxCollider2D>().bounds.center);
        lr.SetPosition(1, p2.currentCell.GetComponent<BoxCollider2D>().bounds.center);
        List<Vector2> poss = new List<Vector2>();
        for (int i = 0;i < lr.positionCount;i++)
        {
            poss.Add(lr.GetPosition(i));
        }
        col.SetPoints(poss);
    }
}
