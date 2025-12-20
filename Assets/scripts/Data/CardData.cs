using System.Collections.Generic;
using UnityEngine;

public enum CardColor
{
    Colorless,
    Red,
    Blue,
    Green
}

public enum CardKind
{
    Unit,
    Spell,
    Evolve
}

public enum CardEffectType
{
    None,
    DamageEnemy,
    DamageAllEnemyUnits,
    DamageAllPlayerUnits,
    HealPlayer,
    DrawCards,
    ReviveUnit,
    FieldEvolve,
    HealUnit,
    UnitBuff,
    Fly
}

// 1. 定义枚举：目标类型
public enum CardTargetType
{
    None,       // 不需要目标 (如 AOE, 抽牌)
    Ally,       // 仅限我方单位 (Buff, 治疗, 进化, 勇鸟猛攻)
    Enemy,      // 仅限敌方单位 (火球术, 单体削弱)
    All         // 任何人 (特殊情况)
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic")]
    public int cost = 0;  // 卡牌费用
    public string cardName;
    public CardKind kind;
    public CardColor color;

    [TextArea]
    public string description;

    [Header("Unit stats")]
    public int unitAttack;
    public int unitHealth;
    public int maxHp => unitHealth;  // maxHp别名,指向unitHealth

    [Header("Unit traits")]
    public bool unitStartsFlying;  // 这只怪物一上场就是起飞状态
    public bool unitHasTaunt;      // 这只怪物自带嘲讽

    [Header("Unit buff options (for UnitBuff cards)")]
    public bool buffGrantFlying;      // 是否让目标单位进入【起飞】状态
    public bool buffFreeAttackNow;    // 是否立刻执行一次额外攻击（不消耗攻击次数）

    [Header("Effect settings (for Spell / Evolve)")]
    public CardEffectType effectType = CardEffectType.None;
    public int value = 0;

    [Header("Equipment settings")]
    public bool isEquipment;
    public bool isFieldEquipment;

    [Header("=== 仅当 Kind 为 Unit (敌人) 时配置 ===")]
    public List<CardData> EnemyMoves; // 敌人的技能池（牌库）

    // 2. 新增字段：目标类型
    [Header("Targeting")]
    public CardTargetType targetType = CardTargetType.None;

    public int equipAttackBonus;
    public int equipHealthBonus;

    public bool shieldEffectDamage;
    public bool shieldEffectDestroy;
    public bool shieldBattleDestroy;
    public bool extraAttackOnFailedKill;
}