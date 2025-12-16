// 所有卡牌效果的基类
public abstract class EffectBase
{
    // bm: 访问所有管理器的入口
    // sourceCard: 使用的那张卡
    // targetUnit: 指定的目标（如果是全场AOE则为null）
    public abstract void Execute(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit = null);

    // === 新增：检查条件是否满足 ===
    // 返回 true 代表可以释放，返回 false 代表条件不足（同时负责打印 Log 提示玩家）
    public virtual bool CheckCondition(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit = null)
    {
        return true; // 默认所有卡都能用
    }
}