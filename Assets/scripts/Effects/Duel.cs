using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

// ============================================
// Lua 专用委托定义
// ============================================
[CSharpCallLua]
public delegate bool RuntimeCardFilter(RuntimeCard card);

/// <summary>
/// 对外暴露给 Lua 的原子 API 集合（模拟游戏王 Duel 命名空间）
/// </summary>
[LuaCallCSharp]
public static class Duel
{
    // ============================================
    // 1. 卡牌操作 (Card Operations)
    // ============================================

    public static RuntimeCard CreateToken(int player, int cardId)
    {
        CardData data = Resources.Load<CardData>($"Data/Card_{cardId}");
        if (data == null) data = Resources.Load<CardData>($"Cards/Card_{cardId}");
        
        if (data == null)
        {
            Debug.LogError($"[Duel API] 找不到 ID 为 {cardId} 的卡牌资源");
            return null;
        }

        RuntimeCard card = new RuntimeCard(data);
        card.Owner = player;
        card.Controller = player;
        return card;
    }

    public static void SendtoDeck(RuntimeCard card, int player, int position, int reason)
    {
        if (BattleManager.Instance?.DeckManager == null || card == null) return;
        
        card.UpdateLocation(Location.DECK, 0);
        var drawPile = BattleManager.Instance.DeckManager.DrawPile;

        switch (position)
        {
            case 0: drawPile.Insert(0, card); break; // 顶部
            case 1: drawPile.Add(card); break;      // 底部
            case 2: // 随机洗入
            default:
                drawPile.Add(card);
                BattleManager.Instance.DeckManager.ShuffleDeck();
                break;
        }
    }

    public static void ShuffleDeck(int player)
    {
        if (BattleManager.Instance?.DeckManager == null) return;
        BattleManager.Instance.DeckManager.ShuffleDeck();
    }

    public static void AddToHand(RuntimeCard card)
    {
        if (BattleManager.Instance == null || card == null) return;
        BattleManager.Instance.DeckManager.AddCardToHand(card);
    }

    public static void SendToGrave(RuntimeCard card)
    {
        if (BattleManager.Instance == null || card == null) return;
        var dm = BattleManager.Instance.DeckManager;
        if (dm.Hand.Contains(card))
        {
            var ui = dm.FindCardUI(card);
            dm.DiscardCard(card, ui);
        }
    }

    public static bool SearchDeckAndAddToHand(RuntimeCardFilter filter)
    {
        if (BattleManager.Instance?.DeckManager == null) return false;
        
        var drawPile = BattleManager.Instance.DeckManager.DrawPile;
        RuntimeCard target = drawPile.Find(c => filter != null && filter(c));
        
        if (target != null)
        {
            drawPile.Remove(target);
            return BattleManager.Instance.DeckManager.AddCardToHand(target);
        }
        return false;
    }

    // ============================================
    // 2. 场地与数值查询 (Queries)
    // ============================================

    public static int GetFieldUnitCount(int player, int location, int range)
    {
        if (BattleManager.Instance?.UnitManager == null) return 0;
        int count = 0;
        foreach (var slot in BattleManager.Instance.UnitManager.Slots)
            if (slot != null) count++;
        return count;
    }

    public static List<RuntimeUnit> GetFieldUnits(int player, int location)
    {
        if (BattleManager.Instance?.UnitManager == null) return new List<RuntimeUnit>();
        return new List<RuntimeUnit>(BattleManager.Instance.UnitManager.PlayerUnits);
    }

    public static int GetUnitCount(bool player = true)
    {
        if (BattleManager.Instance == null) return 0;
        return player ? BattleManager.Instance.UnitManager.PlayerUnits.Count : BattleManager.Instance.EnemyManager.ActiveEnemies.Count;
    }

    public static int GetDeckCount() => BattleManager.Instance?.DeckManager.DrawPile.Count ?? 0;
    public static int GetHandCount() => BattleManager.Instance?.DeckManager.Hand.Count ?? 0;

    // ============================================
    // 3. 单位数值修改 (Unit Modifications)
    // ============================================

    public static void ModifyATK(RuntimeUnit unit, int value, bool permanent)
    {
        if (unit == null) return;
        if (permanent) unit.PermAttackModifier += value;
        else unit.TempAttackModifier += value;
        BattleManager.Instance?.CombatManager?.RecalculateUnitStats(unit);
    }

    public static void ConsolidateTempATK(RuntimeUnit unit)
    {
        if (unit == null) return;
        unit.PermAttackModifier += unit.TempAttackModifier;
        unit.TempAttackModifier = 0;
        BattleManager.Instance?.CombatManager?.RecalculateUnitStats(unit);
    }

    public static void SetBaseMaxHP(RuntimeUnit unit, int value)
    {
        if (unit == null) return;
        unit.BaseMaxHp = value;
        BattleManager.Instance?.CombatManager?.RecalculateUnitStats(unit);
    }

    public static void SetCurrentHP(RuntimeUnit unit, int value)
    {
        if (unit == null) return;
        unit.CurrentHp = Mathf.Clamp(value, 1, unit.MaxHp);
        BattleManager.Instance?.UnitManager?.RefreshUnitUI(unit);
    }

    public static void Heal(RuntimeUnit unit, int amount)
    {
        if (unit == null || amount <= 0) return;
        unit.CurrentHp = Mathf.Min(unit.CurrentHp + amount, unit.MaxHp);
        BattleManager.Instance?.UnitManager?.RefreshUnitUI(unit);
    }

    public static void Damage(RuntimeUnit target, int amount)
    {
        if (BattleManager.Instance == null || target == null) return;
        BattleManager.Instance.CombatManager.ApplyDamage(target, amount);
    }

    public static void DamageAll(int amount, bool playerUnits = false, bool enemyUnits = true)
    {
        if (BattleManager.Instance == null) return;
        if (playerUnits)
            foreach (var u in new List<RuntimeUnit>(BattleManager.Instance.UnitManager.PlayerUnits))
                if (u != null && !u.IsDead) BattleManager.Instance.CombatManager.ApplyDamage(u, amount);
        if (enemyUnits)
            foreach (var e in new List<EnemyManager.RuntimeEnemy>(BattleManager.Instance.EnemyManager.ActiveEnemies))
                if (e?.UnitData != null && !e.UnitData.IsDead) BattleManager.Instance.CombatManager.ApplyDamage(e.UnitData, amount);
    }

    public static void DestroyUnit(RuntimeUnit unit)
    {
        if (BattleManager.Instance == null || unit == null) return;
        BattleManager.Instance.UnitManager.KillUnit(unit);
    }

    public static void SwapUnitPositions(RuntimeUnit unit1, RuntimeUnit unit2)
    {
        if (unit1 == null || unit2 == null || BattleManager.Instance?.UnitManager == null) return;
        var slots = BattleManager.Instance.UnitManager.Slots;
        int idx1 = Array.IndexOf(slots, unit1);
        int idx2 = Array.IndexOf(slots, unit2);
        
        if (idx1 != -1 && idx2 != -1)
        {
            slots[idx1] = unit2;
            slots[idx2] = unit1;
            BattleManager.Instance.UnitManager.ForceRefreshLayout();
        }
    }

    // ============================================
    // 4. 战斗追踪与特殊状态 (Combat & States)
    // ============================================

    private static RuntimeUnit _lastAttacker;
    public static RuntimeUnit GetLastAttacker() => _lastAttacker;
    public static void SetLastAttacker(RuntimeUnit unit) => _lastAttacker = unit;

    public static void DoubleBattleDamage(RuntimeUnit unit)
    {
        // 逻辑提示：此处标记该单位下次伤害翻倍，需在CombatManager的伤害应用中判断此状态
        Log($"{unit.Name} 造成的下一次伤害将翻倍！");
    }

    public static void EvolveUnit(RuntimeUnit unit, string newName, string newNameEn)
    {
        if (unit == null) return;
        unit.IsEvolved = true;
        unit.OverrideName = newName;
        BattleManager.Instance?.CombatManager?.RecalculateUnitStats(unit);
        BattleManager.Instance?.UnitManager?.RefreshUnitUI(unit);
    }

    public static void AddOverload(RuntimeUnit unit, int amount)
    {
        if (BattleManager.Instance == null || unit == null) return;
        BattleManager.Instance.UnitManager.ModifyOverload(unit, amount);
    }

    public static void RemoveOverload(RuntimeUnit unit, int amount)
    {
        if (BattleManager.Instance == null || unit == null) return;
        BattleManager.Instance.UnitManager.ModifyOverload(unit, -amount);
    }

    public static void GrantPlayerImmuneToEffects(int player, bool immune)
    {
        if (BattleManager.Instance?.UnitManager == null) return;
        BattleManager.Instance.UnitManager.PlayerImmuneToEffects = immune;
    }

    // ============================================
    // 5. 目标选择与交互 (Targeting & UI)
    // ============================================

    private static RuntimeUnit _lastSelection;
    public static void SetSelection(RuntimeUnit unit) => _lastSelection = unit;
    public static RuntimeUnit GetFirstTarget() => _lastSelection;
    public static void ClearSelection() => _lastSelection = null;

    public static void SelectTarget(RuntimeCard c, CardTargetType targetType)
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.InitiateEffectTargeting(c, targetType);
    }

    // 重载版本，支持 Lua 简单的参数调用
    public static void SelectTarget(int player, int location, int min, int max)
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.IsTargetingMode = true;
    }

    public static void Log(string msg)
    {
        if (BattleManager.Instance?.UIManager != null)
            BattleManager.Instance.UIManager.Log($"[Lua] {msg}");
        else
            Debug.Log($"[Lua] {msg}");
    }

    // ============================================
    // 6. 效果系统集成 (Effect System)
    // ============================================
    
    public static void TriggerEffect(string effectKey, EffectContext context)
    {
        CardEffectSystem.Instance?.TriggerEffect(effectKey, context);
    }

    public static void RegisterEffect(string effectKey, Action<EffectContext> handler)
    {
        CardEffectSystem.Instance?.RegisterEffect(effectKey, handler);
    }
}