using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text costText; // 如果你有费用的话
    public Image cardImage;   // 如果有图片
    public Button button;

    // === 核心修改：现在持有 RuntimeCard ===
    public RuntimeCard RuntimeCard { get; private set; }

    private BattleManager _bm;

    // 初始化方法
    public void Init(RuntimeCard card, BattleManager bm)
    {
        RuntimeCard = card;
        _bm = bm;

        // 从 Data 模版里读取显示信息
        if (card != null && card.Data != null)
        {
            if (nameText) nameText.text = card.Data.cardName;
            if (descriptionText) descriptionText.text = card.Data.description;

            // 如果你有图片字段
            // if (cardImage) cardImage.sprite = card.Data.artwork;

            // 如果你想显示动态数值（比如临时加攻），可以在这里判断
            // 但目前主要还是显示 Data 里的静态数值
        }

        // 绑定点击事件
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (_bm != null && RuntimeCard != null)
        {
            // 通知管理器：我（这个UI对象）被点了，代表的是这张牌（RuntimeCard）
            _bm.OnCardClicked(this, RuntimeCard);
        }
    }
}