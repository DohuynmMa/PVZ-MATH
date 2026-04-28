using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    public Entity toRemoveEntity;
    public GameObject cancelChecker;
    public GameObject removingSymbol;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Entity")
        {
            var entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.entityState == EntityState.Disable || entity.entityGroup == EntityGroup.Enemy || entity.entityType == EntityType.Shooter) return;
                toRemoveEntity = entity;
                removingSymbol.SetActive(true);
                removingSymbol.transform.position = entity.currentCell.GetComponent<BoxCollider2D>().bounds.center;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Entity")
        {
            toRemoveEntity = null;
            removingSymbol.SetActive(false);
        }
    }
}
