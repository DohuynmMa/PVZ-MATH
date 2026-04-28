using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundDownEntity : DisposableEntity
{
    protected override void dieEvent()
    {
        if(currentCell != null)
        {
            currentCell.setNum(Mathf.Floor(currentCell.num));
        }
    }
}
