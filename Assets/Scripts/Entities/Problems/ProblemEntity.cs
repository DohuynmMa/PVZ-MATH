using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum ProblemType
{
    Greater,
    Less,
    Equals,
}

public class ProblemEntity : Entity
{
    public float moveSpeedX;
    public float num;
    public ProblemType problemType;
    public Transform bodyTransform;
    public Transform blank;
    public TextMeshPro problemText;

    public Entity parent;

    public bool isRespawn = false;

    public bool participateInCounting = true;
    protected override void enableUpdate()
    {
        base.enableUpdate();
        walk();
        attack();
        clickUpdate();
        updateProblemText();
    }
    public override void transitionToEnable()
    {
        var gm = GameManager.Instance;
        foreach (var sp in Utils.getAllSR(gameObject))
        {
            sp.sortingOrder -= gm.summonedProblemCount * 4;
        }
        if (problemText != null)
        {
            var meshRenderer = problemText.GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "Entity";
            meshRenderer.sortingOrder = bodyTransform.GetComponent<SpriteRenderer>().sortingOrder + 3;
        }
        if (blank != null)
        {
            blank.SetParent(bodyTransform);
        }
        gameObject.layer = HandManager.Instance.currentEntity != null ? 2 : 0;
        base.transitionToEnable();
    }
    private void walk()
    {
        if (anim.GetBool("Attacking") || anim.GetBool("Die")) return;
        if(parent != null)
        {
            if(parent.GetComponent<BossZCube>() != null)
            {
                var zCube = parent.GetComponent<BossZCube>();
                if (zCube.inScanningOrShootingAC2) return;
            }
        }
        transform.Translate((currentQuadrant == 2 || currentQuadrant == 3 ? Vector3.right : Vector3.left) * moveSpeedX * Time.deltaTime * (EntityTools.entityCount(EntityType.CounterclockwiseThink) > 0 ? -1f : 1f));
        //todo 逆时针
    }
    protected virtual void clickUpdate()
    {
        //放大镜显示难题详情
        var gum = GamingUIManager.Instance;
        if (LeftClicked())
        {
            if (problemText != null)
            {
                gum.magnifierText.text = problemText.text;
                Sounds.导入.playWithPitch();
            }
        }
    }
    public override void attack()
    {
        base.attack();
        if (aim == null || aim.hitpoint <= 0)
        {
            anim.SetBool("Attacking", false);
            damageTimer = 0;
            return;
        }
        damageTimer += Time.deltaTime;
        if(damageTimer >= damageDuration)
        {
            damageTimer = 0;
            Instantiate(Utils.findEffectPrefabByType(EffectType.ProblemDamageEffect), aim.getEntityBoxColliderPos(), Quaternion.identity);
            if (attackSound != Sounds.none) attackSound.play();
            aim.setHitpoint(aim.hitpoint - damage);
        }
    }
    public override void entityDie()
    {
        var gm = GameManager.Instance;
        gm.onProblemDie(participateInCounting);
        if (parent != null && parent.hitpoint > 0) parent.setHitpoint(parent.hitpoint - 1);
        base.entityDie();
    }
    public override void flipX(bool specSetting = false, bool positive = true)
    {
        base.flipX(specSetting, positive);
        if (currentQuadrant == 2 || currentQuadrant == 3)
        {
            var ls = problemText.GetComponent<RectTransform>().localScale;
            ls.x = -1f;
            problemText.GetComponent<RectTransform>().localScale = ls;
        }
    }
    public override void OnTriggerStay2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            //碰到格子 检测是否可被击杀
            case "Cell":
                var cell = collider.GetComponent<Cell>();
                currentCell = cell;

                updateRowAndColumn();

                //检测格子象限并对实体进行翻转
                checkQuadrantAndFlipX();

                if (entityState == EntityState.Enable)
                {
                    if (anim.GetBool("Die")) return;
                    switch (problemType)
                    {
                        case ProblemType.Greater:
                            if (cell.num > num)
                            {
                                playDieAnim();
                            }
                            break;
                        case ProblemType.Less:
                            if (cell.num < num)
                            {
                                playDieAnim();
                            }
                            break;
                        case ProblemType.Equals:
                            if (cell.num == num)
                            {
                                playDieAnim();
                            }
                            break;
                    }
                }
                break;
            //碰到敌人 检测是否可以攻击
            case "Entity":
                var entity = collider.GetComponent<Entity>();
                if(entity == null) return;
                if (entity.entityGroup == entityGroup || entity.hitpoint <= 0 || entity.entityState == EntityState.Disable || (entity.currentQuadrant != currentQuadrant && entity.entityType != EntityType.Shooter) || isRespawn) return;
                aim = entity;
                anim.SetBool("Attacking", true);
                break;
        }
    }
    public virtual void setRandomProblem(string difficultF, int difficultB, bool isBarrier = false)
    {
        if (isBarrier)
        {
            num = Random.Range(-60, 61);
            problemType = ProblemType.Equals;
            return;
        }
        switch (difficultF)
        {
            case "NEW":
                switch (difficultB)
                {
                    case 0:
                        num = Random.Range(-10, 11);
                        problemType = Random.Range(0, 2) == 0 ? ProblemType.Greater : ProblemType.Less;
                        break;
                    case 1:
                        num = Random.Range(-30, 31);
                        problemType = Random.Range(0, 2) == 0 ? ProblemType.Greater : ProblemType.Less;
                        break;
                    case 2:
                        var random = Random.Range(0, 3);
                        problemType = random == 0 ? ProblemType.Greater : (random == 1 ? ProblemType.Equals : ProblemType.Less);
                        num = Random.Range(-30, 31);
                        break;
                    case 3:
                        num = Random.Range(-60, 61);
                        problemType = ProblemType.Equals;
                        break;
                }
                break;
        }
    }
    protected virtual void updateProblemText()
    {
        var problemSymbol = "";
        if (problemType == ProblemType.Greater) problemSymbol = ">";
        else if (problemType == ProblemType.Less) problemSymbol = "<";
        else problemSymbol = "=";
        problemText.text = problemSymbol + num;
        problemText.fontSize = 13 - problemText.text.Length * 3;
    }
    public void respawn()
    {
        var gm = GameManager.Instance;
        var row = gm.findAvailableSpawningPlace()[0];
        moveSpeedX = 0;
        aim = null;
        isRespawn = true;
        var accuratePos = new Vector3(18 * (currentQuadrant == 2 || currentQuadrant == 3 ? -1 : 1), (8.1f - (6 - Mathf.Abs(row)) * 1.3f) * (row > 0 ? 1 : -1) - (row < 0 ? 0.2f : 0), 0);
        transform.DOPath(new Vector3[] { accuratePos }, Random.Range(1f, 3f), PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (gm.rowNumUp[6 - Mathf.Abs(row)] == 0 && row > 0)
            {
                respawn();
                return;
            }
            if (row < 0 && gm.rowNumDown[Mathf.Abs(row) - 1] == 0)
            {
                respawn();
                return;
            }
            moveSpeedX = 0.35f;
            isRespawn = false;
        });
    }
    protected void playDieAnim()
    {
        anim.SetBool("Die", true);
        problemText.gameObject.SetActive(false);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            Sounds.P倒地.playWithPitch();
        });
    }
}

