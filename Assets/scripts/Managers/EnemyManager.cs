using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Config")]
    public EnemyDatabase EnemyDB;
    public GameObject EnemyPrefab;
    public Transform EnemyContainer; // 可能会保留作为父级或者废弃，看 UIManager 实现

    [Header("Runtime")]
    public List<RuntimeEnemy> ActiveEnemies = new List<RuntimeEnemy>();
    // === 新增：Fixed Slots ===
    public RuntimeEnemy[] EnemySlots = new RuntimeEnemy[5]; 

    // ★ 结算用：记录“最后被击杀的敌人”
    public CardData LastKilledUnitCard { get; private set; }
    public List<CardData> LastKilledDeckCards { get; private set; } = new List<CardData>();

    // === Boss Logic ===
    public RuntimeEnemy BossLeader { get; private set; }

    private BattleManager _bm;
    
    // 优先级：3 -> 2 -> 4 -> 1 -> 5 (对应索引 2 -> 1 -> 3 -> 0 -> 4)
    private readonly int[] _spawnPriority = new int[] { 2, 1, 3, 0, 4 };

    // === Pending Spells (Blue Unit Mechanic) ===
    [System.Serializable]
    public class PendingSpell
    {
        public RuntimeCard Card;
        public int Countdown;
        public RuntimeEnemy Owner; // Who cast it
    }
    public List<PendingSpell> PendingSpells = new List<PendingSpell>();

    public void AddPendingSpell(RuntimeCard card, int countdown, RuntimeEnemy owner)
    {
        PendingSpells.Add(new PendingSpell { Card = card, Countdown = countdown, Owner = owner });
        _bm.UIManager.Log($"【{owner.UnitData.Name}】吟唱「{card.Data.cardName}」... (剩余 {countdown} 回合)");
    }

    public void ProcessCountdowns()
    {
        if (PendingSpells.Count == 0) return;

        // Check for Blue Commander Buff (Accelerate Chanting)
        int reduction = 1;
        var colors = GetAliveCommanderColors();
        if (colors.Contains(CardColor.Blue))
        {
            reduction = colors.Count; // 1 to 3
            _bm.UIManager.Log($"蓝色指挥官加速吟唱！减少 {reduction} 回合。");
        }

        for (int i = PendingSpells.Count - 1; i >= 0; i--)
        {
            var spell = PendingSpells[i];
            spell.Countdown -= reduction;
            if (spell.Countdown <= 0)
            {
                // Cast!
                spell.Card.IsPendingResolved = true; // Flag for Lua to Execute
                ResolveEnemyCard(spell.Owner, spell.Card);
                PendingSpells.RemoveAt(i);
            }
            else
            {
                _bm.UIManager.Log($"「{spell.Card.Data.cardName}」还需 {spell.Countdown} 回合...");
            }
        }
    }

    public RuntimeEnemy FindOwner(RuntimeCard card)
    {
        // Simple search: check decks
        foreach(var enemy in ActiveEnemies)
        {
            if (enemy.Deck.Contains(card)) return enemy;
            // Also check pending?
        }
        // Fallback: If called from Lua during "Cast", the owner is the current turn actor usually?
        // But Lua passes nothing.
        // Let's assume the enemy who has the card "in hand" or "deck".
        return ActiveEnemies.Count > 0 ? ActiveEnemies[0] : null; 
    }

    public HashSet<CardColor> GetAliveCommanderColors()
    {
        HashSet<CardColor> colors = new HashSet<CardColor>();
        foreach (var enemy in ActiveEnemies)
        {
            if (enemy != null && !enemy.UnitData.IsDead && enemy.UnitData.SourceCard != null && enemy.UnitData.SourceCard.Data.isCommander)
            {
                colors.Add(enemy.UnitData.SourceCard.Data.color);
            }
        }
        return colors;
    }

    [System.Serializable]
    public class RuntimeEnemy
    {
        public RuntimeUnit UnitData;
        public EnemyUnitUI UI;
        public int SlotIndex; // 记录自己在哪个槽

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
        // 清理槽位
        for (int i = 0; i < 5; i++) EnemySlots[i] = null;

        // ★ 清空“最后击杀”记录
        LastKilledUnitCard = null;
        LastKilledDeckCards.Clear();
        BossLeader = null; // Reset Boss Leader
        if (_bm.UIManager.BossHPPanel != null) _bm.UIManager.BossHPPanel.gameObject.SetActive(false);

        // 1. 容器清理 (UIManager接管后，这一步主要是清理残余)
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

        // 3. 检查 GameManager
        if (GameManager.Instance == null || GameManager.Instance.CurrentNode == null)
        {
            SpawnTestEnemy();
            return;
        }

        // 4. 一切正常，尝试获取战斗配置
        Debug.Log($"正在获取遭遇战配置，节点类型: {GameManager.Instance.CurrentNode.Type}");

        // === BOSS LOGIC Override ===
        if (GameManager.Instance.CurrentNode.Type == NodeType.Boss)
        {
            Debug.Log("检测到 Boss 节点，生成彩虹首领！");
            DevCardLoader.InjectBossData(); // Ensure data exists
            CardData bossData = DevCardLoader.GetCardDataShim(6000);
            if (bossData != null)
            {
                CreateEnemyAt(bossData);
                return;
            }
            else
            {
                Debug.LogError("Boss Data (6000) fail to load!");
            }
        }

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
        // 寻找空位
        int targetSlot = -1;
        foreach (int idx in _spawnPriority)
        {
            if (EnemySlots[idx] == null)
            {
                targetSlot = idx;
                break;
            }
        }

        if (targetSlot == -1)
        {
            Debug.LogWarning("[EnemyManager] 敌方槽位已满，无法生成更多敌人！");
            return;
        }

        // === Runtime Color Patch for Goblin/Slime (User Feedback) ===
        CardData instanceData = Instantiate(enemyData); // Clone to avoid asset modification
        instanceData.name = enemyData.name; // Keep name
        
        // Detect by Name (Asset Name or Display Name)
        bool isGoblin = instanceData.name.IndexOf("Goblin", System.StringComparison.OrdinalIgnoreCase) >= 0 || instanceData.cardName.IndexOf("Goblin", System.StringComparison.OrdinalIgnoreCase) >= 0;
        bool isSlime = instanceData.name.IndexOf("Slime", System.StringComparison.OrdinalIgnoreCase) >= 0 || instanceData.cardName.IndexOf("Slime", System.StringComparison.OrdinalIgnoreCase) >= 0;

        if (isGoblin)
        {
            instanceData.color = CardColor.Green;
            // Patch Deck
            if (instanceData.EnemyMoves != null)
            {
                List<CardData> newMoves = new List<CardData>();
                foreach(var move in instanceData.EnemyMoves)
                {
                    if (move == null) continue;
                    CardData moveClone = Instantiate(move);
                    moveClone.color = CardColor.Green; 
                    newMoves.Add(moveClone);
                }
                instanceData.EnemyMoves = newMoves;
            }
        }
        else if (isSlime)
        {
            instanceData.color = CardColor.Blue;
             // Patch Deck
            if (instanceData.EnemyMoves != null)
            {
                List<CardData> newMoves = new List<CardData>();
                foreach(var move in instanceData.EnemyMoves)
                {
                    if (move == null) continue;
                    CardData moveClone = Instantiate(move);
                    moveClone.color = CardColor.Blue; 
                    newMoves.Add(moveClone);
                }
                instanceData.EnemyMoves = newMoves;
            }
        }

        RuntimeUnit unit = new RuntimeUnit(instanceData);
        
        // 获取 Slot Transform
        Transform slotTr = _bm.UIManager.GetEnemySlotTransform(targetSlot);
        if (slotTr == null)
        {
            Debug.LogError($"无法获取敌人槽位 {targetSlot} 的Transform");
            return;
        }

        GameObject obj = Instantiate(EnemyPrefab, slotTr);
        // ★ 核心修复：强制 UI 填满 Slot
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            // Fallback if no RectTransform (unlikely for UI)
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }

        EnemyUnitUI ui = obj.GetComponent<EnemyUnitUI>();
        ui.Init(unit, _bm);
        unit.EnemyUI = ui;

        // ★ 把 enemyData（本体卡）和 enemyData.EnemyMoves（卡组）都存进去
        RuntimeEnemy enemy = new RuntimeEnemy(unit, ui, instanceData, instanceData.EnemyMoves);
        enemy.SlotIndex = targetSlot;
        
        ActiveEnemies.Add(enemy);
        EnemySlots[targetSlot] = enemy;

        // === Check if this is a Boss Leader ===
        // ID 6000 = Rainbow Boss, 4001 = Elite Steam Commander (Optional treating as boss)
        if (instanceData.id == 6000) 
        {
            BossLeader = enemy;
            Debug.Log("Boss Leader Spawned: " + enemy.UnitData.Name);
            if (_bm.UIManager != null) _bm.UIManager.ShowBossHP(BossLeader);
        }
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
        // 0. Process Pending Spells (Blue Mechanic)
        ProcessCountdowns();
        yield return new WaitForSeconds(0.5f);

        // 1. Check for Rainbow Boss Logic
        var boss = ActiveEnemies.Find(e => e.UnitData?.SourceCard?.Data?.id == 6000); // 6000 = Rainbow Boss
        if (boss != null && !boss.UnitData.IsDead)
        {
            yield return StartCoroutine(RunRainbowBossAI(boss));
        }
        // 1.5 [NEW] Check for Elite Steam Commander Logic
        else
        {
            var eliteRobot = ActiveEnemies.Find(e => e.UnitData?.SourceCard?.Data?.id == 4001); // 4001 = Steam Commander
            if (eliteRobot != null && !eliteRobot.UnitData.IsDead)
            {
                yield return StartCoroutine(RunEliteRobotAI(eliteRobot));
            }
        }
        
        // Continue with standard logic
        if (boss == null || boss.UnitData.IsDead)
        {
            // Standard Logic for non-boss or other enemies
            // 0. Green Unit Passive: Heal All Enemies based on Player Green Units
            var greenCommander = ActiveEnemies.Find(e => e.UnitData?.SourceCard?.Data?.id == 6002);
            if (greenCommander != null && !greenCommander.UnitData.IsDead)
            {
                int playerGreenCount = _bm.UnitManager.PlayerUnits.FindAll(u => u.SourceCard.Data.color == CardColor.Green).Count;
                if (playerGreenCount > 0)
                {
                    foreach (var e in ActiveEnemies)
                    {
                        if (!e.UnitData.IsDead)
                        {
                            e.UnitData.CurrentHp = Mathf.Min(e.UnitData.CurrentHp + playerGreenCount, e.UnitData.MaxHp);
                            e.UI.UpdateHP();
                            // e.UI.ShowBuffAnim(); 
                        }
                    }
                    _bm.UIManager.Log($"绿色指挥官利用自然之力，全员回复 {playerGreenCount} 点生命！");
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // 0.5 Blue Unit Passive: Deal Damage based on Player Blue Units to Random Targets (BossColor Count)
            var blueCommander = ActiveEnemies.Find(e => e.UnitData?.SourceCard?.Data?.id == 6003);
            if (blueCommander != null && !blueCommander.UnitData.IsDead)
            {
                int playerBlueCount = _bm.UnitManager.PlayerUnits.FindAll(u => u.SourceCard.Data.color == CardColor.Blue).Count;
                int bossColorCount = GetAliveCommanderColors().Count; // X targets
                
                if (playerBlueCount > 0 && bossColorCount > 0)
                {
                    _bm.UIManager.Log($"蓝色指挥官释放奥术波动！对 {bossColorCount} 个目标造成 {playerBlueCount} 点伤害！");
                    for (int i = 0; i < bossColorCount; i++)
                    {
                        // Random Target (Unit or Player)
                        var potentialTargets = new List<RuntimeUnit>(_bm.UnitManager.PlayerUnits); // Clone list
                        // Add Player as valid target? Usually "random grid" implies units, but let's stick to valid units on board.
                        // If no units, maybe hit player? User said "random grid".
                        if (potentialTargets.Count > 0)
                        {
                            var target = potentialTargets[UnityEngine.Random.Range(0, potentialTargets.Count)];
                            _bm.CombatManager.ApplyDamage(target, playerBlueCount);
                        }
                        else
                        {
                            // No units, hit player directly?
                            _bm.PlayerUnit.TakeDamage(playerBlueCount);
                        }
                        yield return new WaitForSeconds(0.2f);
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }

            foreach (var enemy in ActiveEnemies)
            {
                // If it's the boss (and we just ran it), skip
                if (enemy == boss) continue; 

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
                    // Red Unit Passive: ATK Bonus per Player Red Unit
                    if (enemy.UnitData?.SourceCard?.Data?.id == 6001)
                    {
                        int playerRedCount = _bm.UnitManager.PlayerUnits.FindAll(u => u.SourceCard.Data.color == CardColor.Red).Count;
                        if (playerRedCount > 0)
                        {
                            int bonus = playerRedCount * 2;
                            enemy.TempAttackBonus += bonus; // Ensure this is added to damage calculation
                            // Note: PerformAttack uses CurrentAtk. We need to modify CurrentAtk temporarily?
                            // Or does PerformAttack check TempAttackBonus?
                            // Looking at PerformAttack implementation (not visible here but usually uses CurrentAtk).
                            // Let's modify CurrentAtk and reset it later.
                            enemy.UnitData.CurrentAtk += bonus;
                            enemy.ResetAttack = true; // Flag to reset
                             _bm.UIManager.Log($"红色指挥官因玩家的红卡获得 +{bonus} 攻击力！");
                        }
                        PerformAttack(enemy);
                    }
                    else
                    {
                        PerformAttack(enemy);
                    }
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                   // _bm.UIManager.Log($"【{enemy.UnitData.Name}】本回合无法普攻。");
                }
                if (enemy.ResetAttack)
                {
                    enemy.UnitData.CurrentAtk = enemy.UnitData.BaseAtk;
                    enemy.UI.UpdateAttack();
                    enemy.ResetAttack = false;
                }
            }
        }

        _bm.StartPlayerTurn();
    }

    // === Rainbow Boss AI ===
    IEnumerator RunRainbowBossAI(RuntimeEnemy boss)
    {
        _bm.UIManager.Log(">>> <b>彩虹首领</b> 正在思考... <<<");
        yield return new WaitForSeconds(1.0f);

        // Turn 1: Play Field Magic (if not already active)
        if (_bm.FieldCard == null || _bm.FieldCard.Data.id != 5001)
        {
            _bm.UIManager.Log("彩虹首领发动场地魔法：【棱镜战场】！");
            // Create a temporary Field Card 5001
            // Use DevCardLoader to create or just search if we had it in deck.
            // For Boss, we cheat/force it.
            // We need a RuntimeCard.
            // Let's assume we can fetch data for 5001.
            // Note: Boss doesn't "hold" the field card in hand typically, just plays it.
            // We need to access DevCardLoader logic or Resources.
            // Simplified: We assume DevCardLoader can give us raw data, or we just Clone one.
            // Actually, we can use the Deck logic if we seeded it.
            // Check Boss Deck first.
            var fieldCard = boss.Deck.Find(c => c.Data.kind == CardKind.Field && c.Data.id == 5001);
            if (fieldCard != null)
            {
                // Play it
                boss.Deck.Remove(fieldCard);
                // Call BattleManager PlayFieldCard - wait, that takes CardUI (Player UI).
                // Enemy playing Field Card might need backend bypass.
                // We'll manually set it for now or make a helper.
                // BattleManager.PlayFieldCard expects CardUI to destroy.
                // Let's manually set _bm.FieldCard.
                // And trigger OnFieldEnter effects if any.
                // *Hack*: We'll just set it.
                // _bm.PlayFieldCard(fieldCard, null); // We modified PlayFieldCard to possibly accept null UI? 
                // Let's check: "DeckManager.RemoveCardFromHand(card, cardUI.gameObject);" -> Will crash if ui is null.
                // So we do it manually here.
                _bm.SetFieldCardForEnemy(fieldCard); 
            }
            else
            {
                 // Create one if missing (fallback)
                 // _bm.UIManager.Log("(Boss creates Field Magic from void)");
                 // Hack: Create ephemeral
                 // CardData fieldData = DevCardLoader.GetCardDataShim(5001);
                 // if(fieldData != null) _bm.SetFieldCardForEnemy(new RuntimeCard(fieldData));
            }
            yield return new WaitForSeconds(0.5f);
        }

        // Phase 1: Summon One Random Commander (Every Turn)
        // IDs: 6001 (Red), 6002 (Green), 6003 (Blue)
        int[] minionIds = new int[] { 6001, 6002, 6003 };
        int randomId = minionIds[Random.Range(0, minionIds.Length)];
        
        _bm.UIManager.Log($"Debug: Boss attempting to summon ID {randomId}");
        SpawnBossMinion(randomId);
        yield return new WaitForSeconds(1.0f);

        // Phase 2: Play 2 Cards Matching Colors
        var currentColors = GetAliveCommanderColors(); // Update after summon
        int cardsPlayed = 0;
        int maxPlays = 2;

        // Shuffle Deck to randomize
        // boss.Deck.Shuffle(); // List doesn't have Shuffle, use helper or iterate random.
        
        List<RuntimeCard> playable = new List<RuntimeCard>();
        foreach(var c in boss.Deck)
        {
            if (currentColors.Contains(c.Data.color)) playable.Add(c);
        }

        for (int i=0; i<maxPlays; i++)
        {
            if (playable.Count == 0) break;
            int idx = Random.Range(0, playable.Count);
            RuntimeCard cardToPlay = playable[idx];
            playable.RemoveAt(idx);
            boss.Deck.Remove(cardToPlay); // Consume from actual deck

            ResolveEnemyCard(boss, cardToPlay);
            yield return new WaitForSeconds(0.8f);
        }
        
        // Attack Phase
        PerformAttack(boss);
        // Also let minions attack?
        foreach(var minion in ActiveEnemies)
        {
            if (minion == boss) continue;
            PerformAttack(minion);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnBossMinion(int id)
    {
        // 1. Create Unit Data (Need to get from DevCardLoader or Resources)
        // Since we don't have easy lookup, we might rely on the Boss Deck having "Minion Summon" cards, 
        // OR we inject them into EnemyManager knowing checks.
        // For prototype, let's use DevCardLoader to get a fresh copy if possible, 
        // OR construct one on the fly.
        // Let's add a helper in DevCardLoader: GetCardData(int id)
        
        // For now, assume we can load it.
        // If DevCardLoader.GetCardData(id) exists... if not, we must create it.
        // We will assume DevCardLoader.GetCardData(id) will be available.
        CardData data = DevCardLoader.GetCardDataShim(id); 
        if (data != null)
        {
             CreateEnemyAt(data);
             _bm.UIManager.Log($"彩虹首领召唤了：{data.cardName}！");
        }
    }

    private RuntimeCard PickCardForEnemy(RuntimeEnemy enemy)
    {
        if (enemy.Deck == null || enemy.Deck.Count == 0) return null;
        int idx = Random.Range(0, enemy.Deck.Count);
        return enemy.Deck[idx];
    }

    private void ResolveEnemyCard(RuntimeEnemy attacker, RuntimeCard card)
    {
        // === New Lua Integration ===
        // If the card has a script and valid effects, prefer that over hardcoded types.
        if (card.Effects != null && card.Effects.Count > 0)
        {
             // Priority: Ignitition (Active) or Trigger
             // Note: EffectType values are mapped in LuaManager. 4=IGNITION, 8=TRIGGER.
             var luaEffect = card.Effects.Find(x => x.IsHasType(4) || x.IsHasType(8)); // 4=IGNITION, 8=TRIGGER
             if (luaEffect != null)
             {
                 _bm.UIManager.Log($"【{attacker.UnitData.Name}】发动「{card.Data.cardName}」！");
                 // Execute Lua Logic
                 luaEffect.ExecuteOperation(0, null, 0, 0, null, 0, 0);
                 return; // Skip hardcoded switch logic
             }
        }

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

            case CardEffectType.GrantOverload:
                attacker.UnitData.Overload += data.value;
                _bm.UIManager.Log($"【{enemyName}】执行「{data.cardName}」，过载 +{data.value} (当前: {attacker.UnitData.Overload})。");
                break;

            case CardEffectType.DoubleOverload:
                attacker.UnitData.Overload *= 2;
                attacker.UnitData.PendingOverloadSelfDamage += 5; 
                _bm.UIManager.Log($"【{enemyName}】激发「{data.cardName}」，过载翻倍 (当前: {attacker.UnitData.Overload})！");
                break;

            case CardEffectType.ReduceOverloadAndAOE:
                attacker.UnitData.Overload = Mathf.Max(0, attacker.UnitData.Overload - 1);
                _bm.UIManager.Log($"【{enemyName}】释放「{data.cardName}」，过载释放，造成 {data.value} 点全场伤害！");
                _bm.PlayerUnit.TakeDamage(data.value);
                foreach(var u in _bm.UnitManager.PlayerUnits) _bm.CombatManager.ApplyDamage(u, data.value);
                break;

            case CardEffectType.LimitOperationEvolve:
                attacker.UnitData.RobotEvolved = true;
                attacker.UnitData.OverrideName = "极限蒸汽指挥官";
                attacker.UnitData.CurrentAtk = attacker.UnitData.Overload * 2;
                attacker.UI.UpdateAttack();
                if (attacker.UI != null) attacker.UI.RefreshName();
                 _bm.UIManager.Log($"【{enemyName}】进入极限运转状态！攻击力变为过载的2倍 ({attacker.UnitData.CurrentAtk})！");
                break;

            default:
                if (data.kind == CardKind.Unit)
                {
                    _bm.UIManager.Log($"【{enemyName}】正在召唤「{data.cardName}」...");
                    CreateEnemyAt(data);
                }
                else
                {
                    _bm.UIManager.Log($"【{enemyName}】使用了未实现效果的牌: {data.cardName}");
                }
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
            if (target.UI != null) Destroy(target.UI.gameObject);
            
            // 清理槽位
            if (target.SlotIndex >= 0 && target.SlotIndex < 5)
            {
                EnemySlots[target.SlotIndex] = null;
            }
        }

        // === Win Condition Check ===
        if (BossLeader != null)
        {
            // If Boss Leader exists, only win if Boss Leader is dead
            if (BossLeader.UnitData.IsDead || !ActiveEnemies.Contains(BossLeader))
            {
                 _bm.OnGameWin();
            }
        }
        else
        {
            // Normal battle: Win if all enemies are dead
            if (ActiveEnemies.Count == 0)
            {
                _bm.OnGameWin();
            }
        }
    }

    /// <summary>
    /// [NEW] Elite Steam Commander AI Logic
    /// </summary>
    IEnumerator RunEliteRobotAI(RuntimeEnemy eliteRobot)
    {
        Debug.Log("[EnemyManager] Elite Steam Commander AI activated!");
        
        // Get current overload value from the commander itself
        int currentOverload = eliteRobot.UnitData.Overload;
        
        // Count units on enemy field
        int enemyUnitCount = ActiveEnemies.Count;
        
        // Decision Tree
        RuntimeCard chosenCard = null;
        
        if (enemyUnitCount <= 1)
        {
            Debug.Log("[Elite Robot AI] Summoning unit...");
            chosenCard = PickUnitCard(eliteRobot);
        }
        else if (currentOverload == 0)
        {
            Debug.Log("[Elite Robot AI] Using overload card...");
            chosenCard = PickOverloadCard(eliteRobot);
        }
        else if (currentOverload > 0)
        {
            Debug.Log($"[Elite Robot AI] Using synergy card (Overload: {currentOverload})...");
            chosenCard = PickSynergyCard(eliteRobot);
        }
        
        if (chosenCard == null)
        {
            chosenCard = PickUnitCard(eliteRobot);
        }
        
        if (chosenCard != null)
        {
            _bm.UIManager.Log($"精英蒸汽指挥官使用了 {chosenCard.Data.cardName}!");
            yield return new WaitForSeconds(0.5f);
            ResolveEnemyCard(eliteRobot, chosenCard);
            yield return new WaitForSeconds(0.5f);
        }
    }

    RuntimeCard PickUnitCard(RuntimeEnemy enemy)
    {
        var unitCards = enemy.Deck.FindAll(c => 
            c.Data.kind == CardKind.Unit && c.Data.cardTag == CardTag.Robot);
        return unitCards.Count > 0 ? unitCards[UnityEngine.Random.Range(0, unitCards.Count)] : null;
    }

    RuntimeCard PickOverloadCard(RuntimeEnemy enemy)
    {
        // Pick cards that grant or double overload
        var overloadCards = enemy.Deck.FindAll(c => 
            c.Data.effectType == CardEffectType.GrantOverload || 
            c.Data.effectType == CardEffectType.DoubleOverload);
        return overloadCards.Count > 0 ? overloadCards[UnityEngine.Random.Range(0, overloadCards.Count)] : null;
    }

    RuntimeCard PickSynergyCard(RuntimeEnemy enemy)
    {
        // Pick cards that require overload or evolve
        var synergyCards = enemy.Deck.FindAll(c => 
            c.Data.effectType == CardEffectType.ReduceOverloadAndAOE || 
            c.Data.effectType == CardEffectType.LimitOperationEvolve);
        return synergyCards.Count > 0 ? synergyCards[UnityEngine.Random.Range(0, synergyCards.Count)] : null;
    }
}
 
