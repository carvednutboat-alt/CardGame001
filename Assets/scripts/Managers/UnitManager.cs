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

    // === 新增：全局效果标志 ===
    public bool PlayerImmuneToEffects = false; // Rank >= Threshold 触发

    public void Init(BattleManager bm)
    {
        _bm = bm;
        PlayerUnits.Clear();
        Graveyard.Clear();
        _attackStateBackup.Clear();
        PlayerImmuneToEffects = false; // Reset

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
        
        // 3. Refresh All (for Auras)
        RefreshAllUnits();
        
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

        // === 新增：亡语效果 (Deathrattle) ===
        if (unit.SourceCard != null && unit.SourceCard.Data != null)
        {
            if (unit.SourceCard.Data.deathEffect != CardEffectType.None)
            {
                EffectBase effect = EffectFactory.GetEffect(unit.SourceCard.Data.deathEffect);
                if (effect != null)
                {
                    _bm.UIManager.Log($"触发亡语：{unit.Name}");
                    effect.Execute(_bm, unit.SourceCard, null);
                }
            }
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
        
        // Refresh All (for Auras update after death)
        RefreshAllUnits();
    }
    
    public void RefreshAllUnits()
    {
        foreach (var u in PlayerUnits)
        {
            RefreshUnitUI(u);
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

    // === Overload System ===
    public void ModifyOverload(RuntimeUnit unit, int amount)
    {
        if (unit == null || amount == 0) return;

        // Passive: 1/1 Unit (Robot) - If ally gains Overload, +1 to gain
        // "When any friendly unit gains Overload -> increase that gain by +1"
        // Note: Check if we have the 1/1 unit on field. (Assuming Tag or ID, but card details said "1/1 exists on field")
        // Implementation: Iterate units, check for specific card (Robot 1/1). 
        // For efficiency, we might need a flag or just check Stats/Data.
        // Let's check for specific Card Effect property or ID if possible. 
        // Since we don't have IDs, let's look for "Robot" and Stats 1/1 or check 'value' if we use it for ID?
        // Or better: Add a trait to RuntimeUnit/CardData "AuraOverloadBooster"
        
        // Check for Booster (Unit 1/1)
        int boost = 0;
        foreach (var u in PlayerUnits)
        {
            if (u.SourceCard != null && u.SourceCard.Data != null)
            {
                // Assuming we will mark the 1/1 unit with a specific effect type or Name check
                // "3 x张unit 1/1 ... 当自己场上的unit获得过载时 使其过载的数字+1"
                // Let's use a Name check for now as we haven't defined a specific Enum for this aura.
                // Or best practice: defined generic Effect Logic elsewhere?
                // For now, hardcode check for "Robot 1/1" behavior or check a new field.
                // Let's add a specialized check.
                if (u.SourceCard.Data.cardName.Contains("1/1") || (u.BaseAtk == 1 && u.BaseMaxHp == 1 && u.SourceCard.Data.cardTag == CardTag.Robot))
                {
                   boost++;
                }
            }
        }

        if (amount > 0)
        {
            amount += boost;
            _bm.UIManager.Log($"{unit.Name} 获得过载 {amount} (含加成 {boost})");
        }

        unit.Overload += amount;
        if (unit.Overload < 0) unit.Overload = 0;

        RefreshUnitUI(unit);
    }

    public void ProcessOverloadEndTurn()
    {
        foreach (var unit in PlayerUnits)
        {
            // 0. Double Overload Side Effect: Lose HP
            if (unit.PendingOverloadSelfDamage > 0)
            {
                _bm.UIManager.Log($"{unit.Name} 承受过载反噬伤害: -{unit.PendingOverloadSelfDamage}");
                _bm.CombatManager.ApplyDamage(unit, unit.PendingOverloadSelfDamage);
                unit.PendingOverloadSelfDamage = 0;
                
                if (unit.IsDead) continue; // Dead units don't decay/fatigue
            }

            // 1. Fatigue Logic: Clear previous fatigue
            if (unit.IsFatigued)
            {
                unit.IsFatigued = false;
                _bm.UIManager.Log($"{unit.Name} 从疲劳中恢复。");
            }

            // 2. Overload Decay
            if (unit.Overload > 0)
            {
                unit.Overload--;
                _bm.UIManager.Log($"{unit.Name} 过载 -1 (剩余 {unit.Overload})");

                // 3. Trigger Fatigue if dropped to 0
                if (unit.Overload == 0)
                {
                    unit.IsFatigued = true; // Will affect next turn
                    _bm.UIManager.Log($"{unit.Name} 过载耗尽，进入疲劳状态！(下回合无法攻击/无法响应颜色)");
                }
                
                RefreshUnitUI(unit);
            }
        }
    }

    // === 指挥官检查 (EndTurn调用) ===
    public void CheckCommanderStatus()
    {
        // 1. 检查场上是否有指挥官
        bool commanderPresent = false;
        foreach (var u in PlayerUnits)
        {
            if (u.SourceCard != null && u.SourceCard.Data != null && u.SourceCard.Data.isCommander)
            {
                commanderPresent = true;
                break;
            }
        }

        // 2. 如果指挥官不在，摧毁所有依赖指挥官的单位
        if (!commanderPresent)
        {
            // 倒序遍历以安全删除
            for (int i = PlayerUnits.Count - 1; i >= 0; i--)
            {
                var u = PlayerUnits[i];
                if (u.SourceCard != null && u.SourceCard.Data != null && u.SourceCard.Data.dieWithoutCommander)
                {
                    _bm.UIManager.Log($"{u.Name} 因失去指挥官而撤退（自毁）。");
                    KillUnit(u);
                }
            }
        }
    }

    // === 强制刷新布局 (交换位置后调用) ===
    public void ForceRefreshLayout()
    {
        // 1. 清空所有 Slot UI 的子物体 (或者重新SetParent)
        // 这里的逻辑：Slot 0-4 是固定位置。Data 在 Slots[] 里已经交换了。
        // 我们需要把 Unit.UI 的 Parent 换到对应的 Slot Transform 下。
        
        for (int i = 0; i < 5; i++)
        {
            RuntimeUnit unit = Slots[i];
            Transform slotTr = _bm.UIManager.GetPlayerSlotTransform(i);
            
            if (unit != null && unit.UI != null && slotTr != null)
            {
                unit.UI.transform.SetParent(slotTr);
                unit.UI.transform.localPosition = Vector3.zero;
                unit.UI.transform.localScale = Vector3.one;
            }
        }
    }
}