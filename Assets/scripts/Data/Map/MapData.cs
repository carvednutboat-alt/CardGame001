using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    MinorEnemy, // 普通怪
    EliteEnemy, // 精英怪
    Rest,       // 休息点
    Treasure,   // 宝箱
    Store,      // 商店
    Boss,        // Boss
    Event       //不期而遇
}

public enum NodeStatus
{
    Locked,     // 锁住（不可点）
    Attainable, // 可到达（当前玩家所在位置的邻居）
    Visited,    // 已访问
    Current     // 玩家当前位置
}

[System.Serializable]
public class MapNode
{
    public Vector2Int Coordinate; // x:层内索引, y:层数(第几层)
    public NodeType Type;
    public NodeStatus Status = NodeStatus.Locked;

    // 存储父节点和子节点的坐标，用于画线和判断路径
    public List<Vector2Int> Incoming = new List<Vector2Int>();
    public List<Vector2Int> Outgoing = new List<Vector2Int>();

    public MapNode(int x, int y, NodeType type)
    {
        Coordinate = new Vector2Int(x, y);
        Type = type;
    }
}

[System.Serializable]
public class MapData
{
    // 每一层包含若干个节点
    public List<List<MapNode>> Layers = new List<List<MapNode>>();
}