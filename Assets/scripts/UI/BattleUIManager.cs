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
    [Header("Reward UI")]
    public GameObject RewardPanel;
    public GameObject MainWindow; // 独立的主窗口引用
    public TMP_Text TitleText;
    public TMP_Text RewardGoldText;
    public TMP_Text RewardRecruitText;
    public GameObject RecruitSection;
    public Button TakeOnlyButton;
    public TMP_Text TakeOnlyText; // 新增：按钮文字引用
    public Button TakeAndRecruitButton;
    public TMP_Text TakeAndRecruitText; // 新增：按钮文字引用
    
    
    private CanvasGroup _rewardPanelCanvasGroup;

    // === 新增：初始化方法 ===
    public void Init(BattleManager bm)
    {
        _bm = bm;
        // 确保游戏开始时面板是关的
        if (GraveyardPanel != null) GraveyardPanel.SetActive(false);
        if (RewardPanel != null)
        {
            RewardPanel.SetActive(false);
            // 确保样式在显示前应用一次
            SetupUIAttributes();
            // 添加 CanvasGroup 用于淡入动画
            _rewardPanelCanvasGroup = RewardPanel.GetComponent<CanvasGroup>();
            if (_rewardPanelCanvasGroup == null)
            {
                _rewardPanelCanvasGroup = RewardPanel.AddComponent<CanvasGroup>();
            }
            
            // 自动配置 UI 属性，防止 Inspector 设置错误
            SetupUIAttributes();
        }
        if (GameOverPanel != null) GameOverPanel.SetActive(false);

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

    // === 新增：选择模式 ===
    // 参数 onSelect 是一个回调函数，告诉 BattleManager 选了哪张卡
    public void ShowGraveyardSelection(List<RuntimeCard> graveyard, System.Action<RuntimeCard> onSelect)
    {
        if (GraveyardPanel == null) return;

        // 1. 打开面板
        GraveyardPanel.SetActive(true);

        // 2. 清空旧内容
        foreach (Transform child in GraveyardContent) Destroy(child.gameObject);

        // 3. 生成卡牌
        foreach (var card in graveyard)
        {
            CardUI ui = Instantiate(CardPrefab, GraveyardContent);

            // 初始化显示：传 null 给 manager，防止触发原本的手牌点击逻辑
            ui.Init(card, null);

            // 4. 绑定选择事件
            if (ui.button != null)
            {
                // 确保它是可点的
                ui.button.interactable = true;

                // 清除旧事件，绑定新事件
                ui.button.onClick.RemoveAllListeners();
                ui.button.onClick.AddListener(() =>
                {
                    // 触发回调
                    onSelect?.Invoke(card);
                    // 选完自动关闭
                    HideGraveyard();
                });
            }
        }
    }

    public void ShowBattleReward(
        int gold,
        CardData recruitUnit,
        List<CardData> recruitDeck,
        System.Action<bool> onConfirm)
    {
        if (RewardPanel == null)
        {
            Debug.LogError("RewardPanel 未绑定到 BattleUIManager");
            onConfirm?.Invoke(false);
            return;
        }

        // 每次显示前重新应用一次样式配置，确保显示正确
        SetupUIAttributes();
        
        RewardPanel.SetActive(true);
        
        // 播放淡入动画
        StartCoroutine(AnimateRewardPanel());

        // 设置标题
        if (TitleText != null)
        {
            TitleText.text = "战斗胜利";
        }

        // 显示金币信息
        if (RewardGoldText != null)
        {
            RewardGoldText.text = $"获得金币：{gold}";
        }

        bool canRecruit = (recruitUnit != null);
        int deckCount = (recruitDeck != null) ? recruitDeck.Count : 0;

        // 更新招募信息
        UpdateRecruitInfo(recruitUnit, deckCount);

        // 设置按钮内容
        if (TakeOnlyText != null) TakeOnlyText.text = "只领金币";
        if (TakeAndRecruitText != null) TakeAndRecruitText.text = "领取并招募";

        // 设置按钮逻辑
        if (TakeOnlyButton != null)
        {
            TakeOnlyButton.onClick.RemoveAllListeners();
            TakeOnlyButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke(false);
            });
            TakeOnlyButton.interactable = true;
        }

        if (TakeAndRecruitButton != null)
        {
            TakeAndRecruitButton.onClick.RemoveAllListeners();
            TakeAndRecruitButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke(true);
            });

            TakeAndRecruitButton.interactable = canRecruit;
        }
    }

    public void HideBattleReward()
    {
        if (RewardPanel != null) RewardPanel.SetActive(false);
    }

    // === 新增辅助方法 ===
    
    private System.Collections.IEnumerator AnimateRewardPanel()
    {
        if (_rewardPanelCanvasGroup == null) yield break;
        
        _rewardPanelCanvasGroup.alpha = 0f;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _rewardPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        
        _rewardPanelCanvasGroup.alpha = 1f;
    }
    
    private void UpdateRecruitInfo(CardData recruitUnit, int deckCount)
    {
        bool canRecruit = (recruitUnit != null);
        
        if (RecruitSection != null)
        {
            RecruitSection.SetActive(canRecruit);
        }
        
        if (RewardRecruitText != null)
        {
            if (canRecruit)
            {
                string unitInfo = $"{recruitUnit.cardName}\n";
                unitInfo += $"攻击: {recruitUnit.unitAttack} | 生命: {recruitUnit.unitHealth}\n";
                unitInfo += $"可获得 {deckCount} 张卡牌";
                RewardRecruitText.text = unitInfo;
            }
            else
            {
                RewardRecruitText.text = "本次无可招募目标";
            }
        }
    }

    /// <summary>
    /// 自动配置奖励面板的UI属性，确保即便在编辑器中设置错误也能正确显示
    /// </summary>
    private void SetupUIAttributes()
    {
        if (RewardPanel == null) return;

        // 1. 强制重置面板缩放并设置背景
        RectTransform panelRect = RewardPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.localScale = Vector3.one;
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image panelImg = RewardPanel.GetComponent<Image>();
        if (panelImg != null)
        {
            // 给背景一个深色半透明遮罩
            panelImg.color = new Color(0, 0, 0, 0.6f); 
        }

        // 2. 配置主窗口 (MainWindow)
        if (MainWindow != null)
        {
            RectTransform mainRect = MainWindow.GetComponent<RectTransform>();
            if (mainRect != null)
            {
                mainRect.localScale = Vector3.one;
                mainRect.sizeDelta = new Vector2(320, 240);
                mainRect.anchoredPosition = Vector2.zero;
            }

            Image mainImg = MainWindow.GetComponent<Image>();
            if (mainImg != null)
            {
                mainImg.color = new Color(0.12f, 0.12f, 0.15f, 1f); // 深灰色窗口
            }
        }

        // 3. 配置文本属性
        ConfigureText(TitleText, 24, TextAlignmentOptions.Center);
        ConfigureText(RewardGoldText, 18, TextAlignmentOptions.Center);
        ConfigureText(RewardRecruitText, 16, TextAlignmentOptions.Center);
        ConfigureText(TakeOnlyText, 14, TextAlignmentOptions.Center);
        ConfigureText(TakeAndRecruitText, 14, TextAlignmentOptions.Center);

        // 4. 重置按钮视觉
        SetupButtonStyle(TakeOnlyButton, new Color(0.3f, 0.35f, 0.4f, 1f));
        SetupButtonStyle(TakeAndRecruitButton, new Color(0.5f, 0.4f, 0.2f, 1f));
    }

    private void SetupButtonStyle(Button btn, Color color)
    {
        if (btn == null) return;
        btn.transform.localScale = Vector3.one;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = color;
        }
    }

    private void ConfigureText(TMP_Text text, float size, TextAlignmentOptions align)
    {
        if (text == null) return;
        text.transform.localScale = Vector3.one;
        text.fontSize = size;
        text.alignment = align;
        text.color = Color.white;
    }
}