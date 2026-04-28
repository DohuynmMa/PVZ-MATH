using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public Entity currentEntity;
    public Card usingCard;
    public Eraser eraser;
    public Pencil pencil;
    public Cell selectedCell;
    public Cell exportedCell;
    public float toImportCellNum = 0;
    public bool hasToImportData = false;
    public GameObject trueMouse;
    public GameObject placePos;
    public bool usingEraser = false;
    public bool usingPencil = false;
    private void Update()
    {
        if (GameManager.Instance.currentBg == BackgroundType.OriginalDay) return;
        entityFollowMouse();
        eraserUpdate();
        pencilUpdate();
        Cursor.visible = false;
        trueMouse.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            if (!trueMouse.activeSelf || !GameManager.Instance.inGame) return;
            var e = Instantiate(Utils.findEffectPrefabByType(EffectType.ClickEffect));
            e.gameObject.moveToMouse();
            if (isImportingOrExportingCellNum())
            {
                hidePlacePoint();
                GamingUIManager.Instance.rightClickMenu.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!GameManager.Instance.inGame) return;
            if (currentEntity != null)
            {
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    cancelUseCard();
                });
            }
            if (usingEraser)
            {
                eraser.cancelChecker.SetActive(false);
                trueMouse.SetActive(true);
                usingEraser = false;
            }
            if (usingPencil)
            {
                cancelPencil();
            }
            if (isImportingOrExportingCellNum())
            {
                hidePlacePoint();
                GamingUIManager.Instance.rightClickMenu.SetActive(false);
            }
        }
    }
    private void entityFollowMouse()
    {
        if (currentEntity != null)
        {
            currentEntity.gameObject.moveToMouse(0.1f);
        }
    }
    public void cancelUseCard()
    {
        Destroy(currentEntity.gameObject);
        hidePlacePoint();
        currentEntity = null;
        usingCard = null;
    }
    public void useCard(Cell cell)
    {
        var bm = BrainPointManager.Instance;
        var tm = TutorialManager.Instance;
        if (usingCard == null || currentEntity == null || isImportingOrExportingCellNum()) return;
        if (cell.currentEntity != null && !currentEntity.isDisposableEntity())
        {
            return;
        }
        hidePlacePoint();
        if(currentEntity.currentCell != null) currentEntity.transform.position = currentEntity.currentCell.GetComponent<BoxCollider2D>().bounds.center;
        currentEntity.transitionToEnable();

        foreach (var sp in Utils.getAllSR(currentEntity.gameObject))
        {
            sp.sortingLayerName = "Entity";
            sp.sortingOrder -= 200;
        }

        Sounds.放置想法.playWithPitch();
        bm.reduceBrainPoint(usingCard.costBrainPoint);
        usingCard.cooldownTimer = 0;

        if (currentEntity.entityType == EntityType.Plus && tm.needingUsingPlusCard)
        {
            tm.tutorialTemp++;
            tm.needingUsingPlusCard = false;
            switch (tm.chapter)
            {
                case 0:
                    tm.beginnerTutorial();
                    break;
            }
        }

        currentEntity = null;
        usingCard = null;

        foreach(var e in Utils.findAllEntities(EntityGroup.Enemy))
        {
            if(e == null) continue;
            if(e.GetComponent<ProblemEntity>() != null)
            {
                e.gameObject.layer = 0;
            }
        }
    }
    private void eraserUpdate()
    {
        var gum = GamingUIManager.Instance;
        if (usingEraser)
        {
            eraser.gameObject.moveToMouse();
            eraser.gameObject.SetActive(true);
            gum.fakeEraser.SetActive(false);
            eraser.transform.localEulerAngles = new Vector3(0, 0, -217);
        }
        else
        {
            gum.fakeEraser.SetActive(true);
            eraser.gameObject.SetActive(false);
        }
    }
    private void pencilUpdate()
    {
        var gum = GamingUIManager.Instance;
        if (usingPencil)
        {
            pencil.gameObject.moveToMouse();
            pencil.gameObject.SetActive(true);
            gum.fakePencil.SetActive(false);
        }
        else
        {
            gum.fakePencil.SetActive(true);
            pencil.gameObject.SetActive(false);
        }
    }
    public void useOrCancelPencil()
    {
        if (usingEraser) return;
        if (usingPencil)
        {
            if (pencil.selectingPoint != null)
            {
                if (pencil.p1 == null)
                {
                    pencil.p1 = pencil.selectingPoint;
                    pencil.currentMark = pencil.mark2;
                }
                else if (pencil.p2 == null && pencil.p1 != pencil.selectingPoint)
                {
                    pencil.p2 = pencil.selectingPoint;
                    Utils.spawnASegment(pencil.p1, pencil.p2);
                    cancelPencil(); 
                }
                else
                {
                    cancelPencil();
                }
            }
            else
            {
                cancelPencil();
            }
        }
        else
        {
            pencil.cancelChecker.SetActive(true);
            trueMouse.SetActive(false);
            usingPencil = true;
        }
    }
    private void cancelPencil()
    {
        pencil.cancelChecker.SetActive(false);
        trueMouse.SetActive(true);
        usingPencil = false;
        pencil.p1 = null;
        pencil.p2 = null;
        pencil.mark1.SetActive(false);
        pencil.mark2.SetActive(false);
    }
    public void useOrCancelEraser()
    {
        if (usingPencil) return;
        if (usingEraser)
        {
            if (eraser.toRemoveEntity != null)
            {
                Sounds.放置想法.playWithPitch();
                eraser.toRemoveEntity.entityDie();
            }
            eraser.cancelChecker.SetActive(false);
            trueMouse.SetActive(true);
        }
        else
        {
            eraser.cancelChecker.SetActive(true);
            trueMouse.SetActive(false);
        }
        usingEraser = !usingEraser;
    }
    public void showPlacePoint(Cell cell)
    {
        placePos.gameObject.SetActive(true);
        placePos.transform.position = cell.GetComponent<BoxCollider2D>().bounds.center;
    }
    public void showPlacePoint(Vector3 pos)
    {
        if (placePos == null) return;
        placePos.gameObject.SetActive(true);
        placePos.transform.position = pos;
    }
    public void hidePlacePoint()
    {
        if(placePos != null)placePos.gameObject.SetActive(false);
    }
    public void importTheLWToSelectedCell()
    {
        switch (selectedCell.currentEntity.entityType)
        {
            case EntityType.Plus:
                selectedCell.setNum(selectedCell.num + toImportCellNum);
                break;
            case EntityType.Substract:
                selectedCell.setNum(selectedCell.num - toImportCellNum);
                break;
            case EntityType.Multiply:
                selectedCell.numerator *= toImportCellNum;
                break;
            case EntityType.Divide:
                selectedCell.denominator *= toImportCellNum;
                break;
        }
    }
    public bool isImportingOrExportingCellNum()
    {
        return GamingUIManager.Instance.rightClickMenu.activeSelf;
    }
}
