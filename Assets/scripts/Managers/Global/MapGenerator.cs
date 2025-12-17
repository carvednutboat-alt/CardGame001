using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MapGenerator
{
    public static MapData GenerateMap(MapConfig config)
    {
        MapData map = new MapData();

        // 1. 生成节点
        for (int y = 0; y < config.LayersCount; y++)
        {
            List<MapNode> layerNodes = new List<MapNode>();
            // 第一层和最后一层特殊处理
            int count = (y == 0 || y == config.LayersCount - 1) ? 1 : Random.Range(config.MinNodes, config.MaxNodes + 1);

            for (int x = 0; x < count; x++)
            {
                NodeType type = NodeType.MinorEnemy; // 默认
                if (y == config.LayersCount - 1) type = NodeType.Boss;
                else if (y == 0) type = NodeType.MinorEnemy;
                else type = GetRandomType(); // 简单随机

                layerNodes.Add(new MapNode(x, y, type));
            }
            map.Layers.Add(layerNodes);
        }

        // 2. 连接节点 (路径生成)
        for (int y = 0; y < config.LayersCount - 1; y++)
        {
            var currentLayer = map.Layers[y];
            var nextLayer = map.Layers[y + 1];

            foreach (var node in currentLayer)
            {
                // 简单的连接逻辑：连接到下一层索引相近的节点
                int startNext = Mathf.Max(0, Mathf.FloorToInt((float)node.Coordinate.x / currentLayer.Count * nextLayer.Count));
                int endNext = Mathf.Min(nextLayer.Count - 1, Mathf.FloorToInt((float)node.Coordinate.x / currentLayer.Count * nextLayer.Count) + 1);

                for (int i = startNext; i <= endNext; i++)
                {
                    // 必须连接一个，其他的随机
                    if (i == startNext || Random.value > 0.5f)
                    {
                        Connect(node, nextLayer[i]);
                    }
                }
            }
        }

        return map;
    }

    static void Connect(MapNode from, MapNode to)
    {
        if (!from.Outgoing.Contains(to.Coordinate)) from.Outgoing.Add(to.Coordinate);
        if (!to.Incoming.Contains(from.Coordinate)) to.Incoming.Add(from.Coordinate);
    }

    static NodeType GetRandomType()
    {
        float v = Random.value;
        if (v < 0.5f) return NodeType.MinorEnemy;
        if (v < 0.7f) return NodeType.Rest;
        if (v < 0.9f) return NodeType.EliteEnemy;
        return NodeType.Store;
    }
}