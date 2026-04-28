using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VariableType
{
    X,
    Y
}
[Serializable]
public class SingleFunction
{
    public VariableType variableType;
    public float variable;
    public float dVariable;
    public virtual void diff()
    {

    }
    public virtual void integrating()
    {

    }
}
