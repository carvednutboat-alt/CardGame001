using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapUIController : MonoBehaviour
{
    public void OpenDeckBuilder()
    {
        // 1. 在进入构筑界面前，从 GameManager 拉取最新状态
        if (GameManager.Instance != null && PlayerCollection.Instance != null)
        {
            // 清空 PlayerCollection 的当前选择
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();

            // 遍历 GameManager 的总卡组，分拣回去
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit)
                {
                    PlayerCollection.Instance.CurrentUnits.Add(card);
                    // 同时确保它在 Owned 列表里（防止出现“我带着它，但我没拥有它”的逻辑bug）
                    PlayerCollection.Instance.AddCardToCollection(card, true);
                }
                else
                {
                    PlayerCollection.Instance.CurrentDeck.Add(card);
                    PlayerCollection.Instance.AddCardToCollection(card, true);
                }
            }
        }

        // 2. 加载场景
        SceneManager.LoadScene("DeckBuilderScene");
    }
}