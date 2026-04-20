using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public void OnInteract()
    {
        Debug.Log($"Click en: {gameObject.name}");
    }
}