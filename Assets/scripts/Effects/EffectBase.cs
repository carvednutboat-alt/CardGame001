// 所有卡牌效果的基类
public abstract class EffectBase
{
    // bm: 访问所有管理器的入口
    // sourceCard: 使用的那张卡
    // targetUnit: 指定的目标（如果是全场AOE则为null）
    public abstract void Execute(BattleManager bm, RuntimeCard sourceCard, RuntimeUnit targetUnit = null);
}