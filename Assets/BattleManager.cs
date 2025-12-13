using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public Unit player;
    public Unit enemy;

    [Header("Player deck setup")]
    public List<CardData> startingDeck = new List<CardData>();

    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> hand = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    [Header("Hand UI")]
    public Transform handPanel;
    public CardUI cardPrefab;

    [Header("Unit UI")]
    public Transform unitPanel;
    private List<CardData> unitPool = new List<CardData>();
    [Header("Field info")]
    public TMP_Text fieldColorsText;

    [Header("Misc UI")]
    public TMP_Text logText;
    public Button endTurnButton;

    private class FieldUnit
    {
        public CardColor color;
        public string name;
        public int health;
    }

    private List<FieldUnit> playerUnits = new List<FieldUnit>();

    private HashSet<CardColor> activeColors = new HashSet<CardColor>();
    private int rolesOnField = 0;
    public int maxRolesOnField = 5;

    private bool hasSummonedThisTurn;
    private bool hasEvolvedThisTurn;

    private bool playerGoesFirst;
    private bool gameEnded;

    public int enemyAttackDamage = 5;

    private void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        gameEnded = false;
        hasSummonedThisTurn = false;
        hasEvolvedThisTurn = false;

        if (player != null) player.ResetHp();
        if (enemy != null) enemy.ResetHp();

        drawPile.Clear();
        hand.Clear();
        discardPile.Clear();
        activeColors.Clear();
        playerUnits.Clear();
        unitPool.Clear();
        rolesOnField = 0;
        UpdateFieldColorsText();

        foreach (var card in startingDeck)
        {
            if (card == null) continue;

            if (card.kind == CardKind.Unit)
            {
                unitPool.Add(card);
            }
            else
            {
                drawPile.Add(card);
            }
        }

        Shuffle(drawPile);

        CreateUnitCardsUI();

        playerGoesFirst = Random.value < 0.5f;
        Log(playerGoesFirst ? "你是先手。" : "你是后手。");

        DrawCards(5);

        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.onClick.AddListener(OnEndTurnButton);
            endTurnButton.interactable = true;
        }

        StartPlayerTurn(skipDrawAndBattle: playerGoesFirst);
    }

    private void StartPlayerTurn(bool skipDrawAndBattle)
    {
        if (gameEnded) return;

        hasSummonedThisTurn = false;
        hasEvolvedThisTurn = false;

        if (!skipDrawAndBattle)
        {
            DrawCards(1);
        }

        Log(skipDrawAndBattle
            ? "你的第一个回合（先手），本回合不能抽牌和战斗。可以上角色 / 进化 / 出牌。"
            : "轮到你行动。可以上角色 / 进化 / 出牌，然后结束回合。");

        if (endTurnButton != null)
            endTurnButton.interactable = true;
    }

    private void OnEndTurnButton()
    {
        if (gameEnded) return;

        if (endTurnButton != null)
            endTurnButton.interactable = false;

        Log("你的回合结束，轮到敌人。");
        EnemyTurn();
    }

    private void EnemyTurn()
    {
        if (gameEnded) return;

        if (enemy != null && !enemy.IsDead())
        {
            int dmg = enemyAttackDamage;

            if (playerUnits.Count > 0)
            {
                FieldUnit target = playerUnits[0];
                Log($"敌人对你的单位 {target.name} 造成 {dmg} 点伤害。");

                target.health -= dmg;

                if (target.health <= 0)
                {
                    int overkill = -target.health;
                    CardColor deadColor = target.color;

                    Log($"你的单位 {target.name} 被击杀了。");

                    playerUnits.RemoveAt(0);
                    rolesOnField = Mathf.Max(0, rolesOnField - 1);

                    if (deadColor != CardColor.Colorless)
                    {
                        bool stillHasColor = false;
                        foreach (var u in playerUnits)
                        {
                            if (u.color == deadColor)
                            {
                                stillHasColor = true;
                                break;
                            }
                        }
                        if (!stillHasColor)
                        {
                            activeColors.Remove(deadColor);
                        }
                    }

                    UpdateFieldColorsText();

                    if (overkill > 0 && player != null)
                    {
                        Log($"溢出伤害 {overkill} 点打在玩家身上。");
                        player.TakeDamage(overkill);
                        if (player.IsDead())
                        {
                            OnPlayerDefeated();
                            return;
                        }
                    }
                }
                else
                {
                    Log($"单位 {target.name} 剩余 HP：{target.health}");
                }
            }
            else
            {
                if (player != null)
                {
                    Log($"敌人对你造成 {dmg} 点伤害。");
                    player.TakeDamage(dmg);
                    if (player.IsDead())
                    {
                        OnPlayerDefeated();
                        return;
                    }
                }
            }
        }

        StartPlayerTurn(skipDrawAndBattle: false);
    }

    public void OnCardClicked(CardUI cardView)
    {
        if (gameEnded) return;
        if (cardView == null || cardView.Data == null) return;

        CardData card = cardView.Data;

        bool needsColor = card.color != CardColor.Colorless && card.kind != CardKind.Unit;
        if (needsColor && !activeColors.Contains(card.color))
        {
            Log($"你场上没有 {card.color} 角色，无法使用这张牌。");
            return;
        }

        bool played = false;
        bool isUnitCard = (card.kind == CardKind.Unit);

        switch (card.kind)
        {
            case CardKind.Unit:
                played = PlayUnitCard(card);
                break;
            case CardKind.Spell:
                played = PlaySpellCard(card);
                break;
            case CardKind.Evolve:
                played = PlayEvolveCard(card);
                break;
        }

        if (!played) return;

        if (!isUnitCard)
        {
            hand.Remove(card);
            discardPile.Add(card);
            Destroy(cardView.gameObject);
        }
        else
        {
            if (cardView.button != null)
            {
                cardView.button.interactable = false;
            }
        }

        CheckWinLose();
    }

    private bool PlayUnitCard(CardData card)
    {
        if (hasSummonedThisTurn)
        {
            Log("本回合已经上过一个角色，无法再上场。");
            return false;
        }

        if (rolesOnField >= maxRolesOnField)
        {
            Log("场上角色已达到上限，无法再上场。");
            return false;
        }

        if (card.color != CardColor.Colorless)
        {
            activeColors.Add(card.color);
        }

        int hp = card.unitHealth > 0 ? card.unitHealth : 1;
        FieldUnit newUnit = new FieldUnit
        {
            color = card.color,
            name = card.cardName,
            health = hp
        };
        playerUnits.Add(newUnit);

        rolesOnField++;
        hasSummonedThisTurn = true;
        UpdateFieldColorsText();

        Log($"你召唤了一个 {card.color} 角色：{card.cardName}（HP {hp}）。");

        return true;
    }

    private bool PlaySpellCard(CardData card)
    {
        int dmg = Mathf.Max(0, card.value);
        if (enemy != null)
        {
            Log($"你使用 {card.cardName} 对敌人造成 {dmg} 点伤害。");
            enemy.TakeDamage(dmg);
            if (enemy.IsDead())
            {
                OnEnemyDefeated();
            }
        }
        return true;
    }

    private bool PlayEvolveCard(CardData card)
    {
        if (hasEvolvedThisTurn)
        {
            Log("本回合已经进行过一次进化。");
            return false;
        }

        if (!activeColors.Contains(card.color))
        {
            Log($"你场上没有 {card.color} 角色，无法进化。");
            return false;
        }

        int heal = Mathf.Max(0, card.value);
        if (player != null)
        {
            Log($"你使用 {card.cardName}，回复自己 {heal} 点生命。");
            player.Heal(heal);
        }

        hasEvolvedThisTurn = true;
        return true;
    }

    private void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                RefillDrawPile();
                if (drawPile.Count == 0)
                {
                    Log("牌堆和弃牌堆都空了，无法继续抽牌。");
                    return;
                }
            }

            CardData data = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(data);
            CreateCardView(data);
        }
    }

    private void CreateCardView(CardData data)
    {
        if (cardPrefab == null || handPanel == null) return;

        CardUI instance = Instantiate(cardPrefab, handPanel);
        instance.Init(data, this);
    }

    private void CreateUnitCardsUI()
    {
        Transform panel = unitPanel != null ? unitPanel : handPanel;
        if (panel == null || cardPrefab == null) return;

        foreach (var data in unitPool)
        {
            CardUI instance = Instantiate(cardPrefab, panel);
            instance.Init(data, this);
        }
    }

    private void RefillDrawPile()
    {
        if (discardPile.Count == 0) return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
        Log("将弃牌堆洗回牌堆。");
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            CardData temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void UpdateFieldColorsText()
    {
        if (fieldColorsText == null) return;

        if (rolesOnField == 0)
        {
            fieldColorsText.text = "场上角色颜色：无";
            return;
        }

        List<string> colors = new List<string>();
        foreach (var c in activeColors)
        {
            colors.Add(c.ToString());
        }

        fieldColorsText.text =
            "场上角色颜色：" + string.Join(", ", colors) + $"（共 {rolesOnField} 个角色）";
    }

    private void CheckWinLose()
    {
        if (gameEnded) return;

        if (enemy != null && enemy.IsDead())
        {
            OnEnemyDefeated();
        }
        else if (player != null && player.IsDead())
        {
            OnPlayerDefeated();
        }
    }

    private void OnEnemyDefeated()
    {
        if (gameEnded) return;
        gameEnded = true;
        Log("你击败了敌人，战斗胜利！");
        if (endTurnButton != null) endTurnButton.interactable = false;
    }

    private void OnPlayerDefeated()
    {
        if (gameEnded) return;
        gameEnded = true;
        Log("你被击败了，战斗失败。");
        if (endTurnButton != null) endTurnButton.interactable = false;
    }

    private void Log(string msg)
    {
        if (logText != null)
        {
            logText.text += "\n" + msg;
        }
        else
        {
            Debug.Log(msg);
        }
    }
}
