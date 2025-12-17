using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeckBuilderManager : MonoBehaviour
{
    [Header("左侧池子面板（ScrollView 的 Content）")]
    public Transform UnitPoolPanel;      // 可选角色池（左上）
    public Transform CardPoolPanel;      // 可选卡池（左下）

    [Header("右侧当前构筑面板（ScrollView 的 Content）")]
    public Transform CurrentUnitsPanel;  // 当前选中的 Unit（右上）
    public Transform CurrentDeckPanel;   // 当前选中的卡组（右下）

    [Header("UI 预制体")]
    public Button CardButtonPrefab;      // 小按钮预制体（里面有 TMP_Text）

    [Header("底部 UI")]
    // public Button StartBattleButton;
    public Button BackButton;           // <--- 新增：返回按钮
    public TMP_Text DeckInfoText;

    private PlayerCollection _pc;

    private void Start()
    {
        _pc = PlayerCollection.Instance;
        if (_pc == null)
        {
            Debug.LogError("DeckBuilderManager: PlayerCollection.Instance 为 null，请确认场景里有带 PlayerCollection 的 GameObject。");
            enabled = false;
            return;
        }

        // 为了保证每次进构筑界面都是“全新构筑”，这里清空当前构筑
        /*
        if (_pc.CurrentUnits == null) _pc.CurrentUnits = new List<CardData>();
        if (_pc.CurrentDeck  == null) _pc.CurrentDeck  = new List<CardData>();
        _pc.CurrentUnits.Clear();
        _pc.CurrentDeck.Clear();
        */

        RefreshAll();

        if(BackButton != null)
        {
            BackButton.onClick.RemoveAllListeners();
            BackButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    // ==================== 刷新整体 ====================

    private void RefreshAll()
    {
        RefreshUnitPool();       // 左上池
        RefreshCardPool();       // 左下池
        RefreshCurrentUnits();   // 右上当前 Unit
        RefreshCurrentDeck();    // 右下当前卡组
        UpdateDeckInfo();        // 底部文字
    }

    private void ClearChildren(Transform root)
    {
        if (root == null) return;
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }

    // ==================== 左上：可选 Unit 池 ====================

    private void RefreshUnitPool()
    {
        ClearChildren(UnitPoolPanel);

        if (_pc.OwnedUnits == null) return;

        foreach (var data in _pc.OwnedUnits)
        {
            if (data == null) continue;

            Button btn = Instantiate(CardButtonPrefab, UnitPoolPanel);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = data.cardName;

            // 点击左上某个 Unit：在当前构筑里“切换存在 / 不存在”
            btn.onClick.AddListener(() =>
            {
                ToggleUnitInCurrent(data);
                RefreshAll();
            });
        }
    }

    private void ToggleUnitInCurrent(CardData unitCard)
    {
        if (_pc.CurrentUnits.Contains(unitCard))
            _pc.CurrentUnits.Remove(unitCard);
        else
            _pc.CurrentUnits.Add(unitCard);
    }

    // ==================== 左下：可选普通卡池 ====================

    private void RefreshCardPool()
    {
        ClearChildren(CardPoolPanel);

        if (_pc.OwnedCards == null) return;

        foreach (var data in _pc.OwnedCards)
        {
            if (data == null) continue;

            Button btn = Instantiate(CardButtonPrefab, CardPoolPanel);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = data.cardName;

            // 点击左下某张卡：在当前卡组里“切换存在 / 不存在”
            btn.onClick.AddListener(() =>
            {
                ToggleCardInCurrent(data);
                RefreshAll();
            });
        }
    }

    private void ToggleCardInCurrent(CardData card)
    {
        if (_pc.CurrentDeck.Contains(card))
            _pc.CurrentDeck.Remove(card);
        else
            _pc.CurrentDeck.Add(card);
    }

    // ==================== 右上：当前 Unit 显示 ====================

    private void RefreshCurrentUnits()
    {
        ClearChildren(CurrentUnitsPanel);

        if (_pc.CurrentUnits == null) return;

        foreach (var data in _pc.CurrentUnits)
        {
            if (data == null) continue;

            Button btn = Instantiate(CardButtonPrefab, CurrentUnitsPanel);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = data.cardName;

            // 在右上点击某个已选 Unit = 从当前构筑里移除这个 Unit
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentUnits.Remove(data);
                RefreshAll();
            });
        }
    }

    // ==================== 右下：当前卡组显示 ====================

    private void RefreshCurrentDeck()
    {
        ClearChildren(CurrentDeckPanel);

        if (_pc.CurrentDeck == null) return;

        foreach (var data in _pc.CurrentDeck)
        {
            if (data == null) continue;

            Button btn = Instantiate(CardButtonPrefab, CurrentDeckPanel);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = data.cardName;

            // 在右下点击某张已选卡 = 从当前卡组里移除这张卡
            btn.onClick.AddListener(() =>
            {
                _pc.CurrentDeck.Remove(data);
                RefreshAll();
            });
        }
    }

    // ==================== 底部信息 & 开始战斗 ====================

    private void UpdateDeckInfo()
    {
        if (DeckInfoText == null) return;

        int unitCount = _pc.CurrentUnits != null ? _pc.CurrentUnits.Count : 0;
        int deckCount = _pc.CurrentDeck  != null ? _pc.CurrentDeck.Count  : 0;

        DeckInfoText.text = $"单位数量: {unitCount}    卡牌数量: {deckCount}（20–40 张之间才可开始战斗）";
    }

    // ==================== 核心修改：返回大地图 ====================
    private void OnBackButtonClicked()
    {
        // 1. 合法性检查 (比如卡组太少不让退，或者提示警告)
        int deckCount = _pc.CurrentDeck != null ? _pc.CurrentDeck.Count : 0;
        if (deckCount < 5)
        {
            // 这里可以做一个弹窗提示，暂时用Log代替
            Debug.LogWarning("卡组太少，建议多带点牌！");
            // return; // 如果你想强制限制，就取消注释这行
        }

        // 2. 【至关重要】数据同步！
        // 把 PlayerCollection (构筑界面) 的结果，同步给 GameManager (战斗读取的地方)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MasterDeck.Clear();

            // 把选中的怪兽加入总卡组
            if (_pc.CurrentUnits != null)
                GameManager.Instance.MasterDeck.AddRange(_pc.CurrentUnits);

            // 把选中的法术加入总卡组
            if (_pc.CurrentDeck != null)
                GameManager.Instance.MasterDeck.AddRange(_pc.CurrentDeck);

            Debug.Log($"数据已同步，MasterDeck 总数: {GameManager.Instance.MasterDeck.Count}");
        }

        // 3. 返回地图场景
        SceneManager.LoadScene("MapScene");
    }
}
