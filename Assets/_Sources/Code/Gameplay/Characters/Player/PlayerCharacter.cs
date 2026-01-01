using UnityEngine;
using Sources.Characters;
using Sources.Controllers;
using Game.Gameplay.Characters;
using Sources.Code.Interfaces;
using Sources.Code.Gameplay.Interaction;

namespace Game.Managers
{
    public class PlayerCharacter : Entity
    {
        [SerializeField] private GroundMover _mover;
        [SerializeField] private PlayerInteract _interact;

        [Header("Camera")]
        [SerializeField] private CameraController _camera;
        private IInputManager _input;
        public PlayerInteract Interact => _interact;

        public void Construct(IInputManager input)
        {
            _input = input;

            _mover.Construct(input);
            _camera.Construct(input);
            _interact.Construct(input);
        }
        void Update()
        {
            if (_input.IsLocked)
                return;
            _interact.UpdateInteract();
        }
    }
}
