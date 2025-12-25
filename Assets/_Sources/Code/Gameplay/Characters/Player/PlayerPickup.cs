using Sources.Code.Interfaces;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerPickup : MonoBehaviour, IPlayerPickup
    {
        [SerializeField] private Transform holdArea;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private Vector3 holdOffset = new Vector3(0, 0.5f, 0);
        [SerializeField] private float maxPitchAngle = 60f;

        private Camera playerCamera;
        private CharacterController playerController;

        private GameObject heldObj;
        private Rigidbody heldObjRb;
        private Collider heldObjCol;

        void Awake()
        {
            playerCamera = GetComponentInChildren<Camera>();
            playerController = GetComponentInChildren<CharacterController>();
        }

        public void UpdateHeldObjectPosition()
        {
            if (heldObj != null)
            {
                Vector3 forward = playerCamera.transform.forward;
                forward.y = Mathf.Clamp(forward.y, -Mathf.Sin(maxPitchAngle * Mathf.Deg2Rad), Mathf.Sin(maxPitchAngle * Mathf.Deg2Rad));

                Vector3 targetPos = holdArea.position + holdOffset + forward;
                heldObj.transform.position = Vector3.Lerp(heldObj.transform.position, targetPos, moveSpeed * Time.deltaTime);
                heldObj.transform.rotation = Quaternion.Lerp(heldObj.transform.rotation, holdArea.rotation, moveSpeed * Time.deltaTime);
            }
        }

        public void TryPickup()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            if (!Physics.Raycast(ray, out var hit, interactDistance)) return;
            if (hit.collider.CompareTag("Item"))
            {
                PickupObject(hit.collider.gameObject);
            }
        }

        private void PickupObject(GameObject obj)
        {
            heldObj = obj;
            heldObjRb = obj.GetComponent<Rigidbody>();
            heldObjCol = obj.GetComponent<Collider>();

            if (heldObjCol && playerController)
                Physics.IgnoreCollision(heldObjCol, playerController, true);

            if (heldObjRb)
            {
                heldObjRb.isKinematic = true;
                heldObjRb.useGravity = false;
            }
        }

        public void TryThrow()
        {
            if (heldObj == null) return;

            if (heldObjRb)
            {
                heldObjRb.isKinematic = false;
                heldObjRb.useGravity = true;
                heldObjRb.AddForce(playerCamera.transform.forward * 10f, ForceMode.Impulse);
            }

            if (heldObjCol && playerController)
                Physics.IgnoreCollision(heldObjCol, playerController, false);

            heldObj = null;
            heldObjRb = null;
            heldObjCol = null;
        }
    }
}