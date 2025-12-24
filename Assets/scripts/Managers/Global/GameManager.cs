using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData
{
    public int Gold;
    public int PlayerCurrentHP;
    public int PlayerMaxHP;
    public List<string> MasterDeckPaths = new List<string>();    // 玩家拥有的所有卡牌 (仓库)
    public List<string> SelectedDeckPaths = new List<string>();  // 玩家当前选中的卡组 (出战)
    
    // 地图序列化
    public string MapJson; 
    public Vector2Int CurrentNodeCoord;
    public bool HasActiveRun;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Config")]
    public MapConfig MapConfig;

    [Header("Global State")]
    public MapData CurrentMap;
    public MapNode CurrentNode; // 玩家正处于的节点

    [Header("Run State")]
    // 核心修改：这就是你的“总卡组”，包含法术和随从
    public List<CardData> MasterDeck = new List<CardData>();
    
    // [NEW] Inspector 配置的默认卡组
    public List<CardData> defaultStarterDeck = new List<CardData>();

    // 用于传递数据给 EventScene
    

    // 当前战斗的敌人数据
    public List<CardData> CurrentEnemyUnits = new List<CardData>();
    public List<CardData> CurrentEnemyDeck = new List<CardData>();
    
    [Header("Event System")]
    public List<EventProfile> AllEvents = new List<EventProfile>();
    public EventProfile TreasureProfile;
    public EventProfile RestProfile;
    
    public EventProfile CurrentEventProfile;

    // 玩家状态
    public int PlayerCurrentHP = 80;
    public int PlayerMaxHP = 80;

    public List<CardData> CurrentDeck = new List<CardData>();

    [Header("Economy")]
    [SerializeField] private int _gold = 0;
    public int Gold => _gold;

    /// <summary>
    /// 玩家全局状态变化事件：HP / Gold
    /// </summary>
    public event Action OnPlayerStateChanged;


    // --- 游戏流程控制 ---

    public void StartNewGame(List<CardData> starterDeck)
    {
        // 1. 初始化数据
        PlayerCurrentHP = PlayerMaxHP;
        _gold = 0;

        // 初始化 MasterDeck
        MasterDeck = new List<CardData>(starterDeck);

        // 2. === 修复：初始化 PlayerCollection ===
        if (PlayerCollection.Instance != null)
        {
            // 清空旧数据 (防止上一局游戏的残留)
            PlayerCollection.Instance.OwnedUnits.Clear();
            PlayerCollection.Instance.OwnedCards.Clear();
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();

            // 把初始卡组全部录入“仓库”并设置为“当前出战”
            foreach (var card in starterDeck)
            {
                if (card == null) continue;
                PlayerCollection.Instance.AddCardToCollection(card, true);
                
                if (card.kind == CardKind.Unit)
                    PlayerCollection.Instance.CurrentUnits.Add(card);
                else
                    PlayerCollection.Instance.CurrentDeck.Add(card);
            }
        }

        // 2. 生成地图
        CurrentMap = MapGenerator.GenerateMap(MapConfig);

        // 3. 解锁第一层
        foreach (var node in CurrentMap.Layers[0])
            node.Status = NodeStatus.Attainable;

        // === DEV INJECTION REMOVED (Handled via overload) ===
        // DevCardLoader.InjectDevCards();

        NotifyPlayerStateChanged();

        // 4. 进入地图场景
        SceneManager.LoadScene("MapScene");
    }

    public void StartNewGame(DevCardLoader.DevDeckType deckType)
    {
        // 1. Clear MasterDeck explicitly
        MasterDeck.Clear();
        
        // 2. Inject specific deck
        DevCardLoader.InjectDeck(deckType);
        
        // 3. Start Game with this deck
        // Note: StartNewGame(List) creates a copy. 
        StartNewGame(MasterDeck);
    }

    public void SelectNode(MapNode node)
    {
        CurrentNode = node;
        node.Status = NodeStatus.Current;

        // 根据类型跳转场景
        if (node.Type == NodeType.MinorEnemy || node.Type == NodeType.EliteEnemy || node.Type == NodeType.Boss)
        {
            SceneManager.LoadScene("BattleScene");
        }
        else if (node.Type == NodeType.Event)
        {
            // 随机选择一个事件
            if (AllEvents != null && AllEvents.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, AllEvents.Count);
                CurrentEventProfile = AllEvents[idx];
            }
            else
            {
                // Fallback: Use a default one loaded from resources or keep existing logic if assigned elsewhere
                CurrentEventProfile = Resources.Load<EventProfile>("Events/Event_Spring"); 
            }
            SceneManager.LoadScene("EventScene");
        }
        else if (node.Type == NodeType.Treasure)
        {
            // 使用特定的 Treasure 事件
            if (TreasureProfile != null) CurrentEventProfile = TreasureProfile;
            else Debug.LogError("TreasureProfile not assigned in GameManager!");
            
            SceneManager.LoadScene("EventScene"); 
        }
        else if (node.Type == NodeType.Rest)
        {
            // 使用特定的 Rest 事件
            if (RestProfile != null) CurrentEventProfile = RestProfile;
            else Debug.LogError("RestProfile not assigned in GameManager!");
            
            SceneManager.LoadScene("EventScene");
        }
        else if (node.Type == NodeType.Store)
        {
            SceneManager.LoadScene("ShopScene");
        }
        else
        {
            OnNodeCompleted();
        }
    }

    // 战斗胜利或事件完成后调用
    public void OnNodeCompleted()
    {
        if (CurrentNode == null) return;

        // 1. 标记当前为已访问
        CurrentNode.Status = NodeStatus.Visited;

        // 2. 解锁下一层连接的节点
        foreach (var nextCoord in CurrentNode.Outgoing)
        {
            var nextNode = GetNode(nextCoord);
            if (nextNode != null) nextNode.Status = NodeStatus.Attainable;
        }

        // 3. 【新增修复】 锁定同层的所有其他节点
        if (CurrentNode.Coordinate.y < CurrentMap.Layers.Count)
        {
            var currentLayer = CurrentMap.Layers[CurrentNode.Coordinate.y];

            foreach (var node in currentLayer)
            {
                // 如果这个节点不是我刚才打过的那个，并且它是可达的，就把它锁住
                if (node != CurrentNode && node.Status == NodeStatus.Attainable)
                {
                    node.Status = NodeStatus.Locked;
                }
            }
        }

        // 4. 回到地图
        SceneManager.LoadScene("MapScene");
    }

    // 战斗失败
    public void OnRunFailed()
    {
        Debug.Log("游戏结束");
        // SceneManager.LoadScene("MainMenu"); 
    }

    public MapNode GetNode(Vector2Int coord)
    {
        if (coord.y < CurrentMap.Layers.Count && coord.x < CurrentMap.Layers[coord.y].Count)
            return CurrentMap.Layers[coord.y][coord.x];
        return null;
    }

    // --- 接口：获得卡牌 ---
    public void AddCardToDeck(CardData newCard)
    {
        if (newCard == null) return;

        MasterDeck.Add(newCard);

        // 同时加入玩家的收藏库(Owned)
        if (PlayerCollection.Instance != null)
        {
            // true 表示允许重复 (Roguelike通常允许)
            PlayerCollection.Instance.AddCardToCollection(newCard, true);
        }

        Debug.Log($"获得了卡牌: {newCard.cardName}");
    }

    // --- 经济系统：金币 ---
    public void AddGold(int amount)
    {
        if (amount == 0) return;
        _gold = Mathf.Max(0, _gold + amount);
        Debug.Log($"[Global] 获得金币 {amount}, 当前金币: {_gold}");
        NotifyPlayerStateChanged();
    }

    // 获得卡片和unit    
    // GameManager.cs
    public void AcquireEnemyUnitAndDeck(CardData unitCard, List<CardData> deckCards)
    {
        if (unitCard != null)
            AddCardToDeck(unitCard);

        if (deckCards != null)
        {
            foreach (var c in deckCards)
            {
                if (c == null) continue;
                AddCardToDeck(c);
            }
        }
    }
    


    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (_gold < amount) return false;

        _gold -= amount;
        Debug.Log($"[Global] 花费金币 {amount}, 当前金币: {_gold}");
        NotifyPlayerStateChanged();
        return true;
    }

    // --- 事件奖励辅助接口 ---

    public void GainRandomRelic()
    {
        // 1. 获取所有 Relic
        var allRelics = GetAllRelics();
        if (allRelics == null || allRelics.Count == 0)
        {
             Debug.LogWarning("No relics defined!");
             return;
        }

        // 2. 过滤已有的 (Unique)
        // 假设 RelicManager 已经有 CheckRelic
        // 简单处理：随机拿一个，不管有没有重复，或者先不处理重复
        int idx = UnityEngine.Random.Range(0, allRelics.Count);
        RelicData relic = allRelics[idx];

        // 3. 添加到 RelicManager
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.AddRelic(relic);
            Debug.Log($"[Event] Obtained Relic: {relic.relicName}");
        }
    }

    public void OpenCardReward()
    {
        // 触发卡牌奖励 UI
        // 目前我们可能还没有专门的三选UB，可以使用 DeckSelectionUI 的变体，或者直接 Random 一张进卡组
        // 为了目前的需求 (Rest节点获得一张卡)，我们暂时简单实现：随机获得一张卡
        // TODO: 完善为 "RewardUI" 场景或弹窗

        if (_cardRegistry.Count == 0) return;
        
        List<CardData> allCards = new List<CardData>(_cardRegistry.Values);
        CardData randomCard = allCards[UnityEngine.Random.Range(0, allCards.Count)];
        
        AddCardToDeck(randomCard);
        Debug.Log($"[Event] Random Card Reward: {randomCard.cardName}");
    }

    // --- 玩家血量 ---
    public void HealPlayer(int amount)
    {
        if (amount <= 0) return;

        PlayerCurrentHP += amount;
        if (PlayerCurrentHP > PlayerMaxHP) PlayerCurrentHP = PlayerMaxHP;

        Debug.Log($"[Global] 玩家回血 {amount}, 当前: {PlayerCurrentHP}");
        NotifyPlayerStateChanged();
    }

    public void DamagePlayer(int amount)
    {
        if (amount <= 0) return;

        PlayerCurrentHP -= amount;
        if (PlayerCurrentHP < 0) PlayerCurrentHP = 0;

        Debug.Log($"[Global] 玩家扣血 {amount}, 当前: {PlayerCurrentHP}");
        NotifyPlayerStateChanged();
    }

    private void NotifyPlayerStateChanged()
    {
        OnPlayerStateChanged?.Invoke();
    }

    

    /// <summary>
    /// 打开藏品收集场景
    /// </summary>
    public void OpenCollectionScene()
    {
        SceneManager.LoadScene("CollectionScene");
    }
