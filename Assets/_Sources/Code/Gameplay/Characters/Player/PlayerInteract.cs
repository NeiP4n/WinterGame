using Game.Interfaces;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IPlayerInteract
{
    private Camera playerCamera;
    public float interactDistance = 3f;
    public event System.Action<IInteractable> OnInteractableDetected;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    public void HandleInput(IInputManager inputManager)
    {
        if (inputManager.InteractPressed)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance))
        {
            if (hitInfo.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                OnInteractableDetected?.Invoke(interactable);
            }
            else
            {
                OnInteractableDetected?.Invoke(null);
            }
        }
        else
        {
            OnInteractableDetected?.Invoke(null);
        }
    }
}
