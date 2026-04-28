using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrainPointManager : MonoBehaviour
{
    public static BrainPointManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public float brainPoint = 0;
    public float brainPointAddSpeed = 1f;
    private void Update()
    {
        updateBrainPoint();
    }
    public void setBrainPoint(float brainPoint)
    {
        this.brainPoint = brainPoint;
    }
    public void reduceBrainPoint(float reducebrainPoint)
    {
        brainPoint -= reducebrainPoint;
    }
    private void updateBrainPoint()
    {
        if (!GameManager.Instance.inGame) return;
        brainPoint += Time.deltaTime * brainPointAddSpeed * (1 + EntityTools.entityCount(EntityType.BrainFlowerThink) * 0.1f);
        var gum = GamingUIManager.Instance;
        gum.brainPointText.text = Convert.ToInt32(Math.Floor(brainPoint)).ToString();
        gum.brainPointBar.fillAmount = brainPoint - Convert.ToInt32(Math.Floor(brainPoint));
    }
}
