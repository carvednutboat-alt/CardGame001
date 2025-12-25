using UnityEngine;
using System.Collections.Generic;

public class DeckSelectionUI : MonoBehaviour
{
    private bool _isActive = false;
    private DevCardLoader.DevDeckType _selectedType;
    private List<CardData> _previewCards = new List<CardData>();

    public void Show()
    {
        _isActive = true;
        // Default selection
        SelectDeck(DevCardLoader.DevDeckType.ThousandWeapons);
    }

    public void Hide()
    {
        _isActive = false;
    }

    private void SelectDeck(DevCardLoader.DevDeckType type)
    {
        _selectedType = type;
        
        // Load preview
        _previewCards.Clear();
        // Hack: Use GameManager to load into a Temp list?
        // Or refactor DevCardLoader to return a list?
        // DevCardLoader adds to GameManager.MasterDeck.
        // We can backup MasterDeck, Load, Copy, Restore?
        // Or just let user see text description for now.
        // Better: Modify DevCardLoader to *Get* cards? 
        // For now, let's just assume we display the name.
    }

    private void OnGUI()
    {
        if (!_isActive) return;

        float width = 600;
        float height = 400;
        float x = (Screen.width - width) / 2;
        float y = (Screen.height - height) / 2;

        GUI.Box(new Rect(x, y, width, height), "选择初始卡组");

        GUILayout.BeginArea(new Rect(x + 20, y + 40, width - 40, height - 60));

        GUILayout.BeginHorizontal();
        
        // Left Column: Deck Buttons
        GUILayout.BeginVertical(GUILayout.Width(200));
        if (GUILayout.Button("千具武卡组 (Thousand Weapons)", GUILayout.Height(50)))
        {
            SelectDeck(DevCardLoader.DevDeckType.ThousandWeapons);
        }
        if (GUILayout.Button("蒸汽机器人卡组 (Robot)", GUILayout.Height(50)))
        {
            SelectDeck(DevCardLoader.DevDeckType.Robot);
        }
        // [NEW] Custom Button
        if (GUILayout.Button("自定义 (Inspector)", GUILayout.Height(50)))
        {
            // Signal a custom selection by using a dummy value or handling it directly?
            // Let's use a flag or invoke directly? invoke directly is safer but we want Preview.
            _selectedType = (DevCardLoader.DevDeckType)(-1); // -1 for Custom
        }
        GUILayout.EndVertical();

        // Right Column: Preview (Text description mostly since we can't easily render cards in OnGUI)
        GUILayout.BeginVertical();
        GUILayout.Label($"当前选择: {_selectedType}");
        GUILayout.Label("卡组包含:");
        
        // Manual Description
        if ((int)_selectedType == -1) // Custom
        {
             GUILayout.Label($"自定义卡组 (Inspector)\n包含 {GameManager.Instance.defaultStarterDeck.Count} 张卡牌。");
        }
        else if (_selectedType == DevCardLoader.DevDeckType.ThousandWeapons)
        {
            GUILayout.Label("- 千具武·宗师 (Commander 4/4)\n- 3x 侍卫 (2/3 Deathrattle)\n- 3x 新兵 (0/1 Deathrattle)\n- 3x 突击者 (2/1 OnEquip)\n- 2x 试炼之剑");
        }
        else
        {
            GUILayout.Label("- 蒸汽机器人·指挥官 (Commander 3/5 Overload)\n- 3x 蒸汽哨兵 (0/2 Taunt Aura)\n- 3x 蒸汽收割者 (2/1 OnKill Overload)\n- 3x 过载增幅器 (1/1 Aura Boost)\n- 3x 过载模式 (Spell)\n- 3x 二重过载 (Spell)\n- 3x 极限运转 (Spell)\n- 3x 过载释放 (Spell New!)");
        }

        GUILayout.Space(20);
        if (GUILayout.Button("开始游戏 (Start Game)", GUILayout.Height(40)))
        {
            if (GameManager.Instance != null)
            {
                if ((int)_selectedType == -1)
                {
                    // Custom Inspector Deck
                    GameManager.Instance.StartNewGame(GameManager.Instance.defaultStarterDeck);
                }
                else
                {
                    GameManager.Instance.StartNewGame(_selectedType);
                }
            }
        }
        
        GUILayout.Space(10);
        if (GUILayout.Button("取消 (Cancel)"))
        {
            Hide();
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
}
