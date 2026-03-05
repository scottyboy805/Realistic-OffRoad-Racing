using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.Layouts;

namespace RGSK
{
    public class MobileTiltSteer : OnScreenControl
    {
        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        void Update()
        {
            if (Accelerometer.current != null)
            {
                var acceleration = Accelerometer.current.acceleration.ReadValue();
                SendValueToControl(new Vector2(acceleration.x * RGSKCore.Instance.InputSettings.accelerometerSensitivity, 0));
            }
        }
    }
}