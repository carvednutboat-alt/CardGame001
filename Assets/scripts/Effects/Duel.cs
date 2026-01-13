using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// Atomic API exposed to Lua for game operations.
/// "Duel" namespace mimicry.
/// 增强版本 - 提供更完整的YGO风格API
/// </summary>
[LuaCallCSharp]
public static class Duel
{
    // ============================================
    // TARGET SELECTION
    // ============================================
    
    public static void SelectTarget(RuntimeCard c, CardTargetType targetType)
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.InitiateEffectTargeting(c, targetType);
        }
    }

    // ============================================
    // DAMAGE & RECOVERY
    // ============================================

    public static void Damage(RuntimeUnit target, int amount)
    {
        if (BattleManager.Instance == null || target == null) return;
        BattleManager.Instance.CombatManager.ApplyDamage(target, amount);
    }
    
    public static void Recover(RuntimeUnit target, int amount)
    {
        if (BattleManager.Instance == null || target == null) return;
        BattleManager.Instance.CombatManager.ApplyHeal(target, amount);
    }
    
    public static void DamageAll(int amount, bool playerUnits = false, bool enemyUnits = true)
    {
        if (BattleManager.Instance == null) return;
        
        if (playerUnits)
        {
            foreach (var unit in BattleManager.Instance.UnitManager.PlayerUnits)
            {
                if (unit != null && !unit.IsDead)
                {
                    BattleManager.Instance.CombatManager.ApplyDamage(unit, amount);
                }
            }
        }
        
        if (enemyUnits)
        {
            foreach (var enemy in BattleManager.Instance.EnemyManager.ActiveEnemies)
            {
                if (enemy != null && enemy.UnitData != null && !enemy.UnitData.IsDead)
                {
                    BattleManager.Instance.CombatManager.ApplyDamage(enemy.UnitData, amount);
                }
            }
        }
    }

    // ============================================
    // CARD OPERATIONS
    // ============================================

    public static void Draw(int player, int amount)
    {
        if (BattleManager.Instance == null) return;
        if (player == 0)
        {
            BattleManager.Instance.DeckManager.DrawCards(amount);
        }
    }
    
    public static void ShuffleDeck(int player)
    {
        if (BattleManager.Instance == null) return;
        if (player == 0)
        {
            BattleManager.Instance.DeckManager.ShuffleDeck();
        }
    }

    public static void SendToGrave(RuntimeCard card)
    {
        if (BattleManager.Instance == null || card == null) return;
        
        if (BattleManager.Instance.DeckManager.Hand.Contains(card))
        {
            var ui = BattleManager.Instance.DeckManager.FindCardUI(card);
            BattleManager.Instance.DeckManager.DiscardCard(card, ui);
        }
    }
    
    public static void AddToHand(RuntimeCard card)
    {
        if (BattleManager.Instance == null || card == null) return;
        BattleManager.Instance.DeckManager.AddCardToHand(card);
    }
    
    public static RuntimeCard SearchDeck(Func<CardData, bool> predicate)
    {
        if (BattleManager.Instance == null) return null;
        
        foreach (var card in BattleManager.Instance.DeckManager.DrawPile)
        {
            if (card != null && card.Data != null && predicate(card.Data))
            {
                return card;
            }
        }
        return null;
    }
    
    public static bool SearchAndAddToHand(Func<CardData, bool> predicate)
    {
        RuntimeCard card = SearchDeck(predicate);
        if (card != null)
        {
            BattleManager.Instance.DeckManager.DrawPile.Remove(card);
            AddToHand(card);
            return true;
        }
        return false;
    }

    // ============================================
    // UNIT OPERATIONS
    // ============================================
    
    public static void DestroyUnit(RuntimeUnit unit)
    {
        if (BattleManager.Instance == null || unit == null) return;
        BattleManager.Instance.UnitManager.KillUnit(unit);
    }
    
    public static void ModifyATK(RuntimeUnit unit, int amount, bool permanent = false)
    {
        if (unit == null) return;
        
        if (permanent)
        {
            unit.PermAttackModifier += amount;
        }
        else
        {
            unit.TempAttackModifier += amount;
        }
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.CombatManager.RecalculateUnitStats(unit);
        }
    }
    
    public static void ModifyHP(RuntimeUnit unit, int amount)
    {
        if (unit == null) return;
        
        unit.BaseMaxHp += amount;
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.CombatManager.RecalculateUnitStats(unit);
        }
    }
    
    public static void Evolve(RuntimeUnit unit)
    {
        if (unit == null) return;
        
        unit.IsEvolved = true;
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.CombatManager.RecalculateUnitStats(unit);
            BattleManager.Instance.UIManager.Log($"{unit.Name} 进化了！");
        }
    }
    
    // ============================================
    // OVERLOAD SYSTEM
    // ============================================
    
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
    
    // ============================================
    // QUERY OPERATIONS
    // ============================================
    
    public static List<RuntimeUnit> GetPlayerUnits()
    {
        if (BattleManager.Instance == null) return new List<RuntimeUnit>();
        return new List<RuntimeUnit>(BattleManager.Instance.UnitManager.PlayerUnits);
    }
    
    public static List<RuntimeUnit> GetEnemyUnits()
    {
        if (BattleManager.Instance == null) return new List<RuntimeUnit>();
        
        List<RuntimeUnit> units = new List<RuntimeUnit>();
        foreach (var enemy in BattleManager.Instance.EnemyManager.ActiveEnemies)
        {
            if (enemy != null && enemy.UnitData != null)
            {
                units.Add(enemy.UnitData);
            }
        }
        return units;
    }
    
    public static int GetUnitCount(bool player = true)
    {
        if (BattleManager.Instance == null) return 0;
        
        if (player)
        {
            return BattleManager.Instance.UnitManager.PlayerUnits.Count;
        }
        else
        {
            return BattleManager.Instance.EnemyManager.ActiveEnemies.Count;
        }
    }
    
    public static int GetDeckCount()
    {
        if (BattleManager.Instance == null) return 0;
        return BattleManager.Instance.DeckManager.DrawPile.Count;
    }
    
    public static int GetHandCount()
    {
        if (BattleManager.Instance == null) return 0;
        return BattleManager.Instance.DeckManager.Hand.Count;
    }
    
    // ============================================
    // SELECTION & TARGETING
    // ============================================
    
    private static RuntimeUnit _lastSelection;
    public static void SetSelection(RuntimeUnit unit)
    {
        _lastSelection = unit;
    }
    
    public static RuntimeUnit GetFirstTarget()
    {
        return _lastSelection;
    }
    
    public static void ClearSelection()
    {
        _lastSelection = null;
    }

    // ============================================
    // UTILITY
    // ============================================
    
    public static void Log(string msg)
    {
        if (BattleManager.Instance != null && BattleManager.Instance.UIManager != null)
        {
            BattleManager.Instance.UIManager.Log(msg);
        }
        else
        {
            Debug.Log("[Duel] " + msg);
        }
    }
    
    public static void TriggerEffect(string effectKey, EffectContext context)
    {
        if (CardEffectSystem.Instance != null)
        {
            CardEffectSystem.Instance.TriggerEffect(effectKey, context);
        }
    }
    
    public static void RegisterEffect(string effectKey, Action<EffectContext> handler)
    {
        if (CardEffectSystem.Instance != null)
        {
            CardEffectSystem.Instance.RegisterEffect(effectKey, handler);
        }
    }
}