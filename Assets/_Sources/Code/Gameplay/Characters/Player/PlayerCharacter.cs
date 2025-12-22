using Game.Interfaces;
using Game.States;
using UnityEngine;
using Game.Characters;
using Game.Controllers;
using Game.Gameplay.Characters;

namespace Game.Managers
{
    public class PlayerCharacter : Entity
    {
        [SerializeField] private GroundMover _mover;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerStamina _stamina;

        [Header("Camera")]
        [SerializeField] private CameraController _camera;

        private IInputManager _input;
        
        public void Construct(IInputManager input)
        {
            _input = input;

            _mover.Construct(input, _stamina);
            _playerAnimator.Construct(input, _mover);
            _camera.Construct(input);
        }
        void OnEnable()
        {
            _mover.OnJump += _playerAnimator.HandleJump;        
        }
        void OnDisable()
        {
            _mover.OnJump -= _playerAnimator.HandleJump;        
        }

        void Update()
        {
            _playerAnimator.UpdateState();
        }
    }
}
