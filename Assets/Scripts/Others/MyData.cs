using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MyData
{
    public bool completedNewPlayerTutorial = false;
    public string playerName; //名字

    [FloatRange(0f, 1f)]
    public float musicVolume = 1; //音乐音量
    [FloatRange(0f, 1f)]
    public float soundVolume = 1; //音效音量
    [FloatRange(0f, 1f)]
    public float voiceVolume = 1; //语音音量

    public List<CardType> cardTypes = new List<CardType>();//卡牌背包
    public List<KeyCode> cardShortKey = new List<KeyCode>();//对应的卡牌快捷键
    [MappingSimpleListData]
    public List<KeyCode> otherShortKey = new List<KeyCode>() { KeyCode.Space , KeyCode.F,KeyCode.Alpha1,KeyCode.Alpha2, KeyCode.Alpha3};//其他快捷键

    [MappingSimpleListData]
    public List<int> SBlevelProgess = new List<int>() {0,0,0}; //完成情况 list temp = level temp
    [MappingSimpleListData]
    public List<int> SBlevelFinishTime = new List<int>() { 0, 0,0 };//完成次数
    [MappingSimpleListData]
    public List<bool> SBlevelTutorialWatched = new List<bool>() { false, false ,false};//是否观看完过教程

    public int currentChapter = 0;//当前解锁的章节
    public int currentLevel = 1;//当前解锁的关卡
    //闯关模式通关次数
    [MappingSimpleListData]
    public List<int> barrierFinishedTime = new List<int>()
    {
        0,0,0,0,0,/*章节分割*/0,0,0,0,0,
    };
}
public static class MyDataTools
{
    public static void loadCardShortKey(this MyData data)
    {
        var newCardKeys = new List<KeyCode>();
        newCardKeys = Enumerable.Repeat(KeyCode.None, data.cardTypes.Count).ToList();
        for (int i = 0; i < data.cardShortKey.Count; i++)
        {
            //清理多余的卡牌快捷键
            if (i + 1 > newCardKeys.Count)
            {
                data.cardShortKey.RemoveAt(i);
                continue;
            }

            newCardKeys[i] = data.cardShortKey[i];
        }
        data.cardShortKey = newCardKeys;
        ShortKeyManager.Instance.loadShortKey();
    }
}
