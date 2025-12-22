using System;
using Game.Interfaces;
using UnityEngine;

public class PlayerStamina : MonoBehaviour, IPlayerStamina
{
    public event Action<float> OnStaminaChanged;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 5f;
    [SerializeField] private float regenDelay = 2f;
    private float coolDownTime;
    private float currentStamina;
    private bool IsExhausted;

    public float CurrentStamina => currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina);
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        coolDownTime = regenDelay;

        if (currentStamina < 0f)
        {
            currentStamina = 0f;
            IsExhausted = true;
        }
        OnStaminaChanged?.Invoke(currentStamina);
    }

    public void RegenStamina()
    {
        if (coolDownTime > 0f)
        {
            coolDownTime -= Time.deltaTime;
            if (coolDownTime < 0f) coolDownTime = 0f;
            return;
        }

        if (IsExhausted) 
            IsExhausted = false;
            

        if (!IsExhausted && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }
}
