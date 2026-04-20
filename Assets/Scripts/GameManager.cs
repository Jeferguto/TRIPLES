using UnityEngine;

/// <summary>
/// GameManager
/// Este script es el "director de orquesta" de toda la escena
/// Controla qué jugador es activo, cambia entre jugadores y gestiona el flujo del juego
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private int currentPlayerIndex = 0; // Índice del jugador actual (0-3)
    [SerializeField] private Camera mainCamera;
    
    private Transform cameraRig; // El "cuerpo" del jugador (rotaciones horizontales)
    private CameraController cameraController; // El script de control
    private PlayerPositionManager playerPositionManager;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Patrón Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerPositionManager = PlayerPositionManager.Instance;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        cameraRig = mainCamera.transform.parent;
        if (cameraRig == null)
        {
            Debug.LogError("La cámara debe estar dentro de un objeto padre (CameraRig) para rotar horizontalmente");
            return;
        }

        cameraController = mainCamera.GetComponent<CameraController>();

        // Posicionar el jugador inicial (Jugador 1) - posición fija para el jugador local
        SetCurrentPlayer(1);

        Debug.Log("GameManager inicializado. Jugador 1 activo (vista local).");
    }

    private void Update()
    {
        // Permitir cambiar de jugador con las teclas numéricas 1, 2, 3, 4
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCurrentPlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetCurrentPlayer(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetCurrentPlayer(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetCurrentPlayer(4);

        // Tecla ESC para desbloquear el mouse (útil para cerrar el juego)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    /// <summary>
    /// Cambia al jugador especificado
    /// </summary>
    public void SetCurrentPlayer(int playerNumber)
    {
        if (playerNumber < 1 || playerNumber > 4)
        {
            Debug.LogError($"Número de jugador inválido: {playerNumber}");
            return;
        }

        currentPlayerIndex = playerNumber - 1;
        var playerPos = playerPositionManager.GetPlayerPosition(playerNumber);

        if (playerPos != null)
        {
            // Mover la cámara a la posición del nuevo jugador
            cameraRig.position = playerPos.cameraPosition;
            cameraRig.rotation = Quaternion.Euler(playerPos.cameraRotation);

            // Resetear la rotación inicial del CameraController para que mire correctamente
            cameraController.SetInitialRotation(cameraRig.rotation);

            Debug.Log($"Cambiado a {playerPos.playerName}");
        }
    }

    /// <summary>
    /// Obtiene el número del jugador actual
    /// </summary>
    public int GetCurrentPlayer()
    {
        return currentPlayerIndex + 1;
    }
}
