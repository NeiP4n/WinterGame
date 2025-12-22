namespace Game.Interfaces
{
    public interface IInputManager
    {
        public float Horizontal { get; }
        public float Vertical { get; }

        public bool JumpPressed { get; }
        public bool SprintPressed { get; }
        public bool CrouchPressed { get; }
        public bool RagdollPressed { get; }

        public bool InteractPressed { get; }
        public bool GrabPressed { get; }
        public bool ThrowPressed { get; }
        public bool LeftClickPressed { get; }
        public bool RightClickPressed { get; }
    }
}