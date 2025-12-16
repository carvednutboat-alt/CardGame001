using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Unit EnemyUnit;
    public int AttackDamage = 5;

    private BattleManager _bm;

    public void Init(BattleManager bm)
    {
        _bm = bm;
        if (EnemyUnit != null) EnemyUnit.ResetHp();
    }

    public void TakeDamage(int damage)
    {
        if (EnemyUnit == null || EnemyUnit.IsDead()) return;
        EnemyUnit.TakeDamage(damage);
        if (EnemyUnit.IsDead()) _bm.OnGameWin();
    }

    // === 修改：增加了 canAttack 参数 ===
    public void ExecuteTurn(bool canAttack)
    {
        if (EnemyUnit.IsDead()) return;

        // 1. 如果规则禁止攻击（例如先手第一回合），直接跳过
        if (!canAttack)
        {
            _bm.UIManager.Log("【敌人】先手第一回合，无法进行攻击。");
            return;
        }

        // 2. 正常的攻击逻辑
        RuntimeUnit target = _bm.UnitManager.GetTauntUnit();

        if (target != null)
        {
            _bm.UIManager.Log($"敌人攻击了 {target.Name}，造成 {AttackDamage} 点伤害。");
            bool isDead = _bm.CombatManager.ApplyBattleDamage(target, AttackDamage);
            if (isDead) _bm.UnitManager.KillUnit(target);
        }
        else
        {
            _bm.UIManager.Log($"敌人直接攻击玩家，造成 {AttackDamage} 点伤害！");
            // 这里应该调用 PlayerManager.TakeDamage，暂时略过
        }
    }
}