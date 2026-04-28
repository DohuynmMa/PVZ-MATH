using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public Action drawSegmentEvent;//画线段事件
    public Action segmentProblemDieEvent;//SP死亡事件
    private void Awake()
    {
        Instance = this;
    }
}
