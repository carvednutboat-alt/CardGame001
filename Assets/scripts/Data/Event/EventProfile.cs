using UnityEngine;
using System.Collections.Generic;

// 定义选项触发的效果类型
public enum EventEffectType
{
    None,
    Heal,           // 回血
    Damage,         // 扣血
    GainGold,       // 获得金币 (如果有金币系统)
    GainCard,       // 获得卡牌
    UpgradeCard,    // 升级卡牌 (预留)
    Leave           // 离开事件
}

[System.Serializable]
public class EventOptionData
{
    public string OptionText;       // 按钮上显示的字，如 "[离开] 什么都不做"
    public string ResultText;       // 点击后显示的反馈结果，如 "你转身离开了。"
    public EventEffectType Effect;  // 效果类型
    public int Value;               // 数值 (比如回10血，Value=10)
    public CardData TargetCard;     // 如果是获得卡牌，填这里
}

[CreateAssetMenu(fileName = "NewEvent", menuName = "Data/Event Profile")]
public class EventProfile : ScriptableObject
{
    public string EventID;
    public string Title;            // 标题
    [TextArea(3, 10)]
    public string Description;      // 剧情正文
    public Sprite EventImage;       // 插图 (可选)

    public List<EventOptionData> Options; // 选项列表
}