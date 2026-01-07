/// <summary>
/// 事件效果类型枚举 - 定义事件选项的效果类型
/// </summary>
public enum EventEffectType
{
    None,       // 无效果
    Heal,       // 治疗
    Damage,     // 伤害
    GainGold,   // 获得金币
    GainCard,   // 获得卡牌
    LoseCard,   // 失去卡牌
    MaxHPUp,    // 增加最大生命值
    MaxHPDown,  // 减少最大生命值
    GainRelic,      // [New] 获得随机遗物
    OpenCardReward  // [New] 打开卡牌奖励 (三选一)
}