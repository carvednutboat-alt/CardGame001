using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FieldUnitUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text nameText;
    public TMP_Text statsText;   // ATK / HP
    public TMP_Text statusText;  // 状态栏 (进化/飞行/嘲讽)
    public Button clickButton;

    // === 核心修改：持有数据引用，而不是存一堆临时变量 ===
    private BattleManager _bm;
    private RuntimeUnit _unitData;

    // 对外公开数据（如果需要）
    public RuntimeUnit MyUnit => _unitData;

    // =========================================================
    // 1. 初始化 (参数大大简化)
    // =========================================================
    public void Init(RuntimeUnit unit, BattleManager manager)
    {
        _unitData = unit;
        _bm = manager;

        // 设置名字
        if (nameText != null) nameText.text = unit.Name;

        // 绑定按钮事件
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnUnitClicked);
        }

        // 第一次刷新界面
        UpdateState();
    }

    // =========================================================
    // 2. 状态刷新 (CombatManager 调用的就是这个！)
    // =========================================================
    public void UpdateState()
    {
        if (_unitData == null) return;

        // A. 刷新数值 (ATK / HP)
        if (statsText != null)
        {
            // 注意：这里直接读取对象的实时属性
            statsText.text = $"ATK {_unitData.CurrentAtk} / HP {_unitData.CurrentHp}";
        }

        // B. 刷新状态栏
        if (statusText != null)
        {
            List<string> parts = new List<string>();

            if (_unitData.IsEvolved) parts.Add("<color=yellow>进化</color>");
            if (_unitData.IsFlying) parts.Add("起飞");
            if (_unitData.HasTaunt) parts.Add("嘲讽");
            // 如果有装备
            if (_unitData.Equips != null && _unitData.Equips.Count > 0)
                parts.Add($"装备:{_unitData.Equips.Count}");

            statusText.text = string.Join(" | ", parts);
        }

        // C. 刷新按钮状态 (例如：如果晕眩了可能不能点，或者根据能否攻击设置 interactable)
        SetButtonInteractable(_unitData.CanAttack); // 假设 RuntimeUnit 有 CanAttack
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (clickButton != null)
            clickButton.interactable = interactable;
    }

    private void OnUnitClicked()
    {
        if (_bm != null && _unitData != null)
        {
            // 通知管理器：这个 ID 的单位被点了
            _bm.OnFieldUnitClicked(_unitData.Id);
        }
    }
}