using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 藏品场景管理器 - 管理藏品收藏界面
/// </summary>
public class CollectionSceneManager : MonoBehaviour
{
    [Header("UI引用")]
    public Transform collectiblesContainer;
    public TMP_Text progressText;
    public TMP_Text titleText;
    public Button backButton;

    [Header("预制体")]
    public GameObject collectibleButtonPrefab;

    [Header("藏品配置")]
    public List<CollectibleData> allCollectibles = new List<CollectibleData>();

    private PlayerCollection _playerCollection;

    private void Start()
    {
        _playerCollection = PlayerCollection.Instance;
        
        if (titleText != null)
        {
            titleText.text = "藏品收藏";
        }

        // 设置返回按钮
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // 加载所有藏品数据
        LoadAllCollectibles();

        // 刷新显示
        RefreshUI();
    }

    /// <summary>
    /// 从Resources加载所有藏品数据
    /// </summary>
    private void LoadAllCollectibles()
    {
        if (allCollectibles.Count == 0)
        {
            CollectibleData[] loadedCollectibles = Resources.LoadAll<CollectibleData>("");
            allCollectibles.AddRange(loadedCollectibles);
            Debug.Log($"[CollectionSceneManager] 加载了 {allCollectibles.Count} 个藏品");
        }
    }

    /// <summary>
    /// 刷新UI显示
    /// </summary>
    private void RefreshUI()
    {
        if (collectiblesContainer == null)
        {
            Debug.LogWarning("[CollectionSceneManager] collectiblesContainer 为空");
            return;
        }

        // 清空现有UI
        foreach (Transform child in collectiblesContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建藏品UI
        int unlockedCount = 0;
        foreach (var collectible in allCollectibles)
        {
            if (collectible == null) continue;

            bool isUnlocked = _playerCollection != null && 
                            _playerCollection.IsCollectibleUnlocked(collectible);

            if (isUnlocked) unlockedCount++;

            // 实例化UI
            GameObject itemObj = null;
            if (collectibleButtonPrefab != null)
            {
                itemObj = Instantiate(collectibleButtonPrefab, collectiblesContainer);
            }
            else
            {
                // 如果没有预制体，创建简单按钮
                itemObj = CreateSimpleCollectibleButton();
                itemObj.transform.SetParent(collectiblesContainer, false);
            }

            // 初始化UI组件
            CollectibleUI ui = itemObj.GetComponent<CollectibleUI>();
            if (ui == null)
            {
                ui = itemObj.AddComponent<CollectibleUI>();
            }
            ui.Init(collectible, isUnlocked);
        }

        // 更新进度文本
        UpdateProgressText(unlockedCount, allCollectibles.Count);
    }

    /// <summary>
    /// 创建简单的藏品按钮（备用方案）
    /// </summary>
    private GameObject CreateSimpleCollectibleButton()
    {
        GameObject buttonObj = new GameObject("CollectibleButton");
        
        // 添加布局组件
        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 60);

        // 添加按钮组件
        Button btn = buttonObj.AddComponent<Button>();
        
        // 添加背景图片
        Image bgImage = buttonObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);

        // 添加文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 16;
        
        RectTransform textRt = text.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        // 添加CollectibleUI组件引用
        CollectibleUI ui = buttonObj.AddComponent<CollectibleUI>();
        ui.nameText = text;
        ui.button = btn;

        return buttonObj;
    }

    /// <summary>
    /// 更新进度文本
    /// </summary>
    private void UpdateProgressText(int unlockedCount, int totalCount)
    {
        if (progressText != null)
        {
            progressText.text = $"已收集: {unlockedCount}/{totalCount}";
        }
    }

    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
    private void OnBackButtonClicked()
    {
        // 返回地图场景或主菜单
        if (GameManager.Instance != null && GameManager.Instance.CurrentMap != null)
        {
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            SceneManager.LoadScene("MainEntry");
        }
    }

    /// <summary>
    /// 公共方法：从外部刷新UI
    /// </summary>
    public void Refresh()
    {
        RefreshUI();
    }
}
