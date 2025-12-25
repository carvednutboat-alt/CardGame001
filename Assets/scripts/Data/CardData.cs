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



// 1. 定义枚举：目标类型
public enum CardTargetType
{
    None,       // 不需要目标 (如 AOE, 抽牌)
    Ally,       // 仅限我方单位 (Buff, 治疗, 进化, 勇鸟猛攻)
    Enemy,      // 仅限敌方单位 (火球术, 单体削弱)
    All         // 任何人 (特殊情况)
}

// 2. 新增枚举：卡牌标签 (Family Tag)
public enum CardTag
{
    None,
    MartialArtist, // 千具武 (Red Family)
    Robot,         // 机器人 (New Family)
    LinearAlgebra, // 线性代数
}

// 3. 新增枚举：特殊触发效果 (Deathrattle / OnEquip)
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
    Fly,
    // === NEW EFFECTS (Red Units) ===
    SearchEquipmentOnDeath, // 亡语：检索装备
    SearchFamilyOnEquip,    // 装备时：检索本家
    // === NEW EFFECTS (Robot Units) ===
    GrantOverload,          // 法术：给予过载 (Overload +2)
    DoubleOverload,         // 法术：过载翻倍 (Double Overload)
    LimitOperationEvolve,   // 进化：极限运转 (Change Commander Attack Logic)
    // === NEW EFFECTS (Linear Algebra) ===
    LinearAlgebra_SwapColumns, // 初等行 (列) 变换
    LinearAlgebra_ScalarMult,  // 标量乘法
    LinearAlgebra_Transpose,   // 转置
    LinearAlgebra_GramSchmidt, // 施密特正交化
    
    // === NEW: Spell (Robot) ===
    ReduceOverloadAndAOE, // 减少自己场上unit的一点过载 造成aoe伤害5
    // === Passive / Triggers (Handled in UnitManager logic usually, but defining here for clarity if needed) ===
    // Unit 0/2 Aura: Neighbor +1 HP (Implement in CombatManager stats check)
    // Unit 2/1 Trigger: On Kill -> Gain Overload (Implement in CombatManager/UnitManager)
    // Unit 1/1 Trigger: On Gain Overload -> +1 (Implement in UnitManager)
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
    // === NEW: Special Triggers ===
    public CardEffectType deathEffect = CardEffectType.None;          // 亡语效果
    public CardEffectType onReceiveEquipEffect = CardEffectType.None; // 被装备时触发的效果
    
    public int value = 0;

    [Header("Equipment settings")]
    public bool isEquipment;
    public bool isFieldEquipment;

    [Header("=== 仅当 Kind 为 Unit (敌人) 时配置 ===")]
    public List<CardData> EnemyMoves; // 敌人的技能池（牌库）

    // 2. 新增字段：目标类型
    [Header("Targeting")]
    public CardTag cardTag = CardTag.None; // 卡牌标签
    public CardTargetType targetType = CardTargetType.None;

    [Header("Behavior Flags")]
    public bool startsInDeck;        // if true, this unit goes to Deck/Hand instead of Bench
    public bool isCommander;         // is this unit a Commander?
    public bool dieWithoutCommander; // if true, destroy self at EndTurn if no Commander on field

    public int equipAttackBonus;
    public int equipHealthBonus;

    public bool shieldEffectDamage;
    public bool shieldEffectDestroy;
    public bool shieldBattleDestroy;
    public bool extraAttackOnFailedKill;
}