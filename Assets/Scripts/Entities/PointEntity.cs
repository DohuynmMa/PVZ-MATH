using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEntity : ThinkEntity
{
    public static List<PointEntity> points = new List<PointEntity>();
    public float fakeX;
    public float fakeY;
    private void Start()
    {
        points.Add(this);
    }
    public override void Update()
    {
        //¸üÐÂ¼Ù×ø±ê
        fakeX = column;
        fakeY = row;
        base.Update();
    }
}
