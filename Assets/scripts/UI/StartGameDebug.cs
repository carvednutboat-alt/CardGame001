using UnityEngine;
using UnityEngine.UI;

public class StartGameDebug : MonoBehaviour
{
    public Button NewGameBtn;
    public Button LoadGameBtn;

    private DeckSelectionUI _deckSelectionUI;

private void Start()
    {
        _deckSelectionUI = gameObject.AddComponent<DeckSelectionUI>();
        _deckSelectionUI.Hide();

        SetupUI();
        
        // 延迟一帧初始化，确保所有 Awake 都执行完毕
        StartCoroutine(InitializeWithDelay());
    }

    private void SetupUI()
    {
        if (NewGameBtn != null)
            SetButtonText(NewGameBtn, "新建游戏");
        if (LoadGameBtn != null)
            SetButtonText(LoadGameBtn, "继续游戏");
    }

    private void SetButtonText(Button btn, string text)
    {
        var tmp = btn.GetComponentInChildren<TMPro.TMP_Text>();
        if (tmp != null) tmp.text = text;
    }


private void OnLoadGameClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StartGameDebug] GameManager.Instance 为空！");
            return;
        }
        
        Debug.Log("[StartGameDebug] 加载游戏...");
        GameManager.Instance.LoadGame();
    }


private void OnNewGameClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StartGameDebug] GameManager.Instance 为空！");
            return;
        }
        
        if (_deckSelectionUI != null)
        {
            _deckSelectionUI.Show();
        }
        else
        {
             // Fallback
             GameManager.Instance.StartNewGame(DevCardLoader.DevDeckType.ThousandWeapons);
        }
    }


private System.Collections.IEnumerator InitializeWithDelay()
    {
        // 等待一帧，让所有的 Awake() 执行完毕
        yield return null;

        // 检查 GameManager 是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("[StartGameDebug] GameManager.Instance 为空！请确保场景中有 GameManager 对象。");
            
            // 禁用按钮
            if (NewGameBtn != null) NewGameBtn.interactable = false;
            if (LoadGameBtn != null) LoadGameBtn.interactable = false;
            yield break;
        }

        Debug.Log("[StartGameDebug] GameManager 初始化成功！");

        if (NewGameBtn != null)
        {
            NewGameBtn.onClick.AddListener(OnNewGameClicked);
        }

        if (LoadGameBtn != null)
        {
            bool hasSave = GameManager.Instance.HasSaveGame();
            LoadGameBtn.interactable = hasSave;
            LoadGameBtn.onClick.AddListener(OnLoadGameClicked);
        }
    }
}