using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // 必须引用

public class MapSceneManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public GameObject LinePrefab; // <--- 新增：拖入刚才做的 MapLine 预制体
    public Transform Container;
    // --- 新增：拖入刚才写的配置 ---
    public MapIconsConfig IconsConfig;

    // --- 参数调整 ---
    private float xSpacing = 150f; // 节点横向间距
    private float ySpacing = 200f; // 层级纵向间距
    private float bottomPadding = 100f; // 底部留白，防止第一层贴边

    // 用字典存一下生成的节点物体，方便画线时查找坐标
    private Dictionary<Vector2Int, RectTransform> _spawnedNodes = new Dictionary<Vector2Int, RectTransform>();

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentMap != null)
        {
            DrawMap(GameManager.Instance.CurrentMap);
        }
    }

    void DrawMap(MapData map)
    {
        // 1. 动态计算 Content 的高度
        // 高度 = 层数 * 间距 + 上下留白
        float totalHeight = map.Layers.Count * ySpacing + bottomPadding * 2;

        // 获取 Container 的 RectTransform 并设置尺寸
        RectTransform rectTrans = Container.GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            // 保持宽度不变，修改高度
            rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, totalHeight);
        }

        // 2. 生成节点
        for (int y = 0; y < map.Layers.Count; y++)
        {
            var layer = map.Layers[y];

            // 计算这一层的总宽度，为了让节点居中
            float layerWidth = (layer.Count - 1) * xSpacing;
            float xOffset = -layerWidth / 2f;

            for (int x = 0; x < layer.Count; x++)
            {
                MapNode nodeData = layer[x];

                GameObject obj = Instantiate(NodePrefab, Container);

                // --- 坐标计算 ---
                // X: 居中偏移 + 索引 * 间距
                // Y: 底部留白 + 层索引 * 间距
                float xPos = xOffset + x * xSpacing;
                float yPos = bottomPadding + y * ySpacing;

                obj.transform.localPosition = new Vector3(xPos, yPos, 0);

                // 获取图标
                Sprite icon = IconsConfig != null ? IconsConfig.GetIcon(nodeData.Type) : null;
                obj.GetComponent<MapNodeUI>().Init(nodeData, icon);

                // 记录坐标，用于画线 (Key是网格坐标, Value是UI物体)
                _spawnedNodes.Add(nodeData.Coordinate, obj.GetComponent<RectTransform>());
            }
        }

        // 3. 画线 (遍历所有节点，画出连向父节点的线)
        foreach (var layer in map.Layers)
        {
            foreach (var node in layer)
            {
                // 遍历这个节点的“出边” (Outgoing)，也就是连向下一层的线
                foreach (var targetCoord in node.Outgoing)
                {
                    if (_spawnedNodes.ContainsKey(node.Coordinate) && _spawnedNodes.ContainsKey(targetCoord))
                    {
                        CreateLine(_spawnedNodes[node.Coordinate], _spawnedNodes[targetCoord]);
                    }
                }
            }
        }
    }
    // 画线核心数学逻辑
    void CreateLine(RectTransform startNode, RectTransform endNode)
    {
        // 实例化线
        GameObject lineObj = Instantiate(LinePrefab, Container);

        // 线必须放在节点图层下面 (SetAsFirstSibling)
        lineObj.transform.SetAsFirstSibling();

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        // 1. 位置：设为起点的位置
        lineRect.localPosition = startNode.localPosition;

        // 2. 计算方向向量
        Vector3 dir = endNode.localPosition - startNode.localPosition;

        // 3. 长度：两点间距离
        float distance = dir.magnitude;
        lineRect.sizeDelta = new Vector2(distance, 5f); // 5f 是线的粗细

        // 4. 旋转：计算角度
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
    }
}

