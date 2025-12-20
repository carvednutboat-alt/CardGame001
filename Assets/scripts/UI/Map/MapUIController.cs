using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MapUIController : MonoBehaviour
{
    public GameObject OptionsPanel;
    public Button OptionsButton;
    public Button SaveButton;
    public Button ReturnTitleButton;
    public Button CloseButton;

    private void Start()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        if (OptionsButton != null)
        {
            OptionsButton.onClick.AddListener(() => ToggleOptions(true));
            SetButtonText(OptionsButton, "选项");
        }

        if (SaveButton != null)
        {
            SaveButton.onClick.AddListener(OnSaveButtonClicked);
            SetButtonText(SaveButton, "保存游戏");
        }

        if (ReturnTitleButton != null)
        {
            ReturnTitleButton.onClick.AddListener(OnReturnTitleClicked);
            SetButtonText(ReturnTitleButton, "返回主页");
        }

        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(() => ToggleOptions(false));
            SetButtonText(CloseButton, "关闭");
        }

        if (OptionsPanel != null) OptionsPanel.SetActive(false);
    }

    private void SetButtonText(Button btn, string text)
    {
        var tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null) tmp.text = text;
    }

    public void OpenDeckBuilder()
    {
        // ... (保持之前的卡组同步逻辑)
        if (GameManager.Instance != null && PlayerCollection.Instance != null)
        {
            PlayerCollection.Instance.CurrentUnits.Clear();
            PlayerCollection.Instance.CurrentDeck.Clear();
            foreach (var card in GameManager.Instance.MasterDeck)
            {
                if (card.kind == CardKind.Unit) PlayerCollection.Instance.CurrentUnits.Add(card);
                else PlayerCollection.Instance.CurrentDeck.Add(card);
            }
        }
        SceneManager.LoadScene("DeckBuilderScene");
    }

    // === 地图菜单功能 ===

    public void ToggleOptions(bool show)
    {
        if (OptionsPanel != null) OptionsPanel.SetActive(show);
    }

    public void OnSaveButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            // 以后可以在这里弹个“已保存”的小提示
            Debug.Log("地图存档已完成");
        }
    }

    public void OnReturnTitleClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTitle();
        }
    }
}