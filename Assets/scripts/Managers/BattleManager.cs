using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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

    // === 内部状态 ===
    public bool IsTargetingMode = false;
    private RuntimeCard _pendingCard;
    private GameObject _pendingCardUIObj;

    private RuntimeCard _pendingCard2;
    private GameObject _pendingCardUIObj2;

    // 控制当前回合是否允许攻击 (先手限制)
    public bool CurrentTurnCanAttack { get; private set; } = true;

    // === 新增状态 1：召唤限制 ===
    public bool HasSummonedThisTurn { get; private set; } = false;

    // === 新增状态 2：攻击选目标模式 ===
    // 当玩家点击了自己的怪兽，准备攻击时，记录谁要攻击
    private RuntimeUnit _selectedAttacker;

    private void Start()
    {
        UIManager.Log("=== 游戏初始化 ===");
        UIManager.Init(this);
        CombatManager.Init(this);
        EnemyManager.Init(this);
        UnitManager.Init(this);

        // --- 修改开始：对接 GameManager ---
        List<CardData> spellsForHand = new List<CardData>();
        List<CardData> unitsForBench = new List<CardData>();

        // 2. 从全局获取所有卡牌
        if (GameManager.Instance != null)
        {
            // 分类：法术进牌库，随从进备战席
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit) // 假设你的 CardData 有 kind 字段
                {
                    unitsForBench.Add(card);
                }
                else
                {
                    spellsForHand.Add(card);
                }
            }

            // --- 新增：同步玩家血量 ---
            if (GameManager.Instance != null && PlayerUnit != null)
            {
                // 从全局管理器读取血量，赋值给场上的玩家单位
                PlayerUnit.InitData(GameManager.Instance.PlayerCurrentHP, GameManager.Instance.PlayerMaxHP);
                UIManager.Log($"玩家血量已同步: {PlayerUnit.CurrentHp}/{PlayerUnit.maxHp}");
            }
            else if (PlayerUnit != null)
            {
                // 如果没有GameManager (单独调试战斗场景)，默认满血
                PlayerUnit.InitData(PlayerUnit.maxHp, PlayerUnit.maxHp);
            }
        }
        else
        {
            // 调试模式：使用 BattleManager 面板上的默认配置
            spellsForHand = StartingSpellDeck;
            unitsForBench = StartingUnitLibrary;
        }

        // 3. 初始化模块
        DeckManager.Init(this, spellsForHand); // 初始化法术牌库

        // 4. 初始化随从 (修复了你的bug：现在是动态生成的)
        SpawnUnitBench(unitsForBench);

        UIManager.Log("发放初始手牌 (4张)...");
        DeckManager.DrawCards(4);

        bool playerGoesFirst = Random.value > 0.5f;
        if (playerGoesFirst)
        {
            UIManager.Log("【随机结果】玩家先手！");
            StartPlayerTurn(canAttack: false, drawCard: false);
        }
        else
        {
            UIManager.Log("【随机结果】敌人先手！");
            EnemyTurn(canAttack: false);
        }
    }

    // 修改 SpawnUnitBench 接受参数
    private void SpawnUnitBench(List<CardData> units)
    {
        // 清理旧的（如果是重用场景）
        foreach (Transform child in UnitPanel) Destroy(child.gameObject);

        if (DeckManager.CardPrefab == null || UnitPanel == null) return;

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
        HasSummonedThisTurn = false; // === 新增：新回合重置召唤次数 ===
        _selectedAttacker = null;    // 重置攻击选择

        UIManager.Log("--------------------------");
        UIManager.Log(">>> 轮到你的回合 <<<");

        if (drawCard) DeckManager.DrawCards(1);
        else UIManager.Log("（先手第一回合不抽牌）");

        // 设置攻击状态
        UnitManager.SetAllAttackStatus(canAttack);

        if (!canAttack) UIManager.Log("【提示】先手第一回合无法进行战斗攻击。");
    }

    public void OnEndTurnButton()
    {
        if (IsTargetingMode) return;
        // 取消可能的攻击选择
        _selectedAttacker = null;
        UnitManager.SetAllAttackStatus(false);
        EnemyTurn(canAttack: true);
    }

    private void EnemyTurn(bool canAttack)
    {
        UIManager.Log("--------------------------");
        UIManager.Log(">>> 轮到敌人回合 <<<");
        EnemyManager.ExecuteTurn(canAttack);
    }

    // ---------------------------------------------------------
    // 卡牌交互逻辑 
    // ---------------------------------------------------------

    public void OnCardClicked(CardUI ui, RuntimeCard card)
    {
        if (IsTargetingMode)
        {
            CancelTargeting();
            return;
        }

        // 如果正在选攻击目标时点了卡牌，取消攻击选择
        if (_selectedAttacker != null)
        {
            UIManager.Log("取消攻击选择。");
            _selectedAttacker = null;
        }

        // 1. 单位卡 (增加召唤限制)
        if (card.Data.kind == CardKind.Unit)
        {
            // === 修改：检查召唤限制 ===
            if (HasSummonedThisTurn)
            {
                UIManager.Log("<color=red>本回合已经召唤过怪兽了！</color>");
                return;
            }

            if (UnitManager.TrySummonUnit(card))
            {
                HasSummonedThisTurn = true; // 标记已召唤
                Destroy(ui.gameObject);
            }
            return;
        }

        // 2. 装备卡 (修改生命周期)
        if (card.Data.isEquipment)
        {
            if (UnitManager.PlayerUnits.Count == 0)
            {
                UIManager.Log("场上没有怪兽，无法装备。");
                return;
            }
            EnterTargetingMode(card, ui.gameObject);
            return;
        }

        // 3. 需选目标的法术 (Heal / Buff / Evolve)
        // 这些卡需要先点一下进模式，再点怪兽生效
        if (card.Data.effectType == CardEffectType.HealUnit ||
            card.Data.effectType == CardEffectType.UnitBuff ||
            card.Data.effectType == CardEffectType.FieldEvolve ||
            card.Data.effectType == CardEffectType.DamageEnemy)
        {
            EnterTargetingMode(card, ui.gameObject);
            return;
        }

        // === 核心修改：拦截复活卡 ===
        if (card.Data.effectType == CardEffectType.ReviveUnit)
        {
            // 1. 检查墓地是否为空
            if (UnitManager.Graveyard.Count == 0)
            {
                UIManager.Log("墓地是空的，无法使用。");
                return;
            }

            // 2. 暂存这张牌 (因为它还没生效，不能立刻弃牌)
            _pendingCard2 = card;
            _pendingCardUIObj2 = ui.gameObject;

            // 3. 打开墓地选择面板，并传入回调函数
            UIManager.ShowGraveyardSelection(UnitManager.Graveyard, OnGraveyardCardSelected);

            UIManager.Log("请从墓地选择要复活的怪兽...");
            return; // 拦截成功，不再往下执行
        }

        // 4. 不需要选目标的法术 (Revive / Draw / AOE)
        EffectBase effect = EffectFactory.GetEffect(card.Data.effectType);
        if (effect != null)
        {
            // 如果条件不满足 (比如墓地为空)，CheckCondition 内部会报Log并返回 false
            if (!effect.CheckCondition(this, card, null)) return;

            effect.Execute(this, card, null);
            DeckManager.DiscardCard(card, ui.gameObject);
        }
        else
        {
            UIManager.Log($"未配置效果: {card.Data.effectType}");
        }
    }

    public void OnFieldUnitClicked(int unitId)
    {
        // 1. 施法模式
        if (IsTargetingMode)
        {
            // 检查：这张卡必须是针对“友军”或“所有人”的
            if (_pendingCard.Data.targetType != CardTargetType.Ally &&
                _pendingCard.Data.targetType != CardTargetType.All)
            {
                UIManager.Log("<color=red>这张卡不能对己方怪兽使用！</color>");
                return; // 拦截！不退出模式，让玩家重选
            }
            RuntimeUnit target = UnitManager.GetUnitById(unitId);
            if (target == null) return;

            // 2. === 新增：如果是装备牌，执行装备逻辑 ===
            if (_pendingCard.Data.isEquipment)
            {
                ApplyEquipment(_pendingCard, target); // 传参：把卡给目标穿上

                // 装备牌特殊处理：从手牌移除，但不进墓地 (因为它在场上)
                DeckManager.RemoveCardFromHand(_pendingCard, _pendingCardUIObj);

                ExitTargetingMode();
                return;
            }
            // =======================================

            EffectBase effect = EffectFactory.GetEffect(_pendingCard.Data.effectType);
            if (effect != null)
            {
                if (!effect.CheckCondition(this, _pendingCard, target)) { ExitTargetingMode(); return; }
                effect.Execute(this, _pendingCard, target);
                DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
            }
            ExitTargetingMode();
            return;
        }

        // 2. 攻击选择模式 (新逻辑)
        RuntimeUnit unit = UnitManager.GetUnitById(unitId);
        if (unit != null)
        {
            // 检查是否能攻击
            if (!CurrentTurnCanAttack) { UIManager.Log("本回合无法进行攻击。"); return; }
            if (!unit.CanAttack) { UIManager.Log($"{unit.Name} 本回合已经攻击过或无法行动。"); return; }

            // 选中这个单位作为攻击者
            _selectedAttacker = unit;
            UIManager.Log($"已选择 {unit.Name}，<color=yellow>请点击敌人进行攻击！</color>");
        }
    }

    // 接收来自 EnemyUnitUI 的点击
    public void OnEnemyClicked(EnemyUnitUI enemyUI)
    {
        // 1. === 新增：如果是“施法瞄准模式” ===
        if (IsTargetingMode)
        {
            // 检查：这张卡必须是针对“敌人”或“所有人”的
            if (_pendingCard.Data.targetType != CardTargetType.Enemy &&
                _pendingCard.Data.targetType != CardTargetType.All)
            {
                UIManager.Log("<color=red>这张卡只能对自己人使用！</color>");
                return; // 拦截！
            }
            RuntimeUnit target = enemyUI.MyUnit;
            if (target == null) return;

            // 获取待打出的法术效果
            EffectBase effect = EffectFactory.GetEffect(_pendingCard.Data.effectType);
            if (effect != null)
            {
                // 执行效果 (传入火球卡, 和选中的敌人目标)
                effect.Execute(this, _pendingCard, target);

                // 扣费、丢弃手牌
                DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
            }

            // 退出瞄准模式
            ExitTargetingMode();
            return;
        }

        // 2. 怪兽普攻模式
        if (_selectedAttacker != null)
        {
            RuntimeUnit target = enemyUI.MyUnit;
            UIManager.Log($"发起攻击：{_selectedAttacker.Name} -> {target.Name}");

            // === 修改：明确传入 consumeAction: true ===
            CombatManager.ProcessUnitAttack(_selectedAttacker, target, consumeAction: true);

            _selectedAttacker = null;
        }
        else
        {
            UIManager.Log($"请先选择我方怪兽，再点击 {enemyUI.MyUnit.Name}。");
        }
    }

    private void ApplyEquipment(RuntimeCard card, RuntimeUnit target)
    {
        target.Equips.Add(card.Data);
        CombatManager.RecalculateUnitStats(target);
        UnitManager.RefreshUnitUI(target);
        UIManager.Log($"{target.Name} 装备了 {card.Data.cardName}");
    }

    // === 回调函数：玩家点选了某张墓地卡 ===
    private void OnGraveyardCardSelected(RuntimeCard selectedUnitCard)
    {
        // 1. 尝试召唤
        if (UnitManager.TrySummonUnit(selectedUnitCard))
        {
            // 2. 从墓地移除这张卡 (它复活了)
            UnitManager.Graveyard.Remove(selectedUnitCard);

            // 3. 结算“死者苏生”这张牌 (丢进弃牌堆)
            if (_pendingCard2 != null && _pendingCardUIObj2 != null)
            {
                DeckManager.DiscardCard(_pendingCard2, _pendingCardUIObj2);
            }

            UIManager.Log($"复活了 {selectedUnitCard.Data.cardName}！");
        }

        // 清理暂存引用
        _pendingCard2 = null;
        _pendingCardUIObj2 = null;
    }

    private void EnterTargetingMode(RuntimeCard card, GameObject uiObj)
    {
        IsTargetingMode = true;
        _pendingCard = card;
        _pendingCardUIObj = uiObj;
        _selectedAttacker = null; // 施法时取消攻击选择
        UIManager.Log($"请选择 {card.Data.cardName} 的目标...");
        UnitManager.EnableTargetingSelection();
    }

    private void ExitTargetingMode()
    {
        IsTargetingMode = false;
        _pendingCard = null;
        _pendingCardUIObj = null;
        UnitManager.RestoreStateAfterTargeting();
    }

    private void CancelTargeting()
    {
        if (!IsTargetingMode) return;
        UIManager.Log("已取消施法。");
        ExitTargetingMode();
    }

    public void OnGameWin()
    {
        UIManager.Log("战斗胜利！");

        // --- 新增：保存血量回全局 ---
        if (GameManager.Instance != null && PlayerUnit != null)
        {
            GameManager.Instance.PlayerCurrentHP = PlayerUnit.CurrentHp;
            // 如果你有逻辑能在战斗中提升最大生命值，也要保存 MaxHP
            // GameManager.Instance.PlayerMaxHP = PlayerUnit.maxHp; 
        }
        // -------------------------

        // 延迟回地图
        if (GameManager.Instance != null)
        {
            Invoke(nameof(ReturnToMap), 2.0f);
        }
    }

    void ReturnToMap()
    {
        GameManager.Instance.OnNodeCompleted();
    }
    public void OnPlayerDefeated()
    {
        UIManager.Log("<color=red>【失败】你的生命值归零了...</color>");

        // 1. 锁定状态，禁止任何操作
        CurrentTurnCanAttack = false;
        UnitManager.SetAllAttackStatus(false);
        // 如果有“结束回合”按钮，最好也禁用掉（可以通过 UIManager 暴露接口来做）

        // 2. 停止所有协程（防止敌人继续攻击或者动画继续播放）
        StopAllCoroutines();
        EnemyManager.StopAllCoroutines(); // 如果 EnemyManager 也有协程

        // 3. 显示失败 UI
        if (UIManager != null)
        {
            UIManager.ShowGameOver();
        }
    }

    void FailGame()
    {
        GameManager.Instance.OnRunFailed();
    }
}