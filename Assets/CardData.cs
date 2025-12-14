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
    HealPlayer,
    DrawCards,
    ReviveUnit,
    FieldEvolve,
    HealUnit
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
