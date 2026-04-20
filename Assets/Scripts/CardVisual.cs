using UnityEngine;

public enum VisualState { Normal, Shielded, Vulnerable }

public class CardVisual : MonoBehaviour
{
    [Header("Datos de la carta")]
    public CardData data; // el Equipo 3 asigna esto al instanciar

    [Header("Referencias visuales")]
    public GameObject cardFront; // arrastra CardFront aquí en el Inspector
    public GameObject cardBack;  // arrastra CardBack aquí en el Inspector

    [Header("Tokens")]
    public GameObject tokenBlue; // arrastra el TokenBlue aquí (lo crearás después)
    public GameObject tokenRed;  // arrastra el TokenRed aquí

    private VisualState currentState = VisualState.Normal;

    // El GameManager llama esto según el estado de la carta
    public void SetVisualState(VisualState state)
    {
        currentState = state;

        if (tokenBlue != null) tokenBlue.SetActive(state == VisualState.Shielded);
        if (tokenRed  != null) tokenRed.SetActive(state == VisualState.Vulnerable);
    }

    // Muestra frente o dorso (true = frente visible)
    public void SetFaceUp(bool faceUp)
    {
        if (cardFront != null) cardFront.SetActive(faceUp);
        if (cardBack  != null) cardBack.SetActive(!faceUp);
    }

    // Resalta la carta cuando el jugador la selecciona
    public void SetHighlight(bool active)
    {
        var renderer = cardFront?.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = active ? Color.yellow : Color.white;
    }

    // Se llama cuando el GameManager dispara OnCardDrawn
    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCardDrawn += HandleCardDrawn;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCardDrawn -= HandleCardDrawn;
    }

    private void HandleCardDrawn(int playerId, CardData drawnCard)
    {
        // Solo reacciona si esta carta visual es la que se robó
        if (drawnCard == data)
            Debug.Log($"Carta robada: {data.DisplayName}"); // aquí irá la animación después
    }
}