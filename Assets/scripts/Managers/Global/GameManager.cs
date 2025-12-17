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
    public List<CardData> CurrentDeck = new List<CardData>(); // 玩家当前拥有的卡组

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

        // 初始化 MasterDeck
        MasterDeck = new List<CardData>();
        foreach (var card in starterDeck) MasterDeck.Add(card);

        CurrentDeck = new List<CardData>(starterDeck); // 复制初始卡组
        CurrentNode = null;

        // 2. 生成地图
        CurrentMap = MapGenerator.GenerateMap(MapConfig);

        // 3. 解锁第一层
        foreach (var node in CurrentMap.Layers[0])
            node.Status = NodeStatus.Attainable;

        // 4. 进入地图场景
        SceneManager.LoadScene("MapScene");
    }

    // --- 修改 SelectNode (支持事件) ---
    public void SelectNode(MapNode node)
    {
        CurrentNode = node;
        node.Status = NodeStatus.Current;

        // 根据类型跳转场景
        if (node.Type == NodeType.MinorEnemy || node.Type == NodeType.EliteEnemy || node.Type == NodeType.Boss)
        {
            SceneManager.LoadScene("BattleScene");
        }
        else if (node.Type == NodeType.Event) // 假设你在 MapData 里加了这个枚举
        {
            // 随机一个事件 (实际项目中应该从配置表随机)
            // 这里为了演示，你需要自己Load一个资源或者在Inspector配置一个列表
            CurrentEventProfile = Resources.Load<EventProfile>("Events/Event_Fountain");
            SceneManager.LoadScene("EventScene");
        }
        else
        {
            OnNodeCompleted(); // 其他暂时也直接跳过
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

        // 3. 回到地图
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

    // --- 接口：获得卡牌 (解决了你的设计需求) ---
    public void AddCardToDeck(CardData newCard)
    {
        if (newCard == null) return;
        // 建议复制一份数据，防止修改原始配置
        // 如果 CardData 是 ScriptableObject 且不修改内部值，直接添加引用也行
        MasterDeck.Add(newCard);
        Debug.Log($"获得了卡牌: {newCard.cardName}");
    }

    // 在 GameManager 中
    public void HealPlayer(int amount)
    {
        PlayerCurrentHP += amount;
        if (PlayerCurrentHP > PlayerMaxHP) PlayerCurrentHP = PlayerMaxHP;
        Debug.Log($"[Global] 玩家回血 {amount}, 当前: {PlayerCurrentHP}");
    }

    public void DamagePlayer(int amount)
    {
        PlayerCurrentHP -= amount;
        if (PlayerCurrentHP < 0) PlayerCurrentHP = 0;
        Debug.Log($"[Global] 玩家扣血 {amount}, 当前: {PlayerCurrentHP}");
    }

    public void ReturnToTitle()
    {
        Debug.Log("返回标题画面，重置数据...");

        // 1. 清理数据
        CurrentMap = null;
        CurrentNode = null;
        MasterDeck.Clear();
        PlayerCurrentHP = PlayerMaxHP;

        // 2. 加载主菜单场景 (假设你的入口场景叫 MainEntry)
        // 注意：如果你有专门的 MainMenu 场景，就加载 MainMenu
        // 这里我们重新加载 MainEntry 相当于重启游戏
        SceneManager.LoadScene("MainEntry");
    }
}

