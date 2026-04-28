using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Entity
{
    public int num = 1;
    public override void shoot(BulletType type)
    {
        base.shoot(type);
        var bulletPrefab = Utils.findBulletPrefabByType(type).GetComponent<ShooterBullet>();
        var accuratePos = this.getEntityBoxColliderPos();
        accuratePos.x += bulletPosOffsetX * (transform.localScale.x >= 0 ? 1 : -1);
        accuratePos.y += bulletPosOffsetY;
        var bullet = Instantiate(bulletPrefab, accuratePos, Quaternion.identity);
        bullet.shooter = this;
        bullet.direction = transform.localScale.x >= 0 ? Vector3.right : Vector3.left;
        bullet.num = num;
        bullet.numText.text = num.ToString();
        bullet.numText.GetComponent<MeshRenderer>().sortingLayerName = "Bullet";
        bullet.numText.GetComponent<MeshRenderer>().sortingOrder = 1;
    }
    public override void entityDie()
    {
        var gm = GameManager.Instance;
        if(row < 0) gm.rowNumDown[Mathf.Abs(row) - 1] = 0;
        else gm.rowNumUp[6 - Mathf.Abs(row)] = 0;

        //生成废弃特效
        Utils.summonEffectDirectly(EffectType.RowErrorEffect,new Vector3(8,this.getEntityBoxColliderPos().y,0));
        Utils.summonEffectDirectly(EffectType.RowErrorEffect, new Vector3(-8, this.getEntityBoxColliderPos().y, 0));

        List<Entity> entities = Utils.findAllEntitiesByRow(row);
        //一旦该行解题中枢被摧毁,该行存活的难题会移动到其他行,继续施压
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            Entity e = entities[i];
            if (e.GetComponent<ProblemEntity>() != null)
            {
                var p = e.GetComponent<ProblemEntity>();
                p.respawn();
                continue;
            }
            if (e != this) e.anim.SetBool("Die", true);
        }
        foreach(var cell in Utils.findAllCellsByRow(row,row > 0 ? 1 : 4))
        {
            cell.GetComponent<BoxCollider2D>().enabled = false;
            cell.gameObject.SetActive(false);
        }
        foreach (var cell in Utils.findAllCellsByRow(row, row > 0 ? 2 : 3))
        {
            cell.GetComponent<BoxCollider2D>().enabled = false;
            cell.gameObject.SetActive(false);
        }
        var tm = TutorialManager.Instance;
        if (tm.needingWaitingSCBroken && row == 6)
        {
            tm.tutorialTemp++;
            tm.needingWaitingSCBroken = false;
            switch (tm.chapter)
            {
                case 0:
                    tm.beginnerTutorial();
                    break;
            }
        }
        base.entityDie();
    }
}
