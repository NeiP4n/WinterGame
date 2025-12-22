using UnityEngine;
using UnityEngine.InputSystem;
using Game.Interfaces;

namespace Game.Managers
{
    public class InputManager : MonoBehaviour, IInputManager
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        public bool JumpPressed { get; private set; }
        public bool CrouchPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool GrabPressed { get; private set; }
        public bool ThrowPressed { get; private set; }
        public bool SprintPressed { get; private set; }
        public bool RagdollPressed { get; private set; }

        public bool LeftClickPressed { get; private set; }
        public bool RightClickPressed { get; private set; }

        public void OnMove(InputAction.CallbackContext context)
        {            
            Vector2 move = context.ReadValue<Vector2>();
            Horizontal = move.x;
            Vertical = move.y;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed) JumpPressed = true;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintPressed = context.ReadValue<float>() > 0.5f;
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed) CrouchPressed = true;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) InteractPressed = true;
        }

        public void OnGrab(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GrabPressed = true;
                Debug.Log("Grab pressed (OnGrab performed)");
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ThrowPressed = true;
                Debug.Log("Throw pressed (OnThrow performed)");
            }
        }

        public void OnRagdoll(InputAction.CallbackContext context)
        {
            RagdollPressed = context.performed || context.started;
        }
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.performed) LeftClickPressed = true;
        }
        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed) RightClickPressed = true;
        }


        private void LateUpdate()
        {
            JumpPressed = false;
            InteractPressed = false;
            GrabPressed = false;
            ThrowPressed = false;
            LeftClickPressed = false;
            RightClickPressed = false;
        }
    }
}