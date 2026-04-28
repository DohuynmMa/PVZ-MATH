using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinkEntity : Entity
{
    public override void Update()
    {
        lockPos();
        base.Update();
    }
    public void lockPos()
    {
        if (currentCell != null && entityState == EntityState.Enable)
        {
            if (transform.position != currentCell.GetComponent<BoxCollider2D>().bounds.center)
            {
                transform.position = currentCell.GetComponent<BoxCollider2D>().bounds.center;
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Entity")
        {
            if (collider.GetComponent<ProblemEntity>() != null)
            {
                var pe = collider.GetComponent<ProblemEntity>();
                var psps = Utils.getAllSR(pe.gameObject);
                var sps = Utils.getAllSR(gameObject);
                if (sps.Length == 0) return;
                foreach (var sp in sps)
                {
                    if (psps.Length == 0) return;
                    var psp = psps[0];
                    sp.sortingOrder = psp.sortingOrder - 50;
                }
            }
        }
    }
}
