using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<RuntimeUnit> PlayerUnits = new List<RuntimeUnit>();
    // === 新增：Fixed Slots ===
    public RuntimeUnit[] Slots = new RuntimeUnit[5]; // 0-4

    public List<RuntimeCard> Graveyard = new List<RuntimeCard>();
    public int MaxUnits = 5;

    [Header("UI Refs")]
    // public Transform FieldPanel; // 废弃，改用 UIManager 获取 Slot Transform
    public FieldUnitUI FieldUnitPrefab;

    private BattleManager _bm;
    private int _nextId = 1;

    // === 新增：用于选目标时的状态备份 ===
    private Dictionary<int, bool> _attackStateBackup = new Dictionary<int, bool>();

    public void Init(BattleManager bm)
    {
        _bm = bm;
        PlayerUnits.Clear();
        Graveyard.Clear();
        _attackStateBackup.Clear();

        // 重置槽位数据
        for (int i = 0; i < 5; i++) Slots[i] = null;
    }

    // 旧方法废弃或重定向
    public bool TrySummonUnit(RuntimeCard card)
    {
        // 默认寻找第一个空位
        for (int i = 0; i < 5; i++)
        {
            if (Slots[i] == null)
            {
                return TrySummonUnitAt(i, card);
            }
        }
        _bm.UIManager.Log("场上位置已满！");
        return false;
    }

    public bool TrySummonUnitAt(int slotIndex, RuntimeCard card)
    {
        if (slotIndex < 0 || slotIndex >= 5) return false;
        if (Slots[slotIndex] != null)
        {
            _bm.UIManager.Log("该位置已有单位！");
            return false;
        }

        RuntimeUnit unit = new RuntimeUnit(_nextId++, card);
        unit.CanAttack = _bm.CurrentTurnCanAttack;
        
        // 数据存入
        PlayerUnits.Add(unit);
        Slots[slotIndex] = unit;

        // 获取槽位 Transform
        Transform slotTr = _bm.UIManager.GetPlayerSlotTransform(slotIndex);
        if (slotTr == null)
        {
            Debug.LogError($"无法获取玩家槽位 {slotIndex} 的Transform");
            return false;
        }

        // 创建 UI 并挂载到 Slot 下
        FieldUnitUI ui = Instantiate(FieldUnitPrefab, slotTr);
        
        // ★ 统一大小：强制填满 Slot (120x140)
        RectTransform rt = ui.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localScale = Vector3.one;
        }

        // 1. 把 UI 赋值给 Unit
        unit.UI = ui;

        // 2. Init
        ui.Init(unit, _bm);

        _bm.UIManager.Log($"在 {slotIndex + 1} 号位召唤了 {unit.Name}");
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
                RuntimeCard equipCard = new RuntimeCard(equipData);
                _bm.DeckManager.DiscardPile.Add(equipCard);
                _bm.UIManager.Log($"装备牌 {equipData.cardName} 已进入弃牌堆。");
            }
            unit.Equips.Clear();
        }

        // 怪兽本体进入墓地
        Graveyard.Add(unit.SourceCard);

        if (unit.UI != null) Destroy(unit.UI.gameObject);
        
        // 清理数据和槽位引用
        PlayerUnits.Remove(unit);
        for(int i=0; i<5; i++)
        {
            if(Slots[i] == unit) 
            {
                Slots[i] = null;
                break;
            }
        }
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

        unit.UI.UpdateState();
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

    // === 重置临时属性 ===
    public void ResetTempStats()
    {
        foreach (var unit in PlayerUnits)
        {
            if (unit.TempAttackModifier != 0)
            {
                unit.TempAttackModifier = 0;
                // 必须重新计算并刷新UI
                if (_bm != null && _bm.CombatManager != null)
                {
                    _bm.CombatManager.RecalculateUnitStats(unit);
                }
            }
        }
    }
}