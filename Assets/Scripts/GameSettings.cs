using UnityEngine;

public enum VulnerablePenalty
{
    MinusOneDefense,        // La suma defensora se reduce en 1
    PlusOneAttack,          // La suma atacante se incrementa en 1
    SingleCardDefenseOnly   // El defensor solo puede usar 1 carta
}

[CreateAssetMenu(fileName = "GameSettings", menuName = "Triples/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Tooltip("Penalización para cartas vulnerables")]
    public VulnerablePenalty vulnerablePenalty = VulnerablePenalty.SingleCardDefenseOnly;

    [Tooltip("Cartas máximas en mano (numéricas)")]
    public int maxHandSize = 5;

    [Tooltip("Escudos máximos por jugador")]
    public int maxShieldsPerPlayer = 4;

    [Tooltip("Cartas máximas en un duelo")]
    public int maxDuelCards = 3;

    [Tooltip("Tríos necesarios para ganar")]
    public int triosToWin = 3;
}
