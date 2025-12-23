using UnityEngine;

/// <summary>
/// 藏品稀有度枚举
/// </summary>
public enum CollectibleRarity
{
    普通,
    稀有,
    史诗,
    传说
}

/// <summary>
/// 藏品数据 - 可收集物品的配置
/// </summary>
[CreateAssetMenu(fileName = "NewCollectible", menuName = "Card Game/Collectible")]
public class CollectibleData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("藏品唯一ID")]
    public string collectibleId;
    
    [Tooltip("藏品名称")]
    public string collectibleName;
    
    [TextArea(3, 6)]
    [Tooltip("藏品描述")]
    public string description;
    
    [Header("稀有度")]
    public CollectibleRarity rarity = CollectibleRarity.普通;
    
    [Header("关联卡牌")]
    [Tooltip("关联的卡牌数据，如果藏品是卡牌收藏")]
    public CardData relatedCard;
    
    [Header("解锁条件")]
    [TextArea(2, 4)]
    [Tooltip("解锁条件说明")]
    public string unlockCondition;
    
    [Header("显示设置")]
    [Tooltip("藏品图标精灵")]
    public Sprite icon;
}
