using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一的卡牌效果系统
/// 替代硬编码在各个Manager中的效果逻辑
/// </summary>
public class CardEffectSystem : MonoBehaviour
{
    public static CardEffectSystem Instance;
    
    private BattleManager _battleManager;
    
    // 注册的效果处理器
    private Dictionary<string, Action<EffectContext>> _effectHandlers = new Dictionary<string, Action<EffectContext>>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Init(BattleManager bm)
    {
        _battleManager = bm;
        RegisterBuiltInEffects();
    }
    
    /// <summary>
    /// 注册内置效果（原本硬编码的逻辑）
    /// </summary>
    private void RegisterBuiltInEffects()
    {
        // // 单位矩阵召唤效果
        // RegisterEffect("IdentityMatrix_OnSummon", OnIdentityMatrixSummon);
        
        // // 特征向量攻击效果
        // RegisterEffect("Eigenvector_AttackModifier", OnEigenvectorAttack);
        
        // // 零向量亡语效果
        // RegisterEffect("ZeroVector_Deathrattle", OnZeroVectorDeath);
        
        // 蒸汽收割者击杀效果
        RegisterEffect("SteamReaper_OnKill", OnSteamReaperKill);
        
        // 0/2机器人光环效果
        RegisterEffect("RobotDefender_Aura", OnRobotDefenderAura);
        
        // 蒸汽机器人过载加成
        RegisterEffect("SteamRobot_OverloadBonus", OnSteamRobotOverloadBonus);
        
        // 1/1机器人过载加成触发
        RegisterEffect("TinyRobot_OverloadBooster", OnTinyRobotOverloadBoost);
    }
    
    /// <summary>
    /// 注册一个效果处理器
    /// </summary>
    public void RegisterEffect(string effectKey, Action<EffectContext> handler)
    {
        if (!_effectHandlers.ContainsKey(effectKey))
        {
            _effectHandlers.Add(effectKey, handler);
        }
    }
    
    /// <summary>
    /// 触发效果
    /// </summary>
    public void TriggerEffect(string effectKey, EffectContext context)
    {
        if (_effectHandlers.TryGetValue(effectKey, out var handler))
        {
            handler?.Invoke(context);
        }
    }
    
    // ==================== 内置效果实现 ====================
    
    // private void OnIdentityMatrixSummon(EffectContext ctx)
    // {
    //     if (ctx.SourceUnit == null) return;
        
    //     // 添加3张标准基到牌库
    //     CardData basisData = Resources.Load<CardData>("Data/Card_Unit_StandardBasis");
    //     if (basisData != null)
    //     {
    //         for (int i = 0; i < 3; i++)
    //         {
    //             RuntimeCard newCard = new RuntimeCard(basisData);
    //             _battleManager.DeckManager.DrawPile.Add(newCard);
    //         }
    //         _battleManager.UIManager.Log("【单位矩阵】效果触发：3张标准基已加入牌库。");
    //         _battleManager.DeckManager.ShuffleDeck();
            
    //         // 如果只有这一个单位，检索一张到手牌
    //         int unitCount = 0;
    //         foreach (var s in _battleManager.UnitManager.Slots)
    //         {
    //             if (s != null) unitCount++;
    //         }
            
    //         if (unitCount == 1)
    //         {
    //             RuntimeCard target = _battleManager.DeckManager.DrawPile.Find(
    //                 c => c.Data.cardName.Contains("Standard Basis")
    //             );
    //             if (target != null)
    //             {
    //                 _battleManager.DeckManager.DrawPile.Remove(target);
    //                 _battleManager.DeckManager.AddCardToHand(target);
    //                 _battleManager.UIManager.Log("场上仅有单位矩阵，检索一张标准基入手！");
    //             }
    //         }
    //     }
    // }
    
    // private void OnEigenvectorAttack(EffectContext ctx)
    // {
    //     if (ctx.SourceUnit == null) return;
        
    //     // 如果受到变换影响（有永久攻击加成），伤害翻倍
    //     if (ctx.SourceUnit.PermAttackModifier > 0)
    //     {
    //         ctx.Damage *= 2;
    //         _battleManager.UIManager.Log($"【特征向量】受变换影响，伤害翻倍！({ctx.Damage})");
    //     }
    // }
    
    // private void OnZeroVectorDeath(EffectContext ctx)
    // {
    //     if (ctx.TargetUnit == null || ctx.SourceUnit == null) return;
        
