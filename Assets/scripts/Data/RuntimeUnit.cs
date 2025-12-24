using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeUnit
{
    // ==========================================
    // 基础数据
    // ==========================================
    public int Id;
    public string Name;
    public RuntimeCard SourceCard;
    public FieldUnitUI UI;
    public EnemyUnitUI EnemyUI;

    // ==========================================
    // 数值属性
    // ==========================================
    public int BaseMaxHp; // <--- 新增：记录裸装时的最大生命值
    public int MaxHp;     // 当前最大生命值（含装备）
    public int CurrentHp; // 当前生命值

    public int BaseAtk;   // 裸装攻击力
    public int CurrentAtk;// 当前攻击力（含装备）
    public int PermAttackModifier; // 永久攻击力修正
    public int TempAttackModifier; // 临时攻击力修正 (回合结束清零)
    
    // === 新增：显示名称覆盖 (用于Evolve变名) ===
    public string OverrideName;

    public int Attack => CurrentAtk; // 兼容属性

    // ==========================================
    // 状态标签
    // ==========================================
    public bool IsFlying;
    public bool HasTaunt;
    public bool CanAttack;
    public bool IsEvolved;

    public int EvolveTurnsLeft;

    // === 新增：过载系统 (Overload) ===
    public int Overload;       // 当前过载层数 (倒计时)
    public bool IsFatigued;    // 是否处于疲劳状态 (无法攻击，无法使用同色牌)
    public bool RobotEvolved;  // 机器人指挥官特殊进化状态 (攻击力=Overload*2)
    public int PendingOverloadSelfDamage; // 回合结束时结算的自伤 (Double Overload副作用)

    public bool IsDead => CurrentHp <= 0;

    public List<CardData> Equips = new List<CardData>();

    // 构造函数 1：给玩家用
    public RuntimeUnit(int id, RuntimeCard card)
    {
        Id = id;
        SourceCard = card;
        Name = card.Data.cardName;

        // 初始化血量
        int hp = card.Data.unitHealth > 0 ? card.Data.unitHealth : 1;
        BaseMaxHp = hp; // 记录地基
        MaxHp = hp;
        CurrentHp = hp;

        // 初始化攻击
        BaseAtk = card.Data.unitAttack;
        CurrentAtk = BaseAtk;

        IsFlying = card.Data.unitStartsFlying;
        HasTaunt = card.Data.unitHasTaunt;
        CanAttack = false;
        IsEvolved = false;
    }

    // 构造函数 2：给敌人用
    public RuntimeUnit(CardData data)
    {
        Id = -1;
        SourceCard = null;
        Name = data.cardName;

        // 初始化血量
        int hp = data.unitHealth > 0 ? data.unitHealth : 1;
        BaseMaxHp = hp; // 记录地基
        MaxHp = hp;
        CurrentHp = hp;

        // 初始化攻击
        BaseAtk = data.unitAttack;
        if (BaseAtk == 0 && data.value > 0) BaseAtk = data.value;
        CurrentAtk = BaseAtk;

        IsFlying = data.unitStartsFlying;
        HasTaunt = data.unitHasTaunt;
        CanAttack = true;
    }
}