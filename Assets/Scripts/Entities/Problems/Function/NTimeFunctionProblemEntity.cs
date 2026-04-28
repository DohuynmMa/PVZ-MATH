using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Text;
public class NTimeFunctionProblemEntity : FunctionProblemEntity
{
    private List<NTimeFunction> functions = new List<NTimeFunction>();
    protected override void enableUpdate()
    {
        base.enableUpdate();
        updateNumByFunction();
    }
    public override void transitionToEnable()
    {
        base.transitionToEnable();
        variableType = VariableType.X;
    }
    private void updateNumByFunction()
    {
        float t = 0;
        foreach (var f in functions)
        {
            var variable = variableType == VariableType.X ? column : row;
            t += f.getDVariable(variable);
        }
        if (currentCell != null) num = t;
        else
        {
            num = Random.Range(-824123f,8921399f);
        }
    }
    protected override void updateProblemText()
    {
        var problemSymbol = "";
        if (problemType == ProblemType.Greater) problemSymbol = ">";
        else if (problemType == ProblemType.Less) problemSymbol = "<";
        else problemSymbol = "=";
        problemText.text = problemSymbol + num;
        var nonConstantFunctions = functions
            .Where(f => f.k != 0 && f.time != 0)
            .OrderByDescending(f => f.time)
            .ToList();

        var constantFunctions = functions
            .Where(f => f.k != 0 && f.time == 0)
            .ToList();

        var expressionBuilder = new StringBuilder();
        bool hasTerms = false;
        foreach (var func in nonConstantFunctions)
        {
            string term = func.getStringFunciton();
            if (string.IsNullOrEmpty(term)) continue;

            if (!hasTerms)
            {
                expressionBuilder.Append(term);
                hasTerms = true;
            }
            else
            {
                if (func.k > 0)
                    expressionBuilder.Append("+" + term);
                else
                    expressionBuilder.Append("-" + term.Substring(1));
            }
        }
        foreach (var func in constantFunctions)
        {
            string term = func.getStringFunciton();
            if (string.IsNullOrEmpty(term)) continue;

            if (!hasTerms)
            {
                expressionBuilder.Append(term);
                hasTerms = true;
            }
            else
            {
                if (func.k > 0)
                    expressionBuilder.Append("+" + term);
                else
                    expressionBuilder.Append("-" + term.Substring(1));
            }
        }
        problemText.text = problemSymbol + (expressionBuilder.Length > 0 ? expressionBuilder.ToString() : "0");
    }
    public override void setRandomProblem(string difficultF, int difficultB, bool isBarrier = false)
    {
        for (int i = 0; i < functionLens; i++)
        {
            int acTime = 0;
            int acK = 0;
            int acB = 0;
            if (isBarrier)
            {
                acTime = Random.Range(-1, 3);
                acK = Random.Range(-3, 3);
                acB = Random.Range(-3, 3);
            }
            switch (difficultF)
            {
                case "EAZY":
                    acTime = Random.Range(-1, 3);
                    acK = Random.Range(-3, 3);
                    acB = Random.Range(-3, 3);
                    break;
                case "MIDDLE":
                    acTime = Random.Range(-2, 4);
                    acK = Random.Range(-5, 5);
                    acB = Random.Range(-5, 5);
                    break;
                case "HARD":
                    acTime = Random.Range(-3, 4);
                    acK = Random.Range(-10, 10);
                    acB = Random.Range(-30, 30);
                    break;
            }
            float time = acTime;
            float k = acK;
            float b = acB;
            var newF = new NTimeFunction(variableType, time, k);
            functions.Add(newF);
            arrangeFunction();
        }
        base.updateProblemText();
    }
    private bool arrangeFunction()
    {
        bool merged = false;
        while (true)
        {
            bool found = false;
            for (int i = 0; i < functions.Count; i++)
            {
                var f = functions[i];
                for (int j = i + 1; j < functions.Count; j++)
                {
                    var f2 = functions[j];
                    if (f2.time == f.time)
                    {
                        var mixedF = new NTimeFunction(variableType, f.time, f.k + f2.k);
                        functions.Add(mixedF);
                        functions.Remove(f);
                        functions.Remove(f2);
                        found = true;
                        merged = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (!found) break; 
        }
        return merged;
    }
    public override void diff()
    {
        for (int i = 0; i < functions.Count; i++)
        {
            functions[i].diff();
            if (functions[i].k == 0)
            {
                functions.RemoveAt(i);
                i--;
            }
        }
        arrangeFunction();
    }
    public override void integrating()
    {
        for (int i = 0; i < functions.Count; i++)
        {
            functions[i].integrating();
            if (functions[i].k == 0)
            {
                functions.RemoveAt(i);
                i--;
            }
        }
    }
}
