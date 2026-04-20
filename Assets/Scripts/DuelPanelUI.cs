using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gestiona el panel de duelo: cada jugador elige sus cartas y confirma.
// Flujo: Atacante elige → confirma → Defensor elige → confirma → GameManager resuelve.
public class DuelPanelUI : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI instructionLabel;
    [SerializeField] private TextMeshProUGUI attackerSumLabel;
    [SerializeField] private TextMeshProUGUI defenderSumLabel;

    [Header("Botones")]
    [SerializeField] private Button btnConfirm;

    private DuelRequest currentRequest;
    private List<CardData> attackerCards = new List<CardData>();
    private List<CardData> defenderCards = new List<CardData>();
    private bool waitingForDefender = false;

    private void Start()
    {
        btnConfirm?.onClick.AddListener(OnConfirmPressed);
        gameObject.SetActive(false);
    }

    // Llamado por UIManager cuando llega OnDuelStarted
    public void BeginDuel(DuelRequest request)
    {
        currentRequest = request;
        attackerCards.Clear();
        defenderCards.Clear();
        waitingForDefender = false;

        gameObject.SetActive(true);
        UpdateUI();
    }

    // Llamado por CardSelectorUI al hacer clic en una carta durante el duelo
    public void ToggleCard(CardData card)
    {
        var list = waitingForDefender ? defenderCards : attackerCards;

        if (list.Contains(card))
            list.Remove(card);
        else if (list.Count < 3)
            list.Add(card);

        UpdateUI();
    }

    private void OnConfirmPressed()
    {
        if (!waitingForDefender)
        {
            // Atacante confirmó — ahora le toca al defensor
            waitingForDefender = true;
            UpdateUI();
        }
        else
        {
            // Defensor confirmó — resolver duelo
            var attackerSub = new DuelSubmission(currentRequest.AttackerId, new List<CardData>(attackerCards));
            var defenderSub = new DuelSubmission(currentRequest.DefenderId, new List<CardData>(defenderCards));

            GameManager.Instance.ResolveDuel(attackerSub, defenderSub, currentRequest.TargetCard);
            gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        int currentPlayer = waitingForDefender ? currentRequest.DefenderId : currentRequest.AttackerId;
        string role = waitingForDefender ? "Defensor" : "Atacante";

        if (titleLabel != null)
            titleLabel.text = $"Duelo — {role}: Jugador {currentPlayer}";

        if (instructionLabel != null)
            instructionLabel.text = "Selecciona 0–3 cartas (mismo palo o mismo número) y confirma.";

        int aSum = 0; foreach (var c in attackerCards) aSum += c.Value;
        int dSum = 0; foreach (var c in defenderCards) dSum += c.Value;

        if (attackerSumLabel != null) attackerSumLabel.text = $"Atacante: {aSum}";
        if (defenderSumLabel != null) defenderSumLabel.text = $"Defensor: {dSum}";
    }
}
