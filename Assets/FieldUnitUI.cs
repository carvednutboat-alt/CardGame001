using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldUnitUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text statsText;   // ATK / HP
    public TMP_Text statusText;  // 进化 / 装备数
    public Button clickButton;  // 更改了名字，现在不止用作攻击了

    private BattleManager _manager;
    private int _unitId;

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
        _manager = manager;
        _unitId = unitId;

        if (nameText != null)  nameText.text = unitName;
        UpdateStats(attack, hp, evolved, equipCount, isFlying, hasTaunt);
        SetButtonInteractable(canAttack);

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
            var parts = new System.Collections.Generic.List<string>();

            if (evolved)
                parts.Add("进化");
            if (isFlying)
                parts.Add("起飞");
            if (hasTaunt)
                parts.Add("嘲讽");

            parts.Add($"装备:{equipCount}");

            statusText.text = string.Join(" / ", parts);
        }
    }

    // === 修改点 1：把 SetCanAttack 改名为更通用的 SetButtonInteractable ===
    public void SetButtonInteractable(bool interactable)
    {
        if (clickButton != null)
            clickButton.interactable = interactable;
    }

    // === 修改点 2：点击事件 ===
    private void OnUnitClicked()
    {
        if (_manager != null)
        {
            _manager.OnFieldUnitClicked(_unitId);
        }
    }
}
