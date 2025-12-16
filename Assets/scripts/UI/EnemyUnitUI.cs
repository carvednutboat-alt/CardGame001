using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitUI : MonoBehaviour
{
    // 需要在 Inspector 里把敌人身上的 Button 拖进来
    public Button clickButton;

    private BattleManager _bm;

    public void Init(BattleManager bm)
    {
        _bm = bm;
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (_bm != null)
        {
            // 通知 BattleManager：我（敌人）被点了
            _bm.OnEnemyClicked();
        }
    }
}