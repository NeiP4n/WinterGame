using Sources.Code.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private IPlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GetComponentInParent<IPlayerHealth>();
    }
    void Start()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
        playerHealth.OnHealthChanged += UpdateHealthUI;
    }
    private void UpdateHealthUI(float current)
    {
        healthSlider.value = current;
    }
    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
    }
}
