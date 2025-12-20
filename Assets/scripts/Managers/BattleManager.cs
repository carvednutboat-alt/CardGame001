using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Managers")]
    public DeckManager DeckManager;
    public UnitManager UnitManager;
    public EnemyManager EnemyManager;
    public CombatManager CombatManager;
    public BattleUIManager UIManager;

    [Header("Entities")]
    public Unit PlayerUnit;

    [Header("UI Refs")]
    public Transform UnitPanel;

    [Header("Data Config")]
    public List<CardData> StartingSpellDeck;
    public List<CardData> StartingUnitLibrary;

    [Header("Player Status")]
    public int PlayerCurrentHP = 80;
    public int PlayerMaxHP = 80;

    [Header("Rewards")]
    public Vector2Int MinorGoldRange = new Vector2Int(100, 200);
    public Vector2Int EliteGoldRange = new Vector2Int(100, 200);
    public Vector2Int BossGoldRange  = new Vector2Int(100, 200);

    // === 内部状态 ===
    public bool IsTargetingMode = false;
    private RuntimeCard _pendingCard;
    private GameObject _pendingCardUIObj;

    private RuntimeCard _pendingCard2;
    private GameObject _pendingCardUIObj2;
    private bool _battleEnded = false;

    // 控制当前回合是否允许攻击 (先后手机制)
    public bool CurrentTurnCanAttack { get; private set; } = true;

    // === 玩家状态 1：召唤限制 ===
    public bool HasSummonedThisTurn { get; private set; } = false;

    // === 玩家状态 2：攻击选择目标模式 ===
    // 玩家点击自己的随从，准备攻击时，记录谁要攻击
    private RuntimeUnit _selectedAttacker;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // === 添加空值检查 ===
        if (UIManager == null) { Debug.LogError("[BattleManager] UIManager 未配置！"); return; }
        if (EnemyManager == null) { Debug.LogError("[BattleManager] EnemyManager 未配置！"); return; }
        if (DeckManager == null) { Debug.LogError("[BattleManager] DeckManager 未配置！"); return; }
        if (UnitManager == null) { Debug.LogError("[BattleManager] UnitManager 未配置！"); return; }
        if (CombatManager == null) { Debug.LogError("[BattleManager] CombatManager 未配置！"); return; }

        UIManager.Log("=== 游戏开始！ ===");
        UIManager.Init(this);
        CombatManager.Init(this);
        EnemyManager.Init(this);
        UnitManager.Init(this);

        // --- 修改：从全局 GameManager 获取卡组 ---
        List<CardData> spellsForHand = new List<CardData>();
        List<CardData> unitsForBench = new List<CardData>();

        // 从全局获取所有卡牌
        if (GameManager.Instance != null)
        {
            // 分类：单位入战场，法术入手牌
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit)
                {
                    unitsForBench.Add(card);
                }
                else
                {
                    spellsForHand.Add(card);
                }
            }

            // --- 血量同步（重要）---
            if (PlayerUnit != null)
            {
                // 从全局管理器获取血量，并赋值给玩家单位
                PlayerUnit.InitData(GameManager.Instance.PlayerCurrentHP, GameManager.Instance.PlayerMaxHP);
                UIManager.Log($"玩家血量同步: {PlayerUnit.CurrentHp}/{PlayerUnit.maxHp}");
            }
            else
            {
                Debug.LogError("[BattleManager] PlayerUnit 未配置！");
            }
        }
        else
        {
            // 调试模式：使用 BattleManager 本地配置的默认数据
            spellsForHand = StartingSpellDeck;
            unitsForBench = StartingUnitLibrary;
            
            if (PlayerUnit != null)
            {
                PlayerUnit.InitData(PlayerUnit.maxHp, PlayerUnit.maxHp);
            }
        }

        // 初始化卡组
        DeckManager.Init(this, spellsForHand);

        // 初始化单位 (修复：动态生成单位卡片)
        SpawnUnitBench(unitsForBench);

        UIManager.Log("初始抽牌 (4张)...");
        DeckManager.DrawCards(4);

        bool playerGoesFirst = Random.value > 0.5f;
        if (playerGoesFirst)
        {
            UIManager.Log("玩家先手（不能攻击）！");
            StartPlayerTurn(canAttack: false, drawCard: false);
        }
        else
        {
            UIManager.Log("敌人先手（不能攻击）！");
            EnemyTurn(canAttack: false);
        }
    }

    // 修改 SpawnUnitBench 兼容性
    private void SpawnUnitBench(List<CardData> units)
    {
        // 清空动态生成的旧单位卡片
        if (UnitPanel != null)
        {
            foreach (Transform child in UnitPanel) Destroy(child.gameObject);
        }

        if (DeckManager == null || DeckManager.CardPrefab == null || UnitPanel == null) return;

        foreach (var data in units)
        {
            if (data == null) continue;
            RuntimeCard runCard = new RuntimeCard(data);
            CardUI ui = Instantiate(DeckManager.CardPrefab, UnitPanel);
            ui.Init(runCard, this);
        }
    }

    public void StartPlayerTurn(bool canAttack = true, bool drawCard = true)
    {
        CurrentTurnCanAttack = canAttack; // 记录状态
        HasSummonedThisTurn = false; // === 新回合重置召唤限制 ===
        _selectedAttacker = null;    // 重置攻击选择

        if (UIManager != null)
        {
            UIManager.Log("--------------------------");
            UIManager.Log(">>> 你的回合 <<<");
        }

        if (drawCard && DeckManager != null)
        {
            DeckManager.DrawCards(1);
        }
        else if (UIManager != null)
        {
            UIManager.Log("（你的第一回合不抽牌）");
        }

        // 设置攻击状态
        if (UnitManager != null)
        {
            UnitManager.SetAllAttackStatus(canAttack);
        }

        if (!canAttack && UIManager != null)
        {
            UIManager.Log("提示：你的第一回合无法进行战斗（先后手）");
        }
    }

    public void OnEndTurnButton()
    {
        if (IsTargetingMode) return;
        
        // === 添加空值检查 ===
        if (EnemyManager == null)
        {
            Debug.LogError("[BattleManager] EnemyManager 为空，无法结束回合！");
            return;
        }
        
        // 取消可能的攻击选择
        _selectedAttacker = null;
        if (UnitManager != null)
        {
            UnitManager.SetAllAttackStatus(false);
        }
        
        EnemyTurn(canAttack: true);
    }

    private void EnemyTurn(bool canAttack)
    {
        if (UIManager != null)
        {
            UIManager.Log("--------------------------");
            UIManager.Log(">>> 敌人回合 <<<");
        }
        
        if (EnemyManager != null)
        {
            EnemyManager.ExecuteTurn(canAttack);
        }
        else
        {
            Debug.LogError("[BattleManager] EnemyManager 为空！");
            StartPlayerTurn();
        }
    }

    // ---------------------------------------------------------
    // 卡牌交互逻辑 - 修复：使用两参数签名
    // ---------------------------------------------------------

    public void OnCardClicked(CardUI ui, RuntimeCard card)
    {
        // === 添加空值检查 ===
        if (ui == null || card == null || card.Data == null)
        {
            Debug.LogError("[BattleManager] OnCardClicked 参数为空！");
            return;
        }

        if (IsTargetingMode)
        {
            CancelTargeting();
            return;
        }

        // 如果正在选择攻击目标时点了卡牌，取消攻击选择
        if (_selectedAttacker != null)
        {
            if (UIManager != null) UIManager.Log("取消攻击选择");
            _selectedAttacker = null;
        }

        // 1. 单位牌 (增加召唤限制)
        if (card.Data.kind == CardKind.Unit)
        {
            // === 修改：召唤限制 ===
            if (HasSummonedThisTurn)
            {
                if (UIManager != null) UIManager.Log("<color=red>本回合已经召唤过单位了！</color>");
                return;
            }

            if (UnitManager != null && UnitManager.TrySummonUnit(card))
            {
                HasSummonedThisTurn = true; // 标记召唤
                Destroy(ui.gameObject);
            }
            return;
        }

        // 2. 装备牌 (修改：进入瞄准)
        if (card.Data.isEquipment)
        {
            if (UnitManager == null || UnitManager.PlayerUnits.Count == 0)
            {
                if (UIManager != null) UIManager.Log("场上没有随从，无法装备！");
                return;
            }
            EnterTargetingMode(card, ui.gameObject);
            return;
        }

        // 3. 需选目标的法术 (Heal / Buff / Evolve)
        // 这些需要先进入瞄准模式，再点击触发效果
        if (card.Data.effectType == CardEffectType.HealUnit ||
            card.Data.effectType == CardEffectType.UnitBuff ||
            card.Data.effectType == CardEffectType.FieldEvolve ||
            card.Data.effectType == CardEffectType.DamageEnemy)
        {
            EnterTargetingMode(card, ui.gameObject);
            return;
        }

        // === 复活修改：单独处理 ===
        if (card.Data.effectType == CardEffectType.ReviveUnit)
        {
            // 1. 判断墓地是否为空
            if (UnitManager == null || UnitManager.Graveyard.Count == 0)
            {
                if (UIManager != null) UIManager.Log("墓地是空的，无法使用。");
                return;
            }

            // 2. 暂存卡牌 (因为弹窗没有效果回调，需要暂存)
            _pendingCard2 = card;
            _pendingCardUIObj2 = ui.gameObject;

            // 3. 让墓地选择弹窗，并传入回调方法
            if (UIManager != null)
            {
                UIManager.ShowGraveyardSelection(UnitManager.Graveyard, OnGraveyardCardSelected);
                UIManager.Log("从墓地选择要复活的随从...");
            }
            return; // 返回成功（后续在回调执行）
        }

        // 4. 不需要选目标的法术 (Revive / Draw / AOE)
        EffectBase effect = EffectFactory.GetEffect(card.Data.effectType);
        if (effect != null)
        {
            // 检查条件（如果墓地为空）CheckCondition 内部会报Log并返回 false
            if (!effect.CheckCondition(this, card, null)) return;

            effect.Execute(this, card, null);
            if (DeckManager != null)
            {
                DeckManager.DiscardCard(card, ui.gameObject);
            }
        }
        else
        {
            if (UIManager != null) UIManager.Log($"未实现效果: {card.Data.effectType}");
        }
    }

    public void OnFieldUnitClicked(int unitId)
    {
        if (UnitManager == null)
        {
            Debug.LogError("[BattleManager] UnitManager 为空！");
            return;
        }

        // 1. 施法模式
        if (IsTargetingMode)
        {
            // 检测：卡牌如果限制【敌人】或【已经死亡的单位】等
            if (_pendingCard == null || _pendingCard.Data == null)
            {
                ExitTargetingMode();
                return;
            }

            if (_pendingCard.Data.targetType != CardTargetType.Ally &&
                _pendingCard.Data.targetType != CardTargetType.All)
            {
                if (UIManager != null) UIManager.Log("<color=red>这张卡只能对己方随从使用！</color>");
                return; // 返回，不退出模式，继续选择
            }
            RuntimeUnit target = UnitManager.GetUnitById(unitId);
            if (target == null) return;

            // 2. === 如果是装备卡，执行装备逻辑 ===
            if (_pendingCard.Data.isEquipment)
            {
                ApplyEquipment(_pendingCard, target); // 传参：卡、目标随从

                // 装备直接从手牌移除，不进入墓地 (因为附在随从上)
                if (DeckManager != null)
                {
                    DeckManager.RemoveCardFromHand(_pendingCard, _pendingCardUIObj);
                }

                ExitTargetingMode();
                return;
            }
            // =======================================

            EffectBase effect = EffectFactory.GetEffect(_pendingCard.Data.effectType);
            if (effect != null)
            {
                if (!effect.CheckCondition(this, _pendingCard, target)) { ExitTargetingMode(); return; }
                effect.Execute(this, _pendingCard, target);
                if (DeckManager != null)
                {
                    DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
                }
            }
            ExitTargetingMode();
            return;
        }

        // 2. 攻击选择模式 (新逻辑)
        RuntimeUnit unit = UnitManager.GetUnitById(unitId);
        if (unit != null)
        {
            // 检测是否能攻击
            if (!CurrentTurnCanAttack)
            {
                if (UIManager != null) UIManager.Log("本回合无法进行攻击！");
                return;
            }
            if (!unit.CanAttack)
            {
                if (UIManager != null) UIManager.Log($"{unit.Name} 本回合已经攻击过或无法进行攻击");
                return;
            }

            // 选中这个单位作为攻击者
            _selectedAttacker = unit;
            if (UIManager != null) UIManager.Log($"已选中 {unit.Name}，<color=yellow>请点击敌人进行攻击</color>");
        }
    }

    // 处理点击 EnemyUnitUI 的点击
    public void OnEnemyClicked(EnemyUnitUI enemyUI)
    {
        if (enemyUI == null || enemyUI.MyUnit == null)
        {
            Debug.LogError("[BattleManager] EnemyUI 或 MyUnit 为空！");
            return;
        }

        // 1. === 如果正在【施法瞄准模式】 ===
        if (IsTargetingMode)
        {
            if (_pendingCard == null || _pendingCard.Data == null)
            {
                ExitTargetingMode();
                return;
            }

            // 检测：卡牌如果限制【友军】或【所有】等
            if (_pendingCard.Data.targetType != CardTargetType.Enemy &&
                _pendingCard.Data.targetType != CardTargetType.All)
            {
                if (UIManager != null) UIManager.Log("<color=red>这张卡只能对己方使用！</color>");
                return; // 返回，
            }
            RuntimeUnit target = enemyUI.MyUnit;
            if (target == null) return;

            // 获取卡牌对应的法术效果
            EffectBase effect = EffectFactory.GetEffect(_pendingCard.Data.effectType);
            if (effect != null)
            {
                // 执行效果 (例如伤害, 选中的敌人为目标)
                effect.Execute(this, _pendingCard, target);

                // 消耗卡牌（弃牌）
                if (DeckManager != null)
                {
                    DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
                }
            }

            // 退出瞄准模式
            ExitTargetingMode();
            return;
        }

        // 2. 玩家随从攻击模式
        if (_selectedAttacker != null)
        {
            RuntimeUnit target = enemyUI.MyUnit;
            if (UIManager != null) UIManager.Log($"触发攻击：{_selectedAttacker.Name} -> {target.Name}");

            // === 修改：明确传入 consumeAction: true ===
            if (CombatManager != null)
            {
                CombatManager.ProcessUnitAttack(_selectedAttacker, target, consumeAction: true);
            }

            _selectedAttacker = null;
        }
        else
        {
            if (UIManager != null) UIManager.Log($"请先选择己方随从，再点击 {enemyUI.MyUnit.Name}。");
        }
    }

    private void ApplyEquipment(RuntimeCard card, RuntimeUnit target)
    {
        if (card == null || card.Data == null || target == null) return;

        target.Equips.Add(card.Data);
        if (CombatManager != null)
        {
            CombatManager.RecalculateUnitStats(target);
        }
        if (UnitManager != null)
        {
            UnitManager.RefreshUnitUI(target);
        }
        if (UIManager != null)
        {
            UIManager.Log($"{target.Name} 装备了 {card.Data.cardName}");
        }
    }

    // === 回调函数：玩家从墓地选择了某张随从卡 ===
    private void OnGraveyardCardSelected(RuntimeCard selectedUnitCard)
    {
        if (selectedUnitCard == null) return;

        // 1. 尝试召唤
        if (UnitManager != null && UnitManager.TrySummonUnit(selectedUnitCard))
        {
            // 2. 从墓地移除这张卡 (真正复活)
            UnitManager.Graveyard.Remove(selectedUnitCard);

            // 3. 消耗【复活卡】本身 (进入弃牌堆)
            if (_pendingCard2 != null && _pendingCardUIObj2 != null && DeckManager != null)
            {
                DeckManager.DiscardCard(_pendingCard2, _pendingCardUIObj2);
            }

            if (UIManager != null) UIManager.Log($"复活了 {selectedUnitCard.Data.cardName}！");
        }

        // 清除暂存数据
        _pendingCard2 = null;
        _pendingCardUIObj2 = null;
    }

    private void EnterTargetingMode(RuntimeCard card, GameObject uiObj)
    {
        IsTargetingMode = true;
        _pendingCard = card;
        _pendingCardUIObj = uiObj;
        _selectedAttacker = null; // 施法时取消攻击选择
        
        if (UIManager != null && card != null && card.Data != null)
        {
            UIManager.Log($"已选中 {card.Data.cardName} 选择目标...");
        }
        
        if (UnitManager != null)
        {
            UnitManager.EnableTargetingSelection();
        }
    }

    private void ExitTargetingMode()
    {
        IsTargetingMode = false;
        _pendingCard = null;
        _pendingCardUIObj = null;
        if (UnitManager != null)
        {
            UnitManager.RestoreStateAfterTargeting();
        }
    }

    private void CancelTargeting()
    {
        if (!IsTargetingMode) return;
        if (UIManager != null) UIManager.Log("已取消施法！");
        ExitTargetingMode();
    }

    private int RollGoldReward()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentNode == null) return 0;

        var t = GameManager.Instance.CurrentNode.Type;
        Vector2Int r = MinorGoldRange;

        if (t == NodeType.EliteEnemy) r = EliteGoldRange;
        else if (t == NodeType.Boss) r = BossGoldRange;

        // Random.Range(int,int) 上界不包含，所以需要 +1
        return Random.Range(r.x, r.y + 1);
    }

    public void OnGameWin()
    {
        if (_battleEnded) return;
        _battleEnded = true;

        if (UIManager != null) UIManager.Log("战斗胜利！");

        // 血量回传到全局
        if (GameManager.Instance != null && PlayerUnit != null)
        {
            GameManager.Instance.PlayerCurrentHP = PlayerUnit.CurrentHp;
        }

        // 冻结战斗（防止胜利后仍随从攻击）
        CurrentTurnCanAttack = false;
        if (UnitManager != null)
        {
            UnitManager.SetAllAttackStatus(false);
        }
        StopAllCoroutines();
        if (EnemyManager != null)
        {
            EnemyManager.StopAllCoroutines();
        }

        int gold = RollGoldReward();

        // 取得【最后击杀的敌人】单位及其牌库信息（EnemyManager 内部提供这两个属性）
        CardData recruitUnit = (EnemyManager != null) ? EnemyManager.LastKilledUnitCard : null;
        List<CardData> recruitDeck = (EnemyManager != null) ? EnemyManager.LastKilledDeckCards : new List<CardData>();

        if (UIManager != null)
        {
            UIManager.ShowBattleReward(gold, recruitUnit, recruitDeck, (recruit) =>
            {
                if (GameManager.Instance != null)
                {
                    // 给钱
                    if (gold > 0) GameManager.Instance.AddGold(gold);

                    // 决定是否招募
                    if (recruit && recruitUnit != null)
                    {
                        GameManager.Instance.AcquireEnemyUnitAndDeck(recruitUnit, recruitDeck);
                    }
                }

                if (UIManager != null) UIManager.HideBattleReward();
                ReturnToMap();
            });
        }
    }

    void ReturnToMap()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnNodeCompleted();
        }
    }

    public void OnPlayerDefeated()
    {
        if (_battleEnded) return;
        _battleEnded = true;

        if (UIManager != null) UIManager.Log("<color=red>【失败】玩家的单位被击败...</color>");

        // 1. 冻结状态，防止任何操作
        CurrentTurnCanAttack = false;
        if (UnitManager != null)
        {
            UnitManager.SetAllAttackStatus(false);
        }

        // 2. 停止所有协程，防止敌人继续攻击或其他异常行为）
        StopAllCoroutines();
        if (EnemyManager != null)
        {
            EnemyManager.StopAllCoroutines();
        }

        // 3. 显示失败 UI
        if (UIManager != null)
        {
            UIManager.ShowGameOver();
        }
    }

    void FailGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRunFailed();
        }
    }
}
