using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
#if RCC_PRO_SUPPORT
    public class RCCProSupport : RGSKEntityComponent, IVehicle
    {
        RCCP_CarController _rcc;
        RCCP_Inputs _inputs = new RCCP_Inputs();

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
            get => _inputs.nosInput;
            set
            {
                _inputs.nosInput = HasControl ? value : 0;
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
                    return _rcc.Gearbox.transmissionType == RCCP_Gearbox.TransmissionType.Automatic ?
                    TransmissionType.Automatic : TransmissionType.Manual;
                }

                return TransmissionType.Automatic;
            }
            set
            {
                if (_rcc != null)
                {
                    _rcc.Gearbox.transmissionType = value == TransmissionType.Automatic ?
                    RCCP_Gearbox.TransmissionType.Automatic : RCCP_Gearbox.TransmissionType.Manual;
                }
            }
        }

        public VehicleHandlingMode HandlingMode
        {
            get
            {
                var behaviourIndex = RCCP_Settings.Instance.behaviorSelectedIndex;
                return behaviourIndex == 0 ? VehicleHandlingMode.Grip : VehicleHandlingMode.Drift;
            }
            set
            {
                var index = value == VehicleHandlingMode.Grip ? 0 : 2;
                RCCP.SetBehavior(index);
            }
        }

        public float EngineRPM
        {
            get => _rcc?.engineRPM ?? 0;
            set
            {
                if (_rcc != null && _rcc.Engine != null)
                {
                    _rcc.Engine.engineRPM = value;
                }
            }
        }

        public float NitrousCapacity
        {
            get => _rcc?.OtherAddonsManager.Nos?.amount ?? 0;
            set
            {
                if (_rcc.OtherAddonsManager.Nos != null)
                {
                    _rcc.OtherAddonsManager.Nos.amount = value;
                }
            }
        }

        public int CurrentGear
        {
            get
            {
                if (_rcc != null)
                {
                    if (_rcc.NGearNow)
                    {
                        return 1;
                    }

                    return _rcc.direction == 1 ? _rcc.currentGear + 2 : 0;
                }

                return 1;
            }
            set
            {
                if (_rcc.currentGear != value)
                {
                    _rcc?.Gearbox.ShiftToGear(value);
                }
            }
        }

        public bool HeadlightsOn
        {
            get
            {
                if (_rcc != null && _rcc.Lights != null)
                {
                    return _rcc.Lights.lowBeamHeadlights;
                }

                return false;
            }
            set
            {
                if (_rcc != null && _rcc.Lights != null)
                {
                    _rcc.Lights.lowBeamHeadlights = value;
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
            get => _rcc.Damage != null;
            set
            {
                if (_rcc.Damage != null)
                {
                    _rcc.Damage.repairNow = true;
                    _rcc.Damage.enabled = value;
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
        public bool HasNitrous => _rcc.OtherAddonsManager.Nos != null && _rcc.OtherAddonsManager.Nos.enabled;
        public float CurrentSpeed => Entity?.CurrentSpeed ?? 0;
        public int OdometerReading => _odometer?.Distance ?? 0;
        public bool HasControl { get; private set; }
        public bool IsInitialized { get; private set; } = false;

        VehicleDefiner _vehicleDefiner;
        Odometer _odometer;
        List<Rigidbody> _rbs = new List<Rigidbody>();
        float _throttleInput;
        float _brakeInput;
        float _handbrakeInput;
        float _steerInput;
        float _nitrousInput;

        void OnEnable()
        {
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        void OnDisable()
        {
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);
        }

        void Start()
        {
            Initialize();

            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
            gameObject.SetChildLayers(LayerMask.NameToLayer("RCCP_Vehicle"));
            _rbs = gameObject.GetComponentsInChildren<Rigidbody>().ToList();

            if (CameraManager.Instance != null)
            {
                var rccLayers = new string[] { "RCCP_Vehicle", "RCCP_WheelCollider", "RCCP_DetachablePart", "RCCP_Prop" };

                foreach (var layer in rccLayers)
                {
                    CameraManager.Instance.Camera.OutputCamera.cullingMask |= 1 << LayerMask.NameToLayer(layer);
                }
            }

            UpdateAudioSources();
        }

        void Update()
        {
            _rcc?.Inputs.OverrideInputs(_inputs);
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _rcc = GetComponent<RCCP_CarController>();
            _rcc.externalControl = true;
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
            _rcc?.Gearbox.ShiftUp();
        }

        public void ShiftDown()
        {
            _rcc?.Gearbox.ShiftDown();
        }

        public void OnReset()
        {
            CurrentGear = 0;
        }

        public void Repair()
        {
            if (_rcc.Damage != null)
            {
                _rcc.Damage.repairNow = true;
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

        //Non-kinematic rigidbodies seem to cause major performance issues during replay playback
        //Set kinematic to true during replays as a workaround.
        void OnReplayStart()
        {
            _rbs.ForEach(x => x.isKinematic = true);
        }

        void OnReplayStop()
        {
            _rbs.ForEach(x => x.isKinematic = false);
        }
    }
#endif
}