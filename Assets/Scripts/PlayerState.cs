using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerState
{
    public int PlayerNumber { get; }
    public List<CardData> Hand { get; } = new List<CardData>();
    public List<CardData> SpecialsInFront { get; } = new List<CardData>();
    public List<string> CompletedTrios { get; } = new List<string>();

    private Dictionary<CardData, int> shields = new Dictionary<CardData, int>();
    private HashSet<CardData> vulnerable = new HashSet<CardData>();

    private const int MaxHandSize = 5;
    private const int MaxShields = 4;

    public PlayerState(int playerNumber) => PlayerNumber = playerNumber;

    // ── Mano ──────────────────────────────────────────────────────────────────
    public bool CanAddToHand() => Hand.Count < MaxHandSize;

    public bool AddToHand(CardData card)
    {
        if (!CanAddToHand()) return false;
        Hand.Add(card);
        return true;
    }

    public bool RemoveFromHand(CardData card) => Hand.Remove(card);

    // ── Especiales ────────────────────────────────────────────────────────────
    public void AddSpecial(CardData card)
    {
        if (card.Type != CardType.Special) return;
        SpecialsInFront.Add(card);
    }

    // ── Posesión genérica ─────────────────────────────────────────────────────
    public bool OwnsCard(CardData card) => Hand.Contains(card) || SpecialsInFront.Contains(card);

    public void RemoveCard(CardData card)
    {
        Hand.Remove(card);
        SpecialsInFront.Remove(card);
        shields.Remove(card);
        vulnerable.Remove(card);
    }

    // ── Escudos ───────────────────────────────────────────────────────────────
    public bool CanAddShield() => TotalShields() < MaxShields;

    public void AddShield(CardData card)
    {
        if (!OwnsCard(card)) return;
        if (shields.ContainsKey(card)) shields[card]++;
        else shields[card] = 1;
    }

    public bool HasShield(CardData card) => shields.ContainsKey(card) && shields[card] > 0;

    public void RemoveShield(CardData card)
    {
        if (!shields.ContainsKey(card)) return;
        shields[card]--;
        if (shields[card] <= 0) shields.Remove(card);
    }

    public int TotalShields() => shields.Values.Sum();

    // ── Vulnerable ────────────────────────────────────────────────────────────
    public void MarkVulnerable(CardData card) => vulnerable.Add(card);
    public bool IsVulnerable(CardData card) => vulnerable.Contains(card);

    // ── Win condition ─────────────────────────────────────────────────────────
    public string GetCompletedTrio()
    {
        var groups = SpecialsInFront.GroupBy(c => c.SpecialSetId);
        foreach (var g in groups)
        {
            if (g.Count() >= 3) return g.Key;
        }
        return null;
    }

    public void ClaimTrio(string setId)
    {
        CompletedTrios.Add(setId);
        // Las 3 cartas del trío se conservan, no vuelven al mazo
    }

    public void ResetForNewRound()
    {
        // Las cartas del trío ganador se conservan; todo lo demás vuelve al mazo
        var keepSpecials = SpecialsInFront.Where(c => CompletedTrios.Contains(c.SpecialSetId)).ToList();

        Hand.Clear();
        SpecialsInFront.Clear();
        foreach (var c in keepSpecials)
            SpecialsInFront.Add(c);

        shields.Clear();
        vulnerable.Clear();
    }

    public override string ToString() =>
        $"Jugador {PlayerNumber} [mano:{Hand.Count} especiales:{SpecialsInFront.Count} tríos:{CompletedTrios.Count}]";
}
