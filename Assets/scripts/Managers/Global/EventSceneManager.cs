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
        // 这里假设 GameManager 有一个 CurrentEventProfile 字段 (稍后添加)
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

        // 2. 显示结果并离开 (这里简单处理：直接显示结果然后几秒后返回地图)
        DescText.text = option.ResultText;

        // 禁用所有按钮
        foreach (Button btn in OptionsContainer.GetComponentsInChildren<Button>())
            btn.interactable = false;

        Invoke(nameof(ReturnToMap), 2.0f);
    }

    void ApplyEffect(EventOptionData opt)
    {
        if (GameManager.Instance == null) return;

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
                // ... 其他效果
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