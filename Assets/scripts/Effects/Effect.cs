using System;
using XLua;

/// <summary>
/// Represents a card effect defined in Lua.
/// Follows the C-C-T-O pattern (Condition, Cost, Target, Operation).
/// </summary>
[LuaCallCSharp]
public class Effect
{
    // Constants
    public const int TYPE_IGNITION = 1; // Activate manually
    public const int TYPE_TRIGGER  = 2; // Deathrattle etc.
    public const int TYPE_EQUIP    = 3; // Equipment

    // The card that owns this effect
    public RuntimeCard OwnerCard;

    // Delegate types for Lua functions
    [CSharpCallLua]
    public delegate bool ConditionDelegate(Effect e, int output_log_level);

    [CSharpCallLua]
    public delegate void CostDelegate(Effect e, int output_log_level);

    [CSharpCallLua]
    public delegate void TargetDelegate(Effect e, RuntimeCard target, int output_log_level);

    [CSharpCallLua]
    public delegate void OperationDelegate(Effect e, int output_log_level);

    // Lua function references
    public ConditionDelegate Condition;
    public CostDelegate Cost;
    public TargetDelegate Target;
    public OperationDelegate Operation;

    // Optional metadata
    public int EffectCode;
    public string Description;

    // Setters for Lua to call
    public void SetCondition(ConditionDelegate func) { Condition = func; }
    public void SetCost(CostDelegate func) { Cost = func; }
    public void SetTarget(TargetDelegate func) { Target = func; }
    public void SetOperation(OperationDelegate func) { Operation = func; }
    
    public void SetDescription(string desc) { Description = desc; }

    // Helper to check condition
    public bool CheckCondition(BattleManager bm, RuntimeCard card)
    {
        if (Condition != null)
        {
            try
            {
                // Passing 0 for log level or context if needed, currently just placeholder
                return Condition(this, 0);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[Effect] Condition Error: {ex.Message}");
                return false;
            }
        }
        return true;
    }

    // Helper getters/setters for Lua
    public RuntimeCard GetHandler()
    {
        return OwnerCard;
    }

    // === Helpers to Invoke Delegates ===

    public void PayCost(BattleManager bm)
    {
        if (Cost != null)
        {
            try { Cost(this, 0); }
            catch (Exception ex) { UnityEngine.Debug.LogError($"[Effect] Cost Error: {ex.Message}"); }
        }
    }

    public void ResolveTarget(BattleManager bm)
    {
        if (Target != null)
        {
            try { Target(this, null, 0); }  // target param might be null during selection phase
            catch (Exception ex) { UnityEngine.Debug.LogError($"[Effect] Target Error: {ex.Message}"); }
        }
    }

    public void ExecuteOperation(BattleManager bm, RuntimeUnit target = null)
    {
        if (Operation != null)
        {
            try 
            {
                // We might want to pass the specific target to Lua if it was a single-target selection
                // But typically Lua gets targets via Duel.GetTargets().
                // However, for verify simplicity, we can pass it if we update the delegate signature?
                // The delegate is (Effect e, int log).
                // Let's rely on Duel.GetChainInfo or similar mechanism (Card.GetTargetCards).
                // But for this refactor, let's keep it simple.
                // If we want to pass target, we need to change OperationDelegate signature or rely on state.
                // Let's Assume Lua uses a "GetTargets" API or we pass it conceptually.
                // Re-checking the Delegate: public delegate void OperationDelegate(Effect e, int output_log_level);
                
                Operation(this, 0); 
            }
            catch (Exception ex) { UnityEngine.Debug.LogError($"[Effect] Operation Error: {ex.Message}"); }
        }
    }
}
