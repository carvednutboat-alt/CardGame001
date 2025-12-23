using System.Collections.Generic;
using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    public static PlayerCollection Instance { get; private set; }

    [Header("玩家拥有的 Unit 卡（可用于构筑）")]
    public List<CardData> OwnedUnits = new List<CardData>();

    [Header("玩家拥有的普通卡（法术 / 装备等）")]
    public List<CardData> OwnedCards = new List<CardData>();

    [Header("当前卡组里选择使用的 Unit 卡")]
    public List<CardData> CurrentUnits = new List<CardData>();

    [Header("当前卡组里选择使用的所有卡(Unit + Spell + Equip)")]
    public List<CardData> CurrentDeck = new List<CardData>();

    [Header("玩家拥有的藏品")]
    public List<CollectibleData> UnlockedCollectibles = new List<CollectibleData>();

    [Header("玩家拥有的Relic")]
    public List<RelicData> OwnedRelics = new List<RelicData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ==== 修改后的工具方法 ====

    /// <summary>
    /// 添加卡牌到收藏
    /// </summary>
    /// <param name="card">卡牌数据</param>
    /// <param name="allowDuplicates">是否允许重复？默认 true (允许)。如果填 false，已有同名卡时将不会添加。</param>
    public void AddCardToCollection(CardData card, bool allowDuplicates = true)
    {
        if (card == null) return;

        // 1. 确定目标列表
        List<CardData> targetList;
        if (card.kind == CardKind.Unit) // 假设 CardData 有 kind 字段
        {
            targetList = OwnedUnits;
        }
        else
        {
            targetList = OwnedCards;
        }

        // 2. 检查重复逻辑
        if (!allowDuplicates)
        {
            // 如果不允许重复，且列表里已经有了
            if (targetList.Contains(card))
            {
                Debug.Log($"[PlayerCollection] 已拥有 {card.cardName}，跳过添加 (Unique Mode)。");
                return;
            }
        }

        // 3. 添加
        targetList.Add(card);
    }

    /// <summary>
    /// 从外部（如 GameManager）同步当前的构筑状态
    /// </summary>
    public void SyncFromMasterDeck(List<CardData> masterDeck)
    {
        CurrentUnits.Clear();
        CurrentDeck.Clear();

        if (masterDeck == null) return;

        foreach (var card in masterDeck)
        {
            if (card == null) continue;

            // 1. 同步到构筑列表
            if (card.kind == CardKind.Unit)
                CurrentUnits.Add(card);
            else
                CurrentDeck.Add(card);

            // 2. 核心修复：确保同步时，这些卡也必须存在于"拥有"池中
            // 否则左侧列表（拥有-当前）计算会出问题
            AddCardToCollection(card, false); // false = 如果已存在则不重复添加
        }
        
        Debug.Log($"[PlayerCollection] 已按 MasterDeck 同步构筑：{masterDeck.Count} 张卡牌");
    }

    public void SetCurrentDeck(List<CardData> units, List<CardData> deck)
    {
        CurrentUnits = new List<CardData>(units);
        CurrentDeck = new List<CardData>(deck);
    }

    // ==== 藏品系统方法 ====

    /// <summary>
    /// 解锁藏品
    /// </summary>
    public void UnlockCollectible(CollectibleData collectible)
    {
        if (collectible == null) return;
        
        if (!UnlockedCollectibles.Contains(collectible))
        {
            UnlockedCollectibles.Add(collectible);
            Debug.Log($"[PlayerCollection] 解锁藏品: {collectible.collectibleName}");
        }
    }

    /// <summary>
    /// 检查藏品是否已解锁
    /// </summary>
    public bool IsCollectibleUnlocked(CollectibleData collectible)
    {
        if (collectible == null) return false;
        return UnlockedCollectibles.Contains(collectible);
    }

    /// <summary>
    /// 获取收集进度
    /// </summary>
    /// <param name="totalCollectibles">总藏品数量</param>
    /// <returns>已解锁数量</returns>
    public int GetCollectionProgress(int totalCollectibles)
    {
        return UnlockedCollectibles.Count;
    }

    // ==== Relic系统方法 ====

    /// <summary>
    /// 添加Relic到收藏
    /// </summary>
    public void AddRelic(RelicData relic)
    {
        if (relic == null) return;
        
        if (!OwnedRelics.Contains(relic))
        {
            OwnedRelics.Add(relic);
            Debug.Log($"[PlayerCollection] 获得Relic: {relic.relicName}");
        }
        else
        {
            Debug.Log($"[PlayerCollection] 已拥有Relic: {relic.relicName}");
        }
    }

    /// <summary>
    /// 检查是否拥有某个Relic
    /// </summary>
    public bool HasRelic(RelicData relic)
    {
        if (relic == null) return false;
        return OwnedRelics.Contains(relic);
    }

    /// <summary>
    /// 根据ID查找Relic
    /// </summary>
    public RelicData GetRelicById(string relicId)
    {
        if (string.IsNullOrEmpty(relicId)) return null;
        return OwnedRelics.Find(r => r != null && r.relicId == relicId);
    }
}
