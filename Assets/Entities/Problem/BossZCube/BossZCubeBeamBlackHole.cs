using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZCubeBeamBlackHole : MonoBehaviour
{
    private void Start()
    {
        Invoke("disappear", 1f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Entity")
        {
            var entity = collision.GetComponent<Entity>();
            if (entity != null && entity.entityType != EntityType.Shooter)
            {
                if (entity.entityGroup == EntityGroup.Enemy || entity.entityState == EntityState.Disable || entity.hitpoint <= 0) return;
                var cell = entity.currentCell;
                if (cell != null && cell.GetComponent<BoxCollider2D>().enabled)
                {
                    cell.GetComponent<BoxCollider2D>().enabled = false;
                    Utils.summonEffectDirectly(EffectType.CellErrorEffect, cell.GetComponent<BoxCollider2D>().bounds.center);
                }
                entity.entityDie();
            }
        }
    }
    private void disappear()
    {
        Destroy(gameObject);
    }
}
