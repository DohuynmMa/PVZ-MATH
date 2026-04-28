using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
public class SegmentRelationshipProblemEntity : ProblemEntity
{
    public int parallelSegmentCount = 0;
    public int verticalSegmentCount = 0;
    public override void OnTriggerStay2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Cell":
                var cell = collider.GetComponent<Cell>();
                currentCell = cell;
                row = Utils.getCellRowByCellId(currentCell.cellId) != 0 ? Utils.getCellRowByCellId(currentCell.cellId) : row;
                column = Utils.getCellColumnByCellId(currentCell.cellId) != 0 ? Utils.getCellColumnByCellId(currentCell.cellId) : column;

                //todo num die
                break;
            case "Entity":
                var entity = collider.GetComponent<Entity>();
                if (entity == null) return;
                if (entity.entityGroup == entityGroup || entity.hitpoint <= 0 || entity.entityState == EntityState.Disable) return;
                aim = entity;
                anim.SetBool("Attacking", true);
                break;
        }
        if (collider.gameObject.GetComponent<Segment>() != null)
        {
            var segment = collider.gameObject.GetComponent<Segment>();
            var parallelSegmentCountInTotal = 0;
            var verticalSegmentCountInTotal = 0;
            parallelSegmentCountInTotal = segment.parallelSegmentCount;
            verticalSegmentCountInTotal = segment.verticalSegmentCount;

            if (entityState == EntityState.Enable)
            {
                if (anim.GetBool("Die")) return;
                switch (problemType)
                {
                    case ProblemType.Greater:
                        if ((parallelSegmentCount == 0 || parallelSegmentCountInTotal > parallelSegmentCount) && (verticalSegmentCount == 0 || verticalSegmentCountInTotal > verticalSegmentCount))
                        {
                            playDieAnim();
                        }
                        break;
                    case ProblemType.Less:
                        if ((parallelSegmentCount == 0 || parallelSegmentCountInTotal < parallelSegmentCount) && (verticalSegmentCount == 0 || verticalSegmentCountInTotal < verticalSegmentCount))
                        {
                            playDieAnim();
                        }
                        break;
                    case ProblemType.Equals:
                        if ((parallelSegmentCount == 0 || parallelSegmentCountInTotal == parallelSegmentCount) && (verticalSegmentCount == 0 || verticalSegmentCountInTotal == verticalSegmentCount))
                        {
                            playDieAnim();
                        }
                        break;
                }
            }
        }
    }
    public override void setRandomProblem(string difficultF, int difficultB, bool isBarrier = false)
    {
        if (isBarrier)
        {
            problemType = ProblemType.Equals;
            parallelSegmentCount = Random.Range(0, 3);
            verticalSegmentCount = Random.Range(0, 3);
        }
        switch (difficultF)
        {
            case "EAZY":
                problemType = ProblemType.Equals;
                parallelSegmentCount = Random.Range(0,3);
                verticalSegmentCount = Random.Range(0,3);
                break;
            case "MIDDLE":
                problemType = ProblemType.Equals;
                parallelSegmentCount = Random.Range(0, 4);
                verticalSegmentCount = Random.Range(0, 4);
                break;
            case "HARD":
                problemType = ProblemType.Equals;
                parallelSegmentCount = Random.Range(0, 5);
                verticalSegmentCount = Random.Range(0, 5);
                break;
        }
        if (parallelSegmentCount == 0 && verticalSegmentCount == 0) verticalSegmentCount = 1;
    }
    protected override void updateProblemText()
    {
        var problemSymbol = "";
        if (problemType == ProblemType.Greater) problemSymbol = ">";
        else if (problemType == ProblemType.Less) problemSymbol = "<";
        else problemSymbol = "=";
        problemText.text = (verticalSegmentCount != 0 ? problemSymbol + verticalSegmentCount + "[V]\n": "") + (parallelSegmentCount != 0 ? problemSymbol + parallelSegmentCount + "[P]" : "");
        problemText.fontSize = 13 - problemText.text.Length * 3;
    }
    public override void entityDie()
    {
        EventManager.Instance.segmentProblemDieEvent?.Invoke();
        base.entityDie();
    }
}
