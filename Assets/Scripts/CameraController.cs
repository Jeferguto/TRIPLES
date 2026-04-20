using UnityEngine;

/// <summary>
/// CameraController
/// Este script controla la rotación de la cámara en estilo FPV (Primera Persona)
/// Solo permite rotación con el mouse, sin movimiento de posición
/// La rotación está limitada para mantener la vista enfocada en la mesa
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Rotación con Mouse")]
    [SerializeField] private float mouseSensitivity = 2f; // Cuán sensible es la cámara al mouse
    private float xRotation = 0f; // Ángulo vertical (arriba/abajo)

    [Header("Límites de Rotación")]
    [SerializeField] private float maxUpAngle = 30f;   // Máximo ángulo hacia arriba (mirar arriba)
    [SerializeField] private float maxDownAngle = -30f; // Máximo ángulo hacia abajo (mirar abajo)
    [SerializeField] private float maxHorizontalAngle = 45f; // Máximo giro horizontal desde la posición inicial

    private Quaternion initialRotation; // Rotación inicial para calcular límites horizontales
    private float currentHorizontalRotation = 0f; // Ángulo horizontal actual

    private void Start()
    {
        // Bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;

        // Guardar la rotación inicial
        initialRotation = transform.parent.rotation;
    }

    private void Update()
    {
        // Procesar rotación de cámara con mouse
        HandleMouseRotation();
    }

    /// <summary>
    /// Maneja la rotación de la cámara usando el mouse
    /// </summary>
    private void HandleMouseRotation()
    {
        // Obtener el movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Aplicar rotación horizontal (girar el cuerpo - limitado)
        currentHorizontalRotation += mouseX;
        currentHorizontalRotation = Mathf.Clamp(currentHorizontalRotation, -maxHorizontalAngle, maxHorizontalAngle);

        // Aplicar rotación vertical (inclinar la cabeza - limitado)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxDownAngle, maxUpAngle);

        // Aplicar las rotaciones
        transform.parent.rotation = initialRotation * Quaternion.Euler(0f, currentHorizontalRotation, 0f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    /// <summary>
    /// Resetea la rotación inicial cuando cambias de jugador
    /// Esto asegura que cada vista mire correctamente hacia la mesa
    /// </summary>
    public void SetInitialRotation(Quaternion newRotation)
    {
        initialRotation = newRotation;
        currentHorizontalRotation = 0f; // Resetear el giro horizontal
        xRotation = 0f; // Resetear el giro vertical
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
