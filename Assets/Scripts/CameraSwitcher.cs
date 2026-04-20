using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private Transform cameraRig;
    private CameraController cameraController;
    private PlayerPositionManager positionManager;

    public static CameraSwitcher Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        positionManager = PlayerPositionManager.Instance;

        if (mainCamera == null)
            mainCamera = Camera.main;

        cameraRig = mainCamera.transform.parent;
        cameraController = mainCamera.GetComponent<CameraController>();

        if (cameraRig == null)
            Debug.LogError("La cámara debe estar dentro de un CameraRig.");
    }

    public void SwitchTo(int playerNumber)
    {
        positionManager ??= PlayerPositionManager.Instance;

        if (positionManager == null)
        {
            Debug.LogWarning("CameraSwitcher: PlayerPositionManager.Instance is null.");
            return;
        }

        var pos = positionManager.GetPlayerPosition(playerNumber);
        if (pos == null) return;

        cameraRig.position = pos.cameraPosition;
        cameraRig.rotation = Quaternion.Euler(pos.cameraRotation);
        cameraController.SetInitialRotation(cameraRig.rotation);

        Debug.Log($"Cámara → {pos.playerName}");
    }
}
