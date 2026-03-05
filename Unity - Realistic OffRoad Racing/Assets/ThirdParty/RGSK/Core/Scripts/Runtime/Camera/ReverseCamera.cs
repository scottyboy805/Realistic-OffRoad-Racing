using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [RequireComponent(typeof(Camera))]
    public class ReverseCamera : MonoBehaviour
    {
        [SerializeField] RenderTexture texture;

        ReverseCameraTarget _target;
        Camera _camera;
        bool _canLookBack;
        float _lookInput;

        void OnEnable()
        {
            InputManager.CameraLookEvent += OnLook;
            RGSKEvents.OnCameraPerspectiveChanged.AddListener(OnCameraPerspectiveChanged);
        }

        void OnDisable()
        {
            InputManager.CameraLookEvent -= OnLook;
            RGSKEvents.OnCameraPerspectiveChanged.RemoveListener(OnCameraPerspectiveChanged);
        }

        void Start()
        {
            _camera = GetComponent<Camera>();
            _camera.targetTexture = texture;
        }

        void Update()
        {
            if(_target != null)
            {
                transform.SetPositionAndRotation(_target.transform.position, _target.transform.rotation);
            }

            if (!_canLookBack)
            {
                SetRenderTextureMode(true);
                return;
            }

            SetRenderTextureMode(_lookInput < 0.5f);
        }

        public void SetTarget(ReverseCameraTarget target)
        {
            if (target.perspective == null)
                return;

            _target = target;
        }

        void SetRenderTextureMode(bool on)
        {
            if (CameraManager.Instance == null)
                return;

            _camera.targetTexture = on ? texture : null;

            var mainCam = CameraManager.Instance.Camera.OutputCamera;
            var mainCameraDepth = mainCam.depth;
            _camera.depth = !on ? mainCameraDepth + 1 : mainCameraDepth - 1;
            mainCam.enabled = on;
        }

        void OnCameraPerspectiveChanged(CameraPerspective perspective)
        {
            if (perspective == null)
                return;

            _canLookBack = perspective.reverseCameraOnLookBack;
        }

        void OnLook(Vector2 val) => _lookInput = val.y;
    }
}