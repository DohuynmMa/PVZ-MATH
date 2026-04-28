using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatorEntity : DisposableEntity
{
    public CalculatorImplosion implosionPrefab;
    protected override void dieEvent()
    {
        if (currentCell != null)
        {
            var i = Instantiate(implosionPrefab,this.getEntityBoxColliderPos(),Quaternion.identity);
            GameManager.Instance.objsInGame.Add(i.gameObject);
        }
    }
}
