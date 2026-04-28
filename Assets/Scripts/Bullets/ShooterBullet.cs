using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShooterBullet : Bullet
{
    public int num;
    public TextMeshPro numText;
    public Vector3 direction = Vector3.right;//·¢ÉäÕßµ÷¿Ø
    public override void bulletFlying()
    {
        base.bulletFlying();
        transform.Translate(direction * flyingSpeed * Time.deltaTime);
    }
    public override void onHit(Entity entity)
    {
        base.onHit(entity);
        var tm = TutorialManager.Instance;
        if (entity.currentCell != null)
        {
            switch (entity.entityType)
            {
                case EntityType.Plus:
                    if (tm.needingBulletHitThePlusEntity)
                    {
                        tm.tutorialTemp++;
                        tm.needingBulletHitThePlusEntity = false;
                        switch (tm.chapter)
                        {
                            case 0:
                                tm.beginnerTutorial();
                                break;
                        }
                    }
                    entity.currentCell.setNum(entity.currentCell.num + num);
                    break;
                case EntityType.Substract:
                    entity.currentCell.setNum(entity.currentCell.num - num);
                    break;
                case EntityType.Multiply:
                    entity.currentCell.numerator *= num ;
                    break;
                case EntityType.Divide:
                    entity.currentCell.denominator *= num;
                    break;
            }
        }
    }
}
