using System;
using System.Collections.Generic;

public class DiscardPile
{
    private List<CardData> cards = new List<CardData>();

    public event Action<int> OnCountChanged;

    public int Count => cards.Count;

    public void Add(CardData card)
    {
        if (card == null) return;
        cards.Add(card);
        OnCountChanged?.Invoke(cards.Count);
    }

    public CardData Peek() => cards.Count > 0 ? cards[cards.Count - 1] : null;

    public CardData DrawTop()
    {
        if (cards.Count == 0) return null;
        var top = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        OnCountChanged?.Invoke(cards.Count);
        return top;
    }

    public List<CardData> DrawAll()
    {
        var all = new List<CardData>(cards);
        cards.Clear();
        OnCountChanged?.Invoke(0);
        return all;
    }
}
