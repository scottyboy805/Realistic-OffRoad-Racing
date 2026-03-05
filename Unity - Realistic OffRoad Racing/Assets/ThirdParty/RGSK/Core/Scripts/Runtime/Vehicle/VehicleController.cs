using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class VehicleController : RGSKEntityComponent, IVehicle
    {
        public Engine engine;
        public Dynamics dynamics;
        public Brake brakes;
        public VehicleSounds sound;
        public Nitrous nitrous;
        public VehicleExtras extra;
        public VehicleLightController lights = new VehicleLightController();
        public VehicleCollisionFX collision = new VehicleCollisionFX();

        public float ThrottleInput
        {
            get => IsAutoReverse ? _throttleInput2 : _throttleInput;
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
            get => IsAutoReverse ? _brakeInput2 : _brakeInput;
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
            get => engine.TransmissionType;
            set => engine.TransmissionType = value;
        }

        public VehicleHandlingMode HandlingMode
        {
            get => dynamics.handlingMode;
            set => dynamics.handlingMode = value;
        }

        public float EngineRPM
        {
            get => engine.Rpm;
            set => engine.Rpm = value;
        }

        public float NitrousCapacity
        {
            get => nitrous.capacity;
            set => nitrous.capacity = value;
        }

        public int CurrentGear
        {
            get => engine.Gear;
            set => engine.ShiftToGear(value);
        }

        public bool HeadlightsOn
        {
            get => lights.HeadlightsOn;
            set
            {
                lights?.ToggleHeadlights(value);
            }
        }

        public bool HornOn
        {
            get => _hornActive;
            set
            {
                _hornActive = value;
                sound.Horn(value);
            }
        }

        public bool Damageable
        {
            get
            {
                if (_meshDeformer != null)
                {
                    return _meshDeformer.IsDeformable;
                }

                return false;
            }
            set
            {
                if (_meshDeformer != null)
                {
                    _meshDeformer.IsDeformable = value;
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

        public float MaxEngineRPM => engine.maxRpm;
        public bool IsEngineOn => engine.Running;
        public bool HasNitrous => nitrous.enabled;
        public float CurrentSpeed => Entity?.CurrentSpeed ?? 0;
        public int OdometerReading => _odometer?.Distance ?? 0;
        public bool IsInitialized { get; private set; } = false;

        public bool Burnout { get; private set; }
        public bool HasControl { get; private set; }
        public bool IsAutoReverse { get; private set; }
        List<VehicleComponent> _components = new List<VehicleComponent>();
        VehicleDefiner _vehicleDefiner;
        Odometer _odometer;
        MeshDeformation _meshDeformer;
        float _throttleInput, _throttleInput2;
        float _brakeInput, _brakeInput2;
        float _handbrakeInput;
        float _steerInput;
        float _nitrousInput;
        bool _reset;
        bool _hornActive;
        
        public void Initialize()
        {
            if (IsInitialized)
                return;

            _components.Add(engine);
            _components.Add(dynamics);
            _components.Add(brakes);
            _components.Add(sound);
            _components.Add(lights);
            _components.Add(extra);
            _components.Add(nitrous);
            _components.Add(collision);

            foreach (var c in _components)
            {
                c.Initialize(this);
            }

            if (RGSKCore.Instance.VehicleSettings.autoStartEngine)
            {
                StartEngine(0);
            }

            _meshDeformer = gameObject.GetComponent<MeshDeformation>();
            _odometer = gameObject.GetOrAddComponent<Odometer>();
            _odometer.Initialize(this);

            IsInitialized = true;
        }

        void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void Start()
        {
            Initialize();
            gameObject.SetColliderLayer(RGSKCore.Instance.GeneralSettings.vehicleLayerIndex.Index);
        }

        void Update()
        {
            Burnout = !IsAutoReverse && CurrentSpeed < 2 && ThrottleInput > 0.9f && BrakeInput > 0.9f;
            AutoReverse();
        }

        void FixedUpdate()
        {
            foreach (var c in _components)
            {
                if(Entity == null)
                    break;
                
                c.Update();
            }

            if (_reset)
            {
                _reset = false;

                engine?.ShiftToGear(2);
                dynamics?.ResetTyremarks();
                dynamics?.ApplyBrakeTorque(float.MaxValue, WheelAxle.Front);
                dynamics?.ApplyBrakeTorque(float.MaxValue, WheelAxle.Rear);
                Entity.Rigid.isKinematic = true;

                StartCoroutine(ResetRoutine());
            }
        }

        public void OnCollisionEnter(Collision col)
        {
            collision.CollisionEnter(col);
        }

        public void OnCollisionStay(Collision col)
        {
            collision.CollisionStay(col);
        }

        public void OnCollisionExit(Collision col)
        {
            collision.CollisionExit(col);
        }

        void AutoReverse()
        {
            if (!HasControl)
                return;

            if (engine.TransmissionType == TransmissionType.Manual &&
                !RGSKCore.Instance.VehicleSettings.autoReverseManual || Burnout)
            {
                IsAutoReverse = false;
                return;
            }

            if (!IsAutoReverse)
            {
                if (_brakeInput > 0.1f && CurrentSpeed <= 2)
                {
                    engine.ShiftToGear(0);
                }
            }
            else
            {
                if (_throttleInput > 0.1f && CurrentSpeed <= 2)
                {
                    engine.ShiftToGear(2);
                }
            }

            IsAutoReverse = engine.Gear == 0;

            if (IsAutoReverse)
            {
                var val = _throttleInput;
                _throttleInput2 = _brakeInput;
                _brakeInput2 = val;
            }
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

        public void ShiftUp()
        {
            if (!HasControl)
                return;

            engine?.ShiftUp();
        }

        public void ShiftDown()
        {
            if (!HasControl)
                return;

            engine?.ShiftDown();
        }

        public void StartEngine(float delay) => engine?.StartEngine(delay);

        public void StopEngine() => engine?.StopEngine();

        public void OnReset() => _reset = true;

        public void Repair() => _meshDeformer?.Repair();

        IEnumerator ResetRoutine()
        {
            yield return new WaitForFixedUpdate();
            Entity.Rigid.isKinematic = false;
            IsAutoReverse = false;
        }

        void OnCameraTargetChanged(Transform target)
        {
            sound?.ToggleWindSound(target == transform);
        }
    }
}