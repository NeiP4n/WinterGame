using UnityEngine;
using Game.Interfaces;

namespace Game.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform headBone;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Camera cam;
        [SerializeField] private SineMotion sineMotion;

        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0.2f, 0f);
        [SerializeField] private float amplitude = 0.05f;
        [SerializeField] private float frequency = 7f;

        [Header("Rotation Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookUp = 80f;
        [SerializeField] private float minLookDown = -80f;
        [SerializeField] private float rotationSmoothTime = 0.05f;

        private IInputManager _input;
        private ICameraInputProvider _inputProvider;

        private CameraFollow _follow;
        private CameraRotation _rotation;

        public void Construct(IInputManager input)
        {
            _input = input;
            _inputProvider = new MouseInputProvider(_input);

            _follow = new CameraFollow(
                headBone,
                offset,
                amplitude,
                frequency
            );

            _rotation = new CameraRotation(
                mouseSensitivity,
                maxLookUp,
                minLookDown,
                rotationSmoothTime
            );

            _follow.Init(sineMotion);
            _follow.SetInputProvider(_inputProvider);

            _rotation.Init(cam.transform, bodyTransform);
            _rotation.SetInputProvider(_inputProvider);

            cam.enabled = true;
        }

        private void LateUpdate()
        {
            if (_follow != null)
                _follow.UpdateCameraPosition(cam.transform);

            if (_rotation != null)
                _rotation.UpdateRotation(cam.transform, bodyTransform);
        }
    }
}
