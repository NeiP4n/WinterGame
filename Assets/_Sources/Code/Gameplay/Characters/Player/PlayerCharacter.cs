using UnityEngine;
using Game.Characters;
using Game.Controllers;
using Game.Gameplay.Characters;
using Sources.Code.Interfaces;

namespace Game.Managers
{
    public class PlayerCharacter : Entity
    {
        [SerializeField] private GroundMover _mover;
        [SerializeField] private PlayerStamina _stamina;
        [SerializeField] private PlayerInteract _interact;

        [Header("Camera")]
        [SerializeField] private CameraController _camera;

        public PlayerInteract Interact => _interact;

        public void Construct(IInputManager input)
        {
            _mover.Construct(input, _stamina);
            _camera.Construct(input);
            _interact.Construct(input);
        }
        void Update()
        {
            _interact.UpdateInteract();
        }
    }
}
