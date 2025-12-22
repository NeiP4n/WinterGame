using Game.Interfaces;
using UnityEngine;

namespace Game.Controllers
{
    public class CameraRotation
    {
        private float sensitivity;
        private float maxLookUp;
        private float minLookDown;
        private float smoothTime;

        private ICameraInputProvider inputProvider;

        private float targetX;
        private float targetY;
        private float currentX;
        private float currentY;

        private float velX;
        private float velY;

        public CameraRotation(float sensitivity, float maxLookUp, float minLookDown, float smoothTime)
        {
            this.sensitivity  = sensitivity;
            this.maxLookUp    = maxLookUp;
            this.minLookDown  = minLookDown;
            this.smoothTime   = smoothTime;
        }

        public void Init(Transform cam, Transform body)
        {
            currentX = targetX = body.eulerAngles.y;
            currentY = targetY = cam.localEulerAngles.x;
        }

        public void SetInputProvider(ICameraInputProvider provider) =>
            this.inputProvider = provider;

        public void UpdateRotation(Transform cam, Transform body)
        {
            if (inputProvider == null)
                return;

            Vector2 look = inputProvider.GetLookDelta() * sensitivity;

            targetX += look.x;
            targetY = Mathf.Clamp(targetY - look.y, minLookDown, maxLookUp);

            currentX = Mathf.SmoothDamp(currentX, targetX, ref velX, smoothTime);
            currentY = Mathf.SmoothDamp(currentY, targetY, ref velY, smoothTime);

            cam.localRotation = Quaternion.Euler(currentY, 0, 0);
            body.rotation     = Quaternion.Euler(0, currentX, 0);
        }
    }
}
