using System.Collections.Generic;
using UnityEngine;

// Adjunta este script a cualquier GameObject en la escena para validar el Sprint 1.
// Imprime el estado del mazo y el reparto inicial en la consola de Unity.
public class DeckDebugger : MonoBehaviour
{
    [SerializeField] private int testPlayers = 2;
    [SerializeField] private int deckSeed = 42;

    private void Start()
    {
        RunTest();
    }

    private void RunTest()
    {
        Debug.Log("=== DeckDebugger: Sprint 1 ===");

        var allCards = DeckBuilder.Build(deckSeed);
        Debug.Log($"Mazo construido: {allCards.Count} cartas (esperado: 39)");

        int specials = 0, numbers = 0;
        foreach (var c in allCards)
        {
            if (c.Type == CardType.Special) specials++;
            else numbers++;
        }
        Debug.Log($"  Especiales: {specials} (esperado: 15) | Numéricas: {numbers} (esperado: 24)");

        var deck = new Deck(allCards);
        var players = new List<PlayerState>();
        for (int i = 0; i < testPlayers; i++)
            players.Add(new PlayerState(i + 1));

        // Repartir 5 cartas por jugador
        for (int round = 0; round < 5; round++)
        {
            foreach (var p in players)
            {
                var card = deck.Draw();
                if (card == null) break;
                if (card.Type == CardType.Special) p.AddSpecial(card);
                else p.AddToHand(card);
            }
        }

        Debug.Log($"Cartas en mazo tras reparto: {deck.Count} (esperado: {39 - testPlayers * 5})");

        foreach (var p in players)
        {
            Debug.Log($"  {p}");
            Debug.Log($"    Mano: {string.Join(", ", p.Hand)}");
            Debug.Log($"    Especiales: {string.Join(", ", p.SpecialsInFront)}");
        }

        Debug.Log("=== Test completado ===");
    }
}
