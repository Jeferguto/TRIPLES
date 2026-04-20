using UnityEngine;

/// <summary>
/// PlayerPositionManager
/// Este script define y gestiona las 4 posiciones de jugador alrededor de la mesa
/// Las posiciones están estratégicamente colocadas para una vista FPV sentado a la mesa
/// </summary>
public class PlayerPositionManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPosition
    {
        public int playerNumber;           // Número del jugador (1, 2, 3, 4)
        public Vector3 cameraPosition;     // Posición donde se ubica la cámara del jugador
        public Vector3 cameraRotation;     // Rotación de la cámara (qué mira el jugador)
        public Vector3 handPosition;       // Posición donde aparecen las cartas de la mano
        public string playerName;          // Nombre del jugador
    }

    [SerializeField] private PlayerPosition[] playerPositions = new PlayerPosition[4];
    public static PlayerPositionManager Instance { get; private set; }

    private void Awake()
    {
        // Patrón Singleton: asegurar que solo existe una instancia de este manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePlayerPositions();
    }

    /// <summary>
    /// Inicializa las posiciones de los 4 jugadores alrededor de la mesa
    /// Imagine la mesa como un reloj: Jugador 1 arriba, 2 derecha, 3 abajo, 4 izquierda
    /// </summary>
    private void InitializePlayerPositions()
    {
        // Jugador 1 - Posición NORTE (arriba)
        playerPositions[0] = new PlayerPosition
        {
            playerNumber = 1,
            cameraPosition = new Vector3(0f, 2f, 3f),
            cameraRotation = new Vector3(0f, 180f, 0f),  // Mirando hacia la mesa
            handPosition = new Vector3(0f, 0.5f, 2.5f),
            playerName = "Jugador 1 (Norte)"
        };

        // Jugador 2 - Posición ESTE (derecha)
        playerPositions[1] = new PlayerPosition
        {
            playerNumber = 2,
            cameraPosition = new Vector3(3f, 2f, 0f),
            cameraRotation = new Vector3(0f, 270f, 0f), // Mirando hacia la mesa
            handPosition = new Vector3(2.5f, 0.5f, 0f),
            playerName = "Jugador 2 (Este)"
        };

        // Jugador 3 - Posición SUR (abajo)
        playerPositions[2] = new PlayerPosition
        {
            playerNumber = 3,
            cameraPosition = new Vector3(0f, 2f, -3f),
            cameraRotation = new Vector3(0f, 0f, 0f),  // Mirando hacia la mesa
            handPosition = new Vector3(0f, 0.5f, -2.5f),
            playerName = "Jugador 3 (Sur)"
        };

        // Jugador 4 - Posición OESTE (izquierda)
        playerPositions[3] = new PlayerPosition
        {
            playerNumber = 4,
            cameraPosition = new Vector3(-3f, 2f, 0f),
            cameraRotation = new Vector3(0f, 90f, 0f),  // Mirando hacia la mesa
            handPosition = new Vector3(-2.5f, 0.5f, 0f),
            playerName = "Jugador 4 (Oeste)"
        };
    }

    /// <summary>
    /// Obtiene la posición de un jugador específico
    /// </summary>
    public PlayerPosition GetPlayerPosition(int playerNumber)
    {
        if (playerNumber >= 1 && playerNumber <= 4)
        {
            return playerPositions[playerNumber - 1];
        }
        Debug.LogError($"Número de jugador inválido: {playerNumber}. Debe ser entre 1 y 4.");
        return null;
    }

    /// <summary>
    /// Obtiene todas las posiciones de los jugadores
    /// </summary>
    public PlayerPosition[] GetAllPlayerPositions()
    {
        return playerPositions;
    }

    /// <summary>
    /// Muestra visualmente en el editor las posiciones de los jugadores
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Solo mostrar durante la ejecución

        for (int i = 0; i < playerPositions.Length; i++)
        {
            if (playerPositions[i] != null)
            {
                // Dibujar una esfera para cada posición de jugador
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(playerPositions[i].cameraPosition, 0.5f);

                // Dibujar texto en el editor (opcional)
                UnityEditor.Handles.Label(playerPositions[i].cameraPosition, playerPositions[i].playerName);
            }
        }
    }
}
