using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class ReverseCameraUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;

        bool _active;
        bool _isLookingBack;

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

        void OnCameraPerspectiveChanged(CameraPerspective perspective)
        {
            if (perspective == null)
                return;

            _active = perspective.reverseCameraOnUI;

            if (!_isLookingBack)
            {
                canvasGroup.SetAlpha(_active ? 1 : 0);
            }
        }

        void OnLook(Vector2 val)
        {
            _isLookingBack = val.y > 0.5f;

            if (_active)
            {
                canvasGroup.SetAlpha(_isLookingBack ? 0 : 1);
            }
        }
    }
}