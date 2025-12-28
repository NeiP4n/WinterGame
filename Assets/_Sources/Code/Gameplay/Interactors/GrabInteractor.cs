using UnityEngine;

namespace Sources.Code.Gameplay.Grab
{
    public class GrabInteractor : MonoBehaviour
    {
        [Header("Hand Hold Layer")]
        [SerializeField] private LayerMask handHoldLayer;

        [Header("Hand")]
        [SerializeField] private Transform handSocket;

        [Header("Physics Hold")]
        [SerializeField] private float jointAnchorDistance = 2.5f;
        [SerializeField] private float throwingForce = 10f;

        [Header("Joint Settings")]
        [SerializeField] private float drag = 10f;
        [SerializeField] private float angularDrag = 5f;
        [SerializeField] private float damper = 4f;
        [SerializeField] private float spring = 100f;
        [SerializeField] private float massScale = 1f;
        [SerializeField] private float breakingDistance = 3f;

        private GrabInteractible current;
        public bool IsHolding => current != null;

        private void Update()
        {
            if (!IsHolding)
                return;

            if (IsHandHold(current))
                return;

            Vector3 anchor =
                transform.position + transform.forward * jointAnchorDistance;

            if (!current.Follow(anchor))
                Drop();
        }

        public void Grab(GrabInteractible target)
        {
            if (IsHolding || target == null)
                return;

            current = target;

            if (IsHandHold(target))
            {
                if (handSocket == null)
                {
                    Debug.LogError("[GrabInteractor] HandSocket is not assigned", this);
                    return;
                }

                current.AttachToHand(handSocket);
            }
            else
            {
                current.Lock(new JointCreationSettings
                {
                    drag = drag,
                    angularDrag = angularDrag,
                    damper = damper,
                    spring = spring,
                    massScale = massScale,
                    breakingDistance = breakingDistance
                });
            }
        }

        public void Drop(bool throwObject = false)
        {
            if (!IsHolding)
                return;

            if (IsHandHold(current))
                current.DetachFromHand();
            else
                current.Unlock();

            if (throwObject)
                current.Push(transform.forward * throwingForce);

            current = null;
        }

        public void Throw() => Drop(true);

        private bool IsHandHold(GrabInteractible target)
        {
            return ((1 << target.gameObject.layer) & handHoldLayer) != 0;
        }
    }
}
