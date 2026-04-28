using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
public class DerivativesEntity : SymbolEntity
{
    private FunctionProblemEntity toDiffProblem;
    public override void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Entity")
        {
            if (collider.GetComponent<FunctionProblemEntity>() != null)
            {
                var fp = collider.GetComponent<FunctionProblemEntity>();
                if (fp.entityGroup == entityGroup || fp.entityState == EntityState.Disable || fp.hitpoint <= 0)
                {
                    return;
                }
                toDiffProblem = fp;
                anim.SetBool("Attacking", true);
            }
        }
        base.OnTriggerStay2D(collider);
    }
    private void attackAndDieByAnim()
    {
        if (toDiffProblem == null || currentCell.num == 0)
        {
            entityDie();
            return;
        }
        Sounds.µ¼Êý.playWithPitch();
        Instantiate(Utils.findEffectPrefabByType(EffectType.DerivativesEffect), toDiffProblem.getEntityBoxColliderPos(), Quaternion.identity);
        for (int i = 0; i < Mathf.Abs(Mathf.Floor(currentCell.num)); i++)
        {
            toDiffProblem.diff();
        }
        entityDie();
    }
}
