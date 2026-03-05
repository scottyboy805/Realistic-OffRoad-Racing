using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AutoHideCanvasGroup : MonoBehaviour
    {
        [SerializeField] bool autoHide;
        [SerializeField] float autoHideTimeout = 30;

        CanvasGroup _canvasGroup;
        float _autoHideTimer;
        float _lastHideTime;

        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        void Update()
        {
            if (InputHelper.MouseMoved() ||
                InputHelper.KeyboardPressed() ||
                InputHelper.GamepadButtonOrAxisPressed() ||
                InputHelper.ScreenTouched())
            {
                _autoHideTimer = 0;

                if (Time.unscaledTime > _lastHideTime + 0.5f)
                {
                    SetVisible(true);
                }
            }
            else
            {
                if (autoHide && _canvasGroup != null && _canvasGroup.alpha > 0)
                {
                    _autoHideTimer += Time.unscaledDeltaTime;

                    if (_autoHideTimer > autoHideTimeout)
                    {
                        SetVisible(false);
                    }
                }
            }
        }

        public void SetVisible(bool visible)
        {
            _canvasGroup.SetAlpha(visible ? 1 : 0);

            if (!visible)
            {
                _lastHideTime = Time.unscaledTime;
            }
        }
    }
}