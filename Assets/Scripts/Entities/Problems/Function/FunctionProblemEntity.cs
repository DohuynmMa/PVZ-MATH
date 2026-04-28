using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
public enum FunctionType
{
    NTimeFunction,
}

public class FunctionProblemEntity : ProblemEntity
{
    public VariableType variableType;
    public int functionLens = 0;
    /// <summary>
    /// 求导
    /// </summary>
    public virtual void diff()
    {

    }
    /// <summary>
    /// 求积分
    /// </summary>
    public virtual void integrating()
    {

    }
}
