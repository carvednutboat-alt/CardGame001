using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    
    public CardData Data;  // 卡牌数据引用
    public int CurrentAttack;  // 当前攻击力
public int maxHp = 30;
    public TMP_Text hpText;

    // �� _currentHp ��Ϊ public get, private set �����ⲿ��ȡ����������ֱ���޸�
    public int CurrentHp { get; private set; }

    private void Awake()
    {
        // Ĭ�ϳ�ʼ������ֹ��������ʱ����
        CurrentHp = maxHp;
        UpdateHpUI();
    }

    // --- ������ר�����ڴ�ȫ�����ݳ�ʼ��Ѫ�� ---
    public void InitData(int current, int max)
    {
        maxHp = max;
        CurrentHp = current;

        // �ݴ��������ܳ������ޣ�Ҳ����С��0
        if (CurrentHp > maxHp) CurrentHp = maxHp;
        if (CurrentHp < 0) CurrentHp = 0;

        UpdateHpUI();
    }

public void Init(CardData data, bool isPlayer)
    {
        Data = data;
        maxHp = data.maxHp;
        CurrentHp = maxHp;
        CurrentAttack = data.unitAttack;
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