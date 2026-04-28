using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BulletType
{
    ShooterBullet,
}
public class Bullet : MonoBehaviour
{
    public Entity shooter;//Entity发射后会设置
    public BulletType bulletType;
    public bool isBreakBullet = false;
    public bool damageOwnEntities = false;
    public bool limitingDamageEntity = false;
    public List<EntityType> canDamageEntityTypes;
    public float flyingSpeed = 0.5f;
    public float damage = 0;
    public EffectType breakEffectType;
    public Sounds hitSound = Sounds.none;
    private  List<Entity> damagedEntity = new List<Entity>();
    private void Update()
    {
        bulletFlying();
    }
    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag != "Entity") return;
        var entity = collider.GetComponent<Entity>();
        if (entity == null) return;
        if(entity == shooter || (limitingDamageEntity && !canDamageEntityTypes.Contains(entity.entityType))) return;
        if ((damageOwnEntities || shooter.entityGroup != entity.entityGroup) && !damagedEntity.Contains(entity) && entity.entityState == EntityState.Enable)
        {
            damagedEntity.Add(entity);
            onHit(entity);
            if(hitSound != Sounds.none) hitSound.playWithPitch();
            entity.setHitpoint(entity.hitpoint - damage);
            if (isBreakBullet)
            {
                breakBullet();
            }
        }
    }
    public virtual void breakBullet()
    {
        if(breakEffectType != EffectType.None)
        {
            Instantiate(Utils.findEffectPrefabByType(breakEffectType),transform.position,Quaternion.identity);
        }
        Destroy(gameObject);
    }
    public virtual void bulletFlying()
    {
        if (!transform.position.x.numInRange(-20,20))
        {
            breakBullet();
        }
    }
    public virtual void onHit(Entity entity)
    {

    }
}
