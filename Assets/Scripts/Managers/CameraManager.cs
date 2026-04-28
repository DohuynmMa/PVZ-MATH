using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public Camera mainCamera;
    private void Awake()
    {
        Instance = this;
    }
    public void changeViewport(float time = 1, float X = 0, float Y = 0, float W = 1, float H = 1,Action a = null)
    {
        DOTween.To(() => mainCamera.rect, x => mainCamera.rect = x, new Rect(X, Y, W, H), time).OnComplete(() =>
        {
            a?.Invoke();
        });
    }
    public void changeSize(float size = 6, float time = 1, Action a = null)
    {
        DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, size, time).OnComplete(() =>
        {
            a?.Invoke();
        });
    }
}