public void ReturnToTitle()
    {
        Debug.Log("返回标题画面，重置数据...");

        // 1. 清理数据
        CurrentMap = null;
        CurrentNode = null;
        MasterDeck.Clear();
        PlayerCurrentHP = PlayerMaxHP;
        _gold = 0;
        NotifyPlayerStateChanged();

        // 2. 加载主菜单场景 (假设你的入口场景叫 MainEntry)
        SceneManager.LoadScene("MainEntry");
    }

    // === 存档系统实现 ===

    private Dictionary<string, CardData> _cardRegistry = new Dictionary<string, CardData>();
    private Dictionary<string, RelicData> _relicRegistry = new Dictionary<string, RelicData>(); // [New] Relic索引

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildRegistry();
            
            // === 核心修复：自动创建 RelicManager ===
            if (RelicManager.Instance == null)
            {
                var obj = new GameObject("RelicManager");
                obj.AddComponent<RelicManager>();
                Debug.Log("[GameManager] Auto-created RelicManager");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildRegistry()
    {
        BuildCardRegistry();
        BuildRelicRegistry();
    }

    private void BuildCardRegistry()
    {
        // 自动索引所有 Resources 下的 CardData
        CardData[] cards = Resources.LoadAll<CardData>("");
        _cardRegistry.Clear();
        foreach (var c in cards)
        {
            if (c != null && !_cardRegistry.ContainsKey(c.name))
                _cardRegistry.Add(c.name, c);
        }
        Debug.Log($"[GameManager] 已索引 {_cardRegistry.Count} 张卡片");
    }

    public void BuildRelicRegistry()
    {
        RelicData[] relics = Resources.LoadAll<RelicData>("");
        _relicRegistry.Clear();
        foreach (var r in relics)
        {
            if (r != null && !_relicRegistry.ContainsKey(r.relicId))
                _relicRegistry.Add(r.relicId, r);
        }
        Debug.Log($"[GameManager] 已索引 {_relicRegistry.Count} 个遗物");
    }

    // 提供给外部获取所有Relic的列表（例如商店如果没配可以兜底全随机）
    public List<RelicData> GetAllRelics()
    {
        return new List<RelicData>(_relicRegistry.Values);
    }

    [ContextMenu("Refresh Card Registry")]
    public void RefreshRegistry() => BuildCardRegistry();

    public CardData GetCardFromRegistry(string cardName)
    {
        if (_cardRegistry.TryGetValue(cardName, out var card)) return card;
        // 备选方案：尝试直接加载
        return Resources.Load<CardData>($"Cards/{cardName}");
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "cardsave.json");

    public bool HasSaveGame()
    {
        return File.Exists(SavePath);
    }

    public void SaveGame()
    {
        string path = Application.persistentDataPath + "/cardsave.json";
        SaveData data = new SaveData();

        data.Gold = _gold; // Assuming _gold is the backing field for Gold property
        data.PlayerCurrentHP = PlayerCurrentHP;
        data.PlayerMaxHP = PlayerMaxHP;
        data.HasActiveRun = true;

        // 保存所有拥有的卡片 (仓库)
        if (PlayerCollection.Instance != null)
        {
            foreach (var card in PlayerCollection.Instance.OwnedUnits) 
                if (card != null) data.MasterDeckPaths.Add(card.name);
            foreach (var card in PlayerCollection.Instance.OwnedCards) 
                if (card != null) data.MasterDeckPaths.Add(card.name);

            // 保存当前选中的卡组 (出战)
            foreach (var card in PlayerCollection.Instance.CurrentUnits) 
                if (card != null) data.SelectedDeckPaths.Add(card.name);
            foreach (var card in PlayerCollection.Instance.CurrentDeck) 
                if (card != null) data.SelectedDeckPaths.Add(card.name);
        }
        else
        {
            // fallback: 如果 PC 丢失，至少保存 MasterDeck
            foreach (var card in MasterDeck)
            {
                if (card != null) data.MasterDeckPaths.Add(card.name);
                if (card != null) data.SelectedDeckPaths.Add(card.name);
            }
        }

        // 序列化地图
        if (CurrentMap != null)
        {
            data.MapJson = MapDataSerializer.Serialize(CurrentMap);
        }
        data.CurrentNodeCoord = CurrentNode != null ? CurrentNode.Coordinate : new Vector2Int(-1, -1);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Save successful to: " + path);
    }

    public bool LoadGame()
    {
        string path = Application.persistentDataPath + "/cardsave.json";
        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found at " + path);
            return false; // Changed to return false as per original method signature
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 还原基本数据
            _gold = data.Gold; // Assuming _gold is the backing field for Gold property
            PlayerCurrentHP = data.PlayerCurrentHP;
            PlayerMaxHP = data.PlayerMaxHP;

            // 还原卡组数据 (MasterDeck 代表所有拥有的仓库)
            MasterDeck.Clear();
            foreach (var cardName in data.MasterDeckPaths)
            {
                var card = GetCardFromRegistry(cardName);
                if (card != null) MasterDeck.Add(card);
            }

            // 还原地图
            if (!string.IsNullOrEmpty(data.MapJson))
            {
                CurrentMap = MapDataSerializer.Deserialize(data.MapJson);
                CurrentNode = GetNode(data.CurrentNodeCoord);
            }

            // 同步到 PlayerCollection (核心修复点)
            SyncToPlayerCollection(data.SelectedDeckPaths);

            NotifyPlayerStateChanged();
            SceneManager.LoadScene("MapScene");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载存档失败: {e.Message}");
            return false;
        }
    }

    private void SyncToPlayerCollection(List<string> selectedDeckNames = null)
    {
        if (PlayerCollection.Instance != null)
        {
            PlayerCollection.Instance.OwnedUnits.Clear();
            PlayerCollection.Instance.OwnedCards.Clear();
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();

            foreach (var card in MasterDeck)
            {
                if (card == null) continue;

                // 1. 全部加入“拥有池” (仓库)
                PlayerCollection.Instance.AddCardToCollection(card, true); 
                
                // 2. 如果它在“选中出战”名单里，则加入 Current 池
                // 注意：如果玩家拥有多张同名卡，这里的逻辑可能需要更精确（按引用或按索引），
                // 但对于大多数 Roguelike 按名字匹配已选列表是可行的初始方案。
                if (selectedDeckNames != null && selectedDeckNames.Contains(card.name))
                {
                    if (card.kind == CardKind.Unit)
                        PlayerCollection.Instance.CurrentUnits.Add(card);
                    else
                        PlayerCollection.Instance.CurrentDeck.Add(card);
                    
                    // 为了防止多张同名卡全部被加入，每匹配到一个就移除一个（如果需要严格匹配）
                    // 但由于 load 时 MasterDeck 是按存档顺序生成的，暂不处理精细化匹配。
                }
            }
            Debug.Log("[GameManager] 仓库与出战卡组已同步至 PlayerCollection");
        }
    }
}

// 辅助类：处理 MapData 的嵌套列表序列化
public static class MapDataSerializer
{
    [Serializable]
    private class LayerWrapper
    {
        public List<MapNode> Nodes;
    }

    [Serializable]
    private class MapWrapper
    {
        public List<LayerWrapper> Layers = new List<LayerWrapper>();
    }

    public static string Serialize(MapData data)
    {
        MapWrapper wrapper = new MapWrapper();
        foreach (var layer in data.Layers)
        {
            wrapper.Layers.Add(new LayerWrapper { Nodes = layer });
        }
        return JsonUtility.ToJson(wrapper);
    }

    public static MapData Deserialize(string json)
    {
        MapWrapper wrapper = JsonUtility.FromJson<MapWrapper>(json);
        MapData data = new MapData();
        foreach (var lw in wrapper.Layers)
        {
            data.Layers.Add(lw.Nodes);
        }
        return data;
    }
}
