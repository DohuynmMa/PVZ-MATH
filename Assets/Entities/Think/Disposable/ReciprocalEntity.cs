using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReciprocalEntity : DisposableEntity
{
    protected override void dieEvent()
    {
        if(currentCell != null)
        {
            var n = currentCell.numerator;
            var d = currentCell.denominator;
            currentCell.numerator = d;
            currentCell.denominator = n;
        }
    }
}
