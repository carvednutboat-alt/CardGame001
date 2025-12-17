using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Unit EnemyUnit;
    public EnemyUnitUI EnemyUI; // === 新增：敌人的 UI 交互脚本 ===

    [Header("基础攻击力")]
    public int AttackDamage = 5;
    private BattleManager _bm;
    // ================== 敌人牌库相关 ==================

    [Header("敌人起始牌库 (CardData 资源，逻辑与玩家一致)")]
    public List<CardData> StartingDeck = new List<CardData>();

    // 运行时牌库：和玩家一样用 RuntimeCard，而不是随便写数值
    public List<RuntimeCard> EnemyDeck = new List<RuntimeCard>();

    [Header("出牌方式")]
    public bool UseRandomOrder = true;   // true = 每回合随机选一张，false = 顺序循环

    private int _nextIndex = 0;          // 顺序循环用
    private int _tempAttackBonus = 0;    // 本回合临时攻击加成

    // 本场战斗敌人实际用过的卡（只存 CardData，方便结算掉落 / 收藏）
    public List<CardData> CardsUsedThisBattle = new List<CardData>();

    // ================== 初始化 ==================
    public void Init(BattleManager bm)
    {
        _bm = bm;
        if (EnemyUnit != null) EnemyUnit.ResetHp();

        // === 新增：初始化 UI ===
        if (EnemyUI != null)
        {
            EnemyUI.Init(bm);
        }

        // 初始化敌人牌库（把 StartingDeck 转成 RuntimeCard）
        EnemyDeck.Clear();
        foreach (var data in StartingDeck)
        {
            if (data == null) continue;
            EnemyDeck.Add(new RuntimeCard(data));
        }

        CardsUsedThisBattle.Clear();
        _nextIndex = 0;
        _tempAttackBonus = 0;
    }

    public void TakeDamage(int damage)
    {
        if (EnemyUnit == null || EnemyUnit.IsDead()) return;
        EnemyUnit.TakeDamage(damage);
        if (EnemyUnit.IsDead()) _bm.OnGameWin();
    }

    // === 修改：增加了 canAttack 参数 ===
    /// 新机制 敌人回合：先从自己的牌库打出一张“敌人卡”，再进行普通攻击
    public void ExecuteTurn(bool canAttack)
    {
        if (EnemyUnit == null || EnemyUnit.IsDead()) return;

        // 回合开始：本回合临时加攻归零
        _tempAttackBonus = 0;

        // 1. 敌人先从自己的牌库里“出一张牌”
        RuntimeCard chosen = PickEnemyCard();
        if (chosen != null)
        {
            ResolveEnemyCard(chosen);
        }

        // 2. 如果规则禁止普通攻击（例如先手第一回合），只发动技能牌，不普通攻击
        if (!canAttack)
        {
            _bm.UIManager.Log("【敌人】本回合不能进行普通战斗攻击。");
            return;
        }

        // 3. 计算本回合最终攻击力 = 基础攻击 + 本回合临时加成
        int thisTurnAttack = AttackDamage + _tempAttackBonus;

        // 4. 正常攻击逻辑：优先打有嘲讽的友方单位，没有就打玩家
        RuntimeUnit target = _bm.UnitManager.GetTauntUnit();

        if (target != null)
        {
            _bm.UIManager.Log(
                $"【敌人】本回合攻击力 {thisTurnAttack}，攻击了 {target.Name}。"
            );

            bool dead = _bm.CombatManager.ApplyBattleDamage(target, thisTurnAttack);
            if (dead)
            {
                _bm.UnitManager.KillUnit(target);
            }
        }
        else
        {
            _bm.UIManager.Log(
                $"【敌人】本回合攻击力 {thisTurnAttack}，直接攻击你。"
            );

            if (_bm.PlayerUnit != null)
            {
                _bm.PlayerUnit.TakeDamage(thisTurnAttack);
                if (_bm.PlayerUnit.IsDead())
                {
                    _bm.OnPlayerDefeated();
                }
            }
        }

        // 5. 回合结束：临时加成清零（其实下回合开始前也会被重置，这里只是保险）
        _tempAttackBonus = 0;
    }

    // ================== 内部：选一张敌人牌 ==================

    private RuntimeCard PickEnemyCard()
    {
        if (EnemyDeck == null || EnemyDeck.Count == 0)
            return null;

        if (UseRandomOrder)
        {
            int idx = Random.Range(0, EnemyDeck.Count);
            return EnemyDeck[idx];
        }
        else
        {
            if (_nextIndex >= EnemyDeck.Count)
                _nextIndex = 0;

            return EnemyDeck[_nextIndex++];
        }
    }

    // ================== 内部：根据敌人牌的 effectType 结算效果 ==================

    private void ResolveEnemyCard(RuntimeCard card)
    {
        if (card == null || card.Data == null) return;

        CardData data = card.Data;

        // 记录：本局战斗中敌人实际使用过的卡
        if (!CardsUsedThisBattle.Contains(data))
        {
            CardsUsedThisBattle.Add(data);
        }

        switch (data.effectType)
        {
            // 用作“本回合敌人攻击 + value”（比如 value=2）
            case CardEffectType.UnitBuff:
            {
                int bonus = data.value;
                if (bonus <= 0)
                {
                    _bm.UIManager.Log($"【敌人】打出了 {data.cardName}，但攻击加成为 {bonus}，未产生效果。");
                    break;
                }

                _tempAttackBonus += bonus;
                _bm.UIManager.Log(
                    $"【敌人】使用技能牌「{data.cardName}」，本回合攻击力 +{bonus}。"
                );
                break;
            }

            // 从敌人视角：DamageEnemy = 对“玩家”造成效果伤害
            case CardEffectType.DamageEnemy:
            {
                if (_bm.PlayerUnit == null) break;

                int dmg = Mathf.Max(0, data.value);
                if (dmg <= 0)
                {
                    _bm.UIManager.Log($"【敌人】打出了 {data.cardName}，但伤害数值为 {data.value}。");
                    break;
                }

                _bm.UIManager.Log(
                    $"【敌人】使用技能牌「{data.cardName}」，对你造成 {dmg} 点效果伤害。"
                );

                _bm.PlayerUnit.TakeDamage(dmg);
                if (_bm.PlayerUnit.IsDead())
                {
                    _bm.OnPlayerDefeated();
                }
                break;
            }

            // 以后如果要给敌人更多牌型，可以继续在这里加 case
            default:
            {
                _bm.UIManager.Log(
                    $"【敌人】使用了 {data.cardName}（effectType={data.effectType}），当前没有为敌人实现对应逻辑。"
                );
                break;
            }
        }
    }
}