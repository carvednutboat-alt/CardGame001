using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // 用于 List

public class FieldUnitUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text nameText;
    public TMP_Text statsText;   // ATK / HP
    public TMP_Text statusText;  // 状态栏
    public Button clickButton;

    private BattleManager _bm;
    private int _unitId;

    // === 核心修改：参数列表对应 UnitManager.cs 里的 TrySummonUnit ===
    public void Init(
        BattleManager manager,
        int unitId,
        string unitName,
        int attack,
        int hp,
        bool evolved,
        int equipCount,
        bool canAttack,
        bool isFlying,
        bool hasTaunt
    )
    {
        _bm = manager;
        _unitId = unitId;

        if (nameText != null) nameText.text = unitName;

        // 第一次初始化时刷新状态
        UpdateStats(attack, hp, evolved, equipCount, isFlying, hasTaunt);
        SetButtonInteractable(canAttack);

        // 绑定事件
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnUnitClicked);
        }
    }

    public void UpdateStats(
        int attack,
        int hp,
        bool evolved,
        int equipCount,
        bool isFlying,
        bool hasTaunt
    )
    {
        if (statsText != null)
            statsText.text = $"ATK {attack} / HP {hp}";

        if (statusText != null)
        {
            List<string> parts = new List<string>();

            if (evolved) parts.Add("<color=yellow>进化</color>");
            if (isFlying) parts.Add("起飞");
            if (hasTaunt) parts.Add("嘲讽");
            if (equipCount > 0) parts.Add($"装备:{equipCount}");

            statusText.text = string.Join(" | ", parts);
        }
    }

    // 设置按钮是否可点（攻击/选目标）
    public void SetButtonInteractable(bool interactable)
    {
        if (clickButton != null)
            clickButton.interactable = interactable;
    }

    private void OnUnitClicked()
    {
        if (_bm != null)
        {
            // 通知管理器：场上 ID 为 _unitId 的单位被点了
            _bm.OnFieldUnitClicked(_unitId);
        }
    }
}