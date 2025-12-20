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

            // 如果选中己方，则增加攻击
            targetUnit.CurrentAtk += tempAttack;

            bm.UIManager.Log($"{targetUnit.Name} 提升了 {tempAttack} 点攻击力！");
        }

    }
}