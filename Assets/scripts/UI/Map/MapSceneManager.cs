using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // 必须引用

public class MapSceneManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public Transform Container;

    // --- 参数调整 ---
    private float xSpacing = 150f; // 节点横向间距
    private float ySpacing = 200f; // 层级纵向间距
    private float bottomPadding = 100f; // 底部留白，防止第一层贴边

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

                // 初始化 UI
                obj.GetComponent<MapNodeUI>().Init(nodeData);
            }
        }
    }
}