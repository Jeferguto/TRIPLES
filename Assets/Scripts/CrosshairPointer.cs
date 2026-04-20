using UnityEngine;
using UnityEngine.UI;

public class CrosshairPointer : MonoBehaviour
{
    public static CrosshairPointer Instance { get; private set; }

    [Header("Visual")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private float hoverScale = 1.3f;

    [Header("Raycast")]
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask interactableLayer = ~0;

    private Camera cam;
    private IInteractable currentTarget;
    private Outline currentOutline;
    private Vector3 defaultScale;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        cam = Camera.main;
        if (crosshairImage != null)
            defaultScale = crosshairImage.transform.localScale;
    }

    private void Update()
    {
        RaycastForTarget();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
            currentTarget.OnInteract();
    }

    private void RaycastForTarget()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                SetHover(interactable);
                return;
            }
        }

        ClearHover();
    }

    private void SetHover(IInteractable target)
    {
        if (target == currentTarget) return;

        ClearHover();

        currentTarget = target;

        var mono = target as MonoBehaviour;
        if (mono != null)
        {
            currentOutline = mono.GetComponentInChildren<Outline>();
            if (currentOutline != null) currentOutline.enabled = true;
        }

        if (crosshairImage == null) return;
        crosshairImage.color = hoverColor;
        crosshairImage.transform.localScale = defaultScale * hoverScale;
    }

    private void ClearHover()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }

        currentTarget = null;
        if (crosshairImage == null) return;
        crosshairImage.color = defaultColor;
        crosshairImage.transform.localScale = defaultScale;
    }
}
