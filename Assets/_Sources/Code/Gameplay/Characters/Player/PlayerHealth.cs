// using System;
// using Game.Interfaces;
// using UnityEngine;

// public class PlayerHealth : MonoBehaviour, IPlayerHealth
// {
//     public event Action<float> OnHealthChanged;
//     [SerializeField] private float maxHealth = 100f;
//     private float currentHealth;

//     private void Awake()
//     {
//         currentHealth = maxHealth;
//         OnHealthChanged?.Invoke(currentHealth);
//     }
//     public void TakeDamage(float amount)
//     {
//         currentHealth -= amount;
//         if (currentHealth < 0) currentHealth = 0;
//         OnHealthChanged?.Invoke(currentHealth);

//         Debug.Log("TakeDamage вызвался");
//     }
//     public float CurrentHealth => currentHealth;
//     public float MaxHealth => maxHealth;
// }
