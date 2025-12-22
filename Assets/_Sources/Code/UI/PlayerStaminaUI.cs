using UnityEngine;
using UnityEngine.UI;
using Game.Interfaces;

namespace Game.UI
{
    public class PlayerStaminaUI : MonoBehaviour
    {
        [SerializeField] private Slider staminaSlider;
        private IPlayerStamina playerStamina;

        void Awake()
        {
            playerStamina = GetComponentInParent<IPlayerStamina>();
        }
        void Start()
        {
            UpdateStaminaUI(playerStamina.CurrentStamina); 
        }

        private void UpdateStaminaUI(float current)
        {
            staminaSlider.value = current;
        }
        private void OnEnable()
        {
            playerStamina.OnStaminaChanged += UpdateStaminaUI;
        }

        private void OnDestroy()
        {
            playerStamina.OnStaminaChanged -= UpdateStaminaUI;
        }
    }
}
