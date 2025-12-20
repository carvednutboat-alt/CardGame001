using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardData cardData;

    public void Init(CardData data)
    {
        cardData = data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardData != null && CardDetailPanel.Instance != null)
        {
            CardDetailPanel.Instance.Show(cardData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardDetailPanel.Instance != null)
        {
            CardDetailPanel.Instance.Hide();
        }
    }
}
