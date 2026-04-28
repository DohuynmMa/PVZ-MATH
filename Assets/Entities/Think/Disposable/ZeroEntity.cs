using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroEntity : DisposableEntity
{
    protected override void dieEvent()
    {
        if (currentCell != null)
        {
            currentCell.setNum(0);
        }
    }
}
