using UnityEngine;

/// <summary>
/// PointerController
/// Este script controla un puntero visual que sigue al mouse
/// Útil para juegos FPV donde el cursor normal está oculto
/// </summary>
public class PointerController : MonoBehaviour
{
    [SerializeField] private RectTransform pointerRectTransform; // El RectTransform del puntero UI
    [SerializeField] private Canvas canvas; // El Canvas padre
    [SerializeField] private Camera uiCamera; // Cámara para el Canvas (opcional si es Screen Space Overlay)

    private void Start()
    {
        // Si no se asignó el RectTransform, buscarlo en este objeto
        if (pointerRectTransform == null)
        {
            pointerRectTransform = GetComponent<RectTransform>();
        }

        // Si no se asignó el Canvas, buscarlo en los padres
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        // Si no se asignó la cámara UI, usar la cámara principal
        if (uiCamera == null && canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Actualizar la posición del puntero para que siga al mouse
        UpdatePointerPosition();
    }

    /// <summary>
    /// Actualiza la posición del puntero para que siga al mouse
    /// </summary>
    private void UpdatePointerPosition()
    {
        if (pointerRectTransform == null || canvas == null) return;

        Vector2 mousePosition = Input.mousePosition;

        // Convertir la posición del mouse a coordenadas del Canvas
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Para Screen Space Overlay, la posición es directa
            pointerRectTransform.position = mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera && uiCamera != null)
        {
            // Para Screen Space Camera, necesitamos convertir
            Vector2 viewportPoint = uiCamera.ScreenToViewportPoint(mousePosition);
            pointerRectTransform.anchorMin = viewportPoint;
            pointerRectTransform.anchorMax = viewportPoint;
            pointerRectTransform.anchoredPosition = Vector2.zero;
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Para World Space, convertir a coordenadas del mundo
            Vector3 worldPoint = uiCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, canvas.planeDistance));
            pointerRectTransform.position = worldPoint;
        }
    }

    /// <summary>
    /// Muestra u oculta el puntero
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (pointerRectTransform != null)
        {
            pointerRectTransform.gameObject.SetActive(visible);
        }
    }
}
