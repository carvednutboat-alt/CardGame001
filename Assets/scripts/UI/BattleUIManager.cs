using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; // 引用 List

public class BattleUIManager : MonoBehaviour
{
    [Header("Log UI")]
    public TMP_Text LogText;
    public UnityEngine.UI.ScrollRect LogScroll;

    [Header("Graveyard UI")]
    public GameObject GraveyardPanel;      // 墓地大面板
    public Transform GraveyardContent;     // 放卡牌的父物体 (Content)
    public CardUI CardPrefab;              // 卡牌预制体 (用于显示)

    // --- 新增：UI 引用 ---
    [Header("Panels")]
    public GameObject GameOverPanel; // 失败结算界面
    public Button ReturnBtn;

    private BattleManager _bm; // 需要引用 BattleManager 来获取墓地数据

    // === 新增：初始化方法 ===
    public void Init(BattleManager bm)
    {
        _bm = bm;
        // 确保游戏开始时面板是关的
        if (GraveyardPanel != null) GraveyardPanel.SetActive(false);

        // 2. --- 添加按钮绑定逻辑 ---
        if (ReturnBtn != null)
        {
            // 防止重复绑定，先移除所有监听
            ReturnBtn.onClick.RemoveAllListeners();
            // 绑定点击事件：调用 GameManager 的 ReturnToTitle
            ReturnBtn.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ReturnToTitle();
                }
                else
                {
                    Debug.LogError("GameManager 不存在，无法返回！");
                }
            });
        }
        else
        {
            Debug.LogError("请在 Inspector 中把返回按钮拖给 ReturnBtn！");
        }
        // -------------------------
    }

    public void Log(string msg)
    {
        if (LogText != null)
        {
            LogText.text += "\n" + msg;
            Canvas.ForceUpdateCanvases();
            if (LogScroll != null) LogScroll.verticalNormalizedPosition = 0f;
        }
        else
        {
            Debug.Log(msg);
        }
    }

    // === 打开墓地 ===
    public void ShowGraveyard()
    {
        if (GraveyardPanel == null || _bm == null) return;

        // 1. 显示面板
        GraveyardPanel.SetActive(true);

        // 2. 清空旧的显示 (防止重复生成)
        foreach (Transform child in GraveyardContent)
        {
            Destroy(child.gameObject);
        }

        // 3. 读取墓地数据并生成卡牌
        // 注意：我们从 UnitManager 获取数据
        List<RuntimeCard> graveyard = _bm.UnitManager.Graveyard;

        if (graveyard.Count == 0)
        {
            // 可以选做：显示一个“墓地为空”的文字
            return;
        }

        foreach (var card in graveyard)
        {
            // 生成卡牌
            CardUI ui = Instantiate(CardPrefab, GraveyardContent);

            // 初始化显示
            // 传入 _bm 是为了显示数据，但我们需要禁用点击
            ui.Init(card, _bm);

            // ★ 关键：禁用交互，墓地里的卡只能看不能用
            if (ui.button != null)
            {
                ui.button.interactable = false;
            }
        }
    }

    // === 关闭墓地 ===
    public void HideGraveyard()
    {
        if (GraveyardPanel != null)
        {
            GraveyardPanel.SetActive(false);
        }
    }

    public void RefreshGraveyardIfOpen()
    {
        if (GraveyardPanel != null && GraveyardPanel.activeSelf)
        {
            ShowGraveyard(); // 重新读取数据并生成卡牌
        }
    }

    public void ShowGameOver()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            Log("游戏结束。"); // 可选：在Log里也写一句
        }
        else
        {
            Debug.LogError("GameOverPanel 还没有赋值！请在 Inspector 里拖拽。");
        }
    }
}