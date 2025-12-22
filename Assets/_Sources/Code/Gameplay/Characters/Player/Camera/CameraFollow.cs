using Game.Interfaces;
using UnityEngine;

namespace Game.Controllers
{
    public class CameraFollow
    {
        private Transform headBone;
        private Vector3 offset;
        private float amplitude;
        private float frequency;

        private Vector3 currentSineOffset;
        private Vector3 sineVelocity;

        private SineMotion sineMotion;
        private ICameraInputProvider inputProvider;

        public CameraFollow(Transform headBone, Vector3 offset, float amplitude, float frequency)
        {
            this.headBone  = headBone;
            this.offset    = offset;
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public void Init(SineMotion sineMotion) =>
            this.sineMotion = sineMotion;

        public void SetInputProvider(ICameraInputProvider provider) =>
            this.inputProvider = provider;

        public void UpdateCameraPosition(Transform camTransform)
        {
            if (headBone == null)
                return;

            camTransform.position = headBone.position + offset;

            Vector3 desiredSineOffset = Vector3.zero;

            if (inputProvider != null && sineMotion != null)
            {
                if (inputProvider.GetMoveInput() != Vector2.zero)
                    desiredSineOffset = sineMotion.GetSineOffset(amplitude, frequency, Time.time);
            }

            currentSineOffset = Vector3.SmoothDamp(
                currentSineOffset,
                desiredSineOffset,
                ref sineVelocity,
                0.1f
            );

            camTransform.position += currentSineOffset;
        }
    }
}
