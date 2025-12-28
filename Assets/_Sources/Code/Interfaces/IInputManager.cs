namespace Sources.Code.Interfaces
{
    public interface IInputManager
    {
        float Horizontal { get; }
        float Vertical { get; }

        bool SprintPressed { get; }
        bool RagdollPressed { get; }

        bool ConsumeJump();
        bool ConsumeCrouch();
        bool ConsumeInteract();
        bool ConsumeThrow();
        bool ConsumeLeftClick();
        bool ConsumeRightClick();
        bool ConsumeCancel();

    }
}
