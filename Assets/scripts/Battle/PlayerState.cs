using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 负责在战斗中维护玩家的状态（HP, Mana, 场上单位等）
/// </summary>
public class PlayerState : MonoBehaviour
{
    public int MaxHP = 100;
    public int CurrentHP = 100;
    public int MaxMana = 3;
    public int Mana = 3;

    [Header("UI 引用")]
    public TMP_Text HPText;
    public TMP_Text ManaText;

    [Header("战斗中的单位")]
    public List<Unit> ActiveUnits = new List<Unit>();

    void Start()
    {
        // 从 GameManager 获取全局属性
        if (GameManager.Instance != null)
        {
            MaxHP = GameManager.Instance.PlayerMaxHP;
            CurrentHP = GameManager.Instance.PlayerCurrentHP;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (HPText != null) HPText.text = $"HP: {CurrentHP}/{MaxHP}";
        if (ManaText != null) ManaText.text = $"Energy: {Mana}/{MaxMana}";
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP < 0) CurrentHP = 0;
        UpdateUI();

        if (CurrentHP <= 0 && BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerDefeated();
        }
    }
}
