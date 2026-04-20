using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DuelController
{
    private GameSettings settings;

    public DuelController(GameSettings settings)
    {
        this.settings = settings;
    }

    // Valida que las cartas de una submission sean combinables (mismo palo o mismo número)
    public bool ValidateSubmission(List<CardData> cards)
    {
        if (cards == null || cards.Count == 0) return true; // 0 cartas es válido
        if (cards.Count > settings.maxDuelCards) return false;
        if (cards.Any(c => c.Type == CardType.Special)) return false;

        if (cards.Count == 1) return true;

        // Todas del mismo palo o todas del mismo valor
        bool sameSuit = cards.All(c => c.Suit == cards[0].Suit);
        bool sameValue = cards.All(c => c.Value == cards[0].Value);
        return sameSuit || sameValue;
    }

    public DuelResult Resolve(
        PlayerState attacker, PlayerState defender,
        DuelSubmission attackerSub, DuelSubmission defenderSub,
        CardData targetCard)
    {
        int attackerSum = attackerSub.Cards.Sum(c => c.Value);
        int defenderSum = defenderSub.Cards.Sum(c => c.Value);

        // Aplicar penalización si la carta es vulnerable
        if (defender.IsVulnerable(targetCard))
        {
            switch (settings.vulnerablePenalty)
            {
                case VulnerablePenalty.MinusOneDefense:
                    defenderSum = Mathf.Max(0, defenderSum - 1);
                    break;
                case VulnerablePenalty.PlusOneAttack:
                    attackerSum += 1;
                    break;
                // SingleCardDefenseOnly se valida antes de llamar Resolve
            }
        }

        bool shieldBroken = defender.HasShield(targetCard);

        // Empate → defensor gana
        bool attackerWins = attackerSum > defenderSum;
        int winnerId = attackerWins ? attacker.PlayerNumber : defender.PlayerNumber;

        Debug.Log($"Duelo: J{attacker.PlayerNumber}({attackerSum}) vs J{defender.PlayerNumber}({defenderSum}) → ganador J{winnerId}");

        return new DuelResult(
            attacker.PlayerNumber,
            defender.PlayerNumber,
            winnerId,
            shieldBroken,
            attackerSum,
            defenderSum,
            new List<CardData>(attackerSub.Cards),
            new List<CardData>(defenderSub.Cards)
        );
    }

    // Valida que el defensor con carta vulnerable solo use 1 carta (si esa penalización está activa)
    public bool IsDefenderSubmissionAllowed(PlayerState defender, CardData targetCard, List<CardData> cards)
    {
        if (defender.IsVulnerable(targetCard) &&
            settings.vulnerablePenalty == VulnerablePenalty.SingleCardDefenseOnly)
        {
            return cards.Count <= 1;
        }
        return ValidateSubmission(cards);
    }
}
