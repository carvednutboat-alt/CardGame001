using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<RuntimeUnit> PlayerUnits = new List<RuntimeUnit>();
    // === 新增：墓地列表，存卡牌数据 ===
    public List<RuntimeCard> Graveyard = new List<RuntimeCard>();

    public int MaxUnits = 5;

    [Header("UI Refs")]
    public Transform FieldPanel;
    public FieldUnitUI FieldUnitPrefab;

    private BattleManager _bm;
    private int _nextId = 1;

    public void Init(BattleManager bm)
    {
        _bm = bm;
        PlayerUnits.Clear();
        Graveyard.Clear(); // 初始化清空墓地
        // 清空 UI
        foreach (Transform child in FieldPanel) Destroy(child.gameObject);
    }

    public bool TrySummonUnit(RuntimeCard card)
    {
        if (PlayerUnits.Count >= MaxUnits)
        {
            _bm.UIManager.Log("场上位置已满！");
            return false;
        }

        RuntimeUnit unit = new RuntimeUnit(_nextId++, card);
        unit.CanAttack = _bm.CurrentTurnCanAttack;
        PlayerUnits.Add(unit);

        // 创建 UI
        FieldUnitUI ui = Instantiate(FieldUnitPrefab, FieldPanel);
        // 注意：FieldUnitUI 的 Init 方法需要适配
        ui.Init(_bm, unit.Id, unit.Name, unit.CurrentAtk, unit.CurrentHp,
                unit.IsEvolved, unit.Equips.Count, unit.CanAttack, unit.IsFlying, unit.HasTaunt);

        unit.UI = ui; // 绑定引用

        _bm.UIManager.Log($"召唤了 {unit.Name}");
        return true;
    }

    public void KillUnit(RuntimeUnit unit)
    {
        _bm.UIManager.Log($"{unit.Name} 阵亡。");

        // === 新增：加入墓地 ===
        // 注意：我们存的是 SourceCard (原始卡牌数据实例)，这样复活时可以保留之前的状态(如果需要)
        // 或者简单的只存 Data。这里存 RuntimeCard 最稳妥。
        Graveyard.Add(unit.SourceCard);

        if (unit.UI != null) Destroy(unit.UI.gameObject);
        PlayerUnits.Remove(unit);
    }

    public RuntimeUnit GetUnitById(int id)
    {
        return PlayerUnits.Find(u => u.Id == id);
    }

    public RuntimeUnit GetTauntUnit()
    {
        // 优先找嘲讽，没嘲讽返回第一个，没怪返回null
        var taunt = PlayerUnits.Find(u => u.HasTaunt);
        if (taunt != null) return taunt;
        if (PlayerUnits.Count > 0) return PlayerUnits[0];
        return null;
    }

    public void RefreshUnitUI(RuntimeUnit unit)
    {
        if (unit == null || unit.UI == null) return;
        // 重新计算数值后再刷新
        _bm.CombatManager.RecalculateUnitStats(unit);

        unit.UI.UpdateStats(unit.CurrentAtk, unit.CurrentHp, unit.IsEvolved,
                            unit.Equips.Count, unit.IsFlying, unit.HasTaunt);
    }

    // 设置所有单位能否攻击
    public void SetAllAttackStatus(bool canAttack)
    {
        foreach (var u in PlayerUnits)
        {
            u.CanAttack = canAttack;
            if (u.UI != null) u.UI.SetButtonInteractable(canAttack);
        }
    }
}