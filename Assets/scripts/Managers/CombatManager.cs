using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatManager : MonoBehaviour
{
    private BattleManager _bm;

    public void Init(BattleManager bm)
    {
        _bm = bm;
    }

    // ==========================================
    // 1. 处理怪兽普攻 (ProcessUnitAttack)
    //    参数修改为：攻击者, 目标
    // ==========================================
    // 增加参数：consumeAction (是否消耗行动次数)，默认 true
    public void ProcessUnitAttack(RuntimeUnit attacker, RuntimeUnit target, bool consumeAction = true)
    {
        if (attacker == null || target == null) return;

        // 1. 造成伤害
        int damage = attacker.CurrentAtk;
        _bm.UIManager.Log($"{attacker.Name} 攻击了 {target.Name}！");
        ApplyDamage(target, damage);

        // 2. === 修复核心：扣除行动次数 ===
        if (consumeAction)
        {
            attacker.CanAttack = false;

            // 刷新攻击者的 UI (按钮变灰)
            if (attacker.UI != null)
            {
                attacker.UI.UpdateState(); // 也就是 UpdateStats
            }
        }
    }

    // ==========================================
    // 2. 处理通用伤害 (ApplyDamage) - 【这就是你缺失的方法！】
    //    用于法术、效果、AOE等直接扣血的情况
    // ==========================================
    public void ApplyDamage(RuntimeUnit target, int damage)
    {
        if (target == null) return;

        // 1. 扣血逻辑
        target.CurrentHp -= damage;
        if (target.CurrentHp < 0) target.CurrentHp = 0;

        // 2. 刷新 UI
        // 如果是玩家单位
        if (target.UI != null)
        {
            target.UI.UpdateState();
        }
        // 如果是敌人单位
        else if (target.EnemyUI != null)
        {
            target.EnemyUI.UpdateHP();
            target.EnemyUI.UpdateAttack();
        }

        // 3. 死亡判定
        if (target.IsDead)
        {
            _bm.UIManager.Log($"{target.Name} 被击败了！");

            // 区分是敌人还是玩家随从
            // 通常敌人没有 SourceCard 或者 ID 为 -1 (取决于你的构造函数)
            if (target.SourceCard == null || target.Id == -1)
            {
                // 通知 EnemyManager 敌人死了
                _bm.EnemyManager.OnEnemyDie(target);
            }
            else
            {
                // 通知 UnitManager 随从死了
                _bm.UnitManager.KillUnit(target);
            }
        }
    }

    // ==========================================
    // 3. 重新计算数值 (RecalculateUnitStats)
    //    被 EffectLogic.cs 中的 FieldEvolveEffect 调用
    // ==========================================
    // ==========================================
    // 重新计算数值 (核心逻辑)
    // ==========================================
    public void RecalculateUnitStats(RuntimeUnit unit)
    {
        if (unit == null) return;

        // 1. 记录旧的上限，用来计算血量差值
        int oldMaxHp = unit.MaxHp;

        // 2. 重置为裸体数值
        int finalAtk = unit.BaseAtk;
        int finalMaxHp = unit.BaseMaxHp;

        // 3. 计算装备带来的加成
        // 规则：每件装备本身提供 +1/+1 (进化后 +2/+2)
        int statsPerEquip = unit.IsEvolved ? 2 : 1;

        // 累加所有装备
        foreach (var equipData in unit.Equips)
        {
            // A. 机制加成 (每张卡都有)
            finalAtk += statsPerEquip;
            finalMaxHp += statsPerEquip;

            // B. 卡牌自身属性加成 (如果 CardData 里填了 value)
            finalMaxHp += equipData.equipHealthBonus; // 如果卡牌还能额外加血，在这里加
            finalAtk += equipData.equipAttackBonus;
        }

        // 4. 应用攻击力
        unit.CurrentAtk = finalAtk;

        // 5. 应用血量 (关键算法：上限加了多少，当前血就回多少)
        unit.MaxHp = finalMaxHp;

        int diff = finalMaxHp - oldMaxHp;
        if (diff != 0)
        {
            unit.CurrentHp += diff;

            // 修正边界
            if (unit.CurrentHp > unit.MaxHp) unit.CurrentHp = unit.MaxHp;
            if (unit.CurrentHp < 1) unit.CurrentHp = 1; // 装备变更通常不致死
        }

        // 6. 刷新 UI
        if (unit.UI != null) unit.UI.UpdateState();
        else if (unit.EnemyUI != null)
        { 
            unit.EnemyUI.UpdateHP();
            unit.EnemyUI.UpdateAttack();
        }

    }

    // 如果还有 ApplyBattleDamage 这种旧方法，建议删除或重定向到 ApplyDamage
}