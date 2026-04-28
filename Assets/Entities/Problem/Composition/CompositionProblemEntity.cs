using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CompositionProblemEntity : ProblemEntity
{
    private List<string> allSymbols = new List<string>() { "+","-","x","/"};
    public List<string> symbols = new List<string>();
    public List<float> childNums = new List<float>();
    public int childNumCount = 0;
    public override void setRandomProblem(string difficultF, int difficultB, bool isBarrier = false)
    {
        for (int i = 0; i < childNumCount - 1; i++)
        {
            var symbol = allSymbols[Random.Range(0, allSymbols.Count)];
            symbols.Add(symbol);
        }
        symbols = symbols.Where(op => op == "x" || op == "/")
                        .Concat(symbols.Where(op => op == "+" || op == "-"))
                        .ToList();
        for (int i = 0; i < childNumCount; i++)
        {
            int num = 0;
            if (isBarrier)
            {
                num = Random.Range(-30, 30);
            }
            switch (difficultF)
            {
                case "EAZY":
                    num = Random.Range(-30, 30);
                    break;
                case "MIDDLE":
                    num = Random.Range(-50, 50);
                    break;
                case "HARD":
                    num = Random.Range(-99, 99);
                    break;
            }
            childNums.Add(num);
        }
    }
    protected override void enableUpdate()
    {
        base.enableUpdate();
        updateTheNum();
    }
    private void updateTheNum()
    {
        num = getTheResult();
    }
    protected override void updateProblemText()
    {
        var problemSymbol = "";
        if (problemType == ProblemType.Greater) problemSymbol = ">";
        else if (problemType == ProblemType.Less) problemSymbol = "<";
        else problemSymbol = "=";
        problemText.text = problemSymbol + (childNumCount >= 3 ? "\n" : "") + getTheString();
    }
    public float getTheResult()
    {
        var n1 = childNums[0];
        for (int i = 1;i < childNums.Count; i++)
        {
            var num = childNums[i];
            switch(symbols[i - 1])
            {
                case "+":
                    n1 += num;
                    break;
                case "-":
                    n1 -= num;
                    break;
                case "x":
                    n1 *= num;
                    break;
                case "/":
                    while (num == 0)
                    {
                        childNums[i] = spawnANewNum();
                        num = childNums[i];
                    }
                    n1 /= num;
                    break;
            }
            
        }
        return n1;
    }
    private string getTheString()
    {
        getTheResult();
        string s = childNums[0].ToString();
        if (childNums[0] < 0) s = $"({s})";
        for (int i = 1; i < childNums.Count; i++)
        {
            var num = childNums[i];
            string sybStr = symbols[i - 1];
            if (num == 0) sybStr = "";
            string numStr = num.ToString();
            if (num < 0) numStr = $"({numStr})";
            else if (num == 0) numStr = "";
            s += sybStr;
            s += numStr;
        }
        return s;
    }
    private float spawnANewNum()
    {
        int num = 0;
        switch (GameManager.Instance.difficultF)
        {
            case "EAZY":
                num = Random.Range(-30, 30);
                break;
            case "MEDIUM":
                num = Random.Range(-50, 50);
                break;
            case "HARD":
                num = Random.Range(-100, 100);
                break;
        }
        return num;
    }
}
