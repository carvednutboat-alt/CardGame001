using System.Collections.Generic;
using UnityEngine;

// 这是场上的“怪兽/随从”的运行时实例
public class RuntimeUnit
{
    public int Id;
    public string Name;
    public RuntimeCard SourceCard; // 来源卡
    public FieldUnitUI UI;         // 对应的UI引用

    // 基础属性
    public int MaxHp;
    public int CurrentHp;
    public int BaseAtk;
    public int CurrentAtk;

    // 状态
    public bool IsFlying;
    public bool HasTaunt;
    public bool CanAttack;
    public bool IsEvolved;
    public int EvolveTurnsLeft;

    // 装备列表 (存 CardData 即可，除非装备也有动态数值)
    public List<CardData> Equips = new List<CardData>();

    public RuntimeUnit(int id, RuntimeCard card)
    {
        Id = id;
        SourceCard = card;
        Name = card.Data.cardName;

        MaxHp = card.Data.unitHealth > 0 ? card.Data.unitHealth : 1;
        CurrentHp = MaxHp;
        BaseAtk = card.Data.unitAttack;
        CurrentAtk = BaseAtk;

        IsFlying = card.Data.unitStartsFlying;
        HasTaunt = card.Data.unitHasTaunt;
        CanAttack = false; // 刚上场默认不能动(除非有冲锋，这里暂设false)
        IsEvolved = false;
    }

    public bool IsDead => CurrentHp <= 0;
}