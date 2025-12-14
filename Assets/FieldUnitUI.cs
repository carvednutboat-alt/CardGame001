using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldUnitUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text statsText;   // ATK / HP
    public TMP_Text statusText;  // 进化 / 装备数
    public Button attackButton;

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
        bool canAttack
    )
    {
        _manager = manager;
        _unitId = unitId;

        if (nameText != null)  nameText.text = unitName;
        UpdateStats(attack, hp, evolved, equipCount);
        SetCanAttack(canAttack);

        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(OnAttackClicked);
        }
    }

    public void UpdateStats(int attack, int hp, bool evolved, int equipCount)
    {
        if (statsText != null)
            statsText.text = $"ATK {attack} / HP {hp}";

        if (statusText != null)
        {
            string evo = evolved ? "进化" : "";
            statusText.text = $"{evo} 装备:{equipCount}";
        }
    }

    public void SetCanAttack(bool canAttack)
    {
        if (attackButton != null)
            attackButton.interactable = canAttack;
    }

    private void OnAttackClicked()
    {
        if (_manager != null)
            _manager.OnFieldUnitAttack(_unitId);
    }
}
