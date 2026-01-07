using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MapUIController : MonoBehaviour
{
    public GameObject OptionsPanel;
    public Button OptionsButton;
    public Button SaveButton;
    public Button ReturnTitleButton;
    public Button CloseButton;

    // === 藏品 UI ===
    [Header("Relic UI")]
    public Button RelicButton;
    public GameObject RelicPanel;
    public Transform RelicContent;
    public GameObject RelicItemPrefab;
    public Button CloseRelicButton;



    private void Start()
    {
        SetupUI();
        EnsureRelicUIExists(); // [New] 自动补全UI
        SetupRelicUI();
    }

    private void SetupUI()
    {
        if (OptionsButton != null)
        {
            OptionsButton.onClick.RemoveAllListeners(); // 防止重复添加
            OptionsButton.onClick.AddListener(() => ToggleOptions(true));
            SetButtonText(OptionsButton, "选项");
        }

        if (SaveButton != null)
        {
            SaveButton.onClick.RemoveAllListeners();
            SaveButton.onClick.AddListener(OnSaveButtonClicked);
            SetButtonText(SaveButton, "保存游戏");
        }

        if (ReturnTitleButton != null)
        {
            ReturnTitleButton.onClick.RemoveAllListeners();
            ReturnTitleButton.onClick.AddListener(OnReturnTitleClicked);
            SetButtonText(ReturnTitleButton, "返回主页");
        }

        if (CloseButton != null)
        {
            CloseButton.onClick.RemoveAllListeners();
            CloseButton.onClick.AddListener(() => ToggleOptions(false));
            SetButtonText(CloseButton, "关闭");
        }

        if (OptionsPanel != null) OptionsPanel.SetActive(false);
    }

    private void EnsureRelicUIExists()
    {
        // === 核心修复：查找正确的父 Canvas ===
        Transform uiParent = this.transform;
        
        // 如果自己不是 Canvas 的孩子，就去场景里找一个 Canvas
        if (GetComponentInParent<Canvas>() == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                uiParent = canvas.transform;
            }
        }
        
        // 1. 如果没有 RelicButton，创建一个
        if (RelicButton == null)
        {
            // 尝试在 Transform 下找
            var existing = uiParent.Find("TopRightButtons/RelicButton") ?? uiParent.Find("RelicButton");
            if (existing != null) 
            {
                RelicButton = existing.GetComponent<Button>();
            }
            else
            {
                // 创建到 uiParent 下
                RelicButton = UIHelper.CreateButton("RelicButton", "藏品", uiParent);
                
                // === 修改：定位到右下角 HUD 上方 ===
                RectTransform rect = RelicButton.GetComponent<RectTransform>();
                
                // Anchor: Bottom Right
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                
                // Position: Right padding 20, Up padding 150 (Above HUD)
                rect.anchoredPosition = new Vector2(-20, 150);
            }
        }

        // 2. 如果没有 RelicPanel，创建一个
        if (RelicPanel == null)
        {
            var existing = uiParent.Find("RelicPanel");
            if (existing != null)
            {
                RelicPanel = existing.gameObject;
            }
            else
            {
                RelicPanel = UIHelper.CreatePanel("RelicPanel", uiParent);
                // 确保 Panel 在最上层
                RelicPanel.transform.SetAsLastSibling();
            }
        }

        // 3. 在 Panel 里找 Content 和 CloseButton
        if (RelicPanel != null)
        {
            if (RelicContent == null)
            {
                // 假设结构是 ScrollView/Viewport/Content
                var contentTrans = RelicPanel.transform.Find("ScrollView/Viewport/Content");
                if (contentTrans == null)
                {
                    // 如果没有ScrollView，我们可以给 Panel 创建一个简单的 Grid 结构
                    RelicContent = UIHelper.CreateScrollView(RelicPanel.transform);
                }
                else
                {
                    RelicContent = contentTrans;
                }
            }

            if (CloseRelicButton == null)
            {
                var closeTrans = RelicPanel.transform.Find("CloseButton");
                if (closeTrans != null)
                {
                    CloseRelicButton = closeTrans.GetComponent<Button>();
                }
                else
                {
                    CloseRelicButton = UIHelper.CreateButton("CloseButton", "关闭", RelicPanel.transform);
                    // 调整关闭按钮位置到右上角
                    var drect = CloseRelicButton.GetComponent<RectTransform>();
                    drect.anchorMin = new Vector2(1, 1);
                    drect.anchorMax = new Vector2(1, 1);
                    drect.pivot = new Vector2(1, 1);
                    drect.anchoredPosition = new Vector2(-50, -50);
                }
            }
        }
    }

    private void SetupRelicUI()
    {
        if (RelicButton != null)
        {
            RelicButton.onClick.RemoveAllListeners();
            RelicButton.onClick.AddListener(ShowRelics);
            // SetButtonText(RelicButton, "藏品"); 
        }

        if (CloseRelicButton != null)
        {
            CloseRelicButton.onClick.RemoveAllListeners();
            CloseRelicButton.onClick.AddListener(HideRelics);
        }
        
        if (RelicPanel != null) RelicPanel.SetActive(false);
    }

    public void ShowRelics()
    {
        Debug.Log("[MapUI] ShowRelics Clicked");
        if (RelicPanel != null) 
        {
            RelicPanel.SetActive(true);
            RelicPanel.transform.SetAsLastSibling(); // Bring to front
            RefreshRelicList();
        }
    }

    public void HideRelics()
    {
        if (RelicPanel != null) RelicPanel.SetActive(false);
    }

    private void RefreshRelicList()
    {
        if (RelicContent == null) return;
        if (PlayerCollection.Instance == null) return;

        Debug.Log($"[MapUI] Refreshing Relics. Count: {PlayerCollection.Instance.OwnedRelics.Count}");

        // 清空旧列表
        foreach (Transform child in RelicContent)
        {
            Destroy(child.gameObject);
        }

        //生成新列表
        foreach (var relic in PlayerCollection.Instance.OwnedRelics)
        {
            if (relic == null) continue;

            RelicItemUI itemUI = null;

            if (RelicItemPrefab != null)
            {
                GameObject obj = Instantiate(RelicItemPrefab, RelicContent);
                itemUI = obj.GetComponent<RelicItemUI>();
            }
            else
            {
                // 动态生成 Item
                GameObject obj = UIHelper.CreateRelicItemObject(RelicContent);
                itemUI = obj.AddComponent<RelicItemUI>();
                // 绑定 UI 组件
                var titleTrans = obj.transform.Find("Title");
                var descTrans = obj.transform.Find("Desc");
                var iconTrans = obj.transform.Find("Icon");

                if (titleTrans) itemUI.titleText = titleTrans.GetComponent<TMP_Text>();
                if (descTrans) itemUI.descText = descTrans.GetComponent<TMP_Text>();
                if (iconTrans) itemUI.iconImage = iconTrans.GetComponent<Image>();
            }

            if (itemUI != null)
            {
                itemUI.Init(relic);
            }
        }
    }

    private void SetButtonText(Button btn, string text)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null) tmp.text = text;
    }

    // === 简单的 UI 构建工具类 (嵌套定义) ===
    public static class UIHelper
    {
         // 获取字体资源
        public static TMP_FontAsset GetFont()
        {
            var all = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (all != null && all.Length > 0) return all[0];
            return null;
        }

        public static Button CreateButton(string name, string text, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            
            Image img = obj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f); // 深灰色背景

            Button btn = obj.AddComponent<Button>();
            btn.targetGraphic = img;

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.font = GetFont();

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 50);
            
            // Text stretch
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return btn;
        }

        public static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            
            // 全屏拉伸
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image img = obj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.95f); // 更加不透明
            // img.raycastTarget = true; 

            return obj;
        }

        public static Transform CreateScrollView(Transform parent)
        {
            return UIHelper.CreateScrollViewImpl(parent);
        }

        // Split to avoid too long method in replace block matching
        public static Transform CreateScrollViewImpl(Transform parent)
        {
            GameObject scrollObj = new GameObject("ScrollView");
            scrollObj.transform.SetParent(parent, false);
            var scrollRect = scrollObj.AddComponent<RectTransform>();
            // Full Stretch with padding
            scrollRect.anchorMin = new Vector2(0.05f, 0.05f);
            scrollRect.anchorMax = new Vector2(0.95f, 0.95f);
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            // Content
            GameObject contentObj = new GameObject("Viewport_Content");
            contentObj.transform.SetParent(scrollObj.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1); // Top Left
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            
            // Grid Layout
            var grid = contentObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(300, 100);
            grid.spacing = new Vector2(10, 10);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3; // Wider
            grid.childAlignment = TextAnchor.UpperCenter;

            // Add ContentSizeFitter
            var fitter = contentObj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var sr = scrollObj.AddComponent<ScrollRect>();
            sr.content = contentRect;
            sr.viewport = scrollObj.GetComponent<RectTransform>(); 
            sr.horizontal = false;
            sr.vertical = true;
            
            // Scroll Background
            var img = scrollObj.AddComponent<Image>();
            img.color = new Color(1,1,1,0.05f);

            return contentObj.transform;
        }

        public static GameObject CreateRelicItemObject(Transform parent)
        {
            GameObject obj = new GameObject("RelicItem");
            obj.transform.SetParent(parent, false);
            
            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.4f, 0.4f, 0.4f);

            // Icon
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(obj.transform, false);
            Image iconImg = iconObj.AddComponent<Image>();
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(10, 0);
            iconRect.sizeDelta = new Vector2(80, 80);

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(obj.transform, false);
            TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.fontSize = 20;
            titleTxt.color = Color.yellow;
            titleTxt.font = GetFont();
            titleTxt.alignment = TextAlignmentOptions.Left;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(100, 30);
            titleRect.sizeDelta = new Vector2(180, 30); 

            // Desc
            GameObject descObj = new GameObject("Desc");
            descObj.transform.SetParent(obj.transform, false);
            TextMeshProUGUI descTxt = descObj.AddComponent<TextMeshProUGUI>();
            descTxt.fontSize = 14;
            descTxt.color = Color.white;
            descTxt.font = GetFont();
            descTxt.alignment = TextAlignmentOptions.TopLeft;
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchoredPosition = new Vector2(100, -10);
            descRect.sizeDelta = new Vector2(180, 60);

            return obj;
        }
    }


    public void OpenDeckBuilder()
    {
        // ... (保持之前的卡组同步逻辑)
        if (GameManager.Instance != null && PlayerCollection.Instance != null)
        {
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit) PlayerCollection.Instance.CurrentUnits.Add(card);
                else PlayerCollection.Instance.CurrentDeck.Add(card);
            }
        }
        SceneManager.LoadScene("DeckBuilderScene");
    }

    // === 地图菜单功能 ===

    public void ToggleOptions(bool show)
    {
        if (OptionsPanel != null) OptionsPanel.SetActive(show);
    }

    public void OnSaveButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            // 以后可以在这里弹个“已保存”的小提示
            Debug.Log("地图存档已完成");
        }
    }

    public void OnReturnTitleClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTitle();
        }
    }
}