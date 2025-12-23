using UnityEngine;
using System.Collections.Generic;

// 1. 对敌人造成伤害 (单体)
public class DamageEnemyEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        // 必须要有目标才能造成单体伤害
        if (targetUnit == null)
        {
            bm.UIManager.Log("没有指定伤害目标！");
            return;
        }

        int dmg = card.Data.value;

        // 使用 CombatManager 的通用伤害接口
        bm.CombatManager.ApplyDamage(targetUnit, dmg);

        bm.UIManager.Log($"对 {targetUnit.Name} 造成 {dmg} 点法术伤害！");
    }
}

// 2. 治疗单位
public class HealUnitEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        if (targetUnit == null) return;

        int heal = card.Data.value;
        // 逻辑：直接加血，然后Clamp
        targetUnit.CurrentHp = Mathf.Min(targetUnit.CurrentHp + heal, targetUnit.MaxHp);

        // 刷新 UI (兼容玩家和敌人)
        if (targetUnit.UI != null) targetUnit.UI.UpdateState();
        else if (targetUnit.EnemyUI != null) targetUnit.EnemyUI.UpdateHP();

        bm.UIManager.Log($"{targetUnit.Name} 恢复了 {heal} 点生命。");
    }

    public override bool CheckCondition(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CurrentHp >= targetUnit.MaxHp)
        {
            bm.UIManager.Log($"{targetUnit.Name} 已经是满血，治疗卡不会被消耗。");
            return false;
        }
        return true;
    }
}

// 3. 抽牌
public class DrawCardsEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        int count = card.Data.value;
        bm.DeckManager.DrawCards(count);
        bm.UIManager.Log($"抽了 {count} 张牌。");
    }
}

// 4. 飞行
public class FlyEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        if (targetUnit == null) return;

        var data = card.Data;

        // 起飞
        if (data.buffGrantFlying)
        {
            if (!targetUnit.IsFlying)
            {
                targetUnit.IsFlying = true;
                bm.UIManager.Log($"{targetUnit.Name} 获得了【起飞】状态！");
            }
        }

        // 立刻攻击
        if (data.buffFreeAttackNow)
        {
            // 逻辑修正：因为这是给己方单位加Buff，但我们并没有指定要攻击哪个敌人。
            // 所以这里设定为：随机攻击一个前排敌人。
            if (bm.EnemyManager.ActiveEnemies.Count > 0)
            {
                // 随机选一个敌人
                int idx = Random.Range(0, bm.EnemyManager.ActiveEnemies.Count);
                var randomEnemy = bm.EnemyManager.ActiveEnemies[idx];

                bm.UIManager.Log($"{targetUnit.Name} 发动了额外攻击！目标：{randomEnemy.UnitData.Name}");

                // === 修改：额外攻击不消耗行动次数 (consumeAction: false) ===
                bm.CombatManager.ProcessUnitAttack(targetUnit, randomEnemy.UnitData, consumeAction: false);
            }
            else
            {
                bm.UIManager.Log("场上没有敌人，无法发动额外攻击。");
            }
        }

        // 刷新 UI
        if (targetUnit.UI != null) targetUnit.UI.UpdateState();
    }
}

// 5. 字段进化 (混合效果：进化 + 抽牌)
public class FieldEvolveEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        if (targetUnit == null) return;

        // 进化逻辑
        targetUnit.IsEvolved = true;
        targetUnit.EvolveTurnsLeft = 3;

        bm.UIManager.Log($"{targetUnit.Name} 进化了！持续3回合。");

        // 重新计算数值（因为进化可能加属性）
        bm.CombatManager.RecalculateUnitStats(targetUnit);

        // 如果配置了 value > 0，则顺便抽牌（满足你的混合需求）
        if (card.Data.value > 0)
        {
            bm.DeckManager.DrawCards(card.Data.value);
            bm.UIManager.Log($"进化额外效果：抽了 {card.Data.value} 张牌。");
        }

        // === 修复点：进化后重置攻击状态 ===
        // 只有在“本回合允许攻击”的前提下，进化才重置攻击
        if (bm.CurrentTurnCanAttack)
        {
            targetUnit.CanAttack = true;
        }

        if (targetUnit.UI != null) targetUnit.UI.UpdateState();
    }

    public override bool CheckCondition(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit)
    {
        // 检查是否有装备
        if (targetUnit != null)
        {
            if (targetUnit.Equips == null || targetUnit.Equips.Count == 0)
            {
                bm.UIManager.Log("没有装备的单位不能使用进化卡。");
                return false;
            }
        }
        return true;
    }
}

// 6. 全场AOE (敌方)
public class DamageAllEnemiesEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        int dmg = card.Data.value;

        // === 修复：遍历所有敌人造成伤害 ===
        // 注意：要倒序遍历或者拷贝列表，因为如果伤害导致敌人死亡，列表可能会变
        // 但我们在 EnemyManager.OnEnemyDie 里移除的是 ActiveEnemies，所以这里用副本比较安全
        var enemiesSnapshot = new List<EnemyManager.RuntimeEnemy>(bm.EnemyManager.ActiveEnemies);

        foreach (var enemy in enemiesSnapshot)
        {
            // 对每个敌人的 UnitData 造成伤害
            bm.CombatManager.ApplyDamage(enemy.UnitData, dmg);
        }

        bm.UIManager.Log($"对所有敌人造成 {dmg} 点 AOE 伤害！");
    }
}

// 7. 复活单位 (死者苏生)
public class ReviveUnitEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        // 1. 检查墓地有没有东西
        if (bm.UnitManager.Graveyard.Count == 0)
        {
            bm.UIManager.Log("墓地里没有怪兽，无法复活！");
            return;
        }

        // 2. 确定复活数量 (通常是 1)
        int count = Mathf.Max(1, card.Data.value);
        int revivedCount = 0;

        // 3. 倒序遍历墓地 (通常复活刚死的，或者随机，这里演示复活最后进墓地的)
        for (int i = bm.UnitManager.Graveyard.Count - 1; i >= 0; i--)
        {
            if (revivedCount >= count) break;

            // 检查场上位置是否满了
            if (bm.UnitManager.PlayerUnits.Count >= bm.UnitManager.MaxUnits)
            {
                bm.UIManager.Log("场上位置已满，无法继续复活。");
                break;
            }

            RuntimeCard deadCard = bm.UnitManager.Graveyard[i];

            // 执行召唤
            // 注意：TrySummonUnit 会生成新的 RuntimeUnit，血量是满的
            if (bm.UnitManager.TrySummonUnit(deadCard))
            {
                bm.UnitManager.Graveyard.RemoveAt(i); // 从墓地移除
                bm.UIManager.RefreshGraveyardIfOpen();
                bm.UIManager.Log($"【死者苏生】复活了 {deadCard.Data.cardName}！");
                revivedCount++;
            }
        }
    }

    public override bool CheckCondition(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit)
    {
        // 检查墓地
        if (bm.UnitManager.Graveyard.Count == 0)
        {
            bm.UIManager.Log("墓地里没有单位，复活卡不会被消耗。");
            return false;
        }
        return true;
    }
}

// 8. 通用效果
public class UnitBuffEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    { 
        // 必须要有目标
        if (targetUnit == null)
        {
            bm.UIManager.Log("没有指定伤害目标！");
            return;
        }
        if (targetUnit.Id == -1) {
            int dmg = card.Data.value;

            // 如果选中敌人，则造成伤害（类似与火球）
            bm.CombatManager.ApplyDamage(targetUnit, dmg);

            bm.UIManager.Log($"对 {targetUnit.Name} 造成 {dmg} 点法术伤害！");
        }
        if (targetUnit.Id > 0)
        {
            int tempAttack = card.Data.value;

            // 如果选中己方，则增加【临时】攻击 (配合 CombatManager 里的 TempAttackModifier)
            targetUnit.TempAttackModifier += tempAttack;

            // 特殊处理："突袭" 类卡牌通常意味着冲锋/急袭 (CanAttack = true)
            // 虽然 EffectType 是 UnitBuff，但为了体验，我们这里判定一下名字或添加新EffectType
            // 简单起见：如果卡名包含 "突袭" 或 "Rush"，则重置攻击状态
            if (card.Data.cardName.Contains("突袭") || card.Data.cardName.Contains("Rush"))
            {
                targetUnit.CanAttack = true;
                bm.UIManager.Log($"{targetUnit.Name} 获得突袭效果 (本回合可以攻击)！");
            }

            // 重新计算数值以应用并刷新UI
            bm.CombatManager.RecalculateUnitStats(targetUnit);
            
            // ★ 强制刷新确保 UI 同步 (防止 Recalculate 内部逻辑有分支漏掉)
            if (targetUnit.UI != null) 
            {
                targetUnit.UI.UpdateState();
                targetUnit.UI.SetButtonInteractable(targetUnit.CanAttack); // 刷新按钮状态
            }

            bm.UIManager.Log($"{targetUnit.Name} 获得了 {tempAttack} 点临时攻击力！");
        }

    }
}

// 9. 检索卡牌效果 (通用：支持亡语找装备、装备找本家)
public class SearchCardEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        // 判断检索类型
        if (card.Data.deathEffect == CardEffectType.SearchEquipmentOnDeath)
        {
            SearchEquipment(bm);
        }
        else if (card.Data.onReceiveEquipEffect == CardEffectType.SearchFamilyOnEquip)
        {
            SearchFamilyUnit(bm, card);
        }
    }

    private void SearchEquipment(BattleManager bm)
    {
        // 目标：卡组 or 墓地 的随机一张“装备牌”
        // 1. 收集所有符合条件的卡
        List<RuntimeCard> candidates = new List<RuntimeCard>();
        
        // 查牌库
        foreach (var c in bm.DeckManager.DrawPile)
        {
            if (c.Data.isEquipment) candidates.Add(c);
        }
        // 查弃牌堆(墓地? 还是说是DiscardPile?) 
        // 需求说“卡组墓地”，这里假设包含 DiscardPile
        foreach (var c in bm.DeckManager.DiscardPile)
        {
            if (c.Data.isEquipment) candidates.Add(c);
        }

        if (candidates.Count == 0)
        {
            bm.UIManager.Log("卡组和弃牌堆中没有装备牌。");
            return;
        }

        // 2. 随机取一张
        RuntimeCard target = candidates[Random.Range(0, candidates.Count)];
        
        // 3. 从原位置移除
        if (bm.DeckManager.DrawPile.Contains(target)) 
            bm.DeckManager.DrawPile.Remove(target);
        else if (bm.DeckManager.DiscardPile.Contains(target))
            bm.DeckManager.DiscardPile.Remove(target);
            
        // 4. 加入手牌
        bm.DeckManager.AddCardToHand(target);
        bm.UIManager.Log($"亡语触发：检索到了 {target.Data.cardName}！");
    }

    private void SearchFamilyUnit(BattleManager bm, RuntimeCard sourceCard)
    {
        // 目标：卡组中一张“本家怪兽” (Tag == sourceCard.Tag)
        // 注意：sourceCard 这里是“被装备的怪兽卡”，还是“触发效果的装备卡”？
        // 也可以是“触发效果的怪兽本身”。
        // 根据 BattleManager 调用逻辑：card = 触发效果的卡(即怪兽卡本身)
        
        if (sourceCard.Data.cardTag == CardTag.None) return;

        List<RuntimeCard> candidates = new List<RuntimeCard>();
        foreach (var c in bm.DeckManager.DrawPile)
        {
            if (c.Data.kind == CardKind.Unit && c.Data.cardTag == sourceCard.Data.cardTag)
            {
                candidates.Add(c);
            }
        }

        if (candidates.Count == 0)
        {
            bm.UIManager.Log("卡组中没有本家怪兽。");
            return;
        }

        // 随机取一张
        RuntimeCard target = candidates[Random.Range(0, candidates.Count)];
        
        bm.DeckManager.DrawPile.Remove(target);
        bm.DeckManager.AddCardToHand(target);
        bm.UIManager.Log($"本家共鸣：检索到了 {target.Data.cardName}！");
    }
}

// 10. 机器人效果 (Overload 等)
public class RobotEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        if (targetUnit == null) return;
        
        var type = card.Data.effectType;

        if (type == CardEffectType.GrantOverload)
        {
            // 给目标施加过载 (Value defined in CardData, usually 2)
            int amount = card.Data.value; 
            if (amount <= 0) amount = 2; // Default fallback
            
            bm.UnitManager.ModifyOverload(targetUnit, amount);
        }
        else if (type == CardEffectType.DoubleOverload)
        {
            // 魔法 翻倍过载，但是在回合结束时流失相当于过载的血量
            // 翻倍：Amount = CurrentOverload
            int current = targetUnit.Overload;
            if (current == 0)
            {
                bm.UIManager.Log("目标当前无过载，无法翻倍。");
                return;
            }

            bm.UnitManager.ModifyOverload(targetUnit, current);
            
            // Side Effect: Lose HP equivalent to TOTAL Overload at end of turn
            // Note: If Overload was 2, Modify(2) -> Total 4.
            // "Equivalent to Overload". Let's assume the NEW total.
            targetUnit.PendingOverloadSelfDamage += targetUnit.Overload; 
            
            bm.UIManager.Log($"{targetUnit.Name} 过载翻倍！(当前 {targetUnit.Overload}) 回合结束将受到 {targetUnit.Overload} 伤害。");
        }
        else if (type == CardEffectType.LimitOperationEvolve)
        {
            // 进化 极限运转 指挥官特性+2变为+过载*2
            // Prerequisite: Check if target is Steam Robot Commander? Or allowing any robot?
            // "Commander trait... becomes". Implies targeting the Commander.
            // Assumption: This card targets Ally Commander.
            
            if (!targetUnit.RobotEvolved)
            {
                targetUnit.RobotEvolved = true;
                bm.UIManager.Log($"{targetUnit.Name} 极限运转进化！(攻击加成改为 过载x2)");
                bm.CombatManager.RecalculateUnitStats(targetUnit);
            }
            else
            {
                bm.UIManager.Log("该单位已经进化过了。");
            }
        }
    }
}