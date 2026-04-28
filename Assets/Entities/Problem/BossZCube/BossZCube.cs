using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public enum ZCubeState
{ 
    None,
    Shooting,
    ChangingLW,
}
public class BossZCube : ProblemEntity
{
    public BossZCubeSquare squarePrefab;
    public BossZCubeBeamBlackHole blackHolePrefab;
    public BossZCubeChain chainPrefab;
    public GameObject bigEyePrefab;
    public GameObject implosionPrefab;
    public List<BossZCubeSquare> summonedSquares;

    public List<LineRenderer> scannerLines;
    public GameObject scannerAim;
    public GameObject cube;

    public Image hpBarOuter;
    public Image hpBarInner;
    public Image hpBarIcon;
    public bool enableHpBar = false;

    public int squareCount = 3;

    public LineRenderer beam;
    public GameObject bigEye;
    public GameObject implosion;

    public ZCubeState state;
    public float changeStateDuration = 10;
    private float changeStateTimer;
    public float spawnProblemDuration = 12;
    private float spawnProblemTimer;
    private float moveTimer;
    public int actor = 1;
    public bool inScanningOrShootingAC2 = false;
    public bool dying = false;

    private readonly List<EntityType> spawnableProblemTypes = new List<EntityType>
    {
        EntityType.Problem,EntityType.NTimeFunctionProblem,EntityType.CompositionProblem,EntityType.SegmentRelationshipProblem
    };
    private void Start()
    {
        for (int i = 0; i < squareCount; i++)
        {
            var square = Instantiate(squarePrefab,Vector3.zero,Quaternion.identity,transform);
            summonedSquares.Add(square);
        }
        bigEye = Instantiate(bigEyePrefab);
        implosion = Instantiate(implosionPrefab);
        implosion.SetActive(false);

        GameManager.Instance.objsInGame.Add(implosion);
        GameManager.Instance.objsInGame.Add(bigEye);
    }
    public override void transitionToEnable()
    {
        onSpawn();
    }
    protected override void enableUpdate()
    {
        if (hitpoint <= 0) entityDie();
        updateHpBar();
        changeStateTimer += Time.deltaTime;
        moveTimer += Time.deltaTime;
        spawnProblemTimer += Time.deltaTime;
        if (changeStateTimer >= changeStateDuration)
        {
            updateState();
            changeStateTimer = 0;
        }
        if(moveTimer >= 15)
        {
            moveToRandomPosition();
            moveTimer = 0;
        }
        if(spawnProblemTimer >= spawnProblemDuration)
        {
            spawnAndEnhanceARandomProblem();
            spawnProblemTimer = 0;
        }
        switch (state)
        {
            case ZCubeState.Shooting:
                shootingAC1Update();
                break;
        }
    }
    public override void setHitpoint(float hitpoint)
    {
        this.hitpoint = hitpoint <= maxHitpoint ? hitpoint : maxHitpoint;
        if(this.hitpoint / maxHitpoint <= 0.5f && actor != 2)
        {
            transitionToAct2();
        }
        if (this.hitpoint <= 0)
        {
            anim.SetBool("Die", true);
        }
    }
    private void updateState()
    {
        state = (ZCubeState)Random.Range(1, 4);
        switch (state)
        {
            case ZCubeState.None:
                beam.gameObject.SetActive(false);
                break;
            case ZCubeState.Shooting:
                if(actor == 1)
                {
                    var wid = actor > 1 ? 1 : 0.5f;
                    beam.startWidth = 0;
                    beam.endWidth = wid;
                    damage = wid * 0.4f;
                }
                else
                {
                    scanAndShoot();
                }
                break;
            case ZCubeState.ChangingLW:
                if(actor < 2)
                {
                    updateState();
                    break;
                }
                beam.gameObject.SetActive(false);
                var randomQuardant = Random.Range(1, 5);
                Vector3 pos = Vector3.zero;
                switch (randomQuardant)
                {
                    case 1:
                        pos = new Vector3(8.6F,4.85f,0.1f);
                        break;
                    case 2:
                        pos = new Vector3(-8.6F, 4.85f, 0.1f);
                        break;
                    case 3:
                        pos = new Vector3(-8.6F, -4.85f, 0.1f);
                        break;
                    case 4:
                        pos = new Vector3(8.6F, -4.85f, 0.1f);
                        break;
                }
                Utils.summonEffectDirectly(EffectType.CellErrorEffectBig, pos);
                Sounds.ZCUBEπ ’œ.playWithPitch();
                foreach(var cell in Utils.findAllCells(randomQuardant))
                {
                    if(cell == null) continue;
                    cell.setNum(Random.Range(-100,100));
                }
                break;
        }
    }
    private void shootingAC1Update()
    {
        aim = findRandomAim();
        if (aim == null || state != ZCubeState.Shooting || aim.hitpoint <= 0 || actor >= 2)
        {
            beam.gameObject.SetActive(false);
            return;
        }
        shootAC1();
    }
    private void spawnAndEnhanceARandomProblem()
    {
        var gm = GameManager.Instance;
        var place = gm.findAvailableSpawningPlace();
        var row = place[0];
        int randomQua = place[1];
        var randomType = spawnableProblemTypes[Random.Range(0, spawnableProblemTypes.Count)];

        var p = gm.spawnRandomProblemByDifficult(gm.difficultF, gm.difficultB, randomType, row, randomQua);
        p.participateInCounting = false;
        p.moveSpeedX = actor > 1 ? 0.45f : 0.35f;
        p.damage = actor > 1 ? 2 : 1;
        p.parent = this;

        var chain = Instantiate(chainPrefab, Vector3.zero, Quaternion.identity, transform);
        chain.parent = this;
        chain.aim = p;
        chain.enableChain = true;
        if(actor > 1) chain.line.material = ResourceManager.Instance.materials[0];
    }
    private ThinkEntity findRandomAim()
    {
        var aims = Utils.findAllEntities();
        foreach (var aim in aims)
        {
            if(aim == null) continue;
            if(aim.entityGroup == entityGroup || aim.hitpoint <= 0 || aim.GetComponent<ThinkEntity>() == null || aim.entityState == EntityState.Disable) continue;
            return aim.GetComponent<ThinkEntity>();
        }
        return null;
    }
    private void shootAC1()
    {
        if (aim.hitpoint <= 0 || aim == null || state != ZCubeState.Shooting || actor > 1)
        {
            return;
        }
        beam.gameObject.SetActive(true);
        beam.SetPosition(0, transform.localPosition);
        beam.SetPosition(1, aim.getEntityBoxColliderPos());
        attack();
    }
    private void transitionToAct2()
    {
        var rm = ResourceManager.Instance;

        actor = 2;

        spawnProblemDuration = 11;

        SoundsManager.stopMusic();

        //ªª…´
        for (int i = 0; i < squareCount; i++)
        {
            var square = Instantiate(squarePrefab, Vector3.zero, Quaternion.identity, transform);
            summonedSquares.Add(square);
        }
        var material = rm.materials[0];
        material.color = Color.white;
        foreach (var sp in Utils.getAllSR(gameObject))
        {
            sp.material = material;
        }
        foreach (var s in summonedSquares)
        {
            s.GetComponent<SpriteRenderer>().material = material;
        }
        hpBarOuter.material = material;
        hpBarInner.material = material;
        hpBarIcon.material = material;
        hpBarOuter.material.shader = Shader.Find("UI/Default");
        hpBarInner.material.shader = Shader.Find("UI/Default");
        hpBarIcon.material.shader = Shader.Find("UI/Default");

        beam.material = material;
        bigEye.GetComponent<SpriteRenderer>().material = material;

        var currentScale = transform.localScale;
        transform.DOScale(currentScale * 1.5f, Random.Range(1f, 3f));
        material.DOColor(new Color(1, 88f / 255f, 88f / 255f), 3).OnComplete(() =>
        {
            Musics.ZCubeAC2.play(true);
        });

    }
    private void moveToRandomPosition()
    {
        var randomPos = new Vector3(Random.Range(-14f, 14f), Random.Range(-8f, 8f), -3);
        var delay = Random.Range(1f, 3f);
        transform.DOPath(new Vector3[] { randomPos }, delay, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    private void onSpawn()
    {
        transitionToDisable();
        changeStateTimer = 20;
        enableHpBar = false;
        anim.enabled = true;
        actor = 1;
        entityGroup = EntityGroup.Enemy;
        state = ZCubeState.None;
        transform.position = new Vector3(0, 20, 0);
        transform.DOPath(new Vector3[] { Vector3.zero }, 0.5f).OnComplete(() =>
        {
            Sounds.ZCUBEπ ’œ.playWithPitch();
            Sounds.ZCubeº§π‚ª˜÷–AC2.playWithPitch();
            Utils.summonEffectDirectly(EffectType.ZCubeLandEffect, Vector3.zero);
            Camera.main.GetComponent<CameraShakeUtility>().Shake(0.5f, 0.5f);
            Camera.main.transform.DORotate(new Vector3(-15, 0, 0), 1).OnComplete(() =>
            {
                entityState = EntityState.Enable;
                Camera.main.GetComponent<FollowMouseTool>().enabled = true;
                Musics.ZCubeAC1.play(true);
            });
        });
    }
    private void scanAndShoot()
    {
        var cm = CameraManager.Instance;
        var aim = findRandomAim();
        var cameraShake = Camera.main.GetComponent<CameraShakeUtility>();
        var comicEffect = Camera.main.GetComponent<ComicFlashEffect>();
        if (aim == null)
        {
            foreach (var l in scannerLines)
            {
                l.gameObject.SetActive(false);
            }
            return;
        }
        scannerAim.transform.position = Vector3.zero;
        scannerAim.SetActive(false);
        beam.gameObject.SetActive(false);
        state = ZCubeState.None;
        inScanningOrShootingAC2 = true;
        GamingUIManager.Instance.dangerousBound.gameObject.SetActive(true);
        GamingUIManager.Instance.dangerousBound.UI_FadeIn_Event();
        bigEye.GetComponent<SpriteRenderer>().DOColor(new Color32(255, 255, 255, 0), 1f).OnComplete(() =>
        {
            //…®√Ë 4.5s
            Sounds.ZCube…®√ËAC2.playWithPitch();
            bigEye.GetComponent<SpriteRenderer>().sortingLayerName = "EffectB";
            bigEye.transform.localScale = Vector3.one * 2.3f;
            bigEye.GetComponent<SpriteRenderer>().DOColor(new Color32(75, 75, 75, 255), 1f);
            scannerAim.SetActive(true);
            var e1 = Utils.summonEffectDirectly(EffectType.ZCubeBeforeShootEffectAC2, Vector3.zero);
            e1.transform.SetParent(transform);
            e1.transform.localPosition = Vector3.zero;
            foreach (var l in scannerLines)
            {
                l.gameObject.SetActive(true);
                l.SetPosition(0, transform.localPosition);
                l.SetPosition(1, transform.localPosition);
                var currentPos = aim.getEntityBoxColliderPos();
                var temp = scannerLines.IndexOf(l);
                currentPos.x -= temp <= 1 ? 0.6f : -0.6f;
                currentPos.y -= temp == 0 || temp == 3 ? -0.6f : 0.6f;
                DOVirtual.Vector3(transform.localPosition, currentPos, 3.5f, value =>
                {
                    Vector3[] positions = new Vector3[l.positionCount];
                    l.GetPositions(positions);
                    positions[1] = value;
                    l.SetPositions(positions);
                }).SetEase(Ease.OutCubic);
            }
            scannerAim.transform.DOPath(new Vector3[] { aim.getEntityBoxColliderPos() }, 4.5f).OnComplete(() =>
            {
                Sounds.ZCubeº§π‚–Ó¡¶AC2.playWithPitch();
                Sounds.ZCubeº§π‚∑¢…‰AC2.playWithPitch();
                cube.SetActive(false);
                var e2 = Utils.summonEffectDirectly(EffectType.ZCubeBeamEffectAc2, Vector3.zero);
                e2.transform.SetParent(transform);
                e2.transform.LookAt(aim.getEntityBoxColliderPos());
                var e3 = Utils.summonEffectDirectly(EffectType.ZCubeShootEffectAc1,aim.getEntityBoxColliderPos());
                e3.transform.localScale *= 5;
                beam.gameObject.SetActive(true);
                beam.SetPosition(0, transform.localPosition);
                beam.SetPosition(1, aim.getEntityBoxColliderPos());
                beam.startWidth = 0;
                beam.endWidth = 0;
                changeBeamMiddleWidth(beam, 0.5f, 0);
                cameraShake.Shake(0.4f,1);
                Sounds.ZCubeº§π‚ª˜÷–AC2.playWithPitch();
                //ª˜÷–
                foreach (var l in scannerLines)
                {
                    l.gameObject.SetActive(false);
                }
                implosion.transform.localScale = Vector3.one * 0.1f;
                implosion.transform.position = aim.getEntityBoxColliderPos();
                Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                DOVirtual.DelayedCall(1.2f, () =>
                {
                    //º§π‚∫‰’®
                    Sounds.ZCubeº§π‚√¸÷–∫Û±¨’®.playWithPitch();
                    cameraShake.Shake(0.6f, 1);
                    comicEffect.enableEffect = true;
                    implosion.SetActive(true);
                    implosion.GetComponent<SpriteRenderer>().DOColor(new Color32(255, 255, 255, 255), 0.5f);
                    implosion.transform.DORotate(new Vector3(0, 0, Random.Range(-7200f, 7200f)), 3).OnComplete(() =>
                    {
                        cameraShake.Shake(1f, 1);
                        Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                        Sounds.ZCubeº§π‚ª˜÷–AC2.playWithPitch();
                        Sounds.ZCubeº§π‚√¸÷–∫Û±¨’®.playWithPitch();
                        implosion.GetComponent<SpriteRenderer>().DOColor(new Color32(255, 255, 255, 0), 2f).OnComplete(() =>
                        {
                            GamingUIManager.Instance.dangerousBound.UI_FadeOut_Event();
                            GamingUIManager.Instance.dangerousBound.onFadeInOrFadeOut += () =>
                            {
                                GamingUIManager.Instance.dangerousBound.gameObject.SetActive(false);
                            };
                            implosion.SetActive(false);
                            cube.SetActive(true);
                            cameraShake.Shake(0.5f, 0.3f);
                            comicEffect.enableEffect = true;
                            //π ’œ
                            Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                            Sounds.ZCubeº§π‚√¸÷–∫Û±¨’®.playWithPitch();
                            Sounds.ZCUBEπ ’œ.playWithPitch();
                            Camera.main.GetComponent<InvertCamera>().invertColors = !Camera.main.GetComponent<InvertCamera>().invertColors;
                            DOVirtual.DelayedCall(0.9f, () =>
                            {
                                //π ’œ
                                Sounds.ZCUBEπ ’œ.playWithPitch();
                                comicEffect.enableEffect = false;
                                Camera.main.GetComponent<InvertCamera>().invertColors = !Camera.main.GetComponent<InvertCamera>().invertColors;
                                inScanningOrShootingAC2 = false;
                                //end
                            });
                        });
                    });
                    implosion.transform.DOScale(Vector3.one * 30, 3f);
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        Camera.main.GetComponent<InvertCamera>().invertColors = !Camera.main.GetComponent<InvertCamera>().invertColors;
                        Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                        Sounds.ZCUBEπ ’œ.playWithPitch();
                        cameraShake.Shake(2f, 1f);
                    });
                    DOVirtual.DelayedCall(0.7f, () =>
                    {
                        comicEffect.enableEffect = false;
                        Camera.main.GetComponent<InvertCamera>().invertColors = !Camera.main.GetComponent<InvertCamera>().invertColors;
                        Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                        Sounds.ZCUBEπ ’œ.playWithPitch();
                        cameraShake.Shake(2f, 1f);
                    });
                });
                changeBeamMiddleWidth(beam, 1.5f, 1,() =>
                {
                    changeBeamMiddleWidth(beam, 0, 0.3f, () =>
                    {
                        Utils.summonEffectDirectly(EffectType.ZCubeBeamHitEffectAc2, findRandomAim().getEntityBoxColliderPos());
                        Sounds.ZCubeº§π‚ª˜÷–AC2.playWithPitch();
                        scannerAim.transform.position = Vector3.zero;
                        scannerAim.SetActive(false);
                        beam.gameObject.SetActive(false);
                        Instantiate(blackHolePrefab, aim.getEntityBoxColliderPos(), Quaternion.identity);
                    });
                    bigEye.GetComponent<SpriteRenderer>().DOColor(new Color32(255, 255, 255, 0), 1f).OnComplete(() =>
                    {
                        bigEye.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
                        bigEye.transform.localScale = Vector3.one * 1.5f;
                        bigEye.GetComponent<SpriteRenderer>().DOColor(new Color32(255, 255, 255, 9), 1f);
                    });
                });
            });
        });
    }
    public override void OnTriggerStay2D(Collider2D collider)
    {
    }
    public override void OnTriggerExit2D(Collider2D collider)
    {
    }
    protected override void updateProblemText()
    {

    }
    public override void flipX(bool specSetting = false, bool positive = true)
    {
    }
    public override void setRandomProblem(string difficultF, int difficultB, bool isBarrier = false)
    {
    }
    public override void attack()
    {
        if (aim == null || aim.hitpoint <= 0)
        {
            anim.SetBool("Attacking", false);
            beam.gameObject.SetActive(false);
            damageTimer = 0;
            return;
        }
        beam.gameObject.SetActive(true);
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageDuration)
        {
            damageTimer = 0;
            Instantiate(Utils.findEffectPrefabByType(EffectType.ProblemDamageEffect), aim.getEntityBoxColliderPos(), Quaternion.identity);
            Utils.summonEffectDirectly(EffectType.ZCubeShootEffectAc1, aim.getEntityBoxColliderPos());
            if (attackSound != Sounds.none) attackSound.play();
            aim.setHitpoint(aim.hitpoint - damage);
        }
    }
    public override void entityDie()
    {
        if (dying) return;
        var gm = GameManager.Instance;
        var comicEffect = Camera.main.GetComponent<ComicFlashEffect>();
        var invertEffect = Camera.main.GetComponent<InvertCamera>();
        Utils.summonEffectDirectly(EffectType.ZCubeBeforeShootEffectAC2,this.getEntityBoxColliderPos());
        Camera.main.GetComponent<CameraShakeUtility>().Shake(1.5f, 1.5f);
        dying = true;
        Sounds.ZCubeº§π‚–Ó¡¶AC2.playWithPitch();
        DOVirtual.DelayedCall(1f, () =>
        {
            Utils.summonEffectDirectly(EffectType.ZCubeDie, this.getEntityBoxColliderPos());
            comicEffect.enableEffect = true;
            invertEffect.invertColors = !invertEffect.invertColors;
            Utils.summonEffectDirectly(EffectType.ZCubeBeforeShootEffectAC2, this.getEntityBoxColliderPos());
            Utils.summonEffectDirectly(EffectType.ZCubeBeforeShootEffectAC2, this.getEntityBoxColliderPos());
            Sounds.ZcubeÀ¿Õˆ±¨’®.playWithPitch();
            Sounds.ZCUBEπ ’œ.playWithPitch();
            DOVirtual.DelayedCall(0.8f, () =>
            {
                Sounds.ZCUBEπ ’œ.playWithPitch();
                Camera.main.GetComponent<CameraShakeUtility>().Shake(0.4f, 0.5f);
                comicEffect.enableEffect = false;
                invertEffect.invertColors = !invertEffect.invertColors;
                Utils.summonEffectDirectly(EffectType.ZCubeDie, this.getEntityBoxColliderPos());
                gm.onProblemDie(participateInCounting);
                if (dieSound != Sounds.none) dieSound.play();
                Destroy(bigEye);
                Destroy(implosion);
                Destroy(gameObject);
            });
        });
    }
    private LineRenderer changeBeamMiddleWidth(LineRenderer beam, float targetWidth, float duration, Action action = null)
    {
        AnimationCurve widthCurve = beam.widthCurve ?? new AnimationCurve();
        if (widthCurve.keys.Length < 3)
        {
            widthCurve = new AnimationCurve(
                new Keyframe(0, beam.startWidth),  
                new Keyframe(0.5f, 0),
                new Keyframe(1, beam.endWidth)
            );
            beam.widthCurve = widthCurve;
        }
        float currentMW = widthCurve.keys[1].value;
        DOTween.To(() => currentMW, x =>
        {
            var keyframes = widthCurve.keys;
            keyframes[1] = new Keyframe(0.5f, x);
            widthCurve.keys = keyframes;
            beam.widthCurve = widthCurve;
        }, targetWidth, duration)
        .SetEase(Ease.InOutSine)
        .OnComplete(() => { action?.Invoke(); });
        return beam;
    }
    private void updateHpBar()
    {
        if (enableHpBar)
        {
            hpBarInner.fillAmount = hitpoint / maxHitpoint;
        }
    }
    private void disableAnimByAnim()
    {
        anim.enabled = false;
        enableHpBar = true;
        hpBarOuter.color = new Color32(255, 255, 255, 123);
        hpBarInner.color = new Color32(255, 255, 255, 123);
        hpBarIcon.color = new Color32(255, 255, 255, 213);
    }

}
