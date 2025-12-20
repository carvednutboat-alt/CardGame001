/// <summary>
/// 节点状态枚举 - 定义节点的可访问状态
/// </summary>
public enum NodeStatus
{
    Locked,      // 锁定 - 玩家无法访问
    Attainable,  // 可达 - 玩家可以选择访问
    Current,     // 当前 - 玩家正在此节点
    Visited      // 已访问 - 玩家已经访问过
}