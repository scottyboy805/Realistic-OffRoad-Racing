using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

#if NWH2_SUPPORT
using NWH.VehiclePhysics2.Powertrain;
#endif

namespace RGSK
{
#if NWH2_SUPPORT
    public class NWH2Support : RGSKEntityComponent, IVehicle
    {
        NWH.VehiclePhysics2.VehicleController _vehicleController;
        NWH.VehiclePhysics2.Modules.NOSModule _nos;
        NWH.VehiclePhysics2.Damage.DamageHandler _damageHandler;
        NWH.VehiclePhysics2.Effects.LightsMananger _lightsManager;

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
            get => !HasControl ? 1 : _handbrakeInput;
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
                if (_vehicleController != null)
                {
                    return _vehicleController.powertrain.transmission.transmissionType ==
                        TransmissionComponent.TransmissionShiftType.Manual ?
                        TransmissionType.Manual : TransmissionType.Automatic;
                }

                return TransmissionType.Automatic;
            }
            set
            {
                if (_vehicleController != null)
                {
                    _vehicleController.powertrain.transmission.transmissionType = value == TransmissionType.Manual ?
                    TransmissionComponent.TransmissionShiftType.Manual :
                    TransmissionComponent.TransmissionShiftType.Automatic;
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
                if (_vehicleController != null)
                {
                    return _vehicleController.powertrain.engine.InputRPM;
                }

                return 0;
            }
            set
            {
                if (_vehicleController != null)
                {
                    _vehicleController.powertrain.engine.inputAngularVelocity = value;
                }
            }
        }

        public float NitrousCapacity
        {
            get
            {
                if (_nos != null)
                {
                    return _nos.charge / _nos.capacity;
                }

                return 0;
            }
            set
            {
                if (_nos != null)
                {
                    _nos.charge = value;
                }
            }
        }

        public int CurrentGear
        {
            get
            {
                if (_vehicleController != null)
                {
                    if (_vehicleController.powertrain.transmission.Gear == 0)
                    {
                        return 1;
                    }

                    if (_vehicleController.powertrain.transmission.Gear < 0)
                    {
                        return 0;
                    }

                    return _vehicleController.powertrain.transmission.Gear + 1;
                }

                return 1;
            }
            set
            {
                if (_vehicleController != null)
                {
                    _vehicleController.powertrain.transmission.ShiftInto(value, true);
                }
            }
        }

        public bool HeadlightsOn
        {
            get
            {
                if (_vehicleController != null && _vehicleController.effectsManager.lightsManager != null)
                {
                    return _vehicleController.effectsManager.lightsManager.lowBeamLights.On;
                }

                return false;
            }
            set
            {
                if (_vehicleController != null && _vehicleController.effectsManager.lightsManager != null)
                {
                    if (value == true)
                    {
                        _vehicleController.effectsManager.lightsManager.lowBeamLights.TurnOn();
                        _vehicleController.effectsManager.lightsManager.tailLights.TurnOn();
                    }
                    else
                    {
                        _vehicleController.effectsManager.lightsManager.lowBeamLights.TurnOff();
                        _vehicleController.effectsManager.lightsManager.tailLights.TurnOff();
                    }
                }
            }
        }

        public bool HornOn
        {
            get
            {
                if (_vehicleController != null)
                {
                    return _vehicleController.input.Horn;
                }

                return false;
            }
            set
            {
                if (_vehicleController != null)
                {
                    _vehicleController.input.Horn = value;
                }
            }
        }

        public bool Damageable
        {
            get => _damageHandler != null;
            set
            {
                if (_damageHandler != null)
                {
                    _damageHandler.enabled = value;
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

        public float MaxEngineRPM => _vehicleController?.powertrain?.engine?.revLimiterRPM ?? 0;
        public bool IsEngineOn => _vehicleController?.powertrain?.engine?.IsRunning ?? true;
        public bool HasNitrous => _nos != null;
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

        void Awake() 
        {
            var wheels = GetComponentsInChildren<NWH.WheelController3D.WheelController>().ToList();
            wheels.ForEach(x => x.meshColliderLayer = RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
        }

        void Start()
        {
            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
            Initialize();
        }

        void Update()
        {
            if (_vehicleController == null)
                return;

            _vehicleController.input.Throttle = _vehicleController.input.states.throttleRaw = ThrottleInput;
            _vehicleController.input.Steering = _vehicleController.input.states.steeringRaw = SteerInput;
            _vehicleController.input.Brakes = _vehicleController.input.states.brakesRaw = BrakeInput;
            _vehicleController.input.Handbrake = _vehicleController.input.states.handbrakeRaw = HandbrakeInput;
            _vehicleController.input.Boost = _vehicleController.input.states.boost = NitrousInput > 0;
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _vehicleController = GetComponent<NWH.VehiclePhysics2.VehicleController>();
            _damageHandler = GetComponent<NWH.VehiclePhysics2.Damage.DamageHandler>();

            _vehicleController.input.autoSetInput = false;

            var nosModule = _vehicleController.GetComponent<NWH.VehiclePhysics2.Modules.NOS.NOSModuleWrapper>();
            if (nosModule != null && nosModule.module != null)
            {
                _nos = nosModule.module as NWH.VehiclePhysics2.Modules.NOSModule;
            }

            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            IsInitialized = true;

            StartCoroutine(OnPostInitializeRoutine());
        }

        public void EnableControl()
        {
            HasControl = true;

            if (_vehicleController != null)
            {
                _vehicleController.powertrain.clutch.controlType = ClutchComponent.ClutchControlType.Automatic;
            }
        }

        public void DisableControl()
        {
            ThrottleInput = 0;
            BrakeInput = 0;
            HornOn = false;
            HasControl = false;

            if (_vehicleController != null)
            {
                //allow for reving without the car moving forward (simply holding down the handbrake does not work)
                _vehicleController.powertrain.clutch.controlType = ClutchComponent.ClutchControlType.UserInput;
            }
        }

        public void StartEngine(float delay)
        {
            if (_vehicleController == null)
                return;

            _vehicleController.powertrain.engine.StartEngine();
        }

        public void StopEngine()
        {
            if (_vehicleController == null)
                return;

            _vehicleController.powertrain.engine.StopEngine();
        }

        public void ShiftUp()
        {
            if (_vehicleController == null)
                return;

            _vehicleController.input.ShiftUp = true;
        }

        public void ShiftDown()
        {
            if (_vehicleController == null)
                return;

            _vehicleController.input.ShiftDown = true;
        }

        public void OnReset()
        {
            if (_vehicleController == null || !_vehicleController.IsInitialized)
                return;

            CurrentGear = 1;
        }

        public void Repair()
        {
            _damageHandler?.Repair();
        }

        IEnumerator OnPostInitializeRoutine()
        {
            //Wait for initialization to complete
            yield return null;

            foreach (var a in GetComponentsInChildren<AudioSource>())
            {
                a.outputAudioMixerGroup = AudioHelper.GetAudioMixerGroup(AudioGroup.Vehicle.ToString());
            }
        }
    }
#endif
}