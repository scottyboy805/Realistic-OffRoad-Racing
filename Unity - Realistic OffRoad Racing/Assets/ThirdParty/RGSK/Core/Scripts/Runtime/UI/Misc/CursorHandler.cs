using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RGSK
{
    public class CursorHandler : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnInputDeviceChanged.AddListener(OnInputDeviceChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnInputDeviceChanged.RemoveListener(OnInputDeviceChanged);
        }

        void OnInputDeviceChanged(InputDevice device)
        {
            ToggleCursor(InputManager.Instance.ActiveController == InputController.MouseAndKeyboard);
        }

        void ToggleCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}