using UnityEngine;

[CreateAssetMenu(fileName = "MapIconsConfig", menuName = "Config/MapIconsConfig")]
public class MapIconsConfig : ScriptableObject
{
    [Header("节点图标资源")]
    public Sprite MinorEnemyIcon; // 普通怪 (骷髅头)
    public Sprite EliteEnemyIcon; // 精英怪 (带角的骷髅)
    public Sprite BossIcon;       // Boss (大骷髅)
    public Sprite EventIcon;      // 事件 (问号)
    public Sprite RestIcon;       // 休息 (篝火)
    public Sprite ShopIcon;       // 商店 (钱袋)
    public Sprite TreasureIcon;   // 宝箱

    // 辅助方法：根据类型返回对应的图
    public Sprite GetIcon(NodeType type)
    {
        switch (type)
        {
            case NodeType.MinorEnemy: return MinorEnemyIcon;
            case NodeType.EliteEnemy: return EliteEnemyIcon;
            case NodeType.Boss: return BossIcon;
            case NodeType.Event: return EventIcon;
            case NodeType.Rest: return RestIcon;
            case NodeType.Store: return ShopIcon;
            case NodeType.Treasure: return TreasureIcon;
            default: return null;
        }
    }
}