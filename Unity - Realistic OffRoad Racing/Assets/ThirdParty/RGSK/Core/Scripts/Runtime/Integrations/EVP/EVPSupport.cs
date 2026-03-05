using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
#if EVP_SUPPORT
    public class EVPSupport : RGSKEntityComponent, IVehicle
    {
        EVPInput _input = new EVPInput();
        EVP.VehicleController _vc;
        EVP.VehicleAudio _vcAudio;
        EVP.VehicleDamage _vcDamage;

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
            get => TransmissionType.Automatic;
            set
            {

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
                if (_vcAudio != null)
                {
                    return _vcAudio.simulatedEngineRpm;
                }

                return 0;
            }
            set
            {

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
            get => _vcAudio?.simulatedGear + 1 ?? 1;
            set
            {

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
            get => _vcDamage != null;
            set
            {
                if (_vcDamage != null)
                {
                    _vcDamage.RepairImmediate();
                    _vcDamage.enabled = value;
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

        public float MaxEngineRPM => _vcAudio?.engine?.maxRpm ?? 0;
        public bool IsEngineOn => true;
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

        void Start()
        {
            Initialize();
            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
        }

        void Update()
        {
            _input.Update();
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _vc = GetComponent<EVP.VehicleController>();
            _vcAudio = GetComponent<EVP.VehicleAudio>();
            _vcDamage = GetComponent<EVP.VehicleDamage>();

            _input.vehicle = this;
            _input.target = _vc;

            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

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

        }

        public void StopEngine()
        {

        }

        public void ShiftUp()
        {

        }

        public void ShiftDown()
        {

        }

        public void OnReset()
        {

        }

        public void Repair()
        {
            _vcDamage?.RepairImmediate();
        }
    }

    //A copy of VehicleStandardInput.cs with some minor changes
    public class EVPInput
    {
        public EVPSupport vehicle { get; set; }
        public EVP.VehicleController target { get; set; }
        public bool handbrakeOverridesThrottle = false;

        public void Update()
        {
            if (target == null)
                return;

            float steerInput = vehicle.SteerInput;
            float handbrakeInput = vehicle.HandbrakeInput;

            float forwardInput = 0.0f;
            float reverseInput = 0.0f;

            forwardInput = vehicle.ThrottleInput;
            reverseInput = vehicle.BrakeInput;

            float throttleInput = 0.0f;
            float brakeInput = 0.0f;
            float minSpeed = 0.1f;
            float minInput = 0.1f;

            if (target.speed > minSpeed)
            {
                throttleInput = forwardInput;
                brakeInput = reverseInput;
            }
            else
            {
                if (reverseInput > minInput)
                {
                    throttleInput = -reverseInput;
                    brakeInput = 0.0f;
                }
                else if (forwardInput > minInput)
                {
                    if (target.speed < -minSpeed)
                    {
                        throttleInput = 0.0f;
                        brakeInput = forwardInput;
                    }
                    else
                    {
                        throttleInput = forwardInput;
                        brakeInput = 0;
                    }
                }
            }

            if (handbrakeOverridesThrottle)
            {
                throttleInput *= 1.0f - handbrakeInput;
            }

            target.steerInput = steerInput;
            target.throttleInput = throttleInput;
            target.brakeInput = brakeInput;
            target.handbrakeInput = handbrakeInput;
        }
    }
#endif
}