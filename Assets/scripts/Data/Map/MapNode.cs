using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图节点 - 代表地图上的一个位置点
/// </summary>
[Serializable]
public class MapNode
{
    public Vector2Int Coordinate;           // 节点坐标 (x, y)
    public NodeType Type;                   // 节点类型
    public NodeStatus Status;               // 节点状态
    public List<Vector2Int> Incoming;       // 入边 (从哪些节点可以到达这里)
    public List<Vector2Int> Outgoing;       // 出边 (从这里可以到达哪些节点)

    public MapNode(int x, int y, NodeType type)
    {
        Coordinate = new Vector2Int(x, y);
        Type = type;
        Status = NodeStatus.Locked;
        Incoming = new List<Vector2Int>();
        Outgoing = new List<Vector2Int>();
    }
}