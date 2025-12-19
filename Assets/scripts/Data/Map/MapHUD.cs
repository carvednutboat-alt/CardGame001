using TMPro;
using UnityEngine;

public class MapHUD : MonoBehaviour
{
    public TMP_Text HPText;
    public TMP_Text GoldText;

    void OnEnable()
    {
        Refresh();

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged += Refresh;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged -= Refresh;
    }

    void Refresh()
    {
        if (GameManager.Instance == null) return;

        HPText.text = $"HP: {GameManager.Instance.PlayerCurrentHP}/{GameManager.Instance.PlayerMaxHP}";
        GoldText.text = $"Gold: {GameManager.Instance.Gold}";
    }
}
