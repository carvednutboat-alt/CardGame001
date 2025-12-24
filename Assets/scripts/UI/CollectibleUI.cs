using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 单个藏品UI组件 - 用于在藏品列表中显示藏品
/// </summary>
public class CollectibleUI : MonoBehaviour
{
    [Header("UI组件引用")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image iconImage;
    public GameObject lockedOverlay;
    public Button button;

    private CollectibleData _data;
    private bool _isUnlocked;

    /// <summary>
    /// 初始化藏品UI
    /// </summary>
    public void Init(CollectibleData data, bool isUnlocked)
    {
        _data = data;
        _isUnlocked = isUnlocked;

        if (_data == null)
        {
            Debug.LogWarning("[CollectibleUI] 藏品数据为空");
            return;
        }

        // 设置名称
        if (nameText != null)
        {
            nameText.text = isUnlocked ? data.collectibleName : "???";
        }

        // 设置描述
        if (descriptionText != null)
        {
            descriptionText.text = isUnlocked ? data.description : "未解锁";
        }

        // 设置图标
        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.color = isUnlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f);
        }

        // 显示/隐藏锁定蒙层
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!isUnlocked);
        }

        // 设置按钮交互
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            if (isUnlocked)
            {
                button.onClick.AddListener(OnClicked);
                
                // 添加悬停详情（如果有关联卡牌）
                if (data.relatedCard != null)
                {
                    CardHoverHandler hover = button.gameObject.GetComponent<CardHoverHandler>();
                    if (hover == null)
                    {
                        hover = button.gameObject.AddComponent<CardHoverHandler>();
                    }
                    hover.Init(data.relatedCard);
                }
            }
        }
    }

    private void OnClicked()
    {
        if (!_isUnlocked || _data == null) return;
        
        Debug.Log($"[CollectibleUI] 点击了藏品: {_data.collectibleName}");
        // 未来可以在这里显示详细信息弹窗
    }

    /// <summary>
    /// 获取稀有度对应的颜色
    /// </summary>
    public static Color GetRarityColor(CollectibleRarity rarity)
    {
        switch (rarity)
        {
            case CollectibleRarity.普通:
                return new Color(0.8f, 0.8f, 0.8f); // 灰色
            case CollectibleRarity.稀有:
                return new Color(0.3f, 0.7f, 1.0f); // 蓝色
            case CollectibleRarity.史诗:
                return new Color(0.7f, 0.3f, 1.0f); // 紫色
            case CollectibleRarity.传说:
                return new Color(1.0f, 0.7f, 0.2f); // 橙色
            default:
                return Color.white;
        }
    }
}
