using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Relic效果管理器 - 处理所有Relic的效果触发
/// </summary>
public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    // Removed cached _playerCollection to avoid initialization order issues
    // private PlayerCollection _playerCollection; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // private void Start() { } // No need for Start

    /// <summary>
    /// 触发回合结束时的Relic效果（回血）
    /// </summary>
    public void TriggerEndTurnEffects(BattleManager battleManager)
    {
        // 直接使用 Instance，防止缓存为空
        var collection = PlayerCollection.Instance;
        if (collection == null || battleManager == null) 
        {
            Debug.LogWarning("[RelicManager] PlayerCollection or BattleManager is null");
            return;
        }

        Debug.Log($"[RelicManager] Triggering End Turn. Relic Count: {collection.OwnedRelics.Count}");

        foreach (var relic in collection.OwnedRelics)
        {
            if (relic == null) continue;

            if (relic.effectType == RelicEffectType.回合结束回血)
            {
                ApplyEndTurnHealing(battleManager, relic.effectValue);
            }
        }
    }

    /// <summary>
    /// 应用回合结束回血效果
    /// </summary>
    private void ApplyEndTurnHealing(BattleManager battleManager, int healAmount)
    {
        if (battleManager == null || battleManager.UnitManager == null) return;

        var playerUnits = battleManager.UnitManager.PlayerUnits;
        if (playerUnits == null || playerUnits.Count == 0) return;

        foreach (var unit in playerUnits)
        {
            if (unit == null || unit.CurrentHp >= unit.MaxHp) continue;

            // int oldHp = unit.CurrentHp;
            unit.CurrentHp = Mathf.Min(unit.CurrentHp + healAmount, unit.MaxHp);
            
            battleManager.UnitManager.RefreshUnitUI(unit);
            
            if (battleManager.UIManager != null)
            {
                battleManager.UIManager.Log($"[Relic效果] {unit.Name} 回复了 {healAmount} 点生命值");
            }
        }
    }

    /// <summary>
    /// 获取额外抽牌数量
    /// </summary>
    public int GetExtraDrawCount()
    {
        var collection = PlayerCollection.Instance;
        if (collection == null) return 0;

        int extraDraw = 0;
        foreach (var relic in collection.OwnedRelics)
        {
            if (relic == null) continue;

            if (relic.effectType == RelicEffectType.额外抽牌)
            {
                extraDraw += relic.effectValue;
            }
        }
        return extraDraw;
    }

    /// <summary>
    /// 检查是否拥有特定效果的Relic
    /// </summary>
    public bool HasRelicEffect(RelicEffectType effectType)
    {
        var collection = PlayerCollection.Instance;
        if (collection == null) return false;

        foreach (var relic in collection.OwnedRelics)
        {
            if (relic != null && relic.effectType == effectType)
            {
                return true;
            }
        }
        return false;
    }
}
