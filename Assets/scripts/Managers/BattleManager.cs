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

    // === 内部状态 ===
    public bool IsTargetingMode = false;
    private RuntimeCard _pendingCard;
    private GameObject _pendingCardUIObj;

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
        DeckManager.Init(this, StartingSpellDeck);

        SpawnUnitBench();

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

    private void SpawnUnitBench()
    {
        if (DeckManager.CardPrefab == null || UnitPanel == null) return;
        foreach (var data in StartingUnitLibrary)
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
        StartPlayerTurn(canAttack: true, drawCard: true);
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
            ApplyEquipment(card, UnitManager.PlayerUnits[0]);
            // === 修改：移出手牌，但不进弃牌堆 ===
            DeckManager.RemoveCardFromHand(card, ui.gameObject);
            return;
        }

        // 3. 需选目标的法术 (Heal / Buff / Evolve)
        // 这些卡需要先点一下进模式，再点怪兽生效
        if (card.Data.effectType == CardEffectType.HealUnit ||
            card.Data.effectType == CardEffectType.UnitBuff ||
            card.Data.effectType == CardEffectType.FieldEvolve)
        {
            EnterTargetingMode(card, ui.gameObject);
            return;
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
            RuntimeUnit target = UnitManager.GetUnitById(unitId);
            if (target == null) return;

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

    // === 新增：点击了敌人 ===
    // 由 EnemyUnitUI 调用
    public void OnEnemyClicked()
    {
        // 1. 如果处于施法模式，也许未来会有指向敌人的法术，暂时先不管
        if (IsTargetingMode) return;

        // 2. 攻击结算
        if (_selectedAttacker != null)
        {
            // 执行攻击
            CombatManager.ProcessUnitAttack(_selectedAttacker, consumeAction: true);

            // 攻击完重置
            _selectedAttacker = null;
        }
        else
        {
            UIManager.Log("请先选择我方怪兽，再点击敌人进行攻击。");
        }
    }

    private void ApplyEquipment(RuntimeCard card, RuntimeUnit target)
    {
        target.Equips.Add(card.Data);
        UnitManager.RefreshUnitUI(target);
        UIManager.Log($"{target.Name} 装备了 {card.Data.cardName}");
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

    public void OnGameWin() => UIManager.Log("战斗胜利！");
    public void OnPlayerDefeated() => UIManager.Log("【失败】你的生命值归零了...");
}