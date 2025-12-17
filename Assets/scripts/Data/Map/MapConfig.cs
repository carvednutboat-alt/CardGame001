using UnityEngine;
using System.Collections.Generic;
using static TreeEditor.TreeEditorHelper;

[CreateAssetMenu(fileName = "MapConfig", menuName = "Config/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("地图生成配置")]
    public List<NodeType> NodeWeights; // 用于随机权重的简单列表
    public int LayersCount = 12;       // 总层数
    public int MinNodes = 3;           // 每层最少节点
    public int MaxNodes = 5;           // 每层最多节点
}