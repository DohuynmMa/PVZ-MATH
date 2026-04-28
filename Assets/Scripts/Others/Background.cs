using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BackgroundType
{
    OriginalDay,
    MathWorld
}
public class Background : MonoBehaviour
{
    public BackgroundType backgroundType;
    public GameObject cellPrefab;
    public int quadrantCount = 1;
    public float oppositePosX;
    public float oppositePosY;
    private void Start()
    {
        //根据象限数生成格子
        quadrantCount = GameManager.Instance.quardrantInTotal;
        for (int i = 0; i < quadrantCount; i++)
        {
            summonSite(i + 1);
        }
    }
    private void summonSite(int quadrant)
    {
        var accuratePos = new Vector3(oppositePosX * (quadrant == 2 || quadrant == 3 ? -1f : 1f), oppositePosY * (quadrant == 3 || quadrant == 4 ? -1f : 1f), 0);
        var cells = Instantiate(cellPrefab, accuratePos, Quaternion.identity, transform);
        cells.name = "Cells Q: " + quadrant;
        for (int q = 0; q < cells.transform.childCount; q++)
        {
            var rowObj = cells.transform.GetChild(q);
            for (int r = 0; r < rowObj.transform.childCount; r++)
            {
                var cellObj = rowObj.GetChild(r);
                var cell = cellObj.GetComponent<Cell>();
                if (cell != null)
                {
                    cell.quadrant = quadrant;
                }
            }
        }
    }
}
