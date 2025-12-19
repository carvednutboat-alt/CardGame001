using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    // 用于传递数据给 EventScene
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

            // 把初始卡组全部录入“仓库”
            foreach (var card in starterDeck)
            {
                PlayerCollection.Instance.AddCardToCollection(card, true);
            }
        }

        // 2. 生成地图
        CurrentMap = MapGenerator.GenerateMap(MapConfig);

        // 3. 解锁第一层
        foreach (var node in CurrentMap.Layers[0])
            node.Status = NodeStatus.Attainable;

        NotifyPlayerStateChanged();

        // 4. 进入地图场景
        SceneManager.LoadScene("MapScene");
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
            // 随机一个事件 (实际项目中应该从配置表随机)
            // 这里为了演示，你需要自己Load一个资源或者在Inspector配置一个列表
            CurrentEventProfile = Resources.Load<EventProfile>("Events/Event_Spring");
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
}
