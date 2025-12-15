using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public Unit player;
    public Unit enemy;

    [Header("Player deck setup")]
    public List<CardData> startingDeck = new List<CardData>();

    // 非 Unit 卡：牌堆 / 手牌 / 弃牌堆
    private readonly List<CardData> drawPile    = new List<CardData>();
    private readonly List<CardData> hand        = new List<CardData>();
    private readonly List<CardData> discardPile = new List<CardData>();

    [Header("Hand UI")]
    public Transform handPanel;
    public CardUI cardPrefab;

    [Header("Unit UI")]
    public Transform unitPanel;  // “候选单位”（一开始就看到的怪兽）

    [Header("Field Unit UI")]
    public Transform fieldUnitPanel;   // 场上的单位显示区
    public FieldUnitUI fieldUnitPrefab;

    [Header("Field info")]
    public TMP_Text fieldColorsText;

    [Header("Misc UI")]
    public TMP_Text logText;
    public Button endTurnButton;
    // === 新增 ===
    public ScrollRect logScrollRect;

    [Header("Enemy settings")]
    public int enemyAttackDamage = 5;

    // --------- 内部结构：单位状态 ---------

    private enum UnitStatus
    {
        Ready,   // 在候选列表中，未上场
        OnField, // 已在场上
        Dead     // 已死亡，只能靠复活魔法恢复
    }

    private class UnitCardState
    {
        public CardData data;
        public CardUI ui;
        public UnitStatus status;
    }

    private class FieldUnit
    {
        public int id;
        public CardColor color;
        public string name;

        public int baseAttack;
        public int baseHealth;

        public int maxHealth;
        public int currentAttack;
        public int currentHealth;

        public UnitCardState source;
        public List<CardData> equips = new List<CardData>();

        public bool evolved;
        public int evolveTurnsLeft;
        public bool canAttackThisTurn;
        public bool hasOneFreeExtraAttack;

        //状态类
        public bool isFlying;              // 起飞：可以无视嘲讽、将来可以额外设计效果
        public bool hasTaunt;              // 嘲讽：优先被攻击
        public int  damageTakenThisTurn;   // 本回合受到伤害次数（用于“被打两次就落地”）

        public FieldUnitUI ui;
    }

    private readonly List<UnitCardState> unitCards   = new List<UnitCardState>();
    private readonly List<FieldUnit>     playerUnits = new List<FieldUnit>();

    private readonly HashSet<CardColor> activeColors = new HashSet<CardColor>();
    private int rolesOnField = 0;
    public int maxRolesOnField = 5;

    private bool hasSummonedThisTurn;
    private bool hasEvolvedThisTurn;
    private bool playerGoesFirst;
    private bool gameEnded;

    // 当前回合是否允许战斗：先手第一个回合不能战斗
    private bool battleAllowedThisTurn;
    private int  nextFieldUnitId = 1;

    // =========== 新增：目标选择系统变量 ===========
    private bool isTargetingMode = false; // 是否处于“请选择目标”的状态
    private CardUI pendingCardUI;         // 玩家刚才点击的那张等待生效的卡

    // ----------------- 生命周期 -----------------

    private void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        gameEnded            = false;
        hasSummonedThisTurn  = false;
        hasEvolvedThisTurn   = false;
        battleAllowedThisTurn = false;
        nextFieldUnitId      = 1;

        if (player != null) player.ResetHp();
        if (enemy  != null) enemy.ResetHp();

        drawPile.Clear();
        hand.Clear();
        discardPile.Clear();
        unitCards.Clear();
        playerUnits.Clear();
        activeColors.Clear();
        rolesOnField = 0;
        UpdateFieldColorsText();

        // 清空场上单位 UI
        if (fieldUnitPanel != null)
        {
            foreach (Transform child in fieldUnitPanel)
                Destroy(child.gameObject);
        }

        // startingDeck 拆成 Unit 区 + 牌堆
        foreach (var card in startingDeck)
        {
            if (card == null) continue;

            if (card.kind == CardKind.Unit)
            {
                unitCards.Add(new UnitCardState
                {
                    data   = card,
                    ui     = null,
                    status = UnitStatus.Ready
                });
            }
            else
            {
                drawPile.Add(card);
            }
        }

        Shuffle(drawPile);
        CreateUnitCardsUI();

        playerGoesFirst = Random.value < 0.5f;
        Log(playerGoesFirst ? "你是先手。" : "你是后手。");

        DrawCards(4);

        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.onClick.AddListener(OnEndTurnButton);
            endTurnButton.interactable = true;
        }

        StartPlayerTurn(skipDrawAndBattle: playerGoesFirst);
    }

    private void StartPlayerTurn(bool skipDrawAndBattle)
    {
        if (gameEnded) return;

        hasSummonedThisTurn  = false;
        hasEvolvedThisTurn   = false;
        battleAllowedThisTurn = !skipDrawAndBattle;

        if (!skipDrawAndBattle)
        {
            DecreaseEvolutionTimers();
            DrawCards(1);
            SetUnitsCanAttack(true);
        }
        else
        {
            SetUnitsCanAttack(false);
        }

        Log(skipDrawAndBattle
            ? "你的第一个回合（先手），本回合不能抽牌和战斗。可以上角色 / 进化 / 出牌。"
            : "轮到你行动。可以上角色 / 进化 / 出牌，然后使用单位攻击，最后结束回合。");

        if (endTurnButton != null)
            endTurnButton.interactable = true;
    }

    private void OnEndTurnButton()
    {
        if (gameEnded) return;

        if (endTurnButton != null)
            endTurnButton.interactable = false;

        Log("你的回合结束，轮到敌人。");
        EnemyTurn();
    }

    private void EnemyTurn()
    {
        if (gameEnded) return;

        // 重置回合伤害计数
        ResetUnitsDamageTakenThisTurn();

        if (enemy != null && !enemy.IsDead())
        {
            int dmg = enemyAttackDamage;

            if (playerUnits.Count > 0)
            {
                FieldUnit target = null;
                foreach (var u in playerUnits)
                {
                    if (u.hasTaunt)
                    {
                        target = u;
                        break;
                    }
                }

                if (target == null)
                    target = playerUnits[0];

                Log($"敌人对你的单位 {target.name} 造成 {dmg} 点战斗伤害。");

                bool died = ApplyBattleDamageToFieldUnit(target, dmg);
                if (died)
                {
                    HandleUnitDeath(target, "战斗破坏");
                }
                else
                {
                    Log($"单位 {target.name} 剩余 HP：{target.currentHealth}/{target.maxHealth}");
                }
            }
            else
            {
                if (player != null)
                {
                    Log($"敌人对你造成 {dmg} 点伤害。");
                    player.TakeDamage(dmg);
                    if (player.IsDead())
                    {
                        OnPlayerDefeated();
                        return;
                    }
                }
            }
        }

        StartPlayerTurn(skipDrawAndBattle: false);
    }


    // 统一的单位攻击逻辑：攻击当前这个 enemy
    // consumeAttackChance = true 时，会消耗本回合的攻击机会
    private bool DoUnitAttack(FieldUnit unit, bool consumeAttackChance)
    {
        if (unit == null) return false;
        if (gameEnded) return false;

        // 现在还是单一敌人逻辑，就直接用字段 enemy
        if (enemy == null || enemy.IsDead())
        {
            Log("敌人已经被击败。");
            return false;
        }

        int dmg = Mathf.Max(0, unit.currentAttack);
        Log($"{unit.name} 攻击敌人，造成 {dmg} 点伤害。");

        enemy.TakeDamage(dmg);

        // 是否消耗攻击次数
        if (consumeAttackChance)
        {
            unit.canAttackThisTurn = false;
        }

        // 更新按钮交互状态
        if (unit.ui != null)
        {
            bool canClick = unit.canAttackThisTurn;
            unit.ui.SetButtonInteractable(canClick);
        }

        // 胜负判定：沿用你下面已经写好的 OnEnemyDefeated()
        if (enemy.IsDead())
        {
            OnEnemyDefeated();
        }

        return true;
    }


    // 重置所有我方单位的“本回合受到伤害次数”
    private void ResetUnitsDamageTakenThisTurn()
    {
        foreach (var u in playerUnits)
        {
            u.damageTakenThisTurn = 0;
        }
    }


    // ----------------- 玩家点击：手牌 -----------------

    public void OnCardClicked(CardUI cardView)
    {
        if (gameEnded) return;

        // 如果正在选目标的时候点了别的手牌，我们可以取消之前的选择，换成这张
        if (isTargetingMode)
        {
            CancelTargetingMode();
        }

        if (cardView == null || cardView.Data == null) return;

        CardData card   = cardView.Data;
        bool isUnitCard = (card.kind == CardKind.Unit);

        bool needsColor = card.color != CardColor.Colorless && !isUnitCard;
        if (needsColor && !activeColors.Contains(card.color))
        {
            Log($"你场上没有 {card.color} 角色，无法使用这张牌。");
            return;
        }

        bool played = false;

        if (isUnitCard)
        {
            UnitCardState state = FindUnitStateByUI(cardView);
            if (state == null)
            {
                Log("内部错误：未找到对应的 Unit 状态。");
                return;
            }

            played = PlayUnitCard(state);
        }
        else
        {
            // =========== 修改开始 ===========
            // 如果是“指定单位回血”的卡，不要立刻 Play，而是进入选择模式
            if (card.effectType == CardEffectType.HealUnit ||
                card.effectType == CardEffectType.UnitBuff)
            {
                EnterTargetingMode(cardView);
                return; // 直接返回，不执行下面的销毁逻辑
            }
            // =========== 修改结束 ===========

            if (card.kind == CardKind.Spell)
            {
                played = PlaySpellCard(card);
            }
            else if (card.kind == CardKind.Evolve)
            {
                played = PlayEvolveCard(card);
            }
        }

        if (!played) return;

        if (isUnitCard)
        {
            if (cardView.button != null)
                cardView.button.interactable = false;
        }
        else
        {
            hand.Remove(card);
            discardPile.Add(card);
            Destroy(cardView.gameObject);
        }

        CheckWinLose();
    }

    private UnitCardState FindUnitStateByUI(CardUI ui)
    {
        foreach (var s in unitCards)
        {
            if (s.ui == ui) return s;
        }
        return null;
    }

    // ----------------- 玩家点击：场上单位（攻击） -----------------

    // 被 FieldUnitUI 调用
    // 被 FieldUnitUI 调用
    public void OnFieldUnitClicked(int unitId)
    {
        if (gameEnded) return;

        // 1. 如果当前是在“选择目标用卡牌”的模式，就把点击当作选目标
        if (isTargetingMode)
        {
            ApplyPendingCardToUnit(unitId);
            return;
        }

        // 2. 这个回合是否允许战斗（先手第一回合不能战斗）
        if (!battleAllowedThisTurn)
        {
            Log("本回合不能进行战斗阶段。");
            return;
        }

        // 3. 根据 id 找到场上的这个单位
        FieldUnit unit = null;
        foreach (var u in playerUnits)
        {
            if (u.id == unitId)
            {
                unit = u;
                break;
            }
        }
        if (unit == null) return;

        // 4. 检查这个单位本回合是否还能攻击
        if (!unit.canAttackThisTurn)
        {
            Log($"{unit.name} 本回合已经攻击过。");
            return;
        }

        // 5. 统一交给 DoUnitAttack 来处理攻击逻辑
        DoUnitAttack(unit, consumeAttackChance: true);
    }



    private void SetUnitsCanAttack(bool canAttack)
    {
        foreach (var u in playerUnits)
        {
            u.canAttackThisTurn = canAttack;
            if (canAttack)
                u.hasOneFreeExtraAttack = false;

            if (u.ui != null)
                u.ui.SetButtonInteractable(canAttack);
        }
    }

    // ----------------- 怪兽上场 -----------------

    private bool PlayUnitCard(UnitCardState state)
    {
        if (state.status == UnitStatus.Dead)
        {
            Log("这个单位已经死亡，不能再次上场（需要通过复活魔法恢复）。");
            return false;
        }

        if (state.status == UnitStatus.OnField)
        {
            Log("这个单位已经在场上。");
            return false;
        }

        if (hasSummonedThisTurn)
        {
            Log("本回合已经上过一个角色，无法再上场。");
            return false;
        }

        if (rolesOnField >= maxRolesOnField)
        {
            Log("场上角色已达到上限，无法再上场。");
            return false;
        }

        CardData card = state.data;

        if (card.color != CardColor.Colorless)
            activeColors.Add(card.color);

        int baseHp  = card.unitHealth > 0 ? card.unitHealth : 1;
        int baseAtk = card.unitAttack;

        FieldUnit newUnit = new FieldUnit
        {
            id             = nextFieldUnitId++,
            color          = card.color,
            name           = card.cardName,
            baseAttack     = baseAtk,
            baseHealth     = baseHp,
            maxHealth      = baseHp,
            currentAttack  = baseAtk,
            currentHealth  = baseHp,
            source         = state,
            evolved        = false,
            evolveTurnsLeft = 0,
            canAttackThisTurn = battleAllowedThisTurn,
            isFlying       = card.unitStartsFlying,
            hasTaunt       = card.unitHasTaunt,
            damageTakenThisTurn  = 0
        };

        playerUnits.Add(newUnit);
        rolesOnField++;
        hasSummonedThisTurn = true;
        state.status        = UnitStatus.OnField;

        CreateFieldUnitUI(newUnit);
        RecalculateUnitStats(newUnit);
        UpdateFieldColorsText();

        Log($"你召唤了一个 {card.color} 角色：{card.cardName}（ATK {newUnit.currentAttack} / HP {newUnit.currentHealth}）。");

        return true;
    }

    private void CreateFieldUnitUI(FieldUnit unit)
    {
        if (fieldUnitPanel == null || fieldUnitPrefab == null) return;

        FieldUnitUI ui = Instantiate(fieldUnitPrefab, fieldUnitPanel);
        unit.ui = ui;
        ui.Init(this, unit.id, unit.name, unit.currentAttack, unit.currentHealth,
                unit.evolved, unit.equips.Count, unit.canAttackThisTurn, unit.isFlying, unit.hasTaunt);
    }

    // ----------------- 法术 / 装备 / 进化 / 复活 -----------------

    private bool PlaySpellCard(CardData card)
    {
        // 装备牌：先走装备逻辑
        if (card.isEquipment)
            return PlayEquipCard(card);

        switch (card.effectType)
        {
            case CardEffectType.DamageEnemy:
            {
                int dmg = Mathf.Max(0, card.value);
                if (enemy != null)
                {
                    Log($"你使用 {card.cardName} 对敌人造成 {dmg} 点伤害。");
                    enemy.TakeDamage(dmg);
                    if (enemy.IsDead())
                        OnEnemyDefeated();
                }
                return true;
            }

            case CardEffectType.DamageAllPlayerUnits:
            {
                int dmg = Mathf.Max(0, card.value);
                if (dmg <= 0)
                {
                    Log($"卡牌 {card.cardName} 的伤害数值为 {card.value}，不会造成伤害。");
                    return false;
                }

                Log($"你使用 {card.cardName}，对己方所有怪兽造成 {dmg} 点效果伤害；【起飞】单位免疫本次伤害。");
                DamageAllPlayerUnits(dmg, ignoreFlying: true);
                return true;
            }

            case CardEffectType.DamageAllEnemyUnits:
            {
                int dmg = Mathf.Max(0, card.value);
                if (dmg <= 0)
                {
                    Log($"卡牌 {card.cardName} 的伤害数值为 {card.value}，不会造成伤害。");
                    return false;
                }

                Log($"你使用 {card.cardName}，对所有敌人造成 {dmg} 点效果伤害。");
                DamageAllEnemies(dmg);
                return true;
            }

            case CardEffectType.HealPlayer:
            {
                int heal = Mathf.Max(0, card.value);
                if (player != null)
                {
                    Log($"你使用 {card.cardName}，回复自己 {heal} 点生命。");
                    player.Heal(heal);
                }
                return true;
            }

            case CardEffectType.DrawCards:
            {
                int draw = Mathf.Max(0, card.value);
                if (draw > 0)
                {
                    Log($"你使用 {card.cardName}，抽取 {draw} 张牌。");
                    DrawCards(draw);
                }
                return true;
            }

            case CardEffectType.ReviveUnit:
            {
                int reviveCount = Mathf.Max(1, card.value);
                return ReviveDeadUnits(reviveCount);
            }

            case CardEffectType.None:
            default:
                Log($"卡牌 {card.cardName} 未配置效果（effectType=None），不执行任何效果。");
                return false;
        }
    }

    private bool PlayEvolveCard(CardData card)
    {
        if (hasEvolvedThisTurn)
        {
            Log("本回合已经进行过一次进化。");
            return false;
        }

        bool result;

        if (card.effectType == CardEffectType.FieldEvolve)
        {
            result = PlayFieldEvolveCard(card);
        }
        else
        {
            result = PlaySpellCard(card);
        }

        if (result)
            hasEvolvedThisTurn = true;

        return result;
    }

    private bool PlayEquipCard(CardData card)
    {
        if (playerUnits.Count == 0)
        {
            Log("场上没有怪兽，无法装备。");
            return false;
        }

        FieldUnit target = playerUnits[0]; // 暂时装备到第一个单位
        target.equips.Add(card);
        RecalculateUnitStats(target);

        Log($"你为 {target.name} 装备了 {card.cardName}。");

        return true;
    }

    private bool PlayFieldEvolveCard(CardData card)
    {
        FieldUnit target = null;
        foreach (var u in playerUnits)
        {
            if (u.equips.Count > 0)
            {
                target = u;
                break;
            }
        }

        if (target == null)
        {
            Log("没有装备了装备的怪兽，无法使用字段进化卡。");
            return false;
        }

        target.evolved         = true;
        target.evolveTurnsLeft = 3;
        RecalculateUnitStats(target);

        Log($"你为 {target.name} 使用了字段进化卡，接下来 3 个你的回合内，每张装备提供 +2/+2，装备也受到额外保护。");

        // 进化时：从牌组检索 1 张装备卡（不限制字段）
        if (!FetchEquipmentFromDeckToHand(1, onlyFieldEquipment: false))
        {
            Log("牌组中没有可用的装备卡可以检索。");
        }

        return true;
    }

    private void DecreaseEvolutionTimers()
    {
        foreach (var u in playerUnits)
        {
            if (!u.evolved) continue;

            u.evolveTurnsLeft--;
            if (u.evolveTurnsLeft <= 0)
            {
                u.evolved = false;
                RecalculateUnitStats(u);
                Log($"{u.name} 的进化效果结束。");
            }
        }
    }

    // ----------------- 数据计算 -----------------

    private void RecalculateUnitStats(FieldUnit unit)
    {
        if (unit == null) return;

        int equipCount = unit.equips.Count;
        int perEquip   = unit.evolved ? 2 : 1;

        int baseAtk = unit.baseAttack;
        int baseHp  = unit.baseHealth;

        int traitAtkBonus = equipCount * perEquip;
        int traitHpBonus  = equipCount * perEquip;

        int equipExtraAtk = 0;
        int equipExtraHp  = 0;
        foreach (var eq in unit.equips)
        {
            if (eq == null) continue;
            equipExtraAtk += eq.equipAttackBonus;
            equipExtraHp  += eq.equipHealthBonus;
        }

        unit.currentAttack = baseAtk + traitAtkBonus + equipExtraAtk;
        unit.maxHealth     = Mathf.Max(1, baseHp + traitHpBonus + equipExtraHp);

        if (unit.currentHealth <= 0)
            unit.currentHealth = unit.maxHealth;
        if (unit.currentHealth > unit.maxHealth)
            unit.currentHealth = unit.maxHealth;

        if (unit.ui != null)
            unit.ui.UpdateStats(unit.currentAttack, unit.currentHealth, unit.evolved, unit.equips.Count, unit.isFlying,
    unit.hasTaunt);
    }

    // 统一记录单位受到的伤害，用于处理“起飞被打落地”
    private void RegisterDamageOnUnit(FieldUnit unit, int damage, bool isEffectDamage)
    {
        if (unit == null || damage <= 0) return;

        unit.damageTakenThisTurn++;

        //同一回合受到两次伤害 -> 起飞消失
        if (unit.isFlying && unit.damageTakenThisTurn >= 2)
        {
            unit.isFlying = false;
            Log($"{unit.name} 在同一回合受到了两次伤害，起飞状态失效。");

            if (unit.ui != null)
            unit.ui.UpdateStats(unit.currentAttack, unit.currentHealth,
                                unit.evolved, unit.equips.Count,
                                unit.isFlying, unit.hasTaunt);
        }

    }


    // 效果伤害（法术、技能等）对单位造成伤害
    private bool ApplyEffectDamageToFieldUnit(FieldUnit unit, int damage)
    {
        if (unit == null || damage <= 0) return false;

        unit.currentHealth -= damage;
        if (unit.currentHealth < 0) unit.currentHealth = 0;

        // 记录这次是“效果伤害”，用于起飞在同回合被打两次落地
        RegisterDamageOnUnit(unit, damage, isEffectDamage: true);

        if (unit.ui != null)
        {
            unit.ui.UpdateStats(
                unit.currentAttack,
                unit.currentHealth,
                unit.evolved,
                unit.equips.Count,
                unit.isFlying,
                unit.hasTaunt
            );
        }

        return unit.currentHealth <= 0;
    }

    private bool ApplyBattleDamageToFieldUnit(FieldUnit unit, int damage)
    {
        if (unit == null || damage <= 0) return false;

            bool    hasBattleShield = false;
        CardData shieldCard     = null;

        foreach (var equip in unit.equips)
        {
            if (equip != null && equip.shieldBattleDestroy)
            {
                hasBattleShield = true;
                shieldCard      = equip;
                break;
            } 
        }

        if (damage >= unit.currentHealth && hasBattleShield)
        {
            DestroyEquipment(unit, shieldCard, "战斗破坏替代");
            Log($"{unit.name} 将要在战斗中被破坏，改为破坏装备 {shieldCard.cardName} 作为替代。");
            return false;
        }

        unit.currentHealth -= damage;
        if (unit.currentHealth < 0) unit.currentHealth = 0;

        // ★ 登记这次伤害（战斗伤害）
        RegisterDamageOnUnit(unit, damage, isEffectDamage: false);

        if (unit.ui != null)
            unit.ui.UpdateStats(unit.currentAttack, unit.currentHealth,
                                unit.evolved, unit.equips.Count,
                                unit.isFlying, unit.hasTaunt);

        return unit.currentHealth <= 0;
    }

    // 对我方所有场上单位造成效果伤害；ignoreFlying=true 时，起飞单位免疫
    private void DamageAllPlayerUnits(int damage, bool ignoreFlying)
    {
        if (damage <= 0) return;

        List<FieldUnit> deadUnits = new List<FieldUnit>();

        foreach (var unit in playerUnits)
        {
            if (unit == null) continue;

            // 起飞免疫群体伤害
            if (ignoreFlying && unit.isFlying)
            {
                Log($"{unit.name} 处于【起飞】状态，免疫本次群体效果伤害。");
                continue;
            }

            bool died = ApplyEffectDamageToFieldUnit(unit, damage);
            if (died)
            {
                deadUnits.Add(unit);
            }
            else
            {
                Log($"{unit.name} 受到 {damage} 点效果伤害，剩余 HP：{unit.currentHealth}/{unit.maxHealth}");
            }   
        }

        // 统一处理死亡（避免在遍历列表时修改列表）
        foreach (var u in deadUnits)
        {
            HandleUnitDeath(u, "效果破坏");
        }
    }

    private void DamageAllEnemies(int damage)
    {
        if (damage <= 0) return;

        if (enemy != null && !enemy.IsDead())
        {
            Log($"群体伤害对敌人造成 {damage} 点效果伤害。");
            enemy.TakeDamage(damage);

            if (enemy.IsDead())
            {
                OnEnemyDefeated();
            }
        }
    }   
    private void DestroyEquipment(FieldUnit unit, CardData equipCard, string reason)
    {
        if (unit == null || equipCard == null) return;

        if (unit.evolved && reason.Contains("效果"))
        {
            Log($"由于 {unit.name} 处于进化状态，装备 {equipCard.cardName} 不会被效果破坏。");
            return;
        }

        if (unit.equips.Remove(equipCard))
        {
            Log($"装备 {equipCard.cardName} 被破坏（{reason}）。");
            RecalculateUnitStats(unit);
        }
    }

    private void HandleUnitDeath(FieldUnit unit, string reason)
    {
        if (unit == null) return;

        Log($"你的单位 {unit.name} 被{reason}。");

        if (unit.source != null)
        {
            unit.source.status = UnitStatus.Dead;
            if (unit.source.ui != null && unit.source.ui.button != null)
                unit.source.ui.button.interactable = false;
        }

        CardColor deadColor = unit.color;

        if (unit.ui != null)
            Destroy(unit.ui.gameObject);

        playerUnits.Remove(unit);
        rolesOnField = Mathf.Max(0, rolesOnField - 1);

        if (deadColor != CardColor.Colorless)
        {
            bool stillHasColor = false;
            foreach (var u in playerUnits)
            {
                if (u.color == deadColor)
                {
                    stillHasColor = true;
                    break;
                }
            }

            if (!stillHasColor)
                activeColors.Remove(deadColor);
        }

        UpdateFieldColorsText();
    }

    private bool ReviveDeadUnits(int count)
    {
        int revived = 0;

        foreach (var state in unitCards)
        {
            if (state.status != UnitStatus.Dead) continue;

            state.status = UnitStatus.Ready;

            if (state.ui != null && state.ui.button != null)
                state.ui.button.interactable = true;

            revived++;
            if (revived >= count)
                break;
        }

        if (revived == 0)
        {
            Log("没有死亡的单位可以复活。");
            return false;
        }

        Log($"复活了 {revived} 个单位，现在可以再次上场。");
        return true;
    }

    private bool FetchEquipmentFromDeckToHand(int count, bool onlyFieldEquipment)
    {
        int fetched = 0;

        for (int i = drawPile.Count - 1; i >= 0 && fetched < count; i--)
        {
            CardData card = drawPile[i];
            if (card == null) continue;

            if (!card.isEquipment) continue;
            if (onlyFieldEquipment && !card.isFieldEquipment) continue;

            drawPile.RemoveAt(i);
            hand.Add(card);
            CreateCardView(card);
            fetched++;
        }

        return fetched > 0;
    }

    // ----------------- 抽牌 / UI / 杂项 -----------------

    private void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                RefillDrawPile();
                if (drawPile.Count == 0)
                {
                    Log("牌堆和弃牌堆都空了，无法继续抽牌。");
                    return;
                }
            }

            CardData data = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(data);
            CreateCardView(data);
        }
    }

    private void CreateCardView(CardData data)
    {
        if (cardPrefab == null || handPanel == null) return;

        CardUI instance = Instantiate(cardPrefab, handPanel);
        instance.Init(data, this);
    }

    private void CreateUnitCardsUI()
    {
        Transform panel = unitPanel != null ? unitPanel : handPanel;
        if (panel == null || cardPrefab == null) return;

        foreach (var state in unitCards)
        {
            CardUI instance = Instantiate(cardPrefab, panel);
            state.ui = instance;
            instance.Init(state.data, this);

            if (instance.button != null)
                instance.button.interactable = true;
        }
    }

    private void RefillDrawPile()
    {
        if (discardPile.Count == 0) return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
        Log("将弃牌堆洗回牌堆。");
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            CardData tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    private void UpdateFieldColorsText()
    {
        if (fieldColorsText == null) return;

        if (rolesOnField == 0)
        {
            fieldColorsText.text = "场上角色颜色：无";
            return;
        }

        List<string> colors = new List<string>();
        foreach (var c in activeColors)
        {
            colors.Add(c.ToString());
        }

        fieldColorsText.text =
            "场上角色颜色：" + string.Join(", ", colors) + $"（共 {rolesOnField} 个角色）";
    }

    private void CheckWinLose()
    {
        if (gameEnded) return;

        if (enemy != null && enemy.IsDead())
        {
            OnEnemyDefeated();
        }
        else if (player != null && player.IsDead())
        {
            OnPlayerDefeated();
        }
    }

    private void OnEnemyDefeated()
    {
        if (gameEnded) return;
        gameEnded = true;
        Log("你击败了敌人，战斗胜利！");
        if (endTurnButton != null)
            endTurnButton.interactable = false;
    }

    private void OnPlayerDefeated()
    {
        if (gameEnded) return;
        gameEnded = true;
        Log("你被击败了，战斗失败。");
        if (endTurnButton != null)
            endTurnButton.interactable = false;
    }

    private void Log(string msg)
    {
        if (logText != null)
        {
        logText.text += "\n" + msg;

        // === 新增：强制滚动到底部 ===
        // Canvas 的刷新不是实时的，所以我们需要等一帧再滚动，否则滚不到最新的一行
        StartCoroutine(ScrollToBottom());
        }
        else
            Debug.Log(msg);
    }

    // 这是一个协程，用于等待一帧
    private IEnumerator ScrollToBottom()
    {
        // 等待这一帧结束，确保 UI 已经把字加上去了，高度已经算好了
        yield return new WaitForEndOfFrame();

        if (logScrollRect != null)
        {
            // 0 代表最底部，1 代表最顶部
            logScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void EnterTargetingMode(CardUI cardUI)
    {
        if (playerUnits.Count == 0)
        {
            Log("场上没有单位可以治疗。");
            return;
        }

        isTargetingMode = true;
        pendingCardUI = cardUI;
        Log($"请选择一个场上单位来使用 {cardUI.Data.cardName}...");

        // === 关键修改：强制让所有单位按钮变亮，允许点击 ===
        foreach (var unit in playerUnits)
        {
            if (unit.ui != null)
            {
                // 无论它能不能攻击，现在都可以点（因为是选目标）
                unit.ui.SetButtonInteractable(true);
                // 进阶优化：你甚至可以在 FieldUnitUI 里写一个 Highlight() 方法让它闪光
            }
        }
    }

    // 退出/取消选择模式
    private void ExitTargetingMode()
    {
        isTargetingMode = false;
        pendingCardUI = null;

        // === 关键修改：还原按钮状态 ===
        // 恢复成“只有能攻击的单位才能点”的状态
        foreach (var unit in playerUnits)
        {
            if (unit.ui != null)
            {
                // 还原回 unit.canAttackThisTurn 的状态
                unit.ui.SetButtonInteractable(unit.canAttackThisTurn);
            }
        }
    }

    private void CancelTargetingMode()
    {
        isTargetingMode = false;
        pendingCardUI = null;
        Log("已取消使用卡牌。");
    }

    // 回血逻辑
    private void ApplyPendingCardToUnit(int unitId)
    {
        if (pendingCardUI == null) return;

        // 1. 找到被点击的单位
        FieldUnit targetUnit = null;
        foreach (var u in playerUnits)
        {
            if (u.id == unitId)
            {
                targetUnit = u;
                break;
            }
        }
        if (targetUnit == null) return;

        CardData card = pendingCardUI.Data;
        bool effectApplied = false;

        switch (card.effectType)
        {
            // ① 原来的回血卡逻辑
            case CardEffectType.HealUnit:
            {
                int healAmount = Mathf.Max(0, card.value);
                Log($"你对 {targetUnit.name} 使用了 {card.cardName}，恢复 {healAmount} 点生命。");

                targetUnit.currentHealth += healAmount;
                if (targetUnit.currentHealth > targetUnit.maxHealth)
                    targetUnit.currentHealth = targetUnit.maxHealth;

                if (targetUnit.ui != null)
                {
                    targetUnit.ui.UpdateStats(
                        targetUnit.currentAttack,
                        targetUnit.currentHealth,
                        targetUnit.evolved,
                        targetUnit.equips.Count,
                        targetUnit.isFlying,
                        targetUnit.hasTaunt
                    );
                }

                effectApplied = true;
                break;
            }

            // ② 通用单位增益：根据配置决定起飞 / 追加攻击
            case CardEffectType.UnitBuff:
            {
                // 起飞
                if (card.buffGrantFlying)
                {
                    ApplyFlyingBuff(targetUnit);
                }

                // 立刻额外攻击一次（不消耗攻击次数）
                if (card.buffFreeAttackNow)
                {
                    ApplyFreeAttackBuff(targetUnit);
                }

                if (!card.buffGrantFlying && !card.buffFreeAttackNow)
                {
                    Log($"{card.cardName} 没有配置具体的 UnitBuff 效果（buffGrantFlying / buffFreeAttackNow 都是 false）。");
                    effectApplied = false;
                }
                else
                {
                    effectApplied = true;
                }

                break;
            }

            // 如果哪张卡误配了 effectType，却走进了选目标模式，也给个提示
            default:
            {
                Log($"卡牌 {card.cardName} 的效果类型 {card.effectType} 不适用于选目标模式。");
                effectApplied = false;
                break;
            }
        }

        // 只有在确实生效的情况下才消耗手牌
        if (effectApplied)
        {
            hand.Remove(card);
            discardPile.Add(card);
            Destroy(pendingCardUI.gameObject);
        }

        // 退出选目标状态 & 恢复按钮交互
        ExitTargetingMode();
        CheckWinLose();
    }

// ----------------- 效果 / 状态类 -----------------
// 起飞效果：只负责改状态 + 刷 UI
    private void ApplyFlyingBuff(FieldUnit unit)
    {
        if (unit == null) return;

        if (unit.isFlying)
        {
            Log($"{unit.name} 已经处于【起飞】状态。");
            return;
        }

        unit.isFlying = true;
        Log($"{unit.name} 获得了【起飞】状态。");

        if (unit.ui != null)
        {
            unit.ui.UpdateStats(
                unit.currentAttack,
                unit.currentHealth,
                unit.evolved,
                unit.equips.Count,
                unit.isFlying,
                unit.hasTaunt
            );
        }
}

    // 立刻追加一次攻击：只负责调用 DoUnitAttack，不改 canAttackThisTurn
    private void ApplyFreeAttackBuff(FieldUnit unit)
    {
        if (unit == null) return;

        Log($"{unit.name} 立刻发动一次额外攻击（不消耗本回合攻击次数）。");
        DoUnitAttack(unit, consumeAttackChance: false);
    }

}

