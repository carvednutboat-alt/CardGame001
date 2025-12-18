using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Config")]
    public EnemyDatabase EnemyDB;       // 刚才让你创建的数据库
    public GameObject EnemyPrefab;      // 敌人预制体 (必须挂载 EnemyUnitUI)
    public Transform EnemyContainer;    // <-- 加上这行 (把 EnemyPanel 拖进来)

    [Header("Runtime")]
    // 管理当前场上活着的所有敌人
    public List<RuntimeEnemy> ActiveEnemies = new List<RuntimeEnemy>();

    private BattleManager _bm;

    // --- 内部类：封装单个敌人的所有信息 ---
    [System.Serializable]
    public class RuntimeEnemy
    {
        public RuntimeUnit UnitData;       // 核心数据 (HP, MaxHP, Name)
        public EnemyUnitUI UI;             // UI 引用
        public List<RuntimeCard> Deck;     // 属于这个敌人的独立牌库
        public int NextCardIndex;          // 循环出牌的索引
        public int TempAttackBonus;        // 临时攻击力加成

        public RuntimeEnemy(RuntimeUnit unit, EnemyUnitUI ui, List<CardData> sourceDeck)
        {
            UnitData = unit;
            UI = ui;
            Deck = new List<RuntimeCard>();
            NextCardIndex = 0;
            TempAttackBonus = 0;

            // 初始化这个敌人的牌库
            if (sourceDeck != null)
            {
                foreach (var card in sourceDeck)
                {
                    Deck.Add(new RuntimeCard(card));
                }
            }
        }
    }

    public void Init(BattleManager bm)
    {
        _bm = bm;
        ActiveEnemies.Clear();

        // 1. 清理容器
        if (EnemyContainer != null)
        {
            foreach (Transform child in EnemyContainer) Destroy(child.gameObject);
        }

        // 2. 调试 EnemyDB
        if (EnemyDB == null)
        {
            Debug.LogError("【严重错误】EnemyDB 没拖进去！请检查 Inspector。");
            SpawnTestEnemy(); // 兜底
            return;
        }

        // 3. 检查 GameManager 和 地图节点
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("没有找到 GameManager，正在生成测试怪...");
            SpawnTestEnemy();
            return;
        }

        if (GameManager.Instance.CurrentNode == null)
        {
            Debug.LogWarning("GameManager.CurrentNode 是空 (可能直接运行了战斗场景)，正在生成测试怪...");
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

    // 生成一组敌人
    void SpawnEncounter(EnemyEncounterProfile profile)
    {
        for (int i = 0; i < profile.Enemies.Count; i++)
        {
            CreateEnemyAt(profile.Enemies[i]);
        }
    }

    // === 修改：不再需要传入 spawnPoint 参数 ===
    void CreateEnemyAt(CardData enemyData)
    {
        RuntimeUnit unit = new RuntimeUnit(enemyData);

        // 直接生成在 EnemyContainer 下，LayoutGroup 会自动管位置
        GameObject obj = Instantiate(EnemyPrefab, EnemyContainer);

        // 还是建议重置一下，虽然 Layout Group 会覆盖位置，但 Scale 需要重置
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        EnemyUnitUI ui = obj.GetComponent<EnemyUnitUI>();
        ui.Init(unit, _bm);
        unit.EnemyUI = ui;

        RuntimeEnemy enemy = new RuntimeEnemy(unit, ui, enemyData.EnemyMoves);
        ActiveEnemies.Add(enemy);
    }

    // 调试用：生成一个测试怪
    void SpawnTestEnemy()
    {
        // 假设你有一个默认的 CardData 用于测试，或者直接从 CardConfig 里拿
        // 这里仅作逻辑演示，实际你需要传入一个真实的 CardData
        // CreateEnemyAt(SomeTestCardData, SpawnPoints[0]);
    }

    // ================== 回合逻辑 (协程) ==================

    public void ExecuteTurn(bool canAttack)
    {
        StartCoroutine(EnemyTurnRoutine(canAttack));
    }

    // 使用协程让敌人一个接一个行动，而不是瞬间全部打完
    IEnumerator EnemyTurnRoutine(bool canAttack)
    {
        // 遍历所有活着的敌人
        foreach (var enemy in ActiveEnemies)
        {
            if (enemy.UnitData.IsDead) continue;

            // 1. 重置临时攻击
            enemy.TempAttackBonus = 0;

            // 2. 视觉提示：高亮当前行动的敌人 (可选)
            // enemy.UI.Highlight(true); 

            // 3. 抽卡 & 结算卡牌效果
            RuntimeCard chosenCard = PickCardForEnemy(enemy);
            if (chosenCard != null)
            {
                // 模拟思考时间
                yield return new WaitForSeconds(0.5f);
                ResolveEnemyCard(enemy, chosenCard);
                yield return new WaitForSeconds(0.5f); // 卡牌动画时间
            }

            // 4. 普通攻击
            if (canAttack)
            {
                PerformAttack(enemy);
                yield return new WaitForSeconds(0.5f); // 攻击动画时间
            }
            else
            {
                _bm.UIManager.Log($"【{enemy.UnitData.Name}】本回合无法普攻。");
            }

            // enemy.UI.Highlight(false);
        }

        // 所有敌人都动完了，切回玩家回合
        _bm.StartPlayerTurn();
    }

    // ================== 内部逻辑：针对某个特定敌人的操作 ==================

    // 挑选一张牌
    private RuntimeCard PickCardForEnemy(RuntimeEnemy enemy)
    {
        if (enemy.Deck == null || enemy.Deck.Count == 0) return null;

        // 这里沿用你之前的逻辑：可以选择随机或顺序
        // 简单起见，这里演示随机
        int idx = Random.Range(0, enemy.Deck.Count);
        return enemy.Deck[idx];
    }

    // 结算卡牌 (注意：现在需要传入 attacker 是谁)
    private void ResolveEnemyCard(RuntimeEnemy attacker, RuntimeCard card)
    {
        CardData data = card.Data;
        string enemyName = attacker.UnitData.Name;

        switch (data.effectType)
        {
            case CardEffectType.UnitBuff:
                int bonus = data.value;
                attacker.TempAttackBonus += bonus;
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，攻击力 +{bonus}。");
                // 这里可以播放特效，比如 attacker.UI.PlayBuffEffect();
                break;

            case CardEffectType.DamageEnemy: // 对玩家造成伤害
                int dmg = data.value;
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，造成 {dmg} 点法术伤害。");
                _bm.PlayerUnit.TakeDamage(dmg);
                if (_bm.PlayerUnit.IsDead()) _bm.OnPlayerDefeated();
                break;

            case CardEffectType.HealUnit: // 给自己回血
                                          // 如果你想给友军回血，这里逻辑要变复杂，暂时只给自己回
                attacker.UnitData.CurrentHp += data.value;
                attacker.UI.UpdateHP(); // 刷新血条
                _bm.UIManager.Log($"【{enemyName}】使用「{data.cardName}」，恢复 {data.value} 生命。");
                break;

            default:
                _bm.UIManager.Log($"【{enemyName}】使用了未实现效果的牌: {data.cardName}");
                break;
        }
    }

    // 执行普攻
    private void PerformAttack(RuntimeEnemy attacker)
    {
        int totalDamage = attacker.UnitData.Attack + attacker.TempAttackBonus; // 假设 RuntimeUnit 里存了基础 Attack
        // 如果你的基础攻击力是写在 Manager 里的，现在需要移到 RuntimeUnit 或者 CardData 里
        // 这里假设写死一个基础值或者从 CardData 读取：
        if (totalDamage == 0) totalDamage = 5; // 默认值兜底

        // 嘲讽逻辑
        RuntimeUnit target = _bm.UnitManager.GetTauntUnit();
        if (target != null)
        {
            _bm.UIManager.Log($"【{attacker.UnitData.Name}】攻击了嘲讽单位 {target.Name}，伤害 {totalDamage}。");

            // === 修复：直接调用新接口 ===
            // 新的 ApplyDamage 内部已经包含了扣血、刷新UI、检查死亡、调用 KillUnit 的全套逻辑
            // 所以这里只需要这一行就够了
            _bm.CombatManager.ApplyDamage(target, totalDamage);
        }
        else
        {
            _bm.UIManager.Log($"【{attacker.UnitData.Name}】攻击了你，伤害 {totalDamage}。");
            if (_bm.PlayerUnit != null)
            {
                _bm.PlayerUnit.TakeDamage(totalDamage);
                if (_bm.PlayerUnit.IsDead()) _bm.OnPlayerDefeated();
            }
        }
    }

    // ================== 外部调用 ==================

    // 当某个敌人死亡时调用 (由 CombatManager 调用)
    public void OnEnemyDie(RuntimeUnit deadUnitData)
    {
        // 从列表中找到对应的 RuntimeEnemy 并移除
        RuntimeEnemy target = ActiveEnemies.Find(x => x.UnitData == deadUnitData);
        if (target != null)
        {
            ActiveEnemies.Remove(target);
            Destroy(target.UI.gameObject); // 销毁模型
        }

        // 检查胜利
        if (ActiveEnemies.Count == 0)
        {
            _bm.OnGameWin();
        }
    }
}