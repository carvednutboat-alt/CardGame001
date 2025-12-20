using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public Button Btn;
    public Image BackgroundCircle; // 拖入圆圈底图 (Button身上那个)
    public Image TypeIcon;         // 拖入里面的小图标 (刚才新建的子物体)

    [Header("呼吸动画参数")]
    public float PulseSpeed = 5f;  // 呼吸速度
    public float PulseScale = 0.1f; // 缩放幅度 (1.0 -> 1.1)

    private MapNode _data;
    private bool _isBreathing = false; // 开关

    public void Init(MapNode data, Sprite typeIcon)
    {
        _data = data;

        // 设置内容图标
        if (typeIcon != null)
        {
            TypeIcon.sprite = typeIcon;
            TypeIcon.gameObject.SetActive(true);
        }
        else
        {
            TypeIcon.gameObject.SetActive(false); // 如果没配图就关掉
        }

        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(OnClick);

        UpdateVisuals();
    }

    // --- 核心修改：每帧检测是否需要呼吸 ---
    void Update()
    {
        if (_isBreathing)
        {
            // 利用 Time.time 和 Sin 函数制作平滑的波形
            // 结果在 1.0 到 (1.0 + PulseScale) 之间波动
            float scale = 1.0f + Mathf.Abs(Mathf.Sin(Time.time * PulseSpeed)) * PulseScale;

            // 应用缩放
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void UpdateVisuals()
    {
        // 每次更新状态前，先重置一下缩放，防止卡在变大的状态
        transform.localScale = Vector3.one;
        _isBreathing = false; // 默认不呼吸

        if (_data == null) return;

        switch (_data.Status)
        {
            case NodeStatus.Locked:
                Btn.interactable = false;
                SetColor(new Color(0.5f, 0.5f, 0.5f, 0.3f)); // 灰色半透明
                break;

            case NodeStatus.Attainable:
                Btn.interactable = true;
                SetColor(Color.white); // 亮白色

                // === 关键：只有可到达的节点开启呼吸 ===
                _isBreathing = true;
                // 你也可以给圆圈换个显眼的颜色，比如青色
                BackgroundCircle.color = Color.cyan;
                break;

            case NodeStatus.Visited:
                Btn.interactable = false;
                SetColor(new Color(0.6f, 0.6f, 0.6f, 1f)); // 暗灰色
                break;

            case NodeStatus.Current:
                Btn.interactable = false;
                SetColor(Color.white);
                BackgroundCircle.color = Color.yellow; // 当前位置给个黄色
                // 当前位置也可以呼吸，或者静态高亮，看你喜好
                // _isBreathing = true; 
                break;
        }
    }

    // 辅助工具：统一设置颜色，方便管理
    void SetColor(Color c)
    {
        BackgroundCircle.color = c;
        // 如果想让里面的图标保持原色，下面这行可以去掉；
        // 如果想让图标跟着变暗，就保留。
        TypeIcon.color = new Color(1, 1, 1, c.a);
    }

    void OnClick()
    {
        GameManager.Instance.SelectNode(_data);
    }
}