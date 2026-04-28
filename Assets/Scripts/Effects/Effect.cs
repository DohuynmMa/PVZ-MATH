using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EffectType
{
    None,
    ShooterBreakEffect,
    ProblemDamageEffect,
    ClickEffect,
    StarEffect,
    DerivativesEffect,
    RowErrorEffect,
    GameOverEffect,
    ZCubeLandEffect,
    ZCubeBeforeShootEffectAC2,
    ZCubeShootEffectAc1,
    ZCubeShootEffectAc2,
    ZCubeBeamEffectAc2,
    ZCubeBeamHitEffectAc2,
    CellErrorEffect,
    CellErrorEffectBig,
    ZCubeDie
}
public class Effect : MonoBehaviour
{
    public EffectType effectType;
}
