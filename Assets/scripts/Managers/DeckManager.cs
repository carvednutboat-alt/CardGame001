using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<RuntimeCard> DrawPile = new List<RuntimeCard>();
    public List<RuntimeCard> Hand = new List<RuntimeCard>();
    public List<RuntimeCard> DiscardPile = new List<RuntimeCard>();

    [Header("UI References")]
    public Transform HandPanel;
    public CardUI CardPrefab;

    [Header("Settings")]
    public int MaxHandSize = 6;

    private BattleManager _bm;

    public void Init(BattleManager bm, List<CardData> startingData = null)
    {
        _bm = bm;
        DrawPile.Clear();
        Hand.Clear();
        DiscardPile.Clear();

        // 清理手牌 UI
        if (HandPanel != null)
        {
            foreach (Transform child in HandPanel)
            {
                Destroy(child.gameObject);
            }
        }

        // 转换卡组为运行时实例
        if (startingData != null)
        {
            foreach (var data in startingData)
            {
                if (data != null)
                {
                    DrawPile.Add(new RuntimeCard(data));
                }
            }
            Shuffle(DrawPile);
            Debug.Log($"[DeckManager] 初始化完成，卡组中有 {DrawPile.Count} 张牌");
        }
        else
        {
            Debug.LogWarning("[DeckManager] 没有提供起始卡组数据");
        }
    }

    public void DrawCards(int count)
    {
        Debug.Log($"[DeckManager] Requesting Draw: {count} cards. Deck: {DrawPile.Count}, Discard: {DiscardPile.Count}");
        for (int i = 0; i < count; i++)
        {
            // 1. Check Draw Pile
            if (DrawPile.Count == 0)
            {
                Debug.Log("[DeckManager] Deck empty. Attempting reshuffle...");
                if (DiscardPile.Count == 0) 
                {
                    Debug.LogWarning("[DeckManager] Both Deck and Discard are empty. Cannot draw.");
                    return; 
                }
                ReshuffleDiscardToDraw();
                
                // SAFETY
                if (DrawPile.Count == 0)
                {
                    Debug.LogError("[DeckManager] Draw Error: DrawPile remains empty after Reshuffle!");
                    return;
                }
                Debug.Log($"[DeckManager] Reshuffle Success. New Deck Count: {DrawPile.Count}");
            }

            RuntimeCard card = DrawPile[0];
            DrawPile.RemoveAt(0);

            if (Hand.Count >= MaxHandSize)
            {
                Debug.Log($"[DeckManager] Hand Full ({Hand.Count}/{MaxHandSize}). Discarding {card.Name}");
                // Hand Full -> Discard directly
                card.UpdateLocation(Location.GRAVE, 0); 
                DiscardPile.Add(card);
                if (_bm != null && _bm.UIManager != null) 
                    _bm.UIManager.Log($"<color=red>手牌已满，</color> {card.Data.cardName} 被丢弃。");
                continue; 
            }

            // Update Location to HAND
            card.UpdateLocation(Location.HAND, Hand.Count);
            Hand.Add(card);
            Debug.Log($"[DeckManager] Drawing '{card.Name}' to Hand. New Hand Count: {Hand.Count}");

            // Create UI
            CreateCardUI(card);
        }
    }

    public void DiscardCard(RuntimeCard card, GameObject uiObject)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            
            // Update Location to GRAVE
            card.UpdateLocation(Location.GRAVE, DiscardPile.Count);
            
            DiscardPile.Add(card);
            Destroy(uiObject);
        }
    }

    public void RemoveCardFromHand(RuntimeCard card, GameObject uiObj)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
        }
        else
        {
            Debug.LogWarning($"[DeckManager] RemoveCardFromHand: Card {card?.Data?.cardName} not found in Hand list.");
        }

        // 无论是否在列表中，只要传递了 UI 对象且确认消耗，都销毁
        if (uiObj != null)
        {
            Destroy(uiObj);
        }
    }

    // === 新增：直接添加卡牌到手牌 (用于检索效果) ===
    public bool AddCardToHand(RuntimeCard card)
    {
        if (Hand.Count >= MaxHandSize)
        {
            _bm.UIManager.Log($"手牌已满，{card.Data.cardName} 被挤掉了。");
            card.UpdateLocation(Location.GRAVE, DiscardPile.Count);
            DiscardPile.Add(card);
            return false;
        }

        card.UpdateLocation(Location.HAND, Hand.Count);
        Hand.Add(card);
        CreateCardUI(card);
        return true;
    }

    private void CreateCardUI(RuntimeCard card)
    {
        CardUI ui = Instantiate(CardPrefab, HandPanel);
        ui.Init(card, _bm); 
    }

    private void ReshuffleDiscardToDraw()
    {
        // Update Location for ALL cards moving to Deck
        foreach(var c in DiscardPile)
        {
            c.UpdateLocation(Location.DECK, 0); // Sequence isn't strictly maintained in deck until draw
        }

        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        Shuffle(DrawPile);
        _bm.UIManager.Log("弃牌堆已返回牌库");
    }

    public void ShuffleDeck()
    {
        Shuffle(DrawPile);
        if (_bm != null && _bm.UIManager != null)
        {
            _bm.UIManager.Log("牌库已洗切。");
        }
    }

    public void Shuffle(List<RuntimeCard> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            RuntimeCard temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public GameObject FindCardUI(RuntimeCard card)
    {
        if (HandPanel == null) return null;
        foreach (Transform child in HandPanel)
        {
            CardUI ui = child.GetComponent<CardUI>();
            if (ui != null && ui.RuntimeCard == card)
            {
                return ui.gameObject;
            }
        }
        return null;
    }
}