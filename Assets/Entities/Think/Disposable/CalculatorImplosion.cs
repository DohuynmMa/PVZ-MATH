using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatorImplosion : MonoBehaviour
{
    private void Start()
    {
        Sounds.¼ÆËãÆ÷±¬Õ¨.playWithPitch();
        Invoke("disappear", 0.5f);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Entity")
        {
            var entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.entityGroup == EntityGroup.Own || entity.entityState == EntityState.Disable || entity.hitpoint <= 0 || !entity.isProblemEntity() || entity.entityType == EntityType.BossZCube) return;
                var problem = entity.GetComponent<ProblemEntity>();
                problem.anim.SetBool("Die", true);
                problem.problemText.gameObject.SetActive(false);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    Sounds.Pµ¹µØ.playWithPitch();
                });
            }
        }
    }
    private void disappear()
    {
        Destroy(gameObject);
    }
}
