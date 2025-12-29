using UnityEngine;

[CreateAssetMenu(fileName = nameof(TriggerZoneConfig), menuName = "Configs/" + nameof(TriggerZoneConfig), order = 0)]
public class TriggerZoneConfig : ScriptableObject
{
    [Header("Основные настройки")]
    public bool oneShot = false;                   
    [Header("🏃 ДВИЖЕНИЕ ИГРОКА")]
    public MovementSettings movement = new();

    [Header("📷 КАМЕРА")]
    public CameraSettings camera = new();

    [Header("🎵 ЗВУК")]
    public AudioSettings audio = new();

    [Header("✨ ВИЗУАЛ")]
    public VisualSettings visual = new();

    [Header("💫 ПОСТ-ЭФФЕКТЫ")]
    public PostEffectSettings postEffects = new();
}

[System.Serializable]
public class MovementSettings
{
    public bool overrideMovement = false;
    
    [Range(0.1f, 3f)] public float speedMultiplier = 1f;
    public bool disableSprint = false;
    public bool disableJump = false;
}


[System.Serializable]
public class CameraSettings
{
    public bool overrideCamera = false;

    [Header("Rotation")]
    public bool blockRotation = false;
    [Range(0.1f, 3f)]
    public float sensitivityMultiplier = 1f;

    [Header("FOV")]
    public bool overrideFov = false;
    [Range(30f, 120f)]
    public float fov = 60f;

    [Header("Shake")]
    public bool cameraShake = false;
    [Range(0f, 5f)]
    public float shakeIntensity = 1f;
}

[System.Serializable]
public class VisualSettings
{
    public Color overlayColor = new Color(0,0,0,0);
    [Range(0f, 1f)] public float overlayOpacity = 0f;

    public bool vignette = false;
    public bool chromaticAberration = false;

    [Range(0f, 10f)] public float blurAmount = 0f;
}

[System.Serializable]
public class PostEffectSettings
{
    public Color colorTint = Color.white;
    [Range(-100f, 100f)] public float saturation = 0f;
    [Range(-100f, 100f)] public float contrast = 0f;
}
