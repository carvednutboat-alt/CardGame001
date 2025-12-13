using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHp = 30;
    public int currentHp;

    public TMP_Text hpText;

    private void Awake()
    {
        currentHp = maxHp;
        UpdateUI();
    }

    public void ResetHp()
    {
        currentHp = maxHp;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp < 0) currentHp = 0;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        UpdateUI();
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    private void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = $"{unitName} HP: {currentHp}/{maxHp}";
        }
    }
}
