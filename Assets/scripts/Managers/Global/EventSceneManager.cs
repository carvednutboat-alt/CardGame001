using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class EventSceneManager : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_Text TitleText;
    public TMP_Text DescText;
    public Image EventImage;
    public Transform OptionsContainer; // 按钮的父节点
    public GameObject OptionButtonPrefab; // 按钮预制体

    [Header("Test")]
    public EventProfile TestProfile; // 调试用

    private EventProfile _currentProfile;

    void Start()
    {
        // 如果是从大地图进入，GameManager应该持有当前的 EventProfile
        if (GameManager.Instance != null && GameManager.Instance.CurrentEventProfile != null)
        {
            LoadEvent(GameManager.Instance.CurrentEventProfile);
        }
        else if (TestProfile != null)
        {
            LoadEvent(TestProfile); // 测试用
        }
    }

    public void LoadEvent(EventProfile profile)
    {
        _currentProfile = profile;
        TitleText.text = profile.Title;
        DescText.text = profile.Description;
        if (profile.EventImage != null) EventImage.sprite = profile.EventImage;
        else EventImage.gameObject.SetActive(false);

        // 生成按钮
        foreach (Transform child in OptionsContainer) Destroy(child.gameObject);

        foreach (var opt in profile.Options)
        {
            GameObject btnObj = Instantiate(OptionButtonPrefab, OptionsContainer);
            var btnText = btnObj.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = opt.OptionText;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnOptionSelected(opt));
        }
    }

    void OnOptionSelected(EventOptionData option)
    {
        // 1. 执行效果
        ApplyEffect(option);

        // 2. 显示结果并离开
        DescText.text = option.ResultText;

        // 禁用所有按钮
        foreach (Button btn in OptionsContainer.GetComponentsInChildren<Button>())
            btn.interactable = false;

        Invoke(nameof(ReturnToMap), 2.0f);
    }

void ApplyEffect(EventOptionData opt)
    {
        if (GameManager.Instance == null) return;

        // 注意：EffectBase 需要 BattleManager 参数，而事件场景中没有战斗上下文
        // 所以我们目前使用旧版枚举系统来处理事件效果
        // TODO: 如果需要支持复杂的事件效果，可以创建一个不需要 BattleManager 的 EventEffectBase 基类
        
        switch (opt.Effect)
        {
            case EventEffectType.Heal:
                GameManager.Instance.HealPlayer(opt.Value);
                break;
            case EventEffectType.Damage:
                GameManager.Instance.DamagePlayer(opt.Value);
                break;
            case EventEffectType.GainGold:
                GameManager.Instance.AddGold(opt.Value);
                break;
            case EventEffectType.GainCard:
                if (opt.TargetCard != null)
                    GameManager.Instance.AddCardToDeck(opt.TargetCard);
                break;
        }
    }

    void ReturnToMap()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnNodeCompleted();
        else
            SceneManager.LoadScene("MapScene"); // Fallback
    }
}
