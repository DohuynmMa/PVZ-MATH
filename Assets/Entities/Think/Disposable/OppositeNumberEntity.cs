using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppositeNumberThink : DisposableEntity
{
    protected override void dieEvent()
    {
        if(currentCell != null)
        {
            currentCell.numerator *= -1f;
        }
    }
}
