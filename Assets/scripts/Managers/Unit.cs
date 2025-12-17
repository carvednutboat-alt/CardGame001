using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    public int maxHp = 30;
    public TMP_Text hpText;

    // 把 _currentHp 改为 public get, private set 方便外部读取，但不允许直接修改
    public int CurrentHp { get; private set; }

    private void Awake()
    {
        // 默认初始化，防止单独测试时报错
        CurrentHp = maxHp;
        UpdateHpUI();
    }

    // --- 新增：专门用于从全局数据初始化血量 ---
    public void InitData(int current, int max)
    {
        maxHp = max;
        CurrentHp = current;

        // 容错处理：不能超过上限，也不能小于0
        if (CurrentHp > maxHp) CurrentHp = maxHp;
        if (CurrentHp < 0) CurrentHp = 0;

        UpdateHpUI();
    }
    // ---------------------------------------

    public void ResetHp()
    {
        CurrentHp = maxHp;
        UpdateHpUI();
    }

    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
        if (CurrentHp < 0) CurrentHp = 0;
        UpdateHpUI();
    }

    public void Heal(int amount)
    {
        CurrentHp += amount;
        if (CurrentHp > maxHp) CurrentHp = maxHp;
        UpdateHpUI();
    }

    public bool IsDead()
    {
        return CurrentHp <= 0;
    }

    private void UpdateHpUI()
    {
        if (hpText != null)
        {
            hpText.text = $"{CurrentHp}/{maxHp}";
        }
    }
}