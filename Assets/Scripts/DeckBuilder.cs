using System.Collections.Generic;

public static class DeckBuilder
{
    // Cinco tríos de especiales: A–E
    private static readonly string[] SpecialSets = { "trio_A", "trio_B", "trio_C", "trio_D", "trio_E" };

    // Distribución exacta de numéricas según reglas
    private static readonly (int value, int count)[] NumberCards =
    {
        (2, 4), (3, 4), (4, 4), (5, 4), (6, 3), (7, 3), (8, 2)
    };

    private static readonly Suit[] Suits = { Suit.Rojo, Suit.Azul, Suit.Verde, Suit.Amarillo };

    public static List<CardData> Build(int? seed = null)
    {
        var cards = new List<CardData>(39);

        // 15 especiales (5 tríos × 3)
        foreach (var setId in SpecialSets)
        {
            for (int i = 0; i < 3; i++)
            {
                string cardId = $"{setId}_{i}";
                cards.Add(CardData.Create(cardId, CardType.Special, Suit.Rojo, 0, setId, $"Especial {setId[5]}{i + 1}"));
            }
        }

        // 24 numéricas distribuidas en palos de forma cíclica
        int suitIndex = 0;
        foreach (var (value, count) in NumberCards)
        {
            for (int i = 0; i < count; i++)
            {
                Suit suit = Suits[suitIndex % Suits.Length];
                string cardId = $"num_{value}_{i}";
                string name = $"{value} ({suit})";
                cards.Add(CardData.Create(cardId, CardType.Number, suit, value, null, name));
                suitIndex++;
            }
        }

        Shuffle(cards, seed);
        return cards;
    }

    // Fisher-Yates
    private static void Shuffle<T>(List<T> list, int? seed)
    {
        var rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
