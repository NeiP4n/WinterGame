using UnityEngine;
using Sources.Characters;
using System.Collections.Generic;
using Sources.Controllers;
using Game.Managers;

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    [System.Serializable]
    private class PlayerState
    {
        public float speedMultiplier;
        public bool sprintEnabled;
    }

    [SerializeField] private TriggerZoneConfig config;

    private bool triggered;
    private Dictionary<GroundMover, PlayerState> playerStates = new();

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && config.oneShot)
            return;

        if (!IsPlayer(other, out var player))
            return;

        SavePlayerState(player);
        ApplyEnterEffects(player);
        ApplyFreeze(player);

        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other, out var player))
            return;

        RestorePlayerState(player);
        RemoveFreeze(player);
    }

    private bool IsPlayer(Collider other, out PlayerCharacter player)
    {
        player = other.GetComponentInParent<PlayerCharacter>();
        return player != null;
    }

    private void SavePlayerState(PlayerCharacter player)
    {
        var mover = player.GetComponentInChildren<GroundMover>();
        if (mover == null) return;

        if (!playerStates.ContainsKey(mover))
        {
            playerStates[mover] = new PlayerState
            {
                speedMultiplier = mover.SpeedMultiplier,
                sprintEnabled = mover.SprintEnabled
            };
        }
    }

    private void ApplyEnterEffects(PlayerCharacter player)
    {
        var mover = player.GetComponentInChildren<GroundMover>();
        if (mover != null && config.movement.overrideMovement)
        {
            mover.SetSpeedMultiplier(config.movement.speedMultiplier);
            mover.SetSprintEnabled(!config.movement.disableSprint);
        }

        var camera = player.GetComponentInChildren<CameraController>();
        if (camera != null)
            camera.Apply(config.camera);

        var post = FindFirstObjectByType<PostProcessController>();
        if (post != null)
        {
            post.ApplyVisual(config.visual);
            post.ApplyPost(config.postEffects);
        }
    }

    private void RestorePlayerState(PlayerCharacter player)
    {
        var mover = player.GetComponentInChildren<GroundMover>();
        if (mover != null && playerStates.TryGetValue(mover, out var state))
        {
            mover.SetSpeedMultiplier(state.speedMultiplier);
            mover.SetSprintEnabled(state.sprintEnabled);
            playerStates.Remove(mover);
        }

        var camera = player.GetComponentInChildren<CameraController>();
        if (camera != null)
            camera.Restore();

        var post = FindFirstObjectByType<PostProcessController>();
        if (post != null)
            post.Restore();
    }

    private void ApplyFreeze(PlayerCharacter player)
    {
        if (!config.freeze.freezeEnabled)
            return;

        var freeze = player.GetComponent<FrozenController>();
        if (freeze != null)
        {
            freeze.EnterFreezeZone(config.freeze);
        }
    }

    private void RemoveFreeze(PlayerCharacter player)
    {
        var freeze = player.GetComponent<FrozenController>();
        if (freeze != null)
        {
            freeze.ExitFreezeZone();
        }
    }
}
