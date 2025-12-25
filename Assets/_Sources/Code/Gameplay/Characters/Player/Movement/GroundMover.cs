using Sources.Code.Interfaces;
using UnityEngine;

namespace Game.Characters 
{
    public class GroundMover : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float forwardSpeed = 6f;
        [SerializeField] private float backwardSpeed = 4f;
        [SerializeField] private float strafeSpeed = 5f;
        [SerializeField] private float speedRun = 10f;
        [SerializeField] private float staminaDrainRate = 10f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private CharacterController player;
        public float MaxSpeed => speedRun;

        [Header("Jump and gravity specifics")]
        [SerializeField] private float gravity = 20f;
        [SerializeField] private float fallMultiplierJump = 2f;
        [SerializeField] private float fallMultiplierFall = 1.2f;
        [SerializeField] private float groundCheckDistance = 0.15f;
        [SerializeField] private float maxFallSpeed = 50f;
        [SerializeField] private float maxGroundAngle = 60f;
        [SerializeField] private bool active = true;
        private float verticalVelocity = -2f;
        public bool IsGrounded { get; private set; }

        private IInputManager _input;
        private PlayerStamina _stamina;

        public void SetActive(bool value) => active = value;

        public void Construct(IInputManager input, PlayerStamina stamina)
        {
            _input = input;
            _stamina = stamina;
        }

        void Update()
        {
            if (!active) return;

            DoMove();        
            ApplyGravity();   
            _stamina.RegenStamina(); 
        }

        public void DoMove()
        {            
            Vector2 input = new Vector2(_input.Horizontal, _input.Vertical);
            bool running = _input.SprintPressed && _stamina.CurrentStamina > 0f;
            bool isMoving = input.sqrMagnitude > 0.01f;

            MovePlayer(running, input);

            if (running && isMoving)
            {
                float speedFactor = Mathf.Clamp01(input.magnitude); 
                float drainAmount = staminaDrainRate * speedFactor * Time.deltaTime;
                _stamina.UseStamina(drainAmount);
            }
        }

        private void MovePlayer(bool isRunning, Vector2 input)
        {
            if (player == null || playerCamera == null) return;
            
            Vector3 forward = playerCamera.transform.forward;
            Vector3 right = playerCamera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            float speedY = 0f;
            if (input.y > 0)
                speedY = isRunning ? speedRun : forwardSpeed;
            else if (input.y < 0)
                speedY = isRunning ? speedRun : backwardSpeed;

            float speedX = input.x != 0 ? (isRunning ? speedRun : strafeSpeed) : 0f;

            Vector3 moveDirection = forward * input.y * speedY + right * input.x * speedX;
            player.Move(moveDirection * Time.deltaTime);
        }





        public void ApplyGravity()
        {
            if (!active) return;
            if (player == null) return;
            if (!player.enabled) return;

            IsGrounded = CheckGrounded();

            if (IsGrounded)
            {
                if (verticalVelocity < 0f) 
                    verticalVelocity = -2f;
            }
            else
            {
                if (verticalVelocity > 0f)
                    verticalVelocity -= gravity * fallMultiplierJump * Time.deltaTime;
                else
                    verticalVelocity -= gravity * fallMultiplierFall * Time.deltaTime;
            }

            verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, maxFallSpeed);

            Vector3 move = Vector3.up * verticalVelocity * Time.deltaTime;
            player.Move(move);
        }

        public bool CheckGrounded()
        {
            if (player == null) return false;

            Vector3 origin = player.transform.position + Vector3.up * 0.05f;
            float radius = player.radius * 0.9f;
            Vector3[] offsets = 
            { 
                Vector3.zero, 
                Vector3.forward * radius, 
                Vector3.back * radius, 
                Vector3.left * radius, 
                Vector3.right * radius 
            };

            foreach (var offset in offsets)
            {
                Vector3 rayOrigin = origin + offset;
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) <= maxGroundAngle)
                        return true;
                }
            }

            return false;
        }
        public float CurrentSpeed
        {
            get
            {
                if (player == null) return 0f;
                Vector3 v = player.velocity;
                v.y = 0f;
                return v.magnitude;
            }
        }
    }
}
