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
    public TMP_Text TakeAndRecruitText; 
    
    [Header("Reward Card Choices")]
    public Transform CardChoiceContainer;
    public TMP_Text CardChoiceHeaderText; // [NEW] "Choose your reward card"
    public List<CardData> CurrentChoices = new List<CardData>();
    public CardData SelectedChoice;
    
    
    private CanvasGroup _rewardPanelCanvasGroup;

    // === 新增：初始化方法 ===
    public void Init(BattleManager bm)
    {
        _bm = bm;
        // 清空 Log
        if (LogText != null) LogText.text = "";

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

        // 自动生成战场槽位
        SetupFieldSlots();

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
        Debug.Log($"[BattleUI] {msg}"); // Always log to console
        if (LogText != null)
        {
            LogText.text += "\n" + msg;
            Canvas.ForceUpdateCanvases();
            if (LogScroll != null) LogScroll.verticalNormalizedPosition = 0f;
        }
    }

    // === 打开墓地 ===
    public void ShowGraveyard()
    {
        if (GraveyardPanel == null || _bm == null) return;

        // 1. 显示面板
        GraveyardPanel.SetActive(true);
        GraveyardPanel.transform.SetAsLastSibling();

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
            GameOverPanel.transform.SetAsLastSibling();
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
        List<CardData> cardChoices,
        System.Action<CardData, bool> onConfirm) // SelectedCard, ShouldRecruit
    {
        if (RewardPanel == null) { Debug.LogError("RewardPanel is missing!"); return; }

        SetupUIAttributes();
        
        // 1. 设置按钮逻辑 (哪怕报错，也要先保证按钮能点)
        if (TakeOnlyButton != null)
        {
            TakeOnlyButton.onClick.RemoveAllListeners();
            TakeOnlyButton.onClick.AddListener(() => onConfirm?.Invoke(SelectedChoice, false));
            TakeOnlyButton.interactable = true;
        }
        if (TakeAndRecruitButton != null)
        {
            TakeAndRecruitButton.onClick.RemoveAllListeners();
            TakeAndRecruitButton.onClick.AddListener(() => onConfirm?.Invoke(SelectedChoice, true));
            TakeAndRecruitButton.interactable = (recruitUnit != null);
        }

        // 2. 验证并修复容器 (最容易报错的地方)
        try {
            if (CardChoiceContainer == null) 
            {
                Log("Container reference is null, looking for or creating a new one.");
                var found = MainWindow ? MainWindow.transform.Find("CardChoiceContainer") : null;
                if (found != null) CardChoiceContainer = found;
                else 
                {
                    GameObject go = new GameObject("CardChoiceContainer");
                    go.transform.SetParent(MainWindow ? MainWindow.transform : RewardPanel.transform, false);
                    CardChoiceContainer = go.transform;
                }
            }
            
            // 确保不是 Destroyed 的
            if (CardChoiceContainer.gameObject != null) 
            {
                CardChoiceContainer.gameObject.SetActive(true);
                CardChoiceContainer.SetAsLastSibling();
            }
        } catch (System.Exception ex) {
            Debug.LogError($"[BattleUI] Container Rescue Failed: {ex.Message}");
            // 重写引用
            GameObject go = new GameObject("CardChoiceContainer_Rescue");
            go.transform.SetParent(MainWindow ? MainWindow.transform : RewardPanel.transform, false);
            CardChoiceContainer = go.transform;
        }

        RewardPanel.SetActive(true);
        RewardPanel.transform.SetAsLastSibling(); 

        if (MainWindow != null) MainWindow.transform.SetAsLastSibling();

        // 3. 设置文本
        if (TitleText != null) TitleText.text = "战斗胜利";
        if (RewardGoldText != null) RewardGoldText.text = $"获得金币：<color=yellow>{gold}</color>";
        if (CardChoiceHeaderText != null) {
            CardChoiceHeaderText.text = "<b><size=+6>—— 战利品选择 ——</size></b>\n<color=#aaaaaa><size=-2>请点击下方卡牌，确认后加入收藏</size></color>";
            CardChoiceHeaderText.color = new Color(1f, 0.84f, 0f);
        }

        bool canRecruit = (recruitUnit != null);
        CurrentChoices = cardChoices;
        SelectedChoice = null;

        int choiceCount = (cardChoices != null) ? cardChoices.Count : 0;
        Log($"Showing reward choices. Count: {choiceCount}");
        
        // [Diagnostic] Safer Logging
        if (CardChoiceContainer != null)
        {
            Log($"Container Path: {GetHierarchyPath(CardChoiceContainer)}");
            // Check if gameObject is still valid
            try {
                Log($"Container Active: {CardChoiceContainer.gameObject.activeInHierarchy}, Scale: {CardChoiceContainer.lossyScale}");
            } catch (System.Exception e) {
                Log("Error accessing Container properties: " + e.Message);
            }
        }
        else
        {
            Log("Container is STILL NULL after rescue!");
        }

        // 生成卡牌选项
        if (CardChoiceContainer != null)
        {
            Log("Clearing old card choices...");
            foreach (Transform child in CardChoiceContainer) Destroy(child.gameObject);
            
            // [Diagnostic] Create a giant Red Box to see if anything is visible in this container
            GameObject dummy = new GameObject("Diagnostic_RedBox");
            dummy.transform.SetParent(CardChoiceContainer, false);
            var dummyImg = dummy.AddComponent<Image>();
            dummyImg.color = Color.red;
            var dummyRect = dummy.GetComponent<RectTransform>();
            dummyRect.sizeDelta = new Vector2(100, 100);
            Log("Created Diagnostic Red Box");

            if (cardChoices != null)
            {
                Log($"Instantiating {cardChoices.Count} cards...");
                foreach (var cardData in cardChoices)
                {
                    Log($" - {cardData.cardName}");
                    CardUI cardUI = Instantiate(CardPrefab, CardChoiceContainer);
                    
                    // 强制设置缩放、图层和位置，防止不可见
                    cardUI.transform.localScale = Vector3.one;
                    cardUI.transform.localPosition = Vector3.zero; // Reset Z
                    cardUI.gameObject.layer = LayerMask.NameToLayer("UI");
                    
                    // 显式设置大小，防止被 LayoutGroup 缩放成 0
                    RectTransform cardRect = cardUI.GetComponent<RectTransform>();
                    if (cardRect != null) cardRect.sizeDelta = new Vector2(180, 250);
                    
                    var runtimeCard = new RuntimeCard(cardData);
                    cardUI.Init(runtimeCard, _bm);
                    
                    // 确保卡牌里的 Graphic 组件都是开启的，且 Alpha 是 1
                    foreach (var gr in cardUI.GetComponentsInChildren<Graphic>()) 
                    {
                        gr.color = new Color(gr.color.r, gr.color.g, gr.color.b, 1f);
                        gr.raycastTarget = true;
                    }

                    // 如果 CardUI 有 CanvasGroup 强制其 Alpha = 1
                    var cg = cardUI.GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = 1f;
                    
                    Button btn = cardUI.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => {
                            SelectedChoice = cardData;
                            Log($"已选择卡牌: {cardData.cardName}");
                            foreach(Transform c in CardChoiceContainer) 
                            {
                                Image img = c.GetComponent<Image>();
                                if (img != null) img.color = Color.white;
                            }
                            Image selfImg = cardUI.GetComponent<Image>();
                            if (selfImg != null) selfImg.color = Color.yellow;
                        });
                    }
                }
            }
        }

        // 5. 更新招募信息
        UpdateRecruitInfo(recruitUnit, (cardChoices != null ? cardChoices.Count : 0));
        
        if (RecruitSection != null) RecruitSection.SetActive(recruitUnit != null);

        // 启动淡入动画
        StartCoroutine(AnimateRewardPanel());
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
    
    // === 恢复丢失的 Helper 方法 ===
    
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
                string unitInfo = $"<color=green>目标单位: {recruitUnit.cardName}</color>\n";
                unitInfo += $"面板: {recruitUnit.unitAttack}/{recruitUnit.unitHealth}\n";
                RewardRecruitText.text = unitInfo;
            }
            else
            {
                RewardRecruitText.text = "本次无可招募目标";
            }
        }
    }

    private void SetupUIAttributes()
    {
        if (RewardPanel == null) return;

        // Ensure Canvas for layering
        Canvas pCanvas = RewardPanel.GetComponent<Canvas>();
        if (pCanvas == null) pCanvas = RewardPanel.AddComponent<Canvas>();
        pCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pCanvas.overrideSorting = true;
        pCanvas.sortingOrder = 100; // High enough to be on top

        if (RewardPanel.GetComponent<GraphicRaycaster>() == null)
            RewardPanel.AddComponent<GraphicRaycaster>();

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
            panelImg.color = new Color(0, 0, 0, 0.6f); 
        }

        if (MainWindow != null)
        {
            RectTransform mainRect = MainWindow.GetComponent<RectTransform>();
            if (mainRect != null)
            {
                mainRect.localScale = Vector3.one;
                mainRect.sizeDelta = new Vector2(800, 500); // Enlarged
                mainRect.anchoredPosition = Vector2.zero;
            }

            Image mainImg = MainWindow.GetComponent<Image>();
            if (mainImg != null)
            {
                mainImg.color = new Color(0.08f, 0.08f, 0.12f, 0.95f); // Rich Dark Blue/Black
            }
        }

        ConfigureText(TitleText, 32, TextAlignmentOptions.Center);
        SetupButtonStyle(TakeOnlyButton, new Color(0.15f, 0.35f, 0.55f, 1f), "仅领取金币 & 卡牌"); 
        SetupButtonStyle(TakeAndRecruitButton, new Color(0.85f, 0.65f, 0.15f, 1f), "领取并招募单位"); 
    }

    private void SetupButtonStyle(Button btn, Color color, string label)
    {
        if (btn == null) return;
        
        // 强制大小
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        if (btnRect != null) btnRect.sizeDelta = new Vector2(250, 50);

        Image img = btn.GetComponent<Image>();
        if (img == null) img = btn.gameObject.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;

        // 确保按钮有显示文字
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText == null)
        {
            GameObject tObj = new GameObject("Text");
            tObj.transform.SetParent(btn.transform, false);
            btnText = tObj.AddComponent<TextMeshProUGUI>();
            
            RectTransform rt = btnText.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        
        btnText.text = $"<b>{label}</b>";
        btnText.fontSize = 18;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.raycastTarget = false; 
    }


    private void ConfigureText(TMP_Text text, float size, TextAlignmentOptions align)
    {
        if (text == null) return;
        text.transform.localScale = Vector3.one;
        text.fontSize = size;
        text.alignment = align;
        text.color = Color.white;
    }

    // === 新增：战斗槽位相关 ===
    [Header("Field Slots")]
    public Transform PlayerFieldContainer;
    public Transform EnemyFieldContainer;
    public List<BattleSlotUI> PlayerSlots = new List<BattleSlotUI>();
    public List<BattleSlotUI> EnemySlots = new List<BattleSlotUI>();

    private void SetupFieldSlots()
    {
        // 1. 确保容器存在
        if (PlayerFieldContainer == null) PlayerFieldContainer = CreateSlotContainer("PlayerFieldSlots", new Vector2(0, -30));  // 玩家: Runtime Capture (-30)
        if (EnemyFieldContainer == null) EnemyFieldContainer = CreateSlotContainer("EnemyFieldSlots", new Vector2(0, 184));    // 敌人: Runtime Capture (184)

        // 尝试调整手牌位置 (Hardcode adjusting)
        if (_bm != null && _bm.DeckManager != null && _bm.DeckManager.HandPanel != null)
        {
            RectTransform handRT = _bm.DeckManager.HandPanel.GetComponent<RectTransform>();
            if (handRT != null)
            {
                // 假设锚点是中心，强制拉到底部
                handRT.anchoredPosition = new Vector2(0, -350);
            }
        }

        // 2. 生成玩家槽位 (5个)
        GenerateSlots(PlayerFieldContainer, PlayerSlots, true);

        // 3. 生成敌人槽位 (5个)
        GenerateSlots(EnemyFieldContainer, EnemySlots, false);

        // 4. 生成场地魔法槽位 (1个)
        GenerateFieldMagicSlot();

        // 5. 应用保存的布局 (UnitPanel, LogText, HandPanel, DetailPanel)
        ApplySavedLayout();
    }

    [Header("Field Magic Slot")]
    public Transform FieldMagicContainer;
    public BattleSlotUI FieldMagicSlot;

    private void GenerateFieldMagicSlot()
    {
        // 创建容器 (如果不存)
        if (FieldMagicContainer == null) 
        {
            // Position: Center X=0, Y=77.
            // DO NOT use CreateSlotContainer because it adds a HorizontalLayoutGroup!
            // We want a simple container that does not stretch its child.
            var obj = new GameObject("FieldMagicSlot_Container");
            
            // Find Canvas
            Transform canvasTransform = null;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = FindObjectOfType<Canvas>();
            if (canvas != null) canvasTransform = canvas.transform;
            else canvasTransform = this.transform; 

            obj.transform.SetParent(canvasTransform, false);
            var containerRect = obj.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = new Vector2(0, 77);
            containerRect.sizeDelta = new Vector2(120, 140); // Exact size
            obj.layer = LayerMask.NameToLayer("UI");

            FieldMagicContainer = obj.transform;
        }

        // 清理旧的
        for (int i = FieldMagicContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(FieldMagicContainer.GetChild(i).gameObject);
        }

        // 生成 1 个槽位
        GameObject slotObj = new GameObject("Field_Slot");
        slotObj.transform.SetParent(FieldMagicContainer, false);
        slotObj.layer = LayerMask.NameToLayer("UI");
        
        // UI Image
        Image img = slotObj.AddComponent<Image>();
        img.color = new Color(0, 0.5f, 0.5f, 0.5f); // Distinct color (Cyan-ish)

        RectTransform rect = slotObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 140); // Same size as unit slots

        // Component
        BattleSlotUI slotUI = slotObj.AddComponent<BattleSlotUI>();
        // Special Index -1 for Field Magic? Or just use it directly.
        slotUI.Init(-1, true, null); // No click handler for now? Or maybe show detail?
        
        // Text "Field"
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(slotObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");
        var txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = "Field";
        txt.fontSize = 20;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        var tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        FieldMagicSlot = slotUI;
    }

    public void SetFieldMagicCard(GameObject cardPrefab, RuntimeCard card)
    {
        if (FieldMagicSlot == null) return;
        
        // 1. Clear existing children
        foreach(Transform child in FieldMagicSlot.transform)
        {
            // Do not destroy "Label" or other UI elements we added in Generate
            if (child.GetComponent<CardUI>() != null) Destroy(child.gameObject);
        }

        // 2. Instantiate new card
        if (cardPrefab != null)
        {
             var newUI = Instantiate(cardPrefab, FieldMagicSlot.transform, false);
             CardUI uiComp = newUI.GetComponent<CardUI>();
             if (uiComp != null) uiComp.Init(card, null);
             
             // 3. Fix Layout & Scale
             RectTransform rt = newUI.GetComponent<RectTransform>();
             if (rt != null)
             {
                 rt.anchorMin = Vector2.zero;
                 rt.anchorMax = Vector2.one;
                 rt.offsetMin = Vector2.zero;
                 rt.offsetMax = Vector2.zero;
                 rt.sizeDelta = Vector2.zero; 
                 rt.localScale = Vector3.one;
             }
             
             // 4. Strip Interactive Components
             var hover = newUI.GetComponent<CardHoverHandler>();
             if (hover != null) Destroy(hover);

             var le = newUI.GetComponent<UnityEngine.UI.LayoutElement>();
             if (le != null) Destroy(le);

             if (uiComp != null && uiComp.button != null) uiComp.button.interactable = false;
        }
    }

    private void ApplySavedLayout()
    {
        // 1. UnitPanel
        if (_bm != null && _bm.UnitPanel != null)
        {
            RectTransform rt = _bm.UnitPanel.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(450.39f, -195.11f);
        }

        // 2. HandPanel
        if (_bm != null && _bm.DeckManager != null && _bm.DeckManager.HandPanel != null)
        {
            RectTransform rt = _bm.DeckManager.HandPanel.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(-104f, -195.11f);
        }

        // 3. LogText
        if (LogText != null)
        {
            RectTransform rt = LogText.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(92.9f, -131.05f);
                rt.sizeDelta = new Vector2(185.81f, 262.1f);
            }
        }

        // 4. CardDetailPanel
        CardDetailPanel detailPanel = FindObjectOfType<CardDetailPanel>();
        if (detailPanel != null)
        {
            RectTransform rt = detailPanel.GetComponent<RectTransform>();
            if (rt != null) rt.anchoredPosition = new Vector2(513f, 89.42f);
        }
    }

    private Transform CreateSlotContainer(string name, Vector2 anchoredPos)
    {
        // 1. 寻找 Canvas (优先找自己的父级，找不到就找场景里的)
        Transform canvasTransform = null;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        
        if (canvas != null) canvasTransform = canvas.transform;
        else canvasTransform = this.transform; // Fallback

        var obj = new GameObject(name);
        // 重要：先设置父物体，再挂 RectTransform，确保缩放正确
        obj.transform.SetParent(canvasTransform, false);
        
        var rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(800, 150);

        // 如果没有 Layer 可能会导致看不见，设置一下 (通常是 "UI")
        obj.layer = LayerMask.NameToLayer("UI");

        var lg = obj.AddComponent<HorizontalLayoutGroup>();
        lg.childAlignment = TextAnchor.MiddleCenter;
        lg.spacing = 20;
        lg.childControlWidth = false;
        lg.childControlHeight = false;

        return obj.transform;
    }

    private void GenerateSlots(Transform container, List<BattleSlotUI> list, bool isPlayer)
    {
        list.Clear();
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject slotObj = new GameObject($"Slot_{i+1}");
            slotObj.transform.SetParent(container, false);
            slotObj.layer = LayerMask.NameToLayer("UI"); // Ensure Layer
            
            // UI
            Image img = slotObj.AddComponent<Image>();
            // === 调试：加深颜色，确保看得到 ===
            img.color = new Color(0, 0, 0, 0.5f); 

            RectTransform rect = slotObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 140);

            // Component
            BattleSlotUI slotUI = slotObj.AddComponent<BattleSlotUI>();
            slotUI.Init(i, isPlayer, OnSlotClicked);
            
            // 编号显示
            GameObject textObj = new GameObject("Num");
            textObj.transform.SetParent(slotObj.transform, false);
            textObj.layer = LayerMask.NameToLayer("UI");

            var txt = textObj.AddComponent<TextMeshProUGUI>();
            txt.text = (i + 1).ToString();
            txt.fontSize = 24;
            txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.Center; // Changed to Center for safety
            
            var tr = textObj.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;

            list.Add(slotUI);
        }
    }

    private void OnSlotClicked(int index, bool isPlayerSide)
    {
        if (_bm != null)
        {
            _bm.OnBattleSlotClicked(index, isPlayerSide);
        }
    }

    private string GetHierarchyPath(Transform t)
    {
        if (t == null) return "null";
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public void HighlightPlayerSlots(bool active)
    {
        foreach (var slot in PlayerSlots)
        {
            slot.SetHighlight(active);
        }
    }

    public Transform GetPlayerSlotTransform(int index)
    {
        if (index >= 0 && index < PlayerSlots.Count) return PlayerSlots[index].transform;
        return null; // Fallback
    }

    public Transform GetEnemySlotTransform(int index)
    {
        if (index >= 0 && index < EnemySlots.Count) return EnemySlots[index].transform;
        return null;
    }
}