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
            // 清空 PlayerCollection 的“当前携带”状态
            // (我们只重置“当前带了什么”，绝不重置“拥有什么”)
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();

            // 2. 遍历 GameManager 的总卡组，把它们标记为“当前携带”
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit)
                {
                    PlayerCollection.Instance.CurrentUnits.Add(card);

                    // 【关键修改】删除下面这行！
                    // 不要在打开界面时往库存里加卡，否则每次打开都会重复添加！
                    // PlayerCollection.Instance.AddCardToCollection(card, true); <--- 删掉
                }
                else
                {
                    PlayerCollection.Instance.CurrentDeck.Add(card);

                    // 【关键修改】删除下面这行！
                    // PlayerCollection.Instance.AddCardToCollection(card, true); <--- 删掉
                }
            }
        }

        // 3. 加载场景
        SceneManager.LoadScene("DeckBuilderScene");
    }
}