using System;

// 代表“手中的一张牌”，它是 CardData 的运行时包装
[Serializable]
public class RuntimeCard
{
    public string UniqueId { get; private set; } // 唯一ID
    public CardData Data { get; private set; }   // 原始配置

    // 可以在这里加动态数据，比如“本局费用减1”

    public RuntimeCard(CardData data)
    {
        Data = data;
        UniqueId = Guid.NewGuid().ToString(); // 赋予唯一身份
        // CurrentCost = data.cost; 
    }
}