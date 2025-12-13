using UnityEngine;

public enum CardColor
{
    Colorless,
    Red,
    Blue,
    Green
}

public enum CardKind
{
    Unit,
    Spell,
    Evolve
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardKind kind;
    public CardColor color;

    [TextArea]
    public string description;

    [Header("Unit stats")]
    public int unitAttack;
    public int unitHealth;

    [Header("Value for Spell/Evolve")]
    public int value;
}
