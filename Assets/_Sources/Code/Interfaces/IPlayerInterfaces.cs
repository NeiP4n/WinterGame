using System;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IPlayerJump
    {
        void Jump();
        void HandleInput(IInputManager inputManager);
    }
    public interface IJumpableImpulse
    {
        void AddJumpImpulse(float jumpPower);
    }
    public interface IPlayerMovement
    {
        void HandleInput(IInputManager inputManager);
    }
    public interface IGroundChecker
    {
        bool IsGrounded { get; }
    }
    public interface IGravityHandler
    {
        bool IsGrounded { get; }
        float VerticalVelocity { get; }
        void ApplyGravity();
        void SetActive(bool value);
    }
    public interface IPlayerStamina
    {
        float CurrentStamina { get; }
        void UseStamina(float amount);
        void RegenStamina();
        event Action<float> OnStaminaChanged;
    }
    public interface IPlayerPickup
    {
        void UpdateHeldObjectPosition();
        void TryPickup();
        void TryThrow();
    }
    public interface IPlayerController
    {
        Transform GetHeadTransform();
        ulong GetClientId();
    }
    public interface IPlayerInteract
    {
        void HandleInput(IInputManager inputManager); 
    }
    public interface IPlayerHealth
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }
        event Action<float> OnHealthChanged;
        void TakeDamage(float amount);
    }
    public interface IInteractable
    {
        void Interact();
        public event Action<bool> OnItemDetected;
    }
}
