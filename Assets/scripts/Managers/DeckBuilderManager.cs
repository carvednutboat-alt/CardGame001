using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq; // 引入 Linq 方便计数

public class DeckBuilderManager : MonoBehaviour
{
    [Header("左侧池子面板")]
    public Transform UnitPoolPanel;
    public Transform CardPoolPanel;
    [Header("右侧当前构筑面板")]
    public Transform CurrentUnitsPanel;
    public Transform CurrentDeckPanel;
    [Header("UI 预制体")]
    public Button CardButtonPrefab;
    [Header("底部 UI")]
    public Button BackButton;
    public TMP_Text DeckInfoText;

    private PlayerCollection _pc;

    private void Start()
    {
        _pc = PlayerCollection.Instance;
        if (_pc == null) return;

        // 注意：不要在这里 Clear OwnedUnits/OwnedCards，也不要 Clear Current
        // 我们显示的逻辑基于 PlayerCollection 的现有状态

        RefreshAll();

        if (BackButton != null)
        {
            BackButton.onClick.RemoveAllListeners();
            BackButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void RefreshAll()
    {
        RefreshUnitPool();
        RefreshCardPool();
        RefreshCurrentUnits();
        RefreshCurrentDeck();
        UpdateDeckInfo();
    }

    private void ClearChildren(Transform root)
    {
        foreach (Transform child in root) Destroy(child.gameObject);
    }

    // ==================== 左上：可选 Unit 池 (显示剩余可用) ====================
    private void RefreshUnitPool()
    {
        ClearChildren(UnitPoolPanel);
        if (_pc.OwnedUnits == null) return;

        // 算法：创建一个临时列表 = 拥有的 - 当前已选的
        // 这样左边只显示“还没放进卡组的怪”
        List<CardData> availableUnits = new List<CardData>(_pc.OwnedUnits);
        foreach (var used in _pc.CurrentUnits)
        {
            availableUnits.Remove(used); // 移除掉已经用掉的引用
        }

        foreach (var data in availableUnits)
        {
            Button btn = Instantiate(CardButtonPrefab, UnitPoolPanel);
            btn.GetComponentInChildren<TMP_Text>().text = data.cardName;

            // 点击左边 = 添加到右边
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentUnits.Add(data);
                RefreshAll();
            });
        }
    }

    // ==================== 左下：可选卡池 (显示剩余可用) ====================
    private void RefreshCardPool()
    {
        ClearChildren(CardPoolPanel);
        if (_pc.OwnedCards == null) return;

        // 算法：剩余可用 = 拥有 - 当前已选
        List<CardData> availableCards = new List<CardData>(_pc.OwnedCards);
        foreach (var used in _pc.CurrentDeck)
        {
            availableCards.Remove(used);
        }

        foreach (var data in availableCards)
        {
            Button btn = Instantiate(CardButtonPrefab, CardPoolPanel);
            btn.GetComponentInChildren<TMP_Text>().text = data.cardName;

            // 点击左边 = 添加到右边
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentDeck.Add(data);
                RefreshAll();
            });
        }
    }

    // ==================== 右上：当前 Unit ====================
    private void RefreshCurrentUnits()
    {
        ClearChildren(CurrentUnitsPanel);
        if (_pc.CurrentUnits == null) return;

        foreach (var data in _pc.CurrentUnits)
        {
            Button btn = Instantiate(CardButtonPrefab, CurrentUnitsPanel);
            btn.GetComponentInChildren<TMP_Text>().text = data.cardName;

            // 点击右边 = 移除 (退回池子)
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentUnits.Remove(data);
                RefreshAll();
            });
        }
    }

    // ==================== 右下：当前卡组 ====================
    private void RefreshCurrentDeck()
    {
        ClearChildren(CurrentDeckPanel);
        if (_pc.CurrentDeck == null) return;

        foreach (var data in _pc.CurrentDeck)
        {
            Button btn = Instantiate(CardButtonPrefab, CurrentDeckPanel);
            btn.GetComponentInChildren<TMP_Text>().text = data.cardName;

            // 点击右边 = 移除 (退回池子)
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentDeck.Remove(data);
                RefreshAll();
            });
        }
    }

    // ... UpdateDeckInfo 和 OnBackButtonClicked 保持不变 ...
    // (记得保留你之前的 OnBackButtonClicked 同步回 GameManager 的逻辑)
    private void UpdateDeckInfo()
    {
        if (DeckInfoText == null) return;
        int unitCount = _pc.CurrentUnits != null ? _pc.CurrentUnits.Count : 0;
        int deckCount = _pc.CurrentDeck != null ? _pc.CurrentDeck.Count : 0;
        DeckInfoText.text = $"单位: {unitCount} | 卡牌: {deckCount}";
    }

    private void OnBackButtonClicked()
    {
        // ... (保持你之前的代码)
        // 记得：这里是把 Current 同步回 GameManager.MasterDeck
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MasterDeck.Clear();
            if (_pc.CurrentUnits != null) GameManager.Instance.MasterDeck.AddRange(_pc.CurrentUnits);
            if (_pc.CurrentDeck != null) GameManager.Instance.MasterDeck.AddRange(_pc.CurrentDeck);
        }
        SceneManager.LoadScene("MapScene");
    }
}