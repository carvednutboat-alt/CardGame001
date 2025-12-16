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
    UnitBuff
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic")]
    public string cardName;
    public CardKind kind;
    public CardColor color;

    [TextArea]
    public string description;

    [Header("Unit stats")]
    public int unitAttack;
    public int unitHealth;

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

    public int equipAttackBonus;
    public int equipHealthBonus;

    public bool shieldEffectDamage;
    public bool shieldEffectDestroy;
    public bool shieldBattleDestroy;
    public bool extraAttackOnFailedKill;
}
