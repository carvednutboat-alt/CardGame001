using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    private BattleManager _bm;

    public void Init(BattleManager bm)
    {
        _bm = bm;
    }

    public void ProcessUnitAttack(RuntimeUnit attacker, RuntimeUnit target, bool consumeAction = true)
    {
        if (attacker == null || target == null) return;

        // 1. 造成伤害
        int damage = attacker.CurrentAtk;
        
        Duel.SetLastAttacker(attacker);

        // === 使用效果系统处理攻击修正 ===
        if (CardEffectSystem.Instance != null)
        {
            if (CardEffectSystem.Instance.CheckUnitMatchesEffect(attacker, "Eigenvector_AttackModifier"))
            {
                var ctx = new EffectContext { SourceUnit = attacker, Damage = damage };
                CardEffectSystem.Instance.TriggerEffect("Eigenvector_AttackModifier", ctx);
                damage = ctx.Damage;
            }
        }

        _bm.UIManager.Log($"{attacker.Name} 攻击了 {target.Name}！");
        ApplyDamage(target, damage, attacker);

        if (consumeAction)
        {
            attacker.CanAttack = false;
            if (attacker.UI != null)
            {
                attacker.UI.UpdateState();
            }
        }
    }

    public void ApplyHeal(RuntimeUnit target, int amount)
    {
        if (target == null) return;
        
        target.CurrentHp += amount;
        if (target.CurrentHp > target.MaxHp) target.CurrentHp = target.MaxHp;
        
        if (target.UI != null) target.UI.UpdateState();
        else if (target.EnemyUI != null) target.EnemyUI.UpdateHP();
        
        _bm.UIManager.Log($"{target.Name} 恢复了 {amount} 点生命。");
    }

    public void ApplyDamage(RuntimeUnit target, int damage, RuntimeUnit source = null)
    {
        if (target == null) return;

        // Check Immunity (Field Magic Blue Effect)
        // Checks if Source is Enemy (Owner == 1) and Target is Immune
        if (target.IsImmuneToEnemyEffects && source != null && source.SourceCard != null && source.SourceCard.Owner == 1)
        {
            _bm.UIManager.Log($"{target.Name} 免疫了来自 {source.Name} 的伤害！");
            damage = 0;
            return; // Block completely? Or just 0 damage? Return blocks "OnHit" triggers too. Assuming Block.
        }

        target.CurrentHp -= damage;
        if (target.CurrentHp < 0) target.CurrentHp = 0;

        if (target.UI != null)
        {
            target.UI.UpdateState();
        }
        else if (target.EnemyUI != null)
        {
            target.EnemyUI.UpdateHP();
            target.EnemyUI.UpdateAttack();
        }

        if (target.IsDead)
        {
            _bm.UIManager.Log($"{target.Name} 被击败了！");

            // === 使用效果系统处理亡语效果 ===
            if (CardEffectSystem.Instance != null)
            {
                if (CardEffectSystem.Instance.CheckUnitMatchesEffect(target, "ZeroVector_Deathrattle"))
                {
                    var ctx = new EffectContext { TargetUnit = target, SourceUnit = source };
                    CardEffectSystem.Instance.TriggerEffect("ZeroVector_Deathrattle", ctx);
                }
            }

            // === 使用效果系统处理击杀触发 ===
            if (source != null && !source.IsDead && CardEffectSystem.Instance != null)
            {
                if (CardEffectSystem.Instance.CheckUnitMatchesEffect(source, "SteamReaper_OnKill"))
                {
                    var ctx = new EffectContext { SourceUnit = source, TargetUnit = target };
                    CardEffectSystem.Instance.TriggerEffect("SteamReaper_OnKill", ctx);
                }
            }

            if (target.SourceCard == null || target.Id == -1)
            {
                _bm.EnemyManager.OnEnemyDie(target);
            }
            else
            {
                _bm.UnitManager.KillUnit(target);
            }
        }
    }

    public void RecalculateUnitStats(RuntimeUnit unit)
    {
        if (unit == null) return;

        int oldMaxHp = unit.MaxHp;

        int finalAtk = unit.BaseAtk;
        int finalMaxHp = unit.BaseMaxHp;

        int statsPerEquip = unit.IsEvolved ? 2 : 1;

        foreach (var equipData in unit.Equips)
        {
            finalAtk += statsPerEquip;
            finalMaxHp += statsPerEquip;
            finalMaxHp += equipData.equipHealthBonus;
            finalAtk += equipData.equipAttackBonus;
        }

        // === 使用效果系统处理过载加成 ===
        if (unit.Overload > 0 && CardEffectSystem.Instance != null)
        {
            if (unit.SourceCard != null && unit.SourceCard.Data != null 
                && unit.SourceCard.Data.isCommander 
                && unit.SourceCard.Data.cardTag == CardTag.Robot)
            {
                if (unit.RobotEvolved)
                {
                    finalAtk += (unit.Overload * 2);
                    _bm.UIManager.Log($"{unit.Name} 极限运转: +{unit.Overload * 2} 攻击");
                }
                else
                {
                    finalAtk += 2;
                }
            }
        }


        // Initialize Flags
        unit.IsImmuneToEnemyEffects = false;

        // === Field Magic: Prismatic Battleground (ID 5001) ===
        if (_bm != null && _bm.FieldCard != null && _bm.FieldCard.Data.id == 5001)
        {
            HashSet<CardColor> colors;
            // Determine side based on unit's owner or type
            // Assuming unit works for both Player and Enemy (RuntimeUnit shared?)
            // If unit is in EnemyManager, use Enemy colors.
            // Simplified check: if it has EnemyUI, it's Enemy.
            bool isEnemy = (unit.EnemyUI != null);

            if (isEnemy)
            {
                colors = _bm.EnemyManager.GetAliveCommanderColors();
            }
            else
            {
                colors = _bm.UnitManager.GetAliveCommanderColors();
            }

            bool hasRed = colors.Contains(CardColor.Red);
            bool hasBlue = colors.Contains(CardColor.Blue);
            bool hasGreen = colors.Contains(CardColor.Green);
            bool rainbow = hasRed && hasBlue && hasGreen;

            // Red Effect: +3 ATK
            if (rainbow || (hasRed && unit.SourceCard.Data.color == CardColor.Red))
            {
                finalAtk += 3;
            }
            
            // Blue Effect: Immunity
            if (rainbow || (hasBlue && unit.SourceCard.Data.color == CardColor.Blue))
            {
                unit.IsImmuneToEnemyEffects = true;
            }
        }

        // === 使用效果系统处理光环效果 ===
        if (_bm != null && _bm.UnitManager != null && CardEffectSystem.Instance != null)
        {
            int myIndex = -1;
            for(int i=0; i<5; i++)
            {
                if (_bm.UnitManager.Slots[i] == unit)
                {
                    myIndex = i;
                    break;
                }
            }

            if (myIndex != -1)
            {
                if (CheckNeighborAura(_bm.UnitManager.Slots, myIndex - 1)) finalMaxHp += 1;
                if (CheckNeighborAura(_bm.UnitManager.Slots, myIndex + 1)) finalMaxHp += 1;
            }
        }

        finalAtk += unit.TempAttackModifier;
        finalAtk += unit.PermAttackModifier;

        unit.CurrentAtk = finalAtk;
        unit.MaxHp = finalMaxHp;

        int diff = finalMaxHp - oldMaxHp;
        if (diff != 0)
        {
            unit.CurrentHp += diff;
            if (unit.CurrentHp > unit.MaxHp) unit.CurrentHp = unit.MaxHp;
            if (unit.CurrentHp < 1) unit.CurrentHp = 1;
        }

        if (unit.UI != null) unit.UI.UpdateState();
        else if (unit.EnemyUI != null)
        { 
            unit.EnemyUI.UpdateHP();
            unit.EnemyUI.UpdateAttack();
        }
    }

    private bool CheckNeighborAura(RuntimeUnit[] slots, int index)
    {
        if (index < 0 || index >= slots.Length) return false;
        var u = slots[index];
        if (u == null || u.IsDead) return false;
        
        if (CardEffectSystem.Instance != null)
        {
            return CardEffectSystem.Instance.CheckUnitMatchesEffect(u, "RobotDefender_Aura");
        }
        return false;
    }
}