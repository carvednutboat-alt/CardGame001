using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 1. 初等行变换: 交换两个Unit的位置 (并 +1 ATK)
public class SwapColumnsEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        // 需要选择两个目标? 
        // 现有的 TargetType 只能选 1 个。
        // 为了简化操作，我们假设逻辑是：
        // "选择一个目标 unit，将其与'相邻'的 unit 交换" 或者 "随机选择两个" 或者 "UI 拖拽特殊处理"
        // 鉴于目前 UI 只能选 1 个目标，可以设计为：
        // 选中 目标 A，然后找 目标 A 右边 (或左边) 的单位进行交换。
        // 或者：全场随机选两个 (Random Swap)
        
        // 仔细读需求: "交换你战场上两列 unit 的位置"
        // 这通常是一个全场效果，或者需要特殊的 Target logic.
        // 简单实现：选中一个己方 Unit，让它和它 "右边" 的 Unit 交换。如果右边没人，就和左边换。
        
        if (targetUnit == null) return;
        
        var units = bm.UnitManager.PlayerUnits;
        int idx = units.IndexOf(targetUnit);
        if (idx == -1) return;

        RuntimeUnit otherUnit = null;

        // 优先找右边
        if (idx + 1 < units.Count) otherUnit = units[idx + 1];
        // 否则找左边
        else if (idx - 1 >= 0) otherUnit = units[idx - 1];

        if (otherUnit != null)
        {
            // 交换 Logic
            int otherIdx = units.IndexOf(otherUnit);
            
            // Swap in list
            units[idx] = otherUnit;
            units[otherIdx] = targetUnit;
            
            // Apply Buff +1 ATK
            targetUnit.PermAttackModifier += 1;
            otherUnit.PermAttackModifier += 1;
            
            bm.CombatManager.RecalculateUnitStats(targetUnit);
            bm.CombatManager.RecalculateUnitStats(otherUnit);
            
            // Refresh UI Order
            // UnitManager 需要一个方法来根据 List 顺序刷新 Layout，或者直接 Destroy/Instantiate (太重了)
            // 假设 UnitManager 有 RefreshAllUnitsUI 位置的方法
            bm.UnitManager.ForceRefreshLayout(); 
            
            bm.UIManager.Log($"【初等行变换】交换了 {targetUnit.Name} 和 {otherUnit.Name} 的位置！(攻击力+1)");
        }
        else
        {
            bm.UIManager.Log("旁边没有可以交换的单位。");
        }
    }
}

// 2. 标量乘法: Double ATK/HP (Identity Matrix x2 / Nearby LinearAlgebra x2)
public class ScalarMultEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        // 目标通常是 "全部单位" ? 
        // 需求: "使场上的单位矩阵攻击力，生命值翻倍... 若邻接格子上有【线性代数】unit卡，则同样施加该效果"
        // 这听起来像是一个 AOE Buff，但在 Execute 里我们需要遍历。
        // 假设这张卡是 Spell，TargetType = None (Global Effect)

        var units = bm.UnitManager.PlayerUnits;
        List<RuntimeUnit> toBuff = new List<RuntimeUnit>();

        for (int i = 0; i < units.Count; i++)
        {
            var u = units[i];
            bool shouldBuff = false;

            // Condition 1: Is "Identity Matrix" (单位矩阵)
            // 需要判断卡名 (Name contains "单位矩阵" or "Identity Matrix")
            if (u.Name.Contains("单位矩阵") || u.Name.Contains("Identity Matrix")) // 也可以用 ID 判断
            {
                shouldBuff = true;
            }
            // Condition 2: Neighbor is Linear Logic
            else 
            {
                // Check Neighbors
                bool hasLinearNeighbor = false;
                if (i > 0 && units[i-1].SourceCard.Data.cardTag == CardTag.LinearAlgebra) hasLinearNeighbor = true;
                if (i < units.Count - 1 && units[i+1].SourceCard.Data.cardTag == CardTag.LinearAlgebra) hasLinearNeighbor = true;
                
                if (hasLinearNeighbor) shouldBuff = true;
            }

            if (shouldBuff) toBuff.Add(u);
        }

        foreach (var u in toBuff)
        {
            // Double Stats
            int atkToAdd = u.Attack; 
            u.TempAttackModifier += atkToAdd;

            // Heal for CurrentHP amount (Double current HP up to Max)
            u.CurrentHp = Mathf.Min(u.CurrentHp * 2, u.MaxHp);

            bm.UIManager.Log($"{u.Name} 进行了标量乘法！(攻击翻倍)");
             bm.CombatManager.RecalculateUnitStats(u);
        }
        
       // bm.CombatManager.RecalculateAll(); // Not implemented, used loop above
    }
}

