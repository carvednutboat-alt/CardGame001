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
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count == 0)
            {
                if (DiscardPile.Count == 0) return; // û����
                ReshuffleDiscardToDraw();
            }

            RuntimeCard card = DrawPile[0];
            DrawPile.RemoveAt(0);

            if (Hand.Count >= MaxHandSize)
            {
                // �����߼���ֱ�ӽ����ƶѣ������� UI
                DiscardPile.Add(card);
                _bm.UIManager.Log($"<color=red>����������</color> {card.Data.cardName} �����á�");
                continue; // ��������ѭ����������һ��
            }

            Hand.Add(card);

            // ���� UI
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

    private void CreateCardUI(RuntimeCard card)
    {
        CardUI ui = Instantiate(CardPrefab, HandPanel);
        ui.Init(card, _bm); 
    }

    private void ReshuffleDiscardToDraw()
    {
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        Shuffle(DrawPile);
        _bm.UIManager.Log("���ƶ���ϴ���ƿ⡣");
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