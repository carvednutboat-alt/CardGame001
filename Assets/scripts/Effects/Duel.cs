using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// Atomic API exposed to Lua for game operations.
/// "Duel" namespace mimicry.
/// </summary>
[LuaCallCSharp]
public static class Duel
{
    // Constants for Locations, Phases, etc. could go here.

    // ============================================
    // TARGET SELECTION (Callback based or State based)
    // ============================================
    
    // In YGOPRO, SelectTarget returns a Group. Here, because Unity is async/event-driven UI,
    // we need to signal the BattleManager to enter "Targeting Mode".
    // The Operation will likely pause or we handle it by "Setting the Target" in the effect context.
    
    // However, Lua is synchronous. One way is to use Coroutines or split execution.
    // YGOPRO solves this by running the Lua engine in a separate thread/coroutine that can yield.
    // XLua doesn't natively yield C# to wait for UI unless we use micro-threads.
    
    // SIMPLIFICATION for this Refactor:
    // We stick to the pattern: 
    // Effect.Target() -> calls Duel.SelectTarget(...) 
    // -> BattleManager Enters Targeting Mode -> User Clicks -> BattleManager calls Effect.Operation().
    
    public static void SelectTarget(RuntimeCard c, CardTargetType targetType)
    {
        // Tell BattleManager to enter targeting mode for the given card/effect.
        // We assume the "Active Effect" is known or passed.
        // For simplicity, we just trigger the UI state.
        if (BattleManager.Instance != null)
        {
            // We pass null for the UI object here, BattleManager finding it is tricky without context.
            // But usually the user clicked the card, so BattleManager already knows the _pendingCardUIObj.
            // We'll update BattleManager to handle this.
            BattleManager.Instance.InitiateEffectTargeting(c, targetType);
        }
    }

    // ============================================
    // OPERATIONS
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

    public static void Draw(int player, int amount)
    {
        if (BattleManager.Instance == null) return;
        // player 0 = user, 1 = enemy (not impl).
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
        
        // Find where the card is
        // 1. Hand
        if (BattleManager.Instance.DeckManager.Hand.Contains(card))
        {
            // Need UI Object? Usually we track it or DeckManager handles it.
            // But DeckManager.DiscardCard needs UI object to destroy it.
            // Problem: Lua doesn't know UI Object.
            // Solution: DeckManager should track Dictionary<RuntimeCard, GameObject> or find it.
            // Or we assume BattleManager knows pending Card UI.
            
            // For now, let's look it up or rely on DeckManager to find component?
            // "DiscardCard(card, uiObj)"
            
            // Refactor DeckManager to find UI automatically?
            // HandPanel children have CardUI -> RuntimeCard.
            var ui = BattleManager.Instance.DeckManager.FindCardUI(card);
            BattleManager.Instance.DeckManager.DiscardCard(card, ui);
        }
        // 2. Field (Monster)
        // ...
    }
    
    // Helpers for Selection
    private static RuntimeUnit _lastSelection;
    public static void SetSelection(RuntimeUnit unit)
    {
        _lastSelection = unit;
    }
    public static RuntimeUnit GetFirstTarget()
    {
        return _lastSelection;
    }

    // Helper to log from Lua
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
}
