using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareEntity : DisposableEntity
{
    protected override void dieEvent()
    {
        if (currentCell != null)
        {
            currentCell.setNum(currentCell.num * currentCell.num);
        }
    }
}
