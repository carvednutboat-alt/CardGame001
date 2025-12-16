using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    public int maxHp = 30;
    public TMP_Text hpText;

    private int _currentHp;

    private void Awake()
    {
        _currentHp = maxHp;
        UpdateHpUI();
    }

    public void ResetHp()
    {
        _currentHp = maxHp;
        UpdateHpUI();
    }

    public void TakeDamage(int amount)
    {
        _currentHp -= amount;
        if (_currentHp < 0) _currentHp = 0;
        UpdateHpUI();
    }

    public void Heal(int amount)
    {
        _currentHp += amount;
        if (_currentHp > maxHp) _currentHp = maxHp;
        UpdateHpUI();
    }

    public bool IsDead()
    {
        return _currentHp <= 0;
    }

    public int CurrentHp => _currentHp;

    private void UpdateHpUI()
    {
        if (hpText != null)
        {
            hpText.text = $"{_currentHp}/{maxHp}";
        }
    }
}
