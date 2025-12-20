using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Config")]
    public EnemyDatabase EnemyDB;
    public GameObject EnemyPrefab;
    public Transform EnemyContainer;

    [Header("Runtime")]
    public List<RuntimeEnemy> ActiveEnemies = new List<RuntimeEnemy>();

    // ★ 结算用：记录“最后被击杀的敌人”
    public CardData LastKilledUnitCard { get; private set; }
    public List<CardData> LastKilledDeckCards { get; private set; } = new List<CardData>();

    private BattleManager _bm;

    [System.Serializable]
    public class RuntimeEnemy
    {
        public RuntimeUnit UnitData;
        public EnemyUnitUI UI;

        public List<RuntimeCard> Deck;
        public int NextCardIndex;
        public int TempAttackBonus;
        public bool ResetAttack;

        // ★ 用于结算：敌人本体卡 + 敌人原始卡组（CardData）
        public CardData UnitCardData;
        public List<CardData> SourceDeckCardData;

        public RuntimeEnemy(RuntimeUnit unit, EnemyUnitUI ui, CardData unitCardData, List<CardData> sourceDeck)
        {
            UnitData = unit;
            UI = ui;

            UnitCardData = unitCardData;
            SourceDeckCardData = sourceDeck != null ? new List<CardData>(sourceDeck) : new List<CardData>();

            Deck = new List<RuntimeCard>();
            NextCardIndex = 0;
            TempAttackBonus = 0;
            ResetAttack = false;

            // 初始化这个敌人的牌库（RuntimeCard 实例）
            foreach (var card in SourceDeckCardData)
            {
                if (card != null)
                    Deck.Add(new RuntimeCard(card));
            }
        }
    }

    public void Init(BattleManager bm)
    {
        _bm = bm;
        ActiveEnemies.Clear();

        // ★ 清空“最后击杀”记录
        LastKilledUnitCard = null;
        LastKilledDeckCards.Clear();

        // 1. 清理容器
        if (EnemyContainer != null)
        {
            foreach (Transform child in EnemyContainer) Destroy(child.gameObject);
        }

        // 2. 调试 EnemyDB
        if (EnemyDB == null)
        {
            SpawnTestEnemy();
            return;
        }

        // 3. 检查 GameManager 和 地图节点
        if (GameManager.Instance == null)
        {
            SpawnTestEnemy();
            return;
        }

        if (GameManager.Instance.CurrentNode == null)
        {
            SpawnTestEnemy();
            return;
        }

        // 4. 一切正常，尝试获取战斗配置
        Debug.Log($"正在获取遭遇战配置，节点类型: {GameManager.Instance.CurrentNode.Type}");
        var profile = EnemyDB.GetRandomEncounter(GameManager.Instance.CurrentNode.Type);

        if (profile != null)
        {
            Debug.Log($"加载遭遇战: {profile.name}");
            SpawnEncounter(profile);
        }
        else
        {
            Debug.LogError($"【配置错误】数据库里没有类型为 {GameManager.Instance.CurrentNode.Type} 的战斗配置！或者列表是空的。");
            SpawnTestEnemy();
        }
    }

    void SpawnEncounter(EnemyEncounterProfile profile)
    {
        for (int i = 0; i < profile.Enemies.Count; i++)
        {
            CreateEnemyAt(profile.Enemies[i]);
        }
    }

    void CreateEnemyAt(CardData enemyData)
    {
        RuntimeUnit unit = new RuntimeUnit(enemyData);

        GameObject obj = Instantiate(EnemyPrefab, EnemyContainer);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        EnemyUnitUI ui = obj.GetComponent<EnemyUnitUI>();
        ui.Init(unit, _bm);
        unit.EnemyUI = ui;

        // ★ 把 enemyData（本体卡）和 enemyData.EnemyMoves（卡组）都存进去
        RuntimeEnemy enemy = new RuntimeEnemy(unit, ui, enemyData, enemyData.EnemyMoves);
        ActiveEnemies.Add(enemy);
    }

    void SpawnTestEnemy()
    {
        // test use case
    }

    public void ExecuteTurn(bool canAttack)
    {
        StartCoroutine(EnemyTurnRoutine(canAttack));
    }

    IEnumerator EnemyTurnRoutine(bool canAttack)
    {
        foreach (var enemy in ActiveEnemies)
        {
            if (enemy.UnitData.IsDead) continue;

            enemy.TempAttackBonus = 0;

            RuntimeCard chosenCard = PickCardForEnemy(enemy);
            if (chosenCard != null)
            {
                yield return new WaitForSeconds(0.5f);
                ResolveEnemyCard(enemy, chosenCard);
                yield return new WaitForSeconds(0.5f);
            }

            if (canAttack)
            {
                PerformAttack(enemy);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                _bm.UIManager.Log($"【{enemy.UnitData.Name}】本回合无法普攻。");
            }
            if (enemy.ResetAttack)
            {
                enemy.UnitData.CurrentAtk = enemy.UnitData.BaseAtk;
                enemy.UI.UpdateAttack();
                enemy.ResetAttack = false;
            }
        }

        _bm.StartPlayerTurn();
    }

    private RuntimeCard PickCardForEnemy(RuntimeEnemy enemy)
    {
        if (enemy.Deck == null || enemy.Deck.Count == 0) return null;
        int idx = Random.Range(0, enemy.Deck.Count);
        return enemy.Deck[idx];
    }

    private void ResolveEnemyCard(RuntimeEnemy attacker, RuntimeCard card)
    {
        CardData data = card.Data;
        string enemyName = attacker.UnitData.Name;

        switch (data.effectType)
        {
            case CardEffectType.UnitBuff:
                attacker.UnitData.CurrentAtk += data.value;
                attacker.ResetAttack = true;
                attacker.UI.UpdateAttack();
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，攻击力 +{data.value}。");
                break;

            case CardEffectType.DamageEnemy:
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，造成 {data.value} 点法术伤害。");
                _bm.PlayerUnit.TakeDamage(data.value);
                if (_bm.PlayerUnit.CurrentHp <= 0) _bm.OnPlayerDefeated();
                break;

            case CardEffectType.HealUnit:
                attacker.UnitData.CurrentHp += data.value;
                attacker.UI.UpdateHP();
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，恢复 {data.value} 生命。");
                break;

            default:
                _bm.UIManager.Log($"【{enemyName}】使用了未实现效果的牌: {data.cardName}");
                break;
        }
    }

    private void PerformAttack(RuntimeEnemy attacker)
    {
        // ★ 这里用 CurrentAtk（与 CombatManager 体系更一致）
        int totalDamage = attacker.UnitData.CurrentAtk + attacker.TempAttackBonus;
        if (totalDamage <= 0) totalDamage = 5;

        RuntimeUnit target = _bm.UnitManager.GetTauntUnit();
        if (target != null)
        {
            _bm.UIManager.Log($"【{attacker.UnitData.Name}】攻击嘲讽单位 {target.Name}，伤害 {totalDamage}。");
            _bm.CombatManager.ApplyDamage(target, totalDamage);
        }
        else
        {
            _bm.UIManager.Log($"【{attacker.UnitData.Name}】攻击了你，伤害 {totalDamage}。");
            _bm.PlayerUnit.TakeDamage(totalDamage);
            if (_bm.PlayerUnit.CurrentHp <= 0) _bm.OnPlayerDefeated();
        }
    }

    public void OnEnemyDie(RuntimeUnit deadUnitData)
    {
        RuntimeEnemy target = ActiveEnemies.Find(x => x.UnitData == deadUnitData);
        if (target != null)
        {
            // ★ 记录最后击杀
            LastKilledUnitCard = target.UnitCardData;
            LastKilledDeckCards = target.SourceDeckCardData != null
                ? new List<CardData>(target.SourceDeckCardData)
                : new List<CardData>();

            ActiveEnemies.Remove(target);
            Destroy(target.UI.gameObject);
        }

        if (ActiveEnemies.Count == 0)
        {
            _bm.OnGameWin();
        }
    }
}
