using UnityEngine;
using UnityEngine.InputSystem;

namespace RGSK
{
    public class AccelerometerEnabler : MonoBehaviour
    {
        void OnEnable()
        {
            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
            }
        }

        void OnDisable()
        {
            if (Accelerometer.current != null)
            {
                InputSystem.DisableDevice(Accelerometer.current);
            }
        }
    }
}