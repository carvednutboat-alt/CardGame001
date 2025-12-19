using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text GoldText;
    public Transform OffersContainer;
    public GameObject OfferButtonPrefab;
    public Button LeaveButton;

    [Header("Shop Pool")]
    public List<CardData> SellPool = new List<CardData>();
    public int OfferCount = 3;

    [Header("Price")]
    public Vector2Int UnitPrice = new Vector2Int(1, 3);
    public Vector2Int SpellPrice = new Vector2Int(1, 3);
    public Vector2Int EquipPrice = new Vector2Int(1, 3);

    void Start()
    {
        if (LeaveButton != null)
            LeaveButton.onClick.AddListener(OnLeave);

        BuildOffers();
        RefreshGold();

        // 如果你 GameManager 有 OnPlayerStateChanged（你之前用过 MapHUD）
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged += RefreshGold;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerStateChanged -= RefreshGold;
    }

    void BuildOffers()
    {
        if (OffersContainer == null || OfferButtonPrefab == null) return;

        for (int i = OffersContainer.childCount - 1; i >= 0; i--)
            Destroy(OffersContainer.GetChild(i).gameObject);

        if (SellPool == null || SellPool.Count == 0)
        {
            Debug.LogError("[Shop] SellPool 为空：请在 Inspector 填入可售卖的 CardData。");
            return;
        }

        int n = Mathf.Min(OfferCount, SellPool.Count);
        for (int i = 0; i < n; i++)
        {
            CardData card = SellPool[Random.Range(0, SellPool.Count)];
            if (card == null) { i--; continue; }

            int price = RollPrice(card);

            GameObject obj = Instantiate(OfferButtonPrefab, OffersContainer);
            var btn = obj.GetComponent<Button>();
            var txt = obj.GetComponentInChildren<TMP_Text>();

            if (txt != null) txt.text = $"{card.cardName}  -  {price}G";

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => TryBuy(card, price, obj));
            }
        }
    }

    int RollPrice(CardData card)
    {
        if (card.isEquipment) return Random.Range(EquipPrice.x, EquipPrice.y + 1);
        if (card.kind == CardKind.Unit) return Random.Range(UnitPrice.x, UnitPrice.y + 1);
        return Random.Range(SpellPrice.x, SpellPrice.y + 1);
    }

    void TryBuy(CardData card, int price, GameObject offerObj)
    {
        var gm = GameManager.Instance;
        if (gm == null || card == null) return;

        if (!gm.TrySpendGold(price))
        {
            Debug.Log("[Shop] 金币不足");
            return;
        }

        gm.AddCardToDeck(card);
        RefreshGold();

        // 下架已购买商品
        if (offerObj != null) Destroy(offerObj);
    }

    void RefreshGold()
    {
        if (GoldText == null || GameManager.Instance == null) return;
        GoldText.text = $"Gold: {GameManager.Instance.Gold}";
    }

    void OnLeave()
    {
        // 商店结束也算节点完成
        if (GameManager.Instance != null)
            GameManager.Instance.OnNodeCompleted();
    }
}
