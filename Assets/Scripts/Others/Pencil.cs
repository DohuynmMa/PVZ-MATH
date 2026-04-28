using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Pencil : MonoBehaviour
{
    public PointEntity p1;
    public PointEntity p2;
    public PointEntity selectingPoint;
    public GameObject cancelChecker;
    public GameObject currentMark;
    public GameObject mark1;
    public GameObject mark2;
    public GameObject fakeSegment;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Entity" && collision.gameObject.GetComponent<PointEntity>() != null)
        {
            var p = collision.GetComponent<PointEntity>();
            if (p != null)
            {
                if (p.entityState == EntityState.Disable || p.entityGroup == EntityGroup.Enemy) return;
                if(currentMark == null) currentMark = p1 == null ? mark1 : mark2;
                currentMark.SetActive(true);
                currentMark.transform.position = p.currentCell.GetComponent<BoxCollider2D>().bounds.center;
                if(p1 != null && selectingPoint != null)
                {
                    //»æÖÆ°ëÍ¸Ã÷Ïß¶Î
                    fakeSegment.SetActive(true);
                    var lr = fakeSegment.GetComponent<LineRenderer>();
                    lr.SetPosition(0, p1.currentCell.GetComponent<BoxCollider2D>().bounds.center);
                    lr.SetPosition(1, selectingPoint.currentCell.GetComponent<BoxCollider2D>().bounds.center);
                }
                else
                {
                    fakeSegment.SetActive(false);
                }
                selectingPoint = p;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Entity" && collision.gameObject.GetComponent<PointEntity>() != null)
        {
            selectingPoint = null;
            currentMark.SetActive(false);
            fakeSegment.SetActive(false);
        }
    }
}
