using UnityEngine;
using Sources.Code.Interfaces;

namespace Gameplay.Interaction 
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator animator;
        private bool _isOpen;

        public bool CanInteract => true;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        public void Interact()
        {
            _isOpen = !_isOpen;

            if (animator != null)
                animator.SetBool("IsOpen", _isOpen);
        }
    }
}