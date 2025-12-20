using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引用 TextMeshPro

public class EnemyUnitUI : MonoBehaviour
{
    [Header("UI 组件引用")]
    public Button ClickButton;
    public TMP_Text NameText;
    public TMP_Text HPText;
    public TMP_Text AttackText;
    public Image HPBar; // 如果你想做血条的话

    // 内部引用
    private BattleManager _bm;
    private RuntimeUnit _unitData;

    // 公开属性：让外部能获取这个 UI 绑定的数据
    public RuntimeUnit MyUnit => _unitData;

    // ==========================================
    // 初始化
    // ==========================================
    public void Init(RuntimeUnit unit, BattleManager bm)
    {
        _unitData = unit;
        _bm = bm;

        // 1. 设置名字
        if (NameText != null) NameText.text = unit.Name;
        Debug.Log($"怪兽{NameText.text}{unit.Id}被初始化");
        // 2. 绑定按钮事件
        if (ClickButton != null)
        {
            Debug.Log($"怪兽{NameText.text}{unit.Id}的按键正常");
            ClickButton.onClick.RemoveAllListeners();
            ClickButton.onClick.AddListener(OnClicked);
        }

        // 3. 刷新初始状态
        UpdateHP();
        UpdateAttack();
    }

    // ==========================================
    // 状态刷新
    // ==========================================
    public void UpdateHP()
    {
        if (_unitData == null) return;

        // 更新文字
        if (HPText != null)
        {
            HPText.text = $"{_unitData.CurrentHp}/{_unitData.MaxHp}";
        }

        // 更新血条 (如果有)
        if (HPBar != null && _unitData.MaxHp > 0)
        {
            HPBar.fillAmount = (float)_unitData.CurrentHp / _unitData.MaxHp;
        }
    }

    public void UpdateAttack()
    {
        if (_unitData == null) return;
        if (AttackText != null)
        {
            AttackText.text = $"{_unitData.CurrentAtk}";
        }
    }

    // ==========================================
    // 交互事件
    // ==========================================
    private void OnClicked()
    {
        if (_bm != null)
        {
            // 通知 BattleManager：我（这个具体的敌人UI）被点了
            // 注意：你需要去 BattleManager 添加 OnEnemyClicked(EnemyUnitUI ui) 的重载方法
            _bm.OnEnemyClicked(this);
        }
    }
}