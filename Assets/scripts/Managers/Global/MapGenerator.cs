using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MapGenerator
{
    public static MapData GenerateMap(MapConfig config)
    {
        MapData map = new MapData();

        // ==========================================
        // 第一步：生成所有节点 (只是生成，不连线)
        // ==========================================
        for (int y = 0; y < config.LayersCount; y++)
        {
            List<MapNode> layerNodes = new List<MapNode>();

            // 第一层和最后一层强制只有 1 个节点 (或者你可以根据需求改成多个)
            // 杀戮尖塔通常：第一层3-4个，Boss层1个
            int count;
            if (y == 0) count = 3; // 起点层数量
            else if (y == config.LayersCount - 1) count = 1; // Boss层数量
            else count = Random.Range(config.MinNodes, config.MaxNodes + 1);

            for (int x = 0; x < count; x++)
            {
                NodeType type = GetRandomType(y, config.LayersCount);
                // 强制修正：Boss层必须是Boss
                if (y == config.LayersCount - 1) type = NodeType.Boss;

                layerNodes.Add(new MapNode(x, y, type));
            }
            map.Layers.Add(layerNodes);
        }

        // ==========================================
        // 第二步：构建路径 (核心修复)
        // ==========================================

        // 策略：
        // 1. 前向遍历：保证本层每个节点至少连向下一层的一个节点 (防止死路)
        // 2. 后向检查：保证下一层的每个节点至少被本层的一个节点连接 (防止孤儿)

        for (int y = 0; y < config.LayersCount - 1; y++)
        {
            var currentLayer = map.Layers[y];
            var nextLayer = map.Layers[y + 1];

            // --- A. 前向连接：保证当前层都有路走 ---
            foreach (var node in currentLayer)
            {
                // 找到下一层“逻辑上相邻”的节点范围
                // 比如我是第2个，下一层有5个，那我只能连下一层的第1, 2, 3个，不能连第5个，不然线太乱
                int leftIndex = GetListIndex(node.Coordinate.x, currentLayer.Count, nextLayer.Count) - 1;
                int rightIndex = GetListIndex(node.Coordinate.x, currentLayer.Count, nextLayer.Count) + 1;

                // 钳制范围
                leftIndex = Mathf.Clamp(leftIndex, 0, nextLayer.Count - 1);
                rightIndex = Mathf.Clamp(rightIndex, 0, nextLayer.Count - 1);

                // 强制连一个
                int targetIndex = Random.Range(leftIndex, rightIndex + 1);
                Connect(node, nextLayer[targetIndex]);

                // 随机多连一个 (增加分叉)
                if (Random.value < 0.2f && leftIndex != rightIndex)
                {
                    int extra = (targetIndex == leftIndex) ? rightIndex : leftIndex;
                    Connect(node, nextLayer[extra]);
                }
            }

            // --- B. 后向检查：保证下一层没有孤儿 ---
            // 检查下一层的每一个节点，看它是不是还没有任何入边 (Incoming)
            for (int i = 0; i < nextLayer.Count; i++)
            {
                MapNode nextNode = nextLayer[i];
                if (nextNode.Incoming.Count == 0)
                {
                    // 它是孤儿！必须从上一层给它找个爸爸
                    // 寻找上一层逻辑相邻的节点
                    int parentLeft = GetListIndex(i, nextLayer.Count, currentLayer.Count) - 1;
                    int parentRight = GetListIndex(i, nextLayer.Count, currentLayer.Count) + 1;

                    parentLeft = Mathf.Clamp(parentLeft, 0, currentLayer.Count - 1);
                    parentRight = Mathf.Clamp(parentRight, 0, currentLayer.Count - 1);

                    int parentIndex = Random.Range(parentLeft, parentRight + 1);
                    Connect(currentLayer[parentIndex], nextNode);
                }
            }
        }

        return map;
    }

    // 辅助函数：连接两个节点
    static void Connect(MapNode from, MapNode to)
    {
        // 防止重复连接
        if (from.Outgoing.Contains(to.Coordinate)) return;

        from.Outgoing.Add(to.Coordinate);
        to.Incoming.Add(from.Coordinate);
    }

    // 辅助函数：计算相对位置索引
    // 比如上面有3个，下面有5个，上面的第1个对应下面的第几个？
    static int GetListIndex(int index, int srcCount, int targetCount)
    {
        float ratio = (float)index / (srcCount - 1 == 0 ? 1 : srcCount - 1);
        return Mathf.RoundToInt(ratio * (targetCount - 1));
    }

    // 随机类型逻辑 (加入了 Event)
    static NodeType GetRandomType(int layerIndex, int totalLayers)
    {
        // 第一层通常全是普通怪，防止开局被事件搞死
        if (layerIndex == 0) return NodeType.MinorEnemy;

        float v = Random.value;
        if (v < 0.40f) return NodeType.MinorEnemy; // 40% 怪
        if (v < 0.62f) return NodeType.Event;      // 22% 事件
        if (v < 0.77f) return NodeType.Rest;       // 15% 篝火
        if (v < 0.87f) return NodeType.Store;      // 10% 商店
        if (v < 0.92f) return NodeType.Treasure;   // 5% 宝箱
        return NodeType.EliteEnemy;                // 8% 精英
    }
}