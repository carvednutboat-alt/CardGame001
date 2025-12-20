using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardDetailPanel : MonoBehaviour
{
    public static CardDetailPanel Instance;

    [Header("UI Components")]
    public GameObject contentPanel; // 用于控制显示/隐藏
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI statsText; // e.g. "ATK: 5 / HP: 10"
    public TextMeshProUGUI descriptionText;
    public Image cardImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 如果你希望在场景切换时保留，可以加上 DontDestroyOnLoad
            // 但考虑到不同场景可能有不同布局，通常每个场景放一个
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Hide(); // 初始隐藏
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Show(CardData data)
    {
        if (data == null) return;
        
        // 确保面板激活
        if (contentPanel != null) contentPanel.SetActive(true);

        if (nameText != null) nameText.text = data.cardName;
        
        string typeStr = "";
        switch (data.kind)
        {
            case CardKind.Unit: typeStr = "单位"; break;
            case CardKind.Spell: typeStr = "法术"; break;
            case CardKind.Evolve: typeStr = "进化"; break;
        }
        if (typeText != null) typeText.text = typeStr;

        if (statsText != null)
        {
            if (data.kind == CardKind.Unit)
            {
                statsText.gameObject.SetActive(true);
                statsText.text = $"攻击: {data.unitAttack} | 生命: {data.unitHealth}";
            }
            else
            {
                statsText.gameObject.SetActive(false);
            }
        }

        if (descriptionText != null) descriptionText.text = data.description;
    }

    public void Hide()
    {
        if (contentPanel != null) contentPanel.SetActive(false);
    }
}
