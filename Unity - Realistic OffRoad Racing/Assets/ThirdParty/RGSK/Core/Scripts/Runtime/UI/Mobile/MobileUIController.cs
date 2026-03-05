using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class MobileUIController : MonoBehaviour
    {
        [SerializeField] GameObject touchSteer;
        [SerializeField] GameObject wheelSteer;
        [SerializeField] GameObject tiltSteer;
        [SerializeField] GameObject manualButtons;
        [SerializeField] GameObject nitrousButton;

        [Tooltip("Resets all input bindings to ensure On-Screen Button components retain correct control paths.")]
        [SerializeField] bool resetInputBindings = true;

        void Start()
        {
            if(resetInputBindings && !string.IsNullOrWhiteSpace(SaveData.Instance.gameSettingsData.inputBindings))
            {
                InputManager.Instance?.ResetBindings();
                Logger.Log(this, "Reset input bindings to defaults!");
            }
        }

        void FixedUpdate()
        {
            if (touchSteer != null)
            {
                touchSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Touch);
            }

            if (wheelSteer != null)
            {
                wheelSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Wheel);
            }

            if (tiltSteer != null)
            {
                tiltSteer.SetActiveSafe(RGSKCore.Instance.InputSettings.mobileControlType == MobileControlType.Tilt);
            }

            if (manualButtons != null)
            {
                manualButtons.SetActiveSafe(RGSKCore.Instance.VehicleSettings.transmissionType == TransmissionType.Manual);
            }

            if (nitrousButton != null && PlayerVehicleInput.Instance?.Vehicle != null)
            {
                nitrousButton.SetActive(PlayerVehicleInput.Instance.Vehicle.HasNitrous);
            }
        }
    }
}