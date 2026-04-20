using UnityEngine;

public class CardSelectorUI : MonoBehaviour
{
    public static CardSelectorUI Instance { get; private set; }

    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private Camera mainCamera;

    public CardData SelectedCard { get; private set; }
    private CardVisual selectedVisual;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Phase != GamePhase.PlayerTurn) return;

        HandleHover();

        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    private void HandleHover()
    {
        // Solo resalta la carta bajo el cursor sin seleccionarla
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask))
        {
            var visual = hit.collider.GetComponentInParent<CardVisual>();
            if (visual != null && visual != selectedVisual)
                visual.SetHighlight(true);
        }
    }

    private void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Deseleccionar la anterior
        if (selectedVisual != null)
        {
            selectedVisual.SetHighlight(false);
            selectedVisual = null;
            SelectedCard = null;
            UIManager.Instance?.SetProtectAttackVisible(false);
        }

        if (!Physics.Raycast(ray, out RaycastHit hit, 10f, cardLayerMask)) return;

        var clicked = hit.collider.GetComponentInParent<CardVisual>();
        if (clicked == null || clicked.data == null) return;

        selectedVisual = clicked;
        SelectedCard = clicked.data;
        clicked.SetHighlight(true);

        UIManager.Instance?.SetProtectAttackVisible(true);
        Debug.Log($"Carta seleccionada: {SelectedCard.DisplayName}");
    }

    public void ClearSelection()
    {
        if (selectedVisual != null) selectedVisual.SetHighlight(false);
        selectedVisual = null;
        SelectedCard = null;
        UIManager.Instance?.SetProtectAttackVisible(false);
    }
}
