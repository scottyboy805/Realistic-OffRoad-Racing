using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
#if RCC_SUPPORT
    public class RCCSupport : RGSKEntityComponent, IVehicle
    {
        RCC_CarControllerV4 _rcc;
        RCC_Inputs _inputs = new RCC_Inputs();

        public float ThrottleInput
        {
            get => _inputs.throttleInput;
            set
            {
                _inputs.throttleInput = value;
            }
        }

        public float SteerInput
        {
            get => !HasControl ? 0 : _inputs.steerInput;
            set
            {
                _inputs.steerInput = HasControl ? value : 0;
            }
        }

        public float BrakeInput
        {
            get => _inputs.brakeInput;
            set
            {
                _inputs.brakeInput = HasControl ? value : 0;
            }
        }

        public float HandbrakeInput
        {
            get => _inputs.handbrakeInput;
            set
            {
                _inputs.handbrakeInput = HasControl ? value : 1;
            }
        }

        public float NitrousInput
        {
            get => _inputs.boostInput;
            set
            {
                _inputs.boostInput = HasControl ? value : 0;
            }
        }

        public VehicleDefinition VehicleDefinition
        {
            get
            {
                if (_vehicleDefiner == null)
                {
                    _vehicleDefiner = gameObject.GetComponent<VehicleDefiner>();
                }

                if (_vehicleDefiner != null)
                {
                    return _vehicleDefiner.definition;
                }

                return null;
            }
        }

        public TransmissionType TransmissionType
        {
            get
            {
                if (_rcc != null)
                {
                    return _rcc.automaticGear ? TransmissionType.Automatic : TransmissionType.Manual;
                }

                return TransmissionType.Automatic;
            }
            set
            {
                if (_rcc != null)
                {
                    _rcc.automaticGear = value == TransmissionType.Automatic;
                }
            }
        }

        public VehicleHandlingMode HandlingMode
        {
            get
            {
                var behaviourIndex = RCC_Settings.Instance.behaviorSelectedIndex;
                return behaviourIndex == 0 ? VehicleHandlingMode.Grip : VehicleHandlingMode.Drift;
            }
            set
            {
                var index = value == VehicleHandlingMode.Grip ? 0 : 2;
                RCC_SceneManager.Instance?.SetBehavior(index);
            }
        }

        public float EngineRPM
        {
            get => _rcc?.engineRPM ?? 0;
            set
            {
                if (_rcc != null)
                {
                    _rcc.engineRPM = value;
                }
            }
        }

        public float NitrousCapacity
        {
            get => _rcc?.NoS / 100 ?? 0;
            set
            {
                if (_rcc != null)
                {
                    _rcc.NoS = value * 100;
                }
            }
        }

        public int CurrentGear
        {
            get
            {
                if (_rcc != null)
                {
                    if (_rcc.NGear)
                    {
                        return 1;
                    }

                    return _rcc.direction == 1 ? _rcc.currentGear + 2 : 0;
                }

                return 1;
            }
            set
            {
                _rcc?.GearShiftTo(value);
            }
        }

        public bool HeadlightsOn
        {
            get
            {
                if (_rcc != null)
                {
                    return _rcc.highBeamHeadLightsOn;
                }

                return false;
            }
            set
            {
                if (_rcc != null)
                {
                    _rcc.highBeamHeadLightsOn = value;
                }
            }
        }

        public bool HornOn
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool Damageable
        {
            get => _rcc?.useDamage ?? false;
            set
            {
                if (_rcc != null)
                {
                    _rcc.Repair();
                    _rcc.damage.UpdateRepair();
                    _rcc.useDamage = value;
                }
            }
        }

        public bool OdometerEnabled
        {
            get => _odometer?.enabled ?? false;
            set
            {
                if (_odometer != null)
                {
                    _odometer.enabled = value;
                }
            }
        }

        public float MaxEngineRPM => _rcc?.maxEngineRPM ?? 0;
        public bool IsEngineOn => _rcc?.engineRunning ?? true;
        public bool HasNitrous => _rcc?.useNOS ?? false;
        public float CurrentSpeed => Entity?.CurrentSpeed ?? 0;
        public int OdometerReading => _odometer?.Distance ?? 0;
        public bool HasControl { get; private set; }
        public bool IsInitialized { get; private set; } = false;

        VehicleDefiner _vehicleDefiner;
        Odometer _odometer;
        float _throttleInput;
        float _brakeInput;
        float _handbrakeInput;
        float _steerInput;
        float _nitrousInput;

        void Start()
        {
            Initialize();

            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
            gameObject.SetChildLayers(LayerMask.NameToLayer("RCC_Vehicle"));

            if (CameraManager.Instance != null)
            {
                var rccLayers = new string[] { "RCC_Vehicle", "RCC_WheelCollider", "RCC_DetachablePart" };

                foreach (var layer in rccLayers)
                {
                    CameraManager.Instance.Camera.OutputCamera.cullingMask |= 1 << LayerMask.NameToLayer(layer);
                }
            }

            UpdateAudioSources();
        }

        void Update()
        {
            _rcc?.OverrideInputs(_inputs, false);
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _rcc = GetComponent<RCC_CarControllerV4>();
            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            IsInitialized = true;
        }

        public void EnableControl()
        {
            HasControl = true;
        }

        public void DisableControl()
        {
            ThrottleInput = 0;
            BrakeInput = 0;
            HornOn = false;
            HasControl = false;
        }

        public void StartEngine(float delay)
        {
            _rcc.StartEngine();
        }

        public void StopEngine()
        {
            _rcc.KillEngine();
        }

        public void ShiftUp()
        {
            _rcc?.GearShiftUp();
        }

        public void ShiftDown()
        {
            _rcc?.GearShiftDown();
        }

        public void OnReset()
        {
            CurrentGear = 0;
        }

        public void Repair()
        {
            _rcc?.Repair();
        }

        void UpdateAudioSources() => StartCoroutine(UpdateAudioSourcesRoutine());
        IEnumerator UpdateAudioSourcesRoutine()
        {
            //Wait for the creation of any audiosources
            yield return null;

            foreach (var a in GetComponentsInChildren<AudioSource>())
            {
                a.outputAudioMixerGroup = AudioHelper.GetAudioMixerGroup(AudioGroup.Vehicle.ToString());
            }
        }
    }
#endif
}