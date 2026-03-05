using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

#if SCVP_SUPPORT
using Ashsvp;
#endif

namespace RGSK
{
#if SCVP_SUPPORT
    public class SCVPSupport : RGSKEntityComponent, IVehicle
    {
        SimcadeVehicleController _controller;
        GearSystem _gearSystem;

        public float ThrottleInput
        {
            get => _throttleInput;
            set
            {
                _throttleInput = HasControl ? value : 0;
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
            get => _throttleInput * -1;
            set
            {
                if (!HasControl || value < 0.1f)
                    return;

                _throttleInput = -value;
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
                return TransmissionType.Automatic;
            }
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
            get
            {
                if (_gearSystem != null)
                {
                    return _gearSystem.currentGear + 1;
                }

                return 2;
            }
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

        public float MaxEngineRPM => 1;
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

            if (TryGetComponent<InputManager_SVP>(out var input))
            {
                input.enabled = false;
            }

            if (TryGetComponent<ResetVehicle>(out var reset))
            {
                reset.enabled = false;
            }

            if (TryGetComponent<AshDev.Utility.NitroBoost>(out var nitro))
            {
                nitro.enabled = false;
            }
        }

        void Update()
        {
            _controller?.ProvideInputs(ThrottleInput, SteerInput, HandbrakeInput);
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _controller = GetComponent<SimcadeVehicleController>();
            _gearSystem = GetComponent<GearSystem>();
            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            foreach (var a in GetComponentsInChildren<AudioSource>())
            {
                //Set the audiosource output mixer group to "SFX/Vehicle" to ensure audio settings work properly
                //Any audiosources created at runtime will have to be set on creation.
                //This can be done by copy/pasting the code below to where the audiosource is created:
                //runtimeAudioSource.outputAudioMixerGroup = RGSK.Helpers.AudioHelper.GetAudioMixerGroup(RGSK.AudioGroup.Vehicle.ToString());
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

        }
    }
#endif
}