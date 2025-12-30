namespace Sources.Code.Config.Audio
{
    // ---------- FOOTSTEPS / SURFACES ----------
    public enum SurfaceType
    {
        Default,
        Wood,
        Metal,
        Snow
    }

    // ---------- UI ----------
    public enum UISoundType
    {
        Click,
        Hover,
        Confirm,
        Cancel
    }

    // ---------- WORLD / INTERACTIONS ----------
    public enum InteractionSoundType
    {
        DoorOpen,
        DoorClose,
        LeverPull,
        ButtonPress,
        PickupItem,
        UseTerminal
    }
    // ---------- PLAYER ----------
    public enum PlayerSoundType
    {
        Footstep,
        Jump,
        Land,
        Hurt
    }

    // ---------- AMBIENCE / WORLD ----------
    public enum WorldSoundType
    {
        Wind,
        Rain,
        SnowStorm,
        Fire
    }
}
