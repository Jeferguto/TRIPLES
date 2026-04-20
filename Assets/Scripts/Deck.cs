using System;
using System.Collections.Generic;

public class Deck
{
    private List<CardData> cards;

    public event Action<int> OnCountChanged;

    public int Count => cards.Count;

    public Deck(List<CardData> source)
    {
        cards = new List<CardData>(source);
    }

    public CardData Draw()
    {
        if (cards.Count == 0) return null;
        var top = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        OnCountChanged?.Invoke(cards.Count);
        return top;
    }

    public CardData Peek() => cards.Count > 0 ? cards[cards.Count - 1] : null;

    public void Shuffle(int? seed = null)
    {
        var rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
        OnCountChanged?.Invoke(cards.Count);
    }
}
