using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RGSK.Helpers;

namespace RGSK
{
    public class GameSettingsGamplayEntryUI : GameSettingsEntryUI //Gameplay* pardon the typo
    {
        [SerializeField] GameplaySettingType type;

        protected override void Start()
        {
            base.Start();

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(type.ToString());
            }
        }

        public override void SelectOption(int direction)
        {
            switch (type)
            {
                case GameplaySettingType.MeasureUnit:
                    {
                        SetMeasureUnit(direction);
                        break;
                    }

                case GameplaySettingType.SpeedUnit:
                    {
                        SetSpeedUnit(direction);
                        break;
                    }

                case GameplaySettingType.DistanceUnit:
                    {
                        SetDistanceUnit(direction);
                        break;
                    }

                case GameplaySettingType.Transmission:
                    {
                        SetTransmission(direction);
                        break;
                    }

                case GameplaySettingType.Nameplate:
                    {
                        SetNameplate(direction);
                        break;
                    }

                case GameplaySettingType.ProximityArrows:
                    {
                        SetProximityArrow(direction);
                        break;
                    }

                case GameplaySettingType.MobileInput:
                    {
                        SetMobileInput(direction);
                        break;
                    }

                case GameplaySettingType.AccelerometerSensitivity:
                    {
                        SetAccelerometerSensitivity(direction);
                        break;
                    }

                case GameplaySettingType.Vibration:
                    {
                        SetVibration(direction);
                        break;
                    }

                case GameplaySettingType.FPS:
                    {
                        SetFPS(direction);
                        break;
                    }

                case GameplaySettingType.AutoThrottle:
                    {
                        SetAutoThrottle(direction);
                        break;
                    }

                case GameplaySettingType.AutoBrake:
                    {
                        SetAutoBrake(direction);
                        break;
                    }
            }
        }

        void SetMeasureUnit(int direction)
        {
            var count = Enum.GetValues(typeof(MeasureUnit)).Length;
            var index = (int)RGSKCore.Instance.UISettings.measureUnit;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.UISettings.measureUnit = (MeasureUnit)index;
            valueText?.SetText(((MeasureUnit)index).ToString());
        }

        void SetSpeedUnit(int direction)
        {
            valueText?.SetText("N/A");
            Logger.LogWarning("Please use the MeasureUnit type to set speed units!");
        }

        void SetDistanceUnit(int direction)
        {
            valueText?.SetText("N/A");
            Logger.LogWarning("Please use the MeasureUnit type to set distance units!");
        }

        void SetTransmission(int direction)
        {
            var count = Enum.GetValues(typeof(TransmissionType)).Length;
            var index = (int)RGSKCore.Instance.VehicleSettings.transmissionType;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.VehicleSettings.transmissionType = (TransmissionType)index;
            valueText?.SetText(((TransmissionType)index).ToString());
        }

        void SetNameplate(int direction)
        {
            var index = RGSKCore.Instance.UISettings.showNameplates ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.UISettings.showNameplates = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetProximityArrow(int direction)
        {
            var index = RGSKCore.Instance.UISettings.showProximityArrows ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.UISettings.showProximityArrows = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetMobileInput(int direction)
        {
            var count = Enum.GetValues(typeof(MobileControlType)).Length;
            var index = (int)RGSKCore.Instance.InputSettings.mobileControlType;
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);

            RGSKCore.Instance.InputSettings.mobileControlType = (MobileControlType)index;
            valueText?.SetText(((MobileControlType)index).ToString());
        }

        void SetAccelerometerSensitivity(int direction)
        {
            var value = RGSKCore.Instance.InputSettings.accelerometerSensitivity;
            value += direction * 0.1f;
            value = Mathf.Clamp(value, 1, 2);
            RGSKCore.Instance.InputSettings.accelerometerSensitivity = value;
            valueText?.SetText(value.ToString("F1"));
        }

        void SetVibration(int direction)
        {
            var index = RGSKCore.Instance.InputSettings.vibrate ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.InputSettings.vibrate = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetFPS(int direction)
        {
            var index = RGSKCore.Instance.GeneralSettings.enableFpsReader ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.GeneralSettings.enableFpsReader = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetAutoThrottle(int direction)
        {
            var index = RGSKCore.Instance.VehicleSettings.autoThrottle ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.VehicleSettings.autoThrottle = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetAutoBrake(int direction)
        {
            var index = RGSKCore.Instance.VehicleSettings.autoBrake ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);

            RGSKCore.Instance.VehicleSettings.autoBrake = index == 0;
            valueText?.SetText(toggleOptions[index]);
        }
    }
}