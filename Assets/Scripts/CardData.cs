using UnityEngine;

public enum CardType { Number, Special }
public enum Suit { Rojo, Azul, Verde, Amarillo }

[CreateAssetMenu(fileName = "CardData", menuName = "Triples/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private CardType type;
    [SerializeField] private Suit suit;           // solo para Number
    [SerializeField] private int value;           // 2–8 para Number, 0 para Special
    [SerializeField] private string specialSetId; // "trio_A".."trio_E" para Special
    [SerializeField] private string displayName;
    [SerializeField] private Sprite artwork;      // asignado por Equipo 2

    public string Id => id;
    public CardType Type => type;
    public Suit Suit => suit;
    public int Value => value;
    public string SpecialSetId => specialSetId;
    public string DisplayName => displayName;
    public Sprite Artwork => artwork;

    // Comprueba si dos cartas pueden combinarse en un duelo
    public static bool CanCombine(CardData a, CardData b)
    {
        if (a.Type == CardType.Special || b.Type == CardType.Special) return false;
        return a.Suit == b.Suit || a.Value == b.Value;
    }

    public override string ToString() => displayName;

    // Factory para crear cartas en runtime (tests / DeckBuilder)
    public static CardData Create(string id, CardType type, Suit suit, int value, string setId, string name)
    {
        var c = CreateInstance<CardData>();
        c.id = id;
        c.type = type;
        c.suit = suit;
        c.value = value;
        c.specialSetId = setId;
        c.displayName = name;
        return c;
    }
}
