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

    public void Init(BattleManager bm, List<CardData> startingData)
    {
        _bm = bm;
        DrawPile.Clear();
        Hand.Clear();
        DiscardPile.Clear();

        // 转换数据为运行时实例
        foreach (var data in startingData)
        {
            DrawPile.Add(new RuntimeCard(data));
        }
        Shuffle(DrawPile);
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count == 0)
            {
                if (DiscardPile.Count == 0) return; // 没牌了
                ReshuffleDiscardToDraw();
            }

            RuntimeCard card = DrawPile[0];
            DrawPile.RemoveAt(0);

            if (Hand.Count >= MaxHandSize)
            {
                // 爆牌逻辑：直接进弃牌堆，不生成 UI
                DiscardPile.Add(card);
                _bm.UIManager.Log($"<color=red>手牌已满！</color> {card.Data.cardName} 被弃置。");
                continue; // 跳过本次循环，处理下一张
            }

            Hand.Add(card);

            // 生成 UI
            CreateCardUI(card);
        }
    }

    public void DiscardCard(RuntimeCard card, GameObject uiObject)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            DiscardPile.Add(card);
            Destroy(uiObject);
        }
    }

    public void RemoveCardFromHand(RuntimeCard card, GameObject uiObj)
    {
        if (Hand.Contains(card))
        {
            Hand.Remove(card);
            Destroy(uiObj);
            // 注意：这里没有 DiscardPile.Add
        }
    }

    private void CreateCardUI(RuntimeCard card)
    {
        CardUI ui = Instantiate(CardPrefab, HandPanel);
        // CardUI 需要适配 RuntimeCard，请参照我上一条回答修改 CardUI.Init
        ui.Init(card, _bm); // 暂时传 Data，建议重构 CardUI 接收 RuntimeCard
    }

    private void ReshuffleDiscardToDraw()
    {
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        Shuffle(DrawPile);
        _bm.UIManager.Log("弃牌堆已洗回牌库。");
    }

    private void Shuffle(List<RuntimeCard> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}