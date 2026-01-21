using UnityEngine;

/// <summary>
/// Relic效果类型枚举
/// </summary>
public enum RelicEffectType
{
    无,
    回合结束回血,
    额外抽牌
}

/// <summary>
/// Relic数据 - 遗物配置
/// </summary>
[CreateAssetMenu(fileName = "NewRelic", menuName = "Card Game/Relic")]
public class RelicData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("Relic唯一ID")]
    public string relicId;
    
    [Tooltip("Relic名称")]
    public string relicName;
    
    [TextArea(3, 6)]
    [Tooltip("Relic描述")]
    public string description;
    
    [Header("效果")]
    public RelicEffectType effectType = RelicEffectType.无;
    
    [Tooltip("效果数值（如回血量、抽牌数等）")]
    public int effectValue = 1;
    
    [Header("商店设置")]
    [Tooltip("最低价格")]
    public int minPrice = 1;
    
    [Tooltip("最高价格")]
    public int maxPrice = 3;
    
    [Header("显示设置")]
    [Tooltip("Relic图标")]
    public Sprite icon;
}
