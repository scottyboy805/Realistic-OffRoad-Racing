using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

#if VPP_SUPPORT
using VehiclePhysics;
#endif

namespace RGSK
{
#if VPP_SUPPORT
    public class VPPSupport : RGSKEntityComponent, IVehicle
    {
        VPVehicleController _vc;
        VPStandardInput _vcInput;

        public float ThrottleInput
        {
            get => _throttleInput;
            set
            {
                _throttleInput = value;
            }
        }

        public float SteerInput
        {
            get => !HasControl ? 0 : _steerInput;
            set
            {
                _steerInput = HasControl ? value : 0;
            }
        }

        public float BrakeInput
        {
            get => _brakeInput;
            set
            {
                _brakeInput = HasControl ? value : 0;
            }
        }

        public float HandbrakeInput
        {
            get => _handbrakeInput;
            set
            {
                _handbrakeInput = HasControl ? value : 1;
            }
        }

        public float NitrousInput
        {
            get => _nitrousInput;
            set
            {
                _nitrousInput = HasControl ? value : 0;
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
                if (_vc != null)
                {
                    return _vc.gearbox.autoShift ? TransmissionType.Automatic : TransmissionType.Manual;
                }

                return TransmissionType.Automatic;
            }
            set
            {
                //always use manual with "autoShift" toggle
                _vc.gearbox.type = Gearbox.Type.Manual;
                _vc.gearbox.autoShift = value == TransmissionType.Automatic;
            }
        }

        public VehicleHandlingMode HandlingMode
        {
            get
            {
                return VehicleHandlingMode.Grip;
            }
            set
            {

            }
        }

        public float EngineRPM
        {
            get
            {
                if (_vc != null)
                {
                    return _vc.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000.0f;
                }

                return 0;
            }
            set
            {
                if (_vc != null)
                {
                    _vc.data.Set(Channel.Vehicle, VehicleData.EngineRpm, (int)value);
                }
            }
        }

        public float NitrousCapacity
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public int CurrentGear
        {
            get
            {
                if (_vc != null)
                {
                    var gear = _vc.data.Get(Channel.Vehicle, VehicleData.GearboxGear);

                    if (gear < 0)
                    {
                        return 0;
                    }

                    return gear + 1;
                }

                return 0;
            }
            set
            {
                if (_vc != null)
                {
                    _vc.data.Set(Channel.Vehicle, VehicleData.GearboxGear, value);
                }
            }
        }

        public bool HeadlightsOn
        {
            get
            {
                return false;
            }
            set
            {

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
            get
            {
                return false;
            }
            set
            {

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

        public float MaxEngineRPM => _vc != null ? _vc.engine.maxRpm : 0;
        public bool IsEngineOn => _vc != null ? _vc.data.Get(Channel.Vehicle, VehicleData.EngineWorking) == 1 : false;
        public bool HasNitrous => false;
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
        bool _startEngine;

        void Start()
        {
            Initialize();
            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
        }

        void Update()
        {
            if (_startEngine && !IsEngineOn)
            {
                StartEngine(0);
            }
            else if (!_startEngine && IsEngineOn)
            {
                StopEngine();
            }

            _vcInput.externalThrottle = ThrottleInput;
            _vcInput.externalSteer = SteerInput;
            _vcInput.externalBrake = BrakeInput;
            _vcInput.externalHandbrake = HandbrakeInput;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _vc = GetComponent<VPVehicleController>();
            _vcInput = GetComponent<VPStandardInput>();
            _vcInput.throttleAndBrakeMode = VPStandardInput.ThrottleAndBrakeMode.AutoForwardAndReverse;
            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            ToggleEngine(true);

            foreach (var a in GetComponentsInChildren<AudioSource>())
            {
                a.outputAudioMixerGroup = AudioHelper.GetAudioMixerGroup(AudioGroup.Vehicle.ToString());
            }

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
            ToggleEngine(true);
        }

        public void StopEngine()
        {
            ToggleEngine(false);
        }

        void ToggleEngine(bool toggle)
        {
            _startEngine = toggle;
            _vc.data.bus[Channel.Input][InputData.Key] = toggle ? 1 : -1;
        }

        public void ShiftUp()
        {
            _vc.data.bus[Channel.Input][InputData.GearShift] += 1;
        }

        public void ShiftDown()
        {
            _vc.data.bus[Channel.Input][InputData.GearShift] -= 1;
        }

        public void OnReset()
        {
            CurrentGear = 2;
        }

        public void Repair()
        {

        }
    }
#endif
}