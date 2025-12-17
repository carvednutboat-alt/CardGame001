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

    // ==== 工具方法：用于战利品、构筑界面调用 ====

    public void AddCardToCollection(CardData card)
    {
        if (card == null) return;

        if (card.kind == CardKind.Unit)
        {
            if (!OwnedUnits.Contains(card))
                OwnedUnits.Add(card);
        }
        else
        {
            if (!OwnedCards.Contains(card))
                OwnedCards.Add(card);
        }
    }

    public void SetCurrentDeck(List<CardData> units, List<CardData> deck)
    {
        CurrentUnits = new List<CardData>(units);
        CurrentDeck = new List<CardData>(deck);
    }
}
