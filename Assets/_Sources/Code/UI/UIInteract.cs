using UnityEngine;
using TMPro;
using Game.Interfaces;

namespace Game.UI
{
    public class UIInteract : MonoBehaviour, IUIInteract
    {
        [Header("Interact UI Elements")]
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text interactText;
        private IInteractable interactable;

        [Header("Settings")]
        public string message = "Press E to interact";

        public void Init(IInteractable interactable)
        {
            if (interactable != null)
            {
                interactable.OnItemDetected += HandleItemDetected;
            }

            HideTextMessage();
        }

        private void HandleItemDetected(bool show)
        {
            if (show) ShowTextMessage();
            else HideTextMessage();
        }

        public void ShowTextMessage()
        {
            if (root != null)
                root.SetActive(true);

            if (interactText != null)
                interactText.text = message;
        }

        public void HideTextMessage()
        {
            if (root != null)
                root.SetActive(false);
        }
    }
}