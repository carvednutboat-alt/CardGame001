using UnityEngine;
using UnityEngine.UI;

public class StartGameDebug : MonoBehaviour
{
    public Button StartBtn;

    void Start()
    {
        StartBtn.onClick.AddListener(() =>
        {
            // 使用 GameManager 里配置好的 CurrentDeck 作为初始卡组
            GameManager.Instance.StartNewGame(GameManager.Instance.CurrentDeck);
        });
    }
}