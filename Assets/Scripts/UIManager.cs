using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI turnLabel;
    [SerializeField] private TextMeshProUGUI phaseLabel;
    [SerializeField] private TextMeshProUGUI deckCountLabel;

    [Header("Action Panel")]
    [SerializeField] private Button btnDraw;
    [SerializeField] private Button btnProtect;
    [SerializeField] private Button btnAttack;
    [SerializeField] private Button btnEndTurn;

    [Header("Panels")]
    [SerializeField] private GameObject duelPanel;
    [SerializeField] private GameObject roundEndPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageLabel;
    [SerializeField] private TextMeshProUGUI roundEndLabel;
    [SerializeField] private TextMeshProUGUI gameOverLabel;

    private float messageTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnEnable()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.OnPlayerTurnStarted += HandleTurnStarted;
        gm.OnCardDrawn         += HandleCardDrawn;
        gm.OnShieldPlaced      += HandleShieldPlaced;
        gm.OnDuelStarted       += HandleDuelStarted;
        gm.OnDuelResolved      += HandleDuelResolved;
        gm.OnRoundWon          += HandleRoundWon;
        gm.OnGameWon           += HandleGameWon;
    }

    private void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.OnPlayerTurnStarted -= HandleTurnStarted;
        gm.OnCardDrawn         -= HandleCardDrawn;
        gm.OnShieldPlaced      -= HandleShieldPlaced;
        gm.OnDuelStarted       -= HandleDuelStarted;
        gm.OnDuelResolved      -= HandleDuelResolved;
        gm.OnRoundWon          -= HandleRoundWon;
        gm.OnGameWon           -= HandleGameWon;
    }

    private void Start()
    {
        // Botones
        btnDraw?.onClick.AddListener(OnDrawPressed);
        btnEndTurn?.onClick.AddListener(OnEndTurnPressed);
        btnProtect?.onClick.AddListener(OnProtectPressed);
        btnAttack?.onClick.AddListener(OnAttackPressed);

        // Estado inicial
        SetActionButtonsEnabled(false);
        duelPanel?.SetActive(false);
        roundEndPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        messagePanel?.SetActive(false);
    }

    private void Update()
    {
        // Fade del mensaje flotante
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
                messagePanel?.SetActive(false);
        }

        // Sincronizar fase en HUD
        if (phaseLabel != null && GameManager.Instance != null)
            phaseLabel.text = GameManager.Instance.Phase.ToString();
    }

    // ── Handlers de eventos ──────────────────────────────────────────────────

    private void HandleTurnStarted(int playerNumber)
    {
        if (turnLabel != null)
            turnLabel.text = $"Turno del Jugador {playerNumber}";

        SetActionButtonsEnabled(true);
        btnProtect?.GetComponent<Button>()?.gameObject.SetActive(false); // requiere selección de carta
        btnAttack?.GetComponent<Button>()?.gameObject.SetActive(false);
    }

    private void HandleCardDrawn(int playerId, CardData card)
    {
        ShowMessage($"Jugador {playerId} roba: {card.DisplayName}");
    }

    private void HandleShieldPlaced(int playerId, CardData card)
    {
        ShowMessage($"Jugador {playerId} protege: {card.DisplayName}");
    }

    private void HandleDuelStarted(DuelRequest req)
    {
        ShowMessage($"¡Duelo! J{req.AttackerId} ataca a J{req.DefenderId}");
        duelPanel?.SetActive(true);
        SetActionButtonsEnabled(false);
    }

    private void HandleDuelResolved(DuelResult result)
    {
        duelPanel?.SetActive(false);
        SetActionButtonsEnabled(true);

        string msg = result.AttackerWon
            ? $"Jugador {result.AttackerId} gana el duelo"
            : $"Jugador {result.DefenderId} defiende";

        if (result.ShieldBroken) msg += " (¡Escudo roto!)";
        ShowMessage(msg, 3f);
    }

    private void HandleRoundWon(int playerNumber)
    {
        SetActionButtonsEnabled(false);
        if (roundEndLabel != null)
            roundEndLabel.text = $"¡Jugador {playerNumber} completa un trío!";
        roundEndPanel?.SetActive(true);
    }

    private void HandleGameWon(int playerNumber)
    {
        SetActionButtonsEnabled(false);
        if (gameOverLabel != null)
            gameOverLabel.text = $"¡Jugador {playerNumber} gana la partida!";
        gameOverPanel?.SetActive(true);
    }

    // ── Botones ──────────────────────────────────────────────────────────────

    private void OnDrawPressed()
    {
        int player = GameManager.Instance.CurrentPlayerNumber;
        GameManager.Instance.TryDraw(player);
    }

    private void OnEndTurnPressed()
    {
        GameManager.Instance.EndTurn();
        SetActionButtonsEnabled(false);
    }

    private void OnProtectPressed()
    {
        var selected = CardSelectorUI.Instance?.SelectedCard;
        if (selected == null) { ShowMessage("Selecciona una carta primero."); return; }

        int player = GameManager.Instance.CurrentPlayerNumber;
        GameManager.Instance.TryProtect(player, selected);
    }

    private void OnAttackPressed()
    {
        // Requiere carta seleccionada del oponente — CardSelectorUI lo gestiona
        var selected = CardSelectorUI.Instance?.SelectedCard;
        if (selected == null) { ShowMessage("Selecciona la carta a atacar."); return; }

        // Por ahora ataca siempre al jugador 2 (en local se pasa el dispositivo)
        // TODO: permitir elegir defensor con múltiples jugadores
        int attacker = GameManager.Instance.CurrentPlayerNumber;
        int defender = (attacker % GameManager.Instance.GetTotalPlayers()) + 1;
        GameManager.Instance.TryAttack(attacker, defender, selected);
    }

    // ── Botones de paneles ────────────────────────────────────────────────────

    public void OnRoundEndContinue()
    {
        roundEndPanel?.SetActive(false);
        SetActionButtonsEnabled(true);
    }

    public void OnGameOverRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    public void ShowMessage(string text, float duration = 2f)
    {
        if (messageLabel != null) messageLabel.text = text;
        messagePanel?.SetActive(true);
        messageTimer = duration;
    }

    public void SetActionButtonsEnabled(bool enabled)
    {
        if (btnDraw != null)    btnDraw.interactable = enabled;
        if (btnEndTurn != null) btnEndTurn.interactable = enabled;
    }

    public void SetProtectAttackVisible(bool visible)
    {
        btnProtect?.gameObject.SetActive(visible);
        btnAttack?.gameObject.SetActive(visible);
    }
}