    //     // 攻击者攻击力归零（本回合）
    //     _battleManager.UIManager.Log($"【零向量】效果触发！{ctx.SourceUnit.Name} 攻击力归零 (本回合)。");
    //     ctx.SourceUnit.TempAttackModifier -= ctx.SourceUnit.CurrentAtk;
    //     _battleManager.CombatManager.RecalculateUnitStats(ctx.SourceUnit);
    // }
    
    private void OnSteamReaperKill(EffectContext ctx)
    {
        if (ctx.SourceUnit == null) return;
        
        _battleManager.UIManager.Log($"{ctx.SourceUnit.Name} 击杀触发：自身获得过载 1");
        _battleManager.UnitManager.ModifyOverload(ctx.SourceUnit, 1);
        
        // 找指挥官，给予过载
        var commander = _battleManager.UnitManager.PlayerUnits.Find(u => 
            u.SourceCard != null && u.SourceCard.Data != null && u.SourceCard.Data.isCommander
        );
        if (commander != null && commander != ctx.SourceUnit)
        {
            _battleManager.UIManager.Log($"{ctx.SourceUnit.Name} 击杀触发：指挥官 {commander.Name} 获得过载 1");
            _battleManager.UnitManager.ModifyOverload(commander, 1);
        }
    }
    
    private void OnRobotDefenderAura(EffectContext ctx)
    {
        // 这个效果在 CombatManager.RecalculateUnitStats 中处理
        // 这里只是注册，实际逻辑已经在那里
    }
    
    private void OnSteamRobotOverloadBonus(EffectContext ctx)
    {
        // 这个效果在 CombatManager.RecalculateUnitStats 中处理
        // 过载模式的攻击加成
    }
    
    private void OnTinyRobotOverloadBoost(EffectContext ctx)
    {
        // 当任何友方单位获得过载时，增加1点
        // 这个在 UnitManager.ModifyOverload 中处理
    }
    
    /// <summary>
    /// 检查单位是否匹配某个效果条件
    /// </summary>
    public bool CheckUnitMatchesEffect(RuntimeUnit unit, string effectKey)
    {
        if (unit?.SourceCard?.Data == null) return false;
        
        switch (effectKey)
        {
            case "IdentityMatrix_OnSummon":
                return unit.Name.Contains("Identity Matrix") || unit.Name.Contains("单位矩阵");
                
            case "Eigenvector_AttackModifier":
                return unit.Name.Contains("特征向量") || unit.Name.Contains("Eigenvector");
                
            case "ZeroVector_Deathrattle":
                return unit.Name.Contains("零向量") || unit.Name.Contains("Zero Vector");
                
            case "SteamReaper_OnKill":
                return unit.SourceCard.Data.cardTag == CardTag.Robot 
                    && unit.BaseAtk == 2 && unit.BaseMaxHp == 1;
                    
            case "RobotDefender_Aura":
                return unit.SourceCard.Data.cardTag == CardTag.Robot 
                    && unit.BaseAtk == 0 && unit.BaseMaxHp == 2;
                    
            case "TinyRobot_OverloadBooster":
                return unit.BaseAtk == 1 && unit.BaseMaxHp == 1 
                    && unit.SourceCard.Data.cardTag == CardTag.Robot;
                    
            default:
                return false;
        }
    }


/// <summary>
    /// 计算过载增幅数值
    /// </summary>
    public int CalculateOverloadBoost(RuntimeUnit targetUnit, int baseAmount)
    {
        int boost = 0;
        
        if (_battleManager?.UnitManager == null) return boost;
        
        foreach (var unit in _battleManager.UnitManager.PlayerUnits)
        {
            if (unit == null || unit == targetUnit) continue;
            
            if (CheckUnitMatchesEffect(unit, "TinyRobot_OverloadBooster"))
            {
                boost++;
            }
        }
        
        return boost;
    }
}

/// <summary>
/// 效果上下文，传递效果执行所需的所有信息
/// </summary>
public class EffectContext
{
    public RuntimeUnit SourceUnit;      // 效果来源单位
    public RuntimeUnit TargetUnit;      // 目标单位
    public RuntimeCard SourceCard;      // 效果来源卡牌
    public int Damage;                  // 伤害值（可修改）
    public int Healing;                 // 治疗值
    public bool PreventEffect;          // 是否阻止效果
    public Dictionary<string, object> CustomData; // 自定义数据
    
    public EffectContext()
    {
        CustomData = new Dictionary<string, object>();
    }
}