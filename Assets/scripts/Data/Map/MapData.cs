using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图数据类 - 存储整个地图的层级结构
/// </summary>
[Serializable]
public class MapData
{
    public List<List<MapNode>> Layers = new List<List<MapNode>>();
}