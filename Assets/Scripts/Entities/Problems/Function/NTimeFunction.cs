using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NTimeFunction : SingleFunction
{
    public float time;//次数
    public float k;
    public NTimeFunction(VariableType variableType,float time, float k)
    {
        this.variableType = variableType;
        this.time = time;
        this.k = k;
    }
    public override void diff()
    {
        k *= time;
        time--;
    }
    public override void integrating()
    {
        k /= (time + 1);
        //不写了，反比例函数的积分非常复杂
        time++;
    }
    public float getDVariable(float variable)
    {
        this.variable = variable;
        if (variable == 0) return 0;
        dVariable = k * Mathf.Pow(variable, time);
        return dVariable;
    }
    public string getStringFunciton()
    {
        string coefficient =
            (k == 0 || ((k == 1 || k == -1) && time != 0))
                ? (k == -1 ? "-" : "")
                : k.ToString();
        string variableStr =
            (time == 0 || k == 0)
                ? ""
                : (variableType == VariableType.X ? "x" : "y");
        string exponent =
            (time == 1 || time == 0)
                ? ""
                : (time < 0 ? $"<sup><size=165%>({time})</size></sup>" : $"<sup><size=160%>{time}</size></sup>");
        return $"{coefficient}{variableStr}{exponent}";
    }
}
