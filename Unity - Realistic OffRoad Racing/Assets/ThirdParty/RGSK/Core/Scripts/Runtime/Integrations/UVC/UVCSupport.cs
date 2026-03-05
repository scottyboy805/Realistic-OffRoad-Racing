using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

#if UVC_SUPPORT
using PG;
#endif

namespace RGSK
{
#if UVC_SUPPORT
    public class UVCSupport : RGSKEntityComponent, IVehicle, ICarControl
    {
        CarController _controller;
        VehicleDamageController _damageController;
        CarLighting _lights;

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
                if (_controller != null)
                {
                    return _controller.Gearbox.AutomaticGearBox ? TransmissionType.Automatic : TransmissionType.Manual;
                }

                return TransmissionType.Automatic;
            }
            set
            {
                if (_controller != null)
                {
                    _controller.Gearbox.AutomaticGearBox = value == TransmissionType.Automatic ? true : false;
                }
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
                if (_controller != null)
                {
                    return _controller.EngineRPM;
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
                if (_controller != null)
                {
                    return _controller.BoostAmount / _controller.Engine.BoostAmount;
                }

                return 0;
            }
            set
            {
                if (_controller != null)
                {
                    _controller.Engine.BoostAmount = value;
                }
            }
        }

        public int CurrentGear
        {
            get
            {
                if (_controller != null)
                {
                    var gear = _controller.CurrentGear;

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
                if (_controller != null)
                {
                    _controller.CurrentGear = value;
                }
            }
        }

        public bool HeadlightsOn
        {
            get => _lightsOn;
            set
            {
                _lightsOn = value;
                _lights?.SetActiveMainLights(_lightsOn);
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
            get => _damageController != null && _damageController.enabled;
            set
            {
                if (_damageController != null)
                {
                    _damageController.RestoreCar();
                    _damageController.enabled = value;
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

        public float MaxEngineRPM => _controller?.MaxRPM ?? 0;
        public bool IsEngineOn => _controller?.EngineIsOn ?? false;
        public bool HasNitrous => _controller?.Engine?.EnableBoost ?? false;
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

        public float Acceleration => ThrottleInput;
        public float BrakeReverse => BrakeInput;
        public float Horizontal => SteerInput;
        public bool HandBrake => HandbrakeInput > 0;
        public bool Boost => NitrousInput > 0;
        public float Pitch => 0;
        bool _lightsOn;

        void OnEnable()
        {
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        void OnDisable()
        {
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);

            if (IsInitialized && _controller != null)
            {
                _controller.CollisionAction -= OnCollisionAction;
            }
        }

        void Start()
        {
            Initialize();
            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
            UpdateAudioSources();
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _controller = GetComponent<CarController>();
            _damageController = GetComponent<VehicleDamageController>();
            _lights = GetComponent<CarLighting>();
            _controller.CarControl = this;
            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            _controller.CollisionAction += OnCollisionAction;

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
            _controller?.StartEngine();
        }

        public void StopEngine()
        {
            _controller?.StopEngine();
        }

        public void ShiftUp()
        {
            _controller?.NextGear();
        }

        public void ShiftDown()
        {
            _controller?.PrevGear();
        }

        public void OnReset()
        {
            CurrentGear = 0;
            _controller?.ResetVehicle();
        }

        public void Repair()
        {
            if (_damageController == null)
                return;

            if (_damageController.IsInited)
            {
                _damageController.RestoreCar();
            }
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

        public void OnCollisionAction(PG.VehicleController vehicle, Collision collision)
        {
            //Some audiosources are created at runtime on collision so update their output mixer groups
            UpdateAudioSources();
        }

        //Non-kinematic rigidbodies seem to cause major performance issues during replay playback
        //Set kinematic to true during replays as a workaround.
        void OnReplayStart()
        {
            Entity.Rigid.isKinematic = true;
        }

        void OnReplayStop()
        {
            Entity.Rigid.isKinematic = false;
        }
    }
#endif
}