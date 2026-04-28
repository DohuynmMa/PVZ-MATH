using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using System.Xml;
public enum EntityType
{
    Shooter,
    Plus,
    Substract,
    Multiply,
    Divide,
    Problem,
    BrainPointSpawner,
    NTimeFunctionProblem,
    CompositionProblem,
    RoundDownThink,
    ReciprocalThink,
    OppositeNumberThink,
    DerivativeThink,
    PointThink,
    SegmentRelationshipProblem,
    BossZCube,
    ZeroThink,
    CalculatorThink,
    SquareThink,
    BrainFlowerThink,
    ShieldFlowerThink,
    CounterclockwiseThink
}
public enum EntityGroup
{
    Own,
    Enemy
}
public enum EntityState
{
    Disable,
    Enable
}
public class Entity : MonoBehaviour
{
    public Animator anim;
    public BoxCollider2D bc;

    public EntityState entityState;
    public EntityType entityType;
    public EntityGroup entityGroup;
    public Sounds attackSound = Sounds.none;
    public Sounds dieSound = Sounds.none;
    public Cell currentCell;
    public Entity aim;
    public int row = 1;
    public int column = 1;
    public float bulletPosOffsetX;
    public float bulletPosOffsetY;
    public float hitpoint;
    public float maxHitpoint;
    public float damage;
    public float damageDuration;
    protected float damageTimer;

    public int currentQuadrant = 1;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
    }
    public virtual void Update()
    {
        switch (entityState)
        {
            case EntityState.Disable:
                disableUpdate();
                break;
            case EntityState.Enable:
                enableUpdate();
                break;
        }
    }
    protected virtual void disableUpdate()
    {

    }
    protected virtual void enableUpdate()
    {

    }
    public virtual void transitionToDisable()
    {
        entityState = EntityState.Disable;
        anim.enabled = false;
    }
    public virtual void transitionToEnable()
    {
        entityState = EntityState.Enable;
        anim.enabled = true;
    }
    public virtual void shoot(BulletType type)
    {
    }
    public virtual void attack()
    {
    }
    public virtual void setHitpoint(float hitpoint)
    {
        this.hitpoint = hitpoint <= maxHitpoint ? hitpoint : maxHitpoint;
        if(this.hitpoint <= 0)
        {
            anim.SetBool("Die", true);
        }
    }
    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        //更新当前所处的格子状态
        if (collider.tag != "Cell") return;
        var cell = collider.GetComponent<Cell>();

        if (cell.currentEntity != null && !entityType.isDisposableEntity())
        {
            return;
        }

        currentCell = cell;

        //检测格子象限并对实体进行翻转
        checkQuadrantAndFlipX();

        //更新行列
        updateRowAndColumn();

        //更新格子的实体
        if (entityState == EntityState.Enable && !entityType.isDisposableEntity())
        {
            cell.currentEntity = this;
        }
        else
        {
            //显示放置点
            var hm = HandManager.Instance;
            if((hm.currentEntity == this && cell.currentEntity == null) || entityType.isDisposableEntity())
            {
                hm.showPlacePoint(cell);
            }
        }
    }
    public virtual void OnTriggerExit2D(Collider2D collider)
    {
        HandManager.Instance.hidePlacePoint();
    }
    public virtual void beHitByBulletEvent(Bullet bullet)
    {

    }
    public virtual void entityDie()
    {
        if(dieSound != Sounds.none) dieSound.play();
        Destroy(gameObject);
    }
    public virtual void flipX(bool specSetting = false,bool positive = true)
    {
        var s = transform.localScale;
        if (!specSetting)
        {
            transform.localScale = new Vector3(s.x * -1, s.y, s.z);
        }
        else transform.localScale = new Vector3(Mathf.Abs(s.x) * (positive ? 1 : - 1), s.y, s.z);
    }
    public virtual void updateRowAndColumn()
    {
        var fakeRow = Utils.getCellRowByCellId(currentCell.cellId, currentQuadrant);
        var fakeColumn = Utils.getCellColumnByCellId(currentCell.cellId, currentQuadrant);
        row = fakeRow != 0 ? fakeRow : row;
        column = fakeColumn != 0 ? fakeColumn : column;
    }
    public void checkQuadrantAndFlipX()
    {
        if(currentCell != null) currentQuadrant = currentCell.quadrant;
        if (currentQuadrant == 2 || currentQuadrant == 3)
        {
            flipX(true, false);
        }
        else
        {
            flipX(true, true);
        }
    }
    protected bool IsClicked(int button)
    {
        if (Input.GetMouseButtonDown(button))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float planeZ = 0f;
            float t = (planeZ - ray.origin.z) / ray.direction.z;
            Vector3 worldPoint = ray.origin + ray.direction * t;
            Vector2 point = new Vector2(worldPoint.x, worldPoint.y);
            Collider2D hitCollider = Physics2D.OverlapPoint(point);
            return hitCollider != null && hitCollider.gameObject == gameObject;
        }
        return false;
    }
    protected bool RightClicked()
    {
        return IsClicked(1);
    }
    protected bool LeftClicked()
    {
        return IsClicked(0);
    }
}
public static class EntityTools
{
    public static bool isProblemEntity(this EntityType type)
    {
        var entity = Utils.findEntityPrefabByType(type);
        if (entity != null)
        {
            return entity.GetComponent<ProblemEntity>() != null;
        }
        else return false;
    }
    public static bool isProblemEntity(this Entity entity)
    {
        return isProblemEntity(entity.entityType);
    }
    public static bool isDisposableEntity(this EntityType type)
    {
        var entity = Utils.findEntityPrefabByType(type);
        if (entity != null)
        {
            return entity.GetComponent<DisposableEntity>() != null;
        }
        else return false;
    }
    public static bool isDisposableEntity(this Entity entity)
    {
        return isDisposableEntity(entity.entityType);
    }
    public static int entityCount(EntityType type)
    {
        var count = 0;
        foreach (var entity in Utils.findAllEntities())
        {
            if (entity == null) continue;
            if (entity.entityState == EntityState.Disable) continue;
            if (entity.entityType == type) count++;
        }
        return count;
    }
}