// 3. 转置: Swap ATK and HP (Global)
public class TransposeEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        var units = bm.UnitManager.PlayerUnits;
        foreach (var u in units)
        {
            int oldAtk = u.Attack;
            int oldHp = u.CurrentHp;

            // === FIX: Consolidate Temp Stats ===
            // Prevent end-of-turn reset from lowering the new Attack (which comes from HP).
            // We bake the current Temp modifier into Perm so the total remains stable as the new "Base" for the swap.
            u.PermAttackModifier += u.TempAttackModifier;
            u.TempAttackModifier = 0;
            
            // Recalculate to ensure u.Attack reflects the shift (value shouldn't change, but source does)
            // Actually, since Perm + Temp = (Perm+Temp) + 0, Total is same. 
            // We can skip recalc for Atk source check if we trust the logic, 
            // but let's use the 'oldAtk' captured above which is the Target for HP.
            // And use 'oldHp' as Target for Atk.
            
            // 1. Calculate and Apply Attack Change
            // Target: oldHp
            // Current (after consolidation): u.Attack (should be same as oldAtk)
            int currentAtk = u.Attack; // This calls getter which uses modifiers. 
            // Since we just changed modifiers directly without Recalc, 'CurrentAtk' field might be stale 
            // IF getter returns cached 'CurrentAtk'. 
            // RuntimeUnit.Attack => CurrentAtk.
            // CurrentAtk is set by RecalculateUnitStats. 
            // So we DO need to update CurrentAtk or manually calculate diff using known cached value.
            // oldAtk holds the correct Total.
            
            // Equation: Base + Perm_New + 0 + Equip = oldHp
            // Perm_New = oldHp - (Base + Equip)
            // But we don't know Equip easily.
            // Alternative: Perm_New = Perm_Current + (oldHp - currentAtk)
            // Perm_Current has incorporated Temp. currentAtk (Total) is oldAtk.
            
            int atkDiff = oldHp - oldAtk; 
            u.PermAttackModifier += atkDiff;
            
            // 2. Calculate and Apply Health Change
            // New Hp should be oldAtk.
            // We need to adjust BaseMaxHp so that MaxHp >= oldAtk.
            // Current MaxHp = BaseMaxHp + Bonus.
            // We want New MaxHp = oldAtk (roughly).
            // New BaseMaxHp = oldAtk - Bonus = oldAtk - (MaxHp - BaseMaxHp).
            int hpBonus = u.MaxHp - u.BaseMaxHp;
            int newBaseMaxHp = oldAtk - hpBonus;
            if (newBaseMaxHp < 1) newBaseMaxHp = 1; // Minimum 1 Base
            
            u.BaseMaxHp = newBaseMaxHp;
            
            // 3. Recalculate Logic to update MaxHp and Attack
            bm.CombatManager.RecalculateUnitStats(u);
            
            // 4. Force CurrentHp to Target (oldAtk)
            u.CurrentHp = oldAtk;
            // Clamp just in case
            if (u.CurrentHp > u.MaxHp) u.CurrentHp = u.MaxHp;
            if (u.CurrentHp < 1) u.CurrentHp = 1; 
            
            bm.UIManager.Log($"{u.Name} 转置了！({oldAtk}/{oldHp} -> {u.Attack}/{u.CurrentHp})");
            
            // 5. Explicitly Update UI since we modified CurrentHp manually after Recalc
            if (u.UI != null) u.UI.UpdateState();
        }
    }
}

// 4. 施密特正交化: Evolve Basis -> Orthogonal Basis. Count Rank.
public class GramSchmidtEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        var units = bm.UnitManager.PlayerUnits;
        int rank = 0;
        int commanderThreshold = 0;

        foreach (var u in units)
        {
            if (u.Name.Contains("单位基") || u.Name.Contains("Standard Basis"))
            {
                u.IsEvolved = true; 
                u.OverrideName = "正交基";
                rank++;
                bm.UIManager.Log($"{u.Name} 正交化为 正交基！");
                bm.CombatManager.RecalculateUnitStats(u);
            }
            else if (u.Name.Contains("正交基") || (u.IsEvolved && u.Name.Contains("单位基")))
            {
                rank++;
            }

            if (u.SourceCard.Data.isCommander || u.Name.Contains("单位矩阵"))
            {
                commanderThreshold = Mathf.Min(u.Attack, u.CurrentHp);
            }
        }

        // 2. Check Rank vs Commander Stats
        bm.UIManager.Log($"当前 Rank: {rank}, 指挥官阈值: {commanderThreshold}");
        if (rank >= commanderThreshold && commanderThreshold > 0)
        {
            // Grant "Immune to Enemy Effects"
            // We need a Global Flag in BattleManager or UnitManager?
            bm.UnitManager.PlayerImmuneToEffects = true; // Need to add this field
            bm.UIManager.Log("Rank 足够高！我方单位获得【效果免疫】(持续至对方回合结束)");
        }
    }
}
