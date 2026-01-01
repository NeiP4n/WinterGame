using UnityEngine;
using UnityEngine.UI;

public class FrostOverlayController : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private Slider freezeSlider;

    [Header("Intensity")]
    [SerializeField] private float maxAlpha = 0.9f;
    [SerializeField] private float appearSmooth = 5f;
    [SerializeField] private float sliderSmooth = 10f;

    private FrozenController _frozenController;
    private float _targetAlpha;
    private float _currentAlpha;
    private float _sliderVelocity;
    private bool _isSearching = false;

    private void Awake()
    {
        if (overlayImage != null)
        {
            var c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }

        if (freezeSlider != null)
        {
            freezeSlider.minValue = 0f;
            freezeSlider.maxValue = 100f;
            freezeSlider.value = 0f;
            freezeSlider.interactable = false;
        }
    }

    private void OnEnable()
    {
        TryFindPlayer();
    }

    private void OnDisable()
    {
        if (_frozenController != null)
            _frozenController.OnFreezeChanged -= HandleFreezeChanged;
    }

    private void Update()
    {
        if (_frozenController == null && !_isSearching)
        {
            TryFindPlayer();
        }

        if (_frozenController == null)
            return;

        if (overlayImage != null)
        {
            _currentAlpha = Mathf.Lerp(
                _currentAlpha,
                _targetAlpha,
                Time.deltaTime * appearSmooth
            );

            var c = overlayImage.color;
            c.a = _currentAlpha;
            overlayImage.color = c;
        }

        if (freezeSlider != null)
        {
            float targetValue = _frozenController.NormalizedFreeze * 100f;
            
            freezeSlider.value = Mathf.SmoothDamp(
                freezeSlider.value,
                targetValue,
                ref _sliderVelocity,
                1f / sliderSmooth
            );
        }
    }

    private void TryFindPlayer()
    {
        _isSearching = true;
        _frozenController = FindFirstObjectByType<FrozenController>();
        
        if (_frozenController != null)
        {
            _frozenController.OnFreezeChanged += HandleFreezeChanged;
        }
        
        _isSearching = false;
    }

    private void HandleFreezeChanged(float normalizedFreeze)
    {
        _targetAlpha = Mathf.Lerp(0f, maxAlpha, normalizedFreeze);
    }
}
