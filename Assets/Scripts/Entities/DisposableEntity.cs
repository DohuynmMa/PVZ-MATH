using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableEntity : Entity
{
    protected virtual void dieEvent()
    {

    }
    public override void transitionToEnable()
    {
        entityState = EntityState.Enable;
        entityDie();
    }
    public override void transitionToDisable()
    {
        entityState = EntityState.Disable;
    }
    public override void entityDie()
    {
        dieEvent();
        Sounds.prize.play();
        Instantiate(Utils.findEffectPrefabByType(EffectType.StarEffect), currentCell.GetComponent<BoxCollider2D>().bounds.center, Quaternion.identity);
        base.entityDie();
    }
}
