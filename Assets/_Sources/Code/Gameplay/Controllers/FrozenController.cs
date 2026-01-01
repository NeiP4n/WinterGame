using UnityEngine;
using Sources.Characters;
using System;

public class FrozenController : MonoBehaviour
{
    public event Action<float> OnFreezeChanged;

    public float FreezeAmount => _freezeAmount;

    public float NormalizedFreeze =>
        _settings == null || _settings.maxFreeze <= 0f
            ? 0f
            : _freezeAmount / _settings.maxFreeze;

    private float _freezeAmount;
    private bool _insideZone;
    private FreezeSettings _settings;
    private float _unfreezeTimer;

    private GroundMover _mover;
    private float _baseSpeedMultiplier;

    private void Awake()
    {
        _mover = GetComponentInParent<GroundMover>();
        if (_mover != null)
            _baseSpeedMultiplier = _mover.SpeedMultiplier;

        _freezeAmount = 0f;
        _unfreezeTimer = 0f;
    }

    private void Update()
    {
        if (_settings == null || !_settings.freezeEnabled)
            return;

        UpdateFreezeValue();
        ApplyFreeze();
        OnFreezeChanged?.Invoke(NormalizedFreeze);
    }

    private void UpdateFreezeValue()
    {
        float delta = Time.deltaTime;

        if (_insideZone)
        {
            _freezeAmount += delta * GetFreezeSpeed();
            _unfreezeTimer = 0f;
        }
        else
        {
            if (_settings.unfreezeDelay > 0f && _unfreezeTimer < _settings.unfreezeDelay)
            {
                _unfreezeTimer += delta;
            }
            else
            {
                _freezeAmount -= delta * _settings.recoverSpeed;
            }
        }

        _freezeAmount = Mathf.Clamp(_freezeAmount, 0f, _settings.maxFreeze);
    }

    private float GetFreezeSpeed()
    {
        float speed = _settings.freezeSpeed;
        if (_settings.fastFreeze)
            speed *= _settings.fastFreezeMultiplier;
        return speed;
    }

    private void ApplyFreeze()
    {
        if (_mover == null || !_settings.affectMovement)
            return;

        float t = NormalizedFreeze;
        float frozenMultiplier = Mathf.Lerp(_baseSpeedMultiplier, 0f, t);
        _mover.SetSpeedMultiplier(frozenMultiplier);
    }

    public void EnterFreezeZone(FreezeSettings settings)
    {
        _settings = settings;
        _insideZone = true;
        _unfreezeTimer = 0f;
        OnFreezeChanged?.Invoke(NormalizedFreeze);
    }

    public void ExitFreezeZone()
    {
        _insideZone = false;
        OnFreezeChanged?.Invoke(NormalizedFreeze);
    }
}
