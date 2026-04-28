using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExportOrImport : MonoBehaviour
{
    private static readonly Color lightBlue = new Color(160f / 255f, 1, 1);
    private static readonly Color lightYellow = new Color(1, 1, 105f/255f);
    public TextMeshPro text;
    public bool isImport = false;
    private void Start()
    {
        if (text != null)
        {
            var meshRenderer = text.GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "Others";
            meshRenderer.sortingOrder = 101;
        }
    }
    private void OnMouseEnter()
    {
        var hm = HandManager.Instance;
        text.color = lightBlue;
        if (isImport)
        {
            if (!hm.hasToImportData)
            {
                text.text = "无数据";
                return;
            }
            else if (hm.selectedCell.currentEntity == null)
            {
                text.text = "无ST";
                return;
            }
            else if (hm.toImportCellNum == 0 && hm.selectedCell.currentEntity.entityType == EntityType.Divide)
            {
                text.text = "ERROR";
                return;
            }
            if (BrainPointManager.Instance.brainPoint < 5)
            {
                text.color = Color.red;
            }
            text.text = "BP - 5";
        }
    }
    private void OnMouseExit()
    {
        if (isImport)
        {
            text.text = "导 入";
        }
        else
        {
            text.text = "导 出";
        }
        text.color = lightYellow;
    }
    private void OnMouseDown()
    {
        if (isImport)
        {
            import();
        }
        else
        {
            export();
        }
    }
    private void import()
    {
        var hm = HandManager.Instance;
        var tm = TutorialManager.Instance;
        if (BrainPointManager.Instance.brainPoint < 5 || !hm.hasToImportData || hm.selectedCell.currentEntity == null || (hm.toImportCellNum == 0 && hm.selectedCell.currentEntity.entityType == EntityType.Divide))
        {
            return;
        }
        hm.hidePlacePoint();
        hm.importTheLWToSelectedCell();
        Sounds.导入.playWithPitch();
        GamingUIManager.Instance.rightClickMenu.SetActive(false);
        hm.toImportCellNum = 0;
        hm.hasToImportData = false;
        hm.exportedCell.setNum(0);
        hm.exportedCell = null;
        BrainPointManager.Instance.brainPoint -= 5;
        if (tm.needingImportAnyCellNum)
        {
            tm.needingImportAnyCellNum = false;
            tm.tutorialTemp++;
            tm.danceWithTFFO();
        }
    }
    private void export()
    {
        var hm = HandManager.Instance;
        var tm = TutorialManager.Instance;
        Sounds.导出.playWithPitch();
        hm.hidePlacePoint();
        hm.hasToImportData = true;
        hm.toImportCellNum = hm.selectedCell.num;
        hm.exportedCell = hm.selectedCell;
        GamingUIManager.Instance.rightClickMenu.SetActive(false);
        if (tm.needingExportAnyCellNum)
        {
            tm.needingExportAnyCellNum = false;
            tm.tutorialTemp++;
            tm.danceWithTFFO();
        }
    }
}
