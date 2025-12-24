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
        [SerializeField] private PlayerStamina _stamina;

        [Header("Camera")]
        [SerializeField] private CameraController _camera;

        
        public void Construct(IInputManager input)
        {
            _mover.Construct(input, _stamina);
            _camera.Construct(input);
        }
    }
}
