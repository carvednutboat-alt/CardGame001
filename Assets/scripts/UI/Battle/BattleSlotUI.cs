using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int SlotIndex; // 0-4
    public bool IsPlayerSide;
    
    public Image SlotImage;
    public Transform contentParent; // Unit spawned here
    
    private System.Action<int, bool> _onClickCallback;

    public void Init(int index, bool isPlayerSide, System.Action<int, bool> onClick)
    {
        SlotIndex = index;
        IsPlayerSide = isPlayerSide;
        _onClickCallback = onClick;
        
        // Basic Setup
        if(SlotImage == null) SlotImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickCallback?.Invoke(SlotIndex, IsPlayerSide);
    }

    public void SetHighlight(bool active)
    {
        if(SlotImage != null)
        {
            SlotImage.color = active ? new Color(0, 1, 0, 0.5f) : new Color(1, 1, 1, 0.2f);
        }
    }

    public void SetContent(Transform unitTransform)
    {
        if(unitTransform != null)
        {
            unitTransform.SetParent(this.transform, false);
            unitTransform.localPosition = Vector3.zero;
        }
    }
}
// Force Recompile