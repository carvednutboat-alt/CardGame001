using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHPBar : MonoBehaviour
{
    [Header("UI Components")]
    public Slider HPSlider;
    public TMP_Text NameText;
    public TMP_Text HPText;
    public Image BossIcon; // Optional

    private EnemyManager.RuntimeEnemy _boss;

    public void Init(EnemyManager.RuntimeEnemy boss)
    {
        _boss = boss;
        if (_boss == null || _boss.UnitData == null) return;

        if (NameText != null) NameText.text = _boss.UnitData.Name;
        
        UpdateHP();
        gameObject.SetActive(true);
    }

    public void UpdateHP()
    {
        if (_boss == null || _boss.UnitData == null) return;

        int current = _boss.UnitData.CurrentHp;
        int max = _boss.UnitData.MaxHp;

        if (HPSlider != null)
        {
            HPSlider.maxValue = max;
            HPSlider.value = current;
        }

        if (HPText != null)
        {
            HPText.text = $"{current} / {max}";
        }
    }

    void Update()
    {
        // Optional: Smooth slider or periodic check
        UpdateHP(); 
    }
}
