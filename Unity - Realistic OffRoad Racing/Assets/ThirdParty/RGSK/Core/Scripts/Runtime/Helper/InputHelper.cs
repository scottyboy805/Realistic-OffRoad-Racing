using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace RGSK.Helpers
{
    public static class InputHelper
    {
        public static bool MouseMoved()
        {
            if (Mouse.current == null)
                return false;

            var val = Mouse.current.delta.ReadValue();
            return (val.x > 0.1f || val.y > 0.1f);
        }

        public static bool KeyboardPressed()
        {
            if (Keyboard.current == null)
                return false;

            return Keyboard.current.anyKey.isPressed;
        }

        public static bool GamepadButtonOrAxisPressed()
        {
            if (Gamepad.current == null)
                return false;

            var pressed = false;

            for (int i = 0; i < Gamepad.current.allControls.Count; i++)
            {
                var c = Gamepad.current.allControls[i];

                if (c is ButtonControl)
                {
                    if (((ButtonControl)c).wasPressedThisFrame)
                    {
                        pressed = true;
                    }
                }

                if (c is AxisControl)
                {
                    var val = ((AxisControl)c).ReadValue();
                    if (val > 0.1f)
                    {
                        pressed = true;
                    }
                }
            }

            return pressed;
        }

        public static bool ScreenTouched()
        {
            if (Touchscreen.current == null)
                return false;

            var touch = Touchscreen.current.primaryTouch;

            if (touch != null)
            {
                return touch.isInProgress;
            }

            return false;
        }
    }
}