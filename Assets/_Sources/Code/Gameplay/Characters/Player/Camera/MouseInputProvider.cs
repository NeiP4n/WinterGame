using UnityEngine;
using UnityEngine.InputSystem;
using Game.Interfaces;
public class MouseInputProvider : ICameraInputProvider
    {
        private IInputManager _input;

        public MouseInputProvider(IInputManager input)
        {
            _input = input;
        }

        public Vector2 GetLookDelta()
        {
            return Mouse.current.delta.ReadValue();
        }

        public Vector2 GetMoveInput()
        {
            return new Vector2(_input.Horizontal, _input.Vertical);
        }
    }