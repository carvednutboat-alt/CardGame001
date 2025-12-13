using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text extraText;
    public Button button;

    public CardData Data { get; private set; }

    private BattleManager _battleManager;

    public void Init(CardData data, BattleManager manager)
    {
        Data = data;
        _battleManager = manager;

        if (nameText != null) nameText.text = data.cardName;
        if (descText != null) descText.text = data.description;
        if (extraText != null) extraText.text = $"{data.color} {data.kind}";

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (_battleManager != null)
        {
            _battleManager.OnCardClicked(this);
        }
    }
}
