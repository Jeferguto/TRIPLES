using System.Collections.Generic;

public class DuelRequest
{
    public int AttackerId { get; }
    public int DefenderId { get; }
    public CardData TargetCard { get; }

    public DuelRequest(int attackerId, int defenderId, CardData targetCard)
    {
        AttackerId = attackerId;
        DefenderId = defenderId;
        TargetCard = targetCard;
    }
}

public class DuelSubmission
{
    public int PlayerId { get; }
    public List<CardData> Cards { get; }

    public DuelSubmission(int playerId, List<CardData> cards)
    {
        PlayerId = playerId;
        Cards = cards ?? new List<CardData>();
    }
}

public class DuelResult
{
    public int AttackerId { get; }
    public int DefenderId { get; }
    public int WinnerId { get; }
    public bool AttackerWon { get; }
    public bool ShieldBroken { get; }
    public int AttackerSum { get; }
    public int DefenderSum { get; }
    public List<CardData> AttackerCardsUsed { get; }
    public List<CardData> DefenderCardsUsed { get; }

    public DuelResult(int attackerId, int defenderId, int winnerId, bool shieldBroken,
        int attackerSum, int defenderSum,
        List<CardData> attackerCards, List<CardData> defenderCards)
    {
        AttackerId = attackerId;
        DefenderId = defenderId;
        WinnerId = winnerId;
        AttackerWon = winnerId == attackerId;
        ShieldBroken = shieldBroken;
        AttackerSum = attackerSum;
        DefenderSum = defenderSum;
        AttackerCardsUsed = attackerCards;
        DefenderCardsUsed = defenderCards;
    }
}
