using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ── Eventos públicos (Equipos 2 y 4 se suscriben aquí) ──────────────────
    public event Action<int> OnPlayerTurnStarted;
    public event Action<int, CardData> OnCardDrawn;
    public event Action<int, CardData> OnShieldPlaced;
    public event Action<DuelRequest> OnDuelStarted;
    public event Action<DuelResult> OnDuelResolved;
    public event Action<int> OnRoundWon;       // playerNumber ganó la ronda
    public event Action<int> OnGameWon;        // playerNumber ganó la partida

    // ── Config ───────────────────────────────────────────────────────────────
    [SerializeField] private int totalPlayers = 2;
    [SerializeField] private GameSettings settings;

    // ── Estado interno ───────────────────────────────────────────────────────
    public GamePhase Phase { get; private set; } = GamePhase.Setup;
    public int CurrentPlayerNumber { get; private set; } = 1;

    private PlayerState[] players;
    private Deck deck;
    private DiscardPile discardPile;
    private DuelController duelController;
    private bool duelPending = false;

    public static GameManager Instance { get; private set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        OnPlayerTurnStarted += (p) => CameraSwitcher.Instance?.SwitchTo(p);

        if (settings == null)
            settings = ScriptableObject.CreateInstance<GameSettings>();

        BeginSetup();
    }

    private void Update()
    {
        // Cambio manual de jugador con teclas 1–4 (debug / pasada de dispositivo)
        if (Phase == GamePhase.PlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ForceSetPlayer(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ForceSetPlayer(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ForceSetPlayer(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ForceSetPlayer(4);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.Confined;
    }

    // ── Setup ─────────────────────────────────────────────────────────────────
    private void BeginSetup()
    {
        Phase = GamePhase.Setup;

        players = new PlayerState[totalPlayers];
        for (int i = 0; i < totalPlayers; i++)
            players[i] = new PlayerState(i + 1);

        var allCards = DeckBuilder.Build();
        deck = new Deck(allCards);
        discardPile = new DiscardPile();
        duelController = new DuelController(settings);

        DealInitialHands();
        DetermineFirstPlayer();
    }

    private void DealInitialHands()
    {
        for (int i = 0; i < 5; i++)
        {
            foreach (var p in players)
            {
                var card = deck.Draw();
                if (card == null) break;

                if (card.Type == CardType.Special)
                    p.AddSpecial(card);
                else
                    p.AddToHand(card);
            }
        }
        Debug.Log("Reparto inicial completado.");
        LogAllHands();
    }

    private void DetermineFirstPlayer()
    {
        Phase = GamePhase.DetermineFirstPlayer;

        int minSpecials = int.MaxValue;
        int firstPlayer = 1;
        bool tie = false;

        foreach (var p in players)
        {
            int count = p.SpecialsInFront.Count;
            if (count < minSpecials)
            {
                minSpecials = count;
                firstPlayer = p.PlayerNumber;
                tie = false;
            }
            else if (count == minSpecials)
            {
                tie = true;
            }
        }

        if (tie)
        {
            // Empate: cada jugador pone 1 carta; quien tenga mayor valor empieza.
            // Como no hay UI todavía, asignamos aleatoriamente (TODO: duelo real con Equipo 4).
            firstPlayer = UnityEngine.Random.Range(1, totalPlayers + 1);
            Debug.Log($"Empate en especiales. Primer jugador aleatorio: {firstPlayer} (pendiente duelo real).");
        }

        StartTurn(firstPlayer);
    }

    // ── Turno ─────────────────────────────────────────────────────────────────
    private void StartTurn(int playerNumber)
    {
        CurrentPlayerNumber = playerNumber;
        Phase = GamePhase.PlayerTurn;
        Debug.Log($"── Turno del Jugador {playerNumber} ──");
        OnPlayerTurnStarted?.Invoke(playerNumber);
    }

    public void EndTurn()
    {
        if (Phase != GamePhase.PlayerTurn) return;

        int next = (CurrentPlayerNumber % totalPlayers) + 1;
        StartTurn(next);
    }

    // ── Acciones públicas ────────────────────────────────────────────────────

    public bool TryDraw(int playerId)
    {
        if (!IsActivePlayer(playerId)) return false;
        if (Phase != GamePhase.PlayerTurn) return false;

        var card = deck.Draw();
        if (card == null)
        {
            Debug.Log("El mazo está vacío.");
            return false;
        }

        var player = GetPlayer(playerId);

        if (card.Type == CardType.Special)
        {
            player.AddSpecial(card);
            Debug.Log($"J{playerId} roba especial: {card.DisplayName}");
        }
        else
        {
            if (!player.CanAddToHand())
            {
                // Mano llena: la carta va al descarte automáticamente (el jugador debe elegir cuál descartar)
                // TODO: disparar evento para que UI permita elegir qué descartar
                discardPile.Add(card);
                Debug.Log($"J{playerId} mano llena. {card.DisplayName} descartada automáticamente (pendiente selección UI).");
                OnCardDrawn?.Invoke(playerId, card);
                return true;
            }
            player.AddToHand(card);
        }

        Debug.Log($"J{playerId} roba: {card.DisplayName}");
        OnCardDrawn?.Invoke(playerId, card);
        CheckRoundWin(player);
        return true;
    }

    public bool TryProtect(int playerId, CardData target)
    {
        if (!IsActivePlayer(playerId)) return false;
        if (Phase != GamePhase.PlayerTurn) return false;

        var player = GetPlayer(playerId);
        if (!player.CanAddShield())
        {
            Debug.Log($"J{playerId} ya tiene 4 escudos.");
            return false;
        }
        if (!player.OwnsCard(target))
        {
            Debug.Log($"J{playerId} no posee esa carta.");
            return false;
        }

        player.AddShield(target);
        Debug.Log($"J{playerId} protege: {target.DisplayName}");
        OnShieldPlaced?.Invoke(playerId, target);
        return true;
    }

    public bool TryAttack(int attackerId, int defenderId, CardData targetCard)
    {
        if (!IsActivePlayer(attackerId)) return false;
        if (Phase != GamePhase.PlayerTurn) return false;
        if (attackerId == defenderId) return false;

        var defender = GetPlayer(defenderId);
        if (!defender.OwnsCard(targetCard))
        {
            Debug.Log("El defensor no posee esa carta.");
            return false;
        }

        Phase = GamePhase.Duel;
        var request = new DuelRequest(attackerId, defenderId, targetCard);
        Debug.Log($"J{attackerId} ataca a J{defenderId} por: {targetCard.DisplayName}");
        OnDuelStarted?.Invoke(request);
        return true;
    }

    // Llamado por DuelController o UI cuando ambos jugadores enviaron sus cartas.
    public void ResolveDuel(DuelSubmission attackerSub, DuelSubmission defenderSub, CardData targetCard)
    {
        var attacker = GetPlayer(attackerSub.PlayerId);
        var defender = GetPlayer(defenderSub.PlayerId);

        var result = duelController.Resolve(attacker, defender, attackerSub, defenderSub, targetCard);

        ApplyDuelResult(result, attacker, defender, targetCard);
        OnDuelResolved?.Invoke(result);

        Phase = GamePhase.PlayerTurn;
        Debug.Log($"Duelo resuelto. Ganador: J{result.WinnerId}");
    }

    private void ApplyDuelResult(DuelResult result, PlayerState attacker, PlayerState defender, CardData targetCard)
    {
        // Atacante siempre descarta todas sus cartas de duelo
        foreach (var c in result.AttackerCardsUsed)
        {
            attacker.RemoveFromHand(c);
            discardPile.Add(c);
        }

        if (result.AttackerWon)
        {
            // Defensor pierde todas sus cartas de duelo
            foreach (var c in result.DefenderCardsUsed)
            {
                defender.RemoveFromHand(c);
                discardPile.Add(c);
            }

            if (result.ShieldBroken)
            {
                defender.RemoveShield(targetCard);
                defender.MarkVulnerable(targetCard);
                Debug.Log($"Escudo roto. {targetCard.DisplayName} queda vulnerable.");
            }
            // Si no había escudo, el atacante gana la carta
            else if (!result.ShieldBroken)
            {
                defender.RemoveCard(targetCard);
                attacker.AddToHand(targetCard);
                Debug.Log($"J{attacker.PlayerNumber} gana {targetCard.DisplayName}");
            }
        }
        else
        {
            // Defensor gana
            int used = result.DefenderCardsUsed.Count;
            if (used <= 1)
            {
                // Conserva su carta
                Debug.Log("Defensor conserva su carta.");
            }
            else
            {
                // Descarta una (la última usada)
                var toDiscard = result.DefenderCardsUsed[used - 1];
                defender.RemoveFromHand(toDiscard);
                discardPile.Add(toDiscard);
                Debug.Log($"Defensor descarta: {toDiscard.DisplayName}");
            }

            if (result.ShieldBroken)
            {
                defender.RemoveShield(targetCard);
                Debug.Log($"Escudo de {targetCard.DisplayName} roto (defensor ganó, escudo destruido).");
            }
        }
    }

    // ── Win condition ─────────────────────────────────────────────────────────
    private void CheckRoundWin(PlayerState player)
    {
        var trioId = player.GetCompletedTrio();
        if (trioId == null) return;

        player.ClaimTrio(trioId);
        Debug.Log($"J{player.PlayerNumber} completa el trío: {trioId}");
        OnRoundWon?.Invoke(player.PlayerNumber);

        if (player.CompletedTrios.Count >= 3)
        {
            Phase = GamePhase.GameOver;
            Debug.Log($"¡J{player.PlayerNumber} GANA la partida!");
            OnGameWon?.Invoke(player.PlayerNumber);
            return;
        }

        // Nueva ronda
        StartNewRound();
    }

    private void StartNewRound()
    {
        Phase = GamePhase.Setup;
        Debug.Log("── Nueva ronda ──");

        // Recolectar cartas que no son el triple ganador y rebarajar
        var toReshuffle = new List<CardData>();

        foreach (var p in players)
        {
            toReshuffle.AddRange(p.Hand);
            toReshuffle.AddRange(p.SpecialsInFront);
            p.ResetForNewRound();
        }

        while (deck.Count > 0) toReshuffle.Add(deck.Draw());
        while (discardPile.Count > 0) toReshuffle.Add(discardPile.DrawTop());

        deck = new Deck(toReshuffle);
        DealInitialHands();
        DetermineFirstPlayer();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private bool IsActivePlayer(int playerId) => Phase == GamePhase.PlayerTurn && playerId == CurrentPlayerNumber;
    private PlayerState GetPlayer(int playerNumber) => players[playerNumber - 1];

    private void ForceSetPlayer(int playerNumber)
    {
        if (playerNumber < 1 || playerNumber > totalPlayers) return;
        StartTurn(playerNumber);
    }

    private void LogAllHands()
    {
        foreach (var p in players)
        {
            Debug.Log($"{p} | Mano: [{string.Join(", ", p.Hand)}] | Especiales: [{string.Join(", ", p.SpecialsInFront)}]");
        }
    }

    // ── Getters para UI (Equipo 4) ────────────────────────────────────────────
    public int GetHandCount(int playerNumber) => GetPlayer(playerNumber).Hand.Count;
    public List<CardData> GetHand(int playerNumber) => GetPlayer(playerNumber).Hand;
    public int GetTotalPlayers() => totalPlayers;
}
