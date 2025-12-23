using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapHUD : MonoBehaviour
{
    public TMP_Text HPText;
    public TMP_Text GoldText;
    
    [Header("遗物系统")]
    public Button RelicButton;
    public GameObject RelicPanel;
    public Transform RelicContainer;
    public GameObject RelicItemPrefab;

    void OnEnable()
    {
        Refresh();

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged += Refresh;
            
        // 设置遗物按钮
        if (RelicButton != null)
        {
            RelicButton.onClick.RemoveAllListeners();
            RelicButton.onClick.AddListener(ToggleRelicPanel);
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged -= Refresh;
    }

    void Refresh()
    {
        if (GameManager.Instance == null) return;

        HPText.text = $"HP: {GameManager.Instance.PlayerCurrentHP}/{GameManager.Instance.PlayerMaxHP}";
        GoldText.text = $"Gold: {GameManager.Instance.Gold}";
        
        // 刷新遗物显示
        RefreshRelicDisplay();
    }
    
    /// <summary>
    /// 切换遗物面板显示状态
    /// </summary>
    void ToggleRelicPanel()
    {
        if (RelicPanel != null)
        {
            bool isActive = !RelicPanel.activeSelf;
            RelicPanel.SetActive(isActive);
            
            if (isActive)
            {
                RefreshRelicDisplay();
            }
        }
    }
    
    /// <summary>
    /// 刷新遗物显示
    /// </summary>
    void RefreshRelicDisplay()
    {
        if (RelicContainer == null || PlayerCollection.Instance == null) return;
        
        // 清空现有显示
        foreach (Transform child in RelicContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 显示已拥有的遗物
        foreach (var relic in PlayerCollection.Instance.OwnedRelics)
        {
            if (relic == null) continue;
            
            CreateRelicItem(relic);
        }
    }
    
    /// <summary>
    /// 创建遗物UI项
    /// </summary>
    void CreateRelicItem(RelicData relic)
    {
        GameObject itemObj = null;
        
        // 尝试使用预制体
        if (RelicItemPrefab != null)
        {
            itemObj = Instantiate(RelicItemPrefab, RelicContainer);
        }
        else
        {
            // 创建简单的UI项
            itemObj = CreateSimpleRelicItem();
            itemObj.transform.SetParent(RelicContainer, false);
        }
        
        // 设置UI内容
        SetupRelicItem(itemObj, relic);
    }
    
    /// <summary>
    /// 创建简单的遗物UI项
    /// </summary>
    GameObject CreateSimpleRelicItem()
    {
        GameObject itemObj = new GameObject("RelicItem");
        
        RectTransform rt = itemObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 80);
        
        // 背景
        Image bgImage = itemObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.3f);
        
        // 标题文本
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(itemObj.transform, false);
        TMP_Text titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 16;
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform titleRt = titleText.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 0.5f);
        titleRt.anchorMax = new Vector2(0.7f, 1f);
        titleRt.offsetMin = new Vector2(10, 0);
        titleRt.offsetMax = new Vector2(-10, -5);
        
        // 描述文本
        GameObject descObj = new GameObject("DescText");
        descObj.transform.SetParent(itemObj.transform, false);
        TMP_Text descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 12;
        descText.color = new Color(0.8f, 0.8f, 0.8f);
        
        RectTransform descRt = descText.GetComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0, 0);
        descRt.anchorMax = new Vector2(1f, 0.5f);
        descRt.offsetMin = new Vector2(10, 0);
        descRt.offsetMax = new Vector2(-10, 2);
        
        // 保存组件引用
        RelicItemUI itemUI = itemObj.AddComponent<RelicItemUI>();
        itemUI.titleText = titleText;
        itemUI.descText = descText;
        
        return itemObj;
    }
    
    /// <summary>
    /// 设置遗物项内容
    /// </summary>
    void SetupRelicItem(GameObject itemObj, RelicData relic)
    {
        RelicItemUI itemUI = itemObj.GetComponent<RelicItemUI>();
        if (itemUI != null)
        {
            if (itemUI.titleText != null)
                itemUI.titleText.text = relic.relicName;
                
            if (itemUI.descText != null)
                itemUI.descText.text = relic.description;
        }
    }
}
