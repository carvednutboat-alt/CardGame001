using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private BattleManager _bm;

    public void Init(BattleManager bm)
    {
        _bm = bm;
    }

    // 重新计算单位属性 (处理装备、进化)
    public void RecalculateUnitStats(RuntimeUnit unit)
    {
        // 1. 基础数值
        int oldMaxHp = unit.MaxHp; // 记录旧的上限
        int baseAtk = unit.BaseAtk;
        int baseHp = unit.SourceCard.Data.unitHealth > 0 ? unit.SourceCard.Data.unitHealth : 1;

        // 2. 计算加成
        int bonusAtk = 0;
        int bonusHp = 0;
        int equipCount = unit.Equips.Count;

        // 进化加成
        int perEquipStat = unit.IsEvolved ? 2 : 1;
        bonusAtk += equipCount * perEquipStat;
        bonusHp += equipCount * perEquipStat;

        // 装备特定加成
        foreach (var eq in unit.Equips)
        {
            bonusAtk += eq.equipAttackBonus;
            bonusHp += eq.equipHealthBonus;
        }

        // 3. 应用攻击力
        unit.CurrentAtk = baseAtk + bonusAtk;

        // 4. 应用生命值 (核心修改)
        // 计算新的最大生命值
        int newMaxHp = baseHp + bonusHp;

        // 差值：新的上限比旧的上限多了多少？
        int diff = newMaxHp - oldMaxHp;

        // 更新上限
        unit.MaxHp = newMaxHp;

        // === 核心逻辑：如果上限增加了，当前生命值也跟着增加 ===
        // 这实现了“加最大生命值 = 加血”的效果
        if (diff > 0)
        {
            unit.CurrentHp += diff;
        }

        // 5. 边界检查：确保血量不超标 (如果你想完全取消上限，可以把这句删掉，但在UI显示上可能会奇怪)
        // 现在的逻辑是：上限也涨了，所以当前血量涨上去是合法的
        if (unit.CurrentHp > unit.MaxHp)
        {
            unit.CurrentHp = unit.MaxHp;
        }
    }

    // 处理单位攻击敌人
    public void ProcessUnitAttack(RuntimeUnit unit, bool consumeAction)
    {
        if (consumeAction && !unit.CanAttack) return;
        if (_bm.EnemyManager.EnemyUnit.IsDead()) return;

        int dmg = unit.CurrentAtk;
        _bm.EnemyManager.TakeDamage(dmg);
        _bm.UIManager.Log($"{unit.Name} 攻击造成 {dmg} 点伤害！");

        if (consumeAction)
        {
            unit.CanAttack = false;
            if (unit.UI != null) unit.UI.SetButtonInteractable(false);
        }
    }

    // 处理单位受到伤害 (战斗伤害)
    // 返回 true 表示死亡
    public bool ApplyBattleDamage(RuntimeUnit unit, int dmg)
    {
        // 这里可以加护盾逻辑
        unit.CurrentHp -= dmg;
        _bm.UnitManager.RefreshUnitUI(unit);
        return unit.IsDead;
    }
}