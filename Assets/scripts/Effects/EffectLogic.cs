using UnityEngine;

// 1. 对敌人造成伤害
public class DamageEnemyEffect : EffectBase
{
    public override void Execute(BattleManager bm, RuntimeCard card, RuntimeUnit targetUnit)
    {
        int dmg = card.Data.value;
        bm.EnemyManager.TakeDamage(dmg);
        bm.UIManager.Log($"对敌人造成 {dmg} 点伤害！");
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

        bm.UnitManager.RefreshUnitUI(targetUnit);
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

// 4. 单位Buff (万具武 / 飞行)
public class UnitBuffEffect : EffectBase
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

        // 立刻攻击 (万具武效果)
        if (data.buffFreeAttackNow)
        {
            bm.UIManager.Log($"{targetUnit.Name} 发动了额外攻击！");
            // 调用战斗管理器让单位攻击，不消耗行动机会
            bm.CombatManager.ProcessUnitAttack(targetUnit, consumeAction: false);
        }

        bm.UnitManager.RefreshUnitUI(targetUnit);
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

        bm.UnitManager.RefreshUnitUI(targetUnit);
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
        bm.EnemyManager.TakeDamage(dmg); // 简单处理：目前只有一个敌人Boss
        bm.UIManager.Log($"全场伤害对敌人造成 {dmg} 点！");
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