using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 遗物UI项组件 - 用于显示单个遗物信息
/// </summary>
public class RelicItemUI : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_Text titleText;
    public TMP_Text descText;
    public Image iconImage;
    
    private RelicData _relicData;
    
    /// <summary>
    /// 初始化遗物项
    /// </summary>
    public void Init(RelicData relic)
    {
        _relicData = relic;
        
        if (relic == null) return;
        
        // 设置标题
        if (titleText != null)
        {
            titleText.text = relic.relicName;
        }
        
        // 设置描述
        if (descText != null)
        {
            descText.text = relic.description;
        }
        
        // 设置图标
        if (iconImage != null && relic.icon != null)
        {
            iconImage.sprite = relic.icon;
            iconImage.color = Color.white;
        }
    }
    
    /// <summary>
    /// 获取关联的遗物数据
    /// </summary>
    public RelicData GetRelicData()
    {
        return _relicData;
    }
}