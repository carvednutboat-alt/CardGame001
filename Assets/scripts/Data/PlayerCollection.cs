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

    public void SetCurrentDeck(List<CardData> units, List<CardData> deck)
    {
        CurrentUnits = new List<CardData>(units);
        CurrentDeck = new List<CardData>(deck);
    }
}
