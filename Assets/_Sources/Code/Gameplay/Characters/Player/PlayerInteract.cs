using UnityEngine;
using System;
using Sources.Code.Interfaces;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private IInputManager input;

    public float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask;

    private IInteractable current;
    public event Action<IInteractable> OnFocusChanged;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    public void Construct(IInputManager inputManager)
    {
        input = inputManager;
    }

    private void Update()
    {
        UpdateInteract();
    }

    public void UpdateInteract()
    {
        if (input == null)
            return;

        UpdateDetection();

        if (input.ConsumeInteract() && current != null && current.CanInteract)
        {
            current.Interact();
        }
    }

    private void UpdateDetection()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        IInteractable detected = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            detected = hit.collider.GetComponentInParent<IInteractable>();
        }

        if (detected != current)
        {
            current = detected;
            OnFocusChanged?.Invoke(current);
        }
    }
}
