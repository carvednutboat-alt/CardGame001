using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public Button Btn;
    public Image Icon;
    public Image Outline; // 用于显示当前选中或可选状态

    private MapNode _data;

    public void Init(MapNode data)
    {
        _data = data;
        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(OnClick);

        // 设置颜色状态
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // 简单通过颜色区分类型
        switch (_data.Type)
        {
            case NodeType.MinorEnemy: Icon.color = Color.red; break;
            case NodeType.EliteEnemy: Icon.color = new Color(0.5f, 0, 0); break;
            case NodeType.Rest: Icon.color = Color.green; break;
            case NodeType.Store: Icon.color = Color.yellow; break;
            case NodeType.Boss: Icon.color = Color.black; break;
        }

        // 状态表现
        if (_data.Status == NodeStatus.Locked)
        {
            Btn.interactable = false;
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0.3f); // 变暗
        }
        else if (_data.Status == NodeStatus.Attainable)
        {
            Btn.interactable = true;
            Outline.color = Color.cyan; // 可选高亮
        }
        else if (_data.Status == NodeStatus.Visited)
        {
            Btn.interactable = false;
            Icon.color = Color.gray;
        }
        else if (_data.Status == NodeStatus.Current)
        {
            Btn.interactable = false;
            Outline.color = Color.white;
        }
    }

    void OnClick()
    {
        GameManager.Instance.SelectNode(_data);
    }
}