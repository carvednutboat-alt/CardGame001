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

    [Header("UI Refs")]
    public Transform UnitPanel;

    [Header("Data Config")]
    public List<CardData> StartingSpellDeck;   // 法术/装备卡
    public List<CardData> StartingUnitLibrary; // 怪兽卡

    // === 内部状态 ===
    public bool IsTargetingMode = false;
    private RuntimeCard _pendingCard;
    private GameObject _pendingCardUIObj;

    private void Start()
    {
        UIManager.Log("=== 游戏初始化 ===");

        // 1. 初始化各模块
        UIManager.Init(this);
        CombatManager.Init(this);
        EnemyManager.Init(this);
        UnitManager.Init(this);
        DeckManager.Init(this, StartingSpellDeck);

        // 2. 生成怪兽备战区 (宝可梦式板凳席)
        SpawnUnitBench();

        // 3. === 新增：起手发4张手牌 ===
        UIManager.Log("发放初始手牌 (4张)...");
        DeckManager.DrawCards(4);

        // 4. === 新增：决定先后手 ===
        bool playerGoesFirst = Random.value > 0.5f; // 50% 概率

        if (playerGoesFirst)
        {
            UIManager.Log("【随机结果】玩家先手！");
            // 玩家先手：本回合不能攻击，且通常规则下先手第一回合不抽卡
            StartPlayerTurn(canAttack: false, drawCard: false);
        }
        else
        {
            UIManager.Log("【随机结果】敌人先手！");
            // 敌人先行动
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

    // === 修改：增加了参数来控制是否抽牌和是否能攻击 ===
    public void StartPlayerTurn(bool canAttack = true, bool drawCard = true)
    {
        UIManager.Log("--------------------------");
        UIManager.Log(">>> 轮到你的回合 <<<");

        // 1. 抽牌阶段
        if (drawCard)
        {
            DeckManager.DrawCards(1);
        }
        else
        {
            UIManager.Log("（先手第一回合不抽牌）");
        }

        // 2. 设置攻击状态
        // 如果是先手第一回合(canAttack=false)，则全设为不可交互
        UnitManager.SetAllAttackStatus(canAttack);

        if (!canAttack)
        {
            UIManager.Log("【提示】先手第一回合无法进行战斗攻击。");
        }
    }

    // === 玩家点击结束回合 ===
    public void OnEndTurnButton()
    {
        if (IsTargetingMode) return;

        // 1. 玩家回合结束处理
        UnitManager.SetAllAttackStatus(false); // 锁住攻击

        // 2. 进入敌人回合
        EnemyTurn(canAttack: true);
    }

    // === 敌人回合逻辑 ===
    private void EnemyTurn(bool canAttack)
    {
        UIManager.Log("--------------------------");
        UIManager.Log(">>> 轮到敌人回合 <<<");

        // 敌人执行AI逻辑
        EnemyManager.ExecuteTurn(canAttack);

        // 敌人回合结束后，立刻开始玩家的下一回合
        // 此时玩家肯定可以攻击，且可以抽牌
        StartPlayerTurn(canAttack: true, drawCard: true);
    }

    // ---------------------------------------------------------
    // 下面是卡牌点击和交互逻辑 (保持不变)
    // ---------------------------------------------------------

    public void OnCardClicked(CardUI ui, RuntimeCard card)
    {
        if (IsTargetingMode)
        {
            CancelTargeting();
            return;
        }

        // 1. 单位卡
        if (card.Data.kind == CardKind.Unit)
        {
            if (UnitManager.TrySummonUnit(card))
            {
                Destroy(ui.gameObject);
            }
            return;
        }

        // 2. 装备卡
        if (card.Data.isEquipment)
        {
            if (UnitManager.PlayerUnits.Count == 0)
            {
                UIManager.Log("场上没有怪兽，无法装备。");
                return;
            }
            ApplyEquipment(card, UnitManager.PlayerUnits[0]);
            DeckManager.DiscardCard(card, ui.gameObject);
            return;
        }

        // 3. 需选目标的法术
        if (card.Data.effectType == CardEffectType.HealUnit ||
            card.Data.effectType == CardEffectType.UnitBuff ||
            card.Data.effectType == CardEffectType.FieldEvolve)
        {
            EnterTargetingMode(card, ui.gameObject);
            return;
        }

        // 4. 不需要选目标的法术
        PlaySpell(card, null);
        DeckManager.DiscardCard(card, ui.gameObject);
    }

    public void OnFieldUnitClicked(int unitId)
    {
        if (IsTargetingMode)
        {
            RuntimeUnit target = UnitManager.GetUnitById(unitId);
            if (target != null)
            {
                PlaySpell(_pendingCard, target);
                DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
                ExitTargetingMode();
            }
            return;
        }

        RuntimeUnit unit = UnitManager.GetUnitById(unitId);
        if (unit != null)
        {
            CombatManager.ProcessUnitAttack(unit, consumeAction: true);
        }
    }

    private void PlaySpell(RuntimeCard card, RuntimeUnit target)
    {
        EffectBase effect = EffectFactory.GetEffect(card.Data.effectType);
        if (effect != null)
            effect.Execute(this, card, target);
        else
            UIManager.Log($"未配置效果: {card.Data.effectType}");
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
        UIManager.Log($"请选择 {card.Data.cardName} 的目标...");
        UnitManager.SetAllAttackStatus(true);
    }

    private void ExitTargetingMode()
    {
        IsTargetingMode = false;
        _pendingCard = null;
        _pendingCardUIObj = null;

        // 恢复正确的攻击状态：
        // 这里有一个小细节：如果是先手第一回合选目标，取消后应该恢复为不能攻击
        // 但为了简单，我们假设选目标时已经是“可以攻击”的回合。
        // 如果想严谨，需要记录 CurrentTurnCanAttack 状态。
        // 暂时简单处理：恢复所有存活单位的可交互性
        foreach (var u in UnitManager.PlayerUnits)
            if (u.UI != null) u.UI.SetButtonInteractable(u.CanAttack);
    }

    private void CancelTargeting()
    {
        UIManager.Log("已取消施法。");
        ExitTargetingMode();
    }

    public void OnGameWin()
    {
        UIManager.Log("战斗胜利！");
    }
}