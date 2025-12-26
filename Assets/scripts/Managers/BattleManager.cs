using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [ContextMenu("Debug: Inject Lua Deck")]
    public void DebugInjectLuaDeck()
    {
        if (DeckManager == null) return;
        var cards = DevCardLoader.GetLuaTestCards();
        foreach (var data in cards)
        {
            // Add to DrawPile
            DeckManager.DrawPile.Add(new RuntimeCard(data));
        }
        DeckManager.ShuffleDeck();
        UIManager.Log("Debug: Lua Test Cards injected into Draw Pile!");
    }

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
    public bool IsSlotSelectionMode = false; // 新增：选槽位模式

    private RuntimeCard _pendingCard;
    private GameObject _pendingCardUIObj;

    private RuntimeCard _pendingCard2;
    private GameObject _pendingCardUIObj2;
    private bool _battleEnded = false;

    // === Lua Effect Integration ===
    private Effect _pendingEffect; // The effect currently gathering targets

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
                    // === 修改：如果单位标记为 startsInDeck，则放入手牌/抽牌堆 ===
                    if (card.startsInDeck)
                    {
                        spellsForHand.Add(card);
                    }
                    else
                    {
                        unitsForBench.Add(card);
                    }
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
            int baseDraw = 1;
            int extraDraw = 0;
            
            // 检查Relic额外抽牌效果
            if (RelicManager.Instance != null)
            {
                extraDraw = RelicManager.Instance.GetExtraDrawCount();
            }
            
            int totalDraw = baseDraw + extraDraw;
            DeckManager.DrawCards(totalDraw);
            
            if (extraDraw > 0 && UIManager != null)
            {
                UIManager.Log($"额外抽牌数: +{extraDraw}");
            }
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
        if (IsTargetingMode || IsSlotSelectionMode) return;
        
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
            // 重置本回合临时属性 (如突袭加攻)
            UnitManager.ResetTempStats();
            // === NEW: Check Commander ===
            UnitManager.CheckCommanderStatus();
            // === NEW: Overload Processing ===
            UnitManager.ProcessOverloadEndTurn();
        }
        
        
        // 触发Relic回合结束效果
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.TriggerEndTurnEffects(this);
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

    // 卡牌交互逻辑 - 修复：使用两参数签名
    
    public void OnCardClicked(CardUI ui, RuntimeCard card)
    {
        if (IsTargetingMode || IsSlotSelectionMode) return;

        // 检查是否有注册 Lua 效果
        if (card.Effects != null && card.Effects.Count > 0)
        {
            foreach (var e in card.Effects)
            {
                // 如果是起动效果 (IGNITION)
                if (e.EffectCode == Effect.TYPE_IGNITION) 
                {
                    if (e.CheckCondition(this, card))
                    {
                        // 暂存上下文，进入 C-C-T-O 流程
                        _pendingEffect = e;
                        _pendingCard = card;
                        _pendingCardUIObj = ui.gameObject;

                        e.PayCost(this);
                        e.ResolveTarget(this); // 如果 Lua 里调用了 SelectTarget，会开启瞄准模式

                        if (!IsTargetingMode) // 如果不需要选目标，直接执行
                        {
                            e.ExecuteOperation(this);
                            FinishEffect(card, ui.gameObject);
                        }
                        return; 
                    }
                }
            }
        }

        // 如果没有任何 Lua 效果，且是怪兽牌，才走默认召唤逻辑
        if (card.Data.kind == CardKind.Unit)
        {
            EnterSlotSelectionMode(card, ui.gameObject);
        }
    }

    private void FinishEffect(RuntimeCard card, GameObject uiObj)
    {
        if (card.Data.kind != CardKind.Unit && DeckManager != null)
        {
            DeckManager.DiscardCard(card, uiObj);
        }
        _pendingEffect = null;
    }

    public void OnFieldUnitClicked(int unitId)
    {
        if (UnitManager == null) return;
        
        // 1. Targerting Resolution
        if (IsTargetingMode && _pendingEffect != null)
        {
            // Resolve effect on this target
            RuntimeUnit target = UnitManager.GetUnitById(unitId);
            if (target != null)
            {
                // We should validate target with the filter?
                // Lua side does `Duel.SelectTarget(f)`. 
                // We need to pass this target back to Lua or set it in a context.
                
                // For this implementation, we simply execute the Operation.
                // Ideally, we should set `Duel.Target = target` before calling Op.
                // We will add a temporary hack: `Duel.LastSelectedTarget = target;`
                
                // But wait, `Effect.ExecuteOperation` calls `Operation(e)`.
                // Lua script does `local tc = Duel.GetFirstTarget()`.
                
                // Let's Add `Duel.SetCurrentTarget(target)` helper?
                // Or just assume the `Operation` uses the target we pass (if we modify delegate).
                
                // Let's set it in a way `Duel.GetFirstTarget()` can retrieve.
                Duel.SetSelection(target);

                _pendingEffect.ExecuteOperation(this, target);
                
                if (DeckManager != null && _pendingCard != null && _pendingCard.Data.kind != CardKind.Unit)
                {
                    DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
                }

                ExitTargetingMode();
            }
            return;
        }
        
        // (Legacy targeting logic removed...)

        // 2. Attack Selection Mode (Keep this)
        RuntimeUnit unit = UnitManager.GetUnitById(unitId);
        if (unit != null)
        {
            if (!CurrentTurnCanAttack) { if (UIManager != null) UIManager.Log("本回合无法进行攻击！"); return; }
            if (!unit.CanAttack) { if (UIManager != null) UIManager.Log($"{unit.Name} 无法攻击"); return; }
            if (unit.IsFatigued) { if (UIManager != null) UIManager.Log($"{unit.Name} 处于疲劳状态"); return; }

            _selectedAttacker = unit;
            if (UIManager != null) UIManager.Log($"已选中 {unit.Name}，请选择敌人");
        }
    }
    
    // Helper to start targeting
    public void InitiateEffectTargeting(RuntimeCard card, CardTargetType type)
    {
        // Called by Duel.SelectTarget
        IsTargetingMode = true;
        // _pendingCard should already be set in OnCardClicked
        if (UIManager != null)
        {
            UIManager.Log($"请为 {card.Data.cardName} 选择目标...");
            UnitManager.EnableTargetingSelection();
        }
    }

    // 处理点击 EnemyUnitUI 的点击
    // 处理点击 EnemyUnitUI 的点击
    public void OnEnemyClicked(EnemyUnitUI enemyUI)
    {
        if (enemyUI == null || enemyUI.MyUnit == null)
        {
            Debug.LogError("[BattleManager] EnemyUI 或 MyUnit 为空！");
            return;
        }

        // 1. === 如果正在【施法瞄准模式】 (Lua & Legacy) ===
        if (IsTargetingMode)
        {
            // Resolve Lua Effect
            if (_pendingEffect != null)
            {
                RuntimeUnit target = enemyUI.MyUnit;
                // Lua side filter checks are assumed done or we could check here if Filter is passed.
                
                // Set selection for Duel.GetTargets()
                Duel.SetSelection(target);
                
                _pendingEffect.ExecuteOperation(this, target);
                
                if (DeckManager != null && _pendingCard != null && _pendingCard.Data.kind != CardKind.Unit)
                {
                    DeckManager.DiscardCard(_pendingCard, _pendingCardUIObj);
                }

                ExitTargetingMode();
                return;
            }
            
            // Allow Legacy Fallback? 
            // If _pendingEffect is null but _pendingCard is set, it might be Legacy Equipment or simple card...
            // But we removed legacy logic in proper refactor. 
            // In case we are mixing, keep minimal fallback if needed, but for "Refactor to Lua" strict mode:
            // return;
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

    // === 新增：槽位点击处理 ===
    public void OnBattleSlotClicked(int index, bool isPlayerSide)
    {
        // 只有在选槽位模式且点击的是己方槽位才有效
        if (!IsSlotSelectionMode || !isPlayerSide) return;
        if (_pendingCard == null || UnitManager == null) return;

        // 尝试召唤
        if (UnitManager.TrySummonUnitAt(index, _pendingCard))
        {
            // === 修改：只有非 Deck Unit 才消耗召唤次数 ===
            if (!_pendingCard.Data.startsInDeck)
            {
                HasSummonedThisTurn = true;
            }
            
            // 消耗卡牌
            if (DeckManager != null)
            {
                // 单位召唤后，卡牌离开手牌但 *不* 进墓地（除非它死了）
                DeckManager.RemoveCardFromHand(_pendingCard, _pendingCardUIObj);
            }
            
            // 退出模式
            ExitTargetingMode(); // 复用退出逻辑
        }
    }

    private void EnterSlotSelectionMode(RuntimeCard card, GameObject uiObj)
    {
        IsSlotSelectionMode = true;
        _pendingCard = card;
        _pendingCardUIObj = uiObj;
        _selectedAttacker = null;

        if (UIManager != null)
        {
            UIManager.Log("请选择召唤位置...");
            UIManager.HighlightPlayerSlots(true);
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

        // === 新增：被装备时触发本家检索 ===
        if (target.SourceCard != null && target.SourceCard.Data != null)
        {
            if (target.SourceCard.Data.onReceiveEquipEffect != CardEffectType.None)
            {
                EffectBase effect = EffectFactory.GetEffect(target.SourceCard.Data.onReceiveEquipEffect);
                if (effect != null)
                {
                    // 参数：sourceCard 为【被装备的怪兽卡本身】
                    effect.Execute(this, target.SourceCard, target);
                }
            }
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
        IsSlotSelectionMode = false; // 同时重置这个
        
        _pendingEffect = null; // Clear pending effect
        _pendingCard = null;
        _pendingCardUIObj = null;
        if (UnitManager != null)
        {
            UnitManager.RestoreStateAfterTargeting();
        }
        // 取消高亮
        if (UIManager != null) UIManager.HighlightPlayerSlots(false);
    }

    private void CancelTargeting()
    {
        if (!IsTargetingMode && !IsSlotSelectionMode) return;
        if (UIManager != null) UIManager.Log("已取消操作！");
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

    // === Color Mechanism ===
    private bool CheckColorCondition(RuntimeCard card)
    {
        // 1. Colorless cards are always free
        if (card.Data.color == CardColor.Colorless) return true;

        // 2. Units are exempt (to allow bootstrapping)
        if (card.Data.kind == CardKind.Unit) return true;

        // 3. Check if we have a unit of the same color
        if (UnitManager != null)
        {
            foreach (var unit in UnitManager.PlayerUnits)
            {
                // === NEW: 疲劳状态的单位无法提供颜色响应 ===
                if (unit.IsFatigued) continue;

                // Assuming RuntimeUnit references original CardData or we store color on Unit
                // RuntimeUnit usually has 'Data' or 'Template' which is CardData
                if (unit.SourceCard != null && unit.SourceCard.Data != null && unit.SourceCard.Data.color == card.Data.color)
                {
                    return true;
                }
            }
        }

        // 4. Failed
        if (UIManager != null)
        {
            string colorName = card.Data.color.ToString();
            UIManager.Log($"<color=red>需场上有 {colorName} 单位才能使用此卡！</color>");
        }
        return false;
    }
    // Called by Duel.SelectTarget
    public void InitiateEffectTargeting(Effect e)
    {
        if (e == null) return;
        _pendingEffect = e;
        
        // Find UI Object if possible?
        // Maybe we don't need UI Obj immediately, only for Discard animation.
        // We set _pendingCard
        EnterTargetingMode(e.OwnerCard, null); // OwnerCard is known. UI Obj might be null initially.
    }
}
