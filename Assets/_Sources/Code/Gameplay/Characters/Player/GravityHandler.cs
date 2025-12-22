// using UnityEngine;
// using Game.Interfaces;

// namespace Game.Controllers
// {
//     public class GravityHandler : MonoBehaviour, IGravityHandler, IGroundChecker, IJumpableImpulse
//     {
//         [SerializeField] CharacterController player;
        

        
        
//         public void ApplyGravity()
//         {
//             if (!active) return;
//             if (player == null) return;
//             if (!player.enabled) return;

//             IsGrounded = CheckGrounded();

//             if (IsGrounded)
//             {
//                 if (verticalVelocity < 0f) 
//                     verticalVelocity = -2f;
//                 jumped = false;
//             }
//             else
//             {
//                 if (verticalVelocity > 0f && jumped)
//                     verticalVelocity -= gravity * fallMultiplierJump * Time.deltaTime;
//                 else
//                     verticalVelocity -= gravity * fallMultiplierFall * Time.deltaTime;
//             }

//             verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, maxFallSpeed);

//             Vector3 move = Vector3.up * verticalVelocity * Time.deltaTime;
//             player.Move(move);
//         }

//         public bool CheckGrounded()
//         {
//             if (player == null) return false;

//             Vector3 origin = player.transform.position + Vector3.up * 0.05f;
//             float radius = player.radius * 0.9f;
//             Vector3[] offsets = 
//             { 
//                 Vector3.zero, 
//                 Vector3.forward * radius, 
//                 Vector3.back * radius, 
//                 Vector3.left * radius, 
//                 Vector3.right * radius 
//             };

//             foreach (var offset in offsets)
//             {
//                 Vector3 rayOrigin = origin + offset;
//                 if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
//                 {
//                     if (Vector3.Angle(hit.normal, Vector3.up) <= maxGroundAngle)
//                         return true;
//                 }
//             }

//             return false;
//         }

//         public void AddJumpImpulse(float jumpPower)
//         {
//             verticalVelocity = Mathf.Sqrt(jumpPower * 2f * gravity);
//             jumped = true;
//         }
//     }
// }
