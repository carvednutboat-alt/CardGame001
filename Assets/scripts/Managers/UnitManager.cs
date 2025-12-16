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

    // === 新增：用于选目标时的状态备份 ===
    private Dictionary<int, bool> _attackStateBackup = new Dictionary<int, bool>();

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

        // === 新增：处理装备牌进弃牌堆逻辑 ===
        if (unit.Equips.Count > 0)
        {
            foreach (var equipData in unit.Equips)
            {
                // 1. 因为 RuntimeUnit 里只存了 CardData，我们需要重新把它包装成 RuntimeCard
                // 这样它才能进入 DeckManager 的弃牌堆列表
                RuntimeCard equipCard = new RuntimeCard(equipData);

                // 2. 加入弃牌堆
                _bm.DeckManager.DiscardPile.Add(equipCard);

                // 3. 打印日志
                _bm.UIManager.Log($"装备牌 {equipData.cardName} 已进入弃牌堆。");
            }

            // 清空单位身上的装备（虽然单位马上要没了，但这是一个好习惯）
            unit.Equips.Clear();
        }
        // ======================================

        // 怪兽本体进入墓地
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

    // 1. 进入选目标模式：备份状态，开启交互
    public void EnableTargetingSelection()
    {
        _attackStateBackup.Clear();
        foreach (var unit in PlayerUnits)
        {
            // 备份当前的 CanAttack
            _attackStateBackup[unit.Id] = unit.CanAttack;

            // 强制开启按钮交互，让玩家可以点击选择
            if (unit.UI != null)
            {
                unit.UI.SetButtonInteractable(true);
            }
        }
    }

    // 2. 退出选目标模式：恢复状态
    public void RestoreStateAfterTargeting()
    {
        foreach (var unit in PlayerUnits)
        {
            // 恢复备份的状态
            if (_attackStateBackup.ContainsKey(unit.Id))
            {
                unit.CanAttack = _attackStateBackup[unit.Id];
            }

            // 刷新按钮交互状态
            if (unit.UI != null)
            {
                unit.UI.SetButtonInteractable(unit.CanAttack);
            }
        }
        _attackStateBackup.Clear();
    }
}