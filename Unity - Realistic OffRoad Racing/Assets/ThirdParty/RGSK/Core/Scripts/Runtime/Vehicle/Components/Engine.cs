using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [System.Serializable]
    public class Engine : VehicleComponent
    {
        [Header("Engine")]
        public float maximumEngineTorque = 300;
        public float maxTorqueRPM = 6500;
        public float idleRpm = 1000;
        public float maxRpm = 8000;
        public float torqueMultiplier = 1;
        public float startIgnitionDelay => 1;
        public bool autoCreateTorqueCurve = true;
        public AnimationCurve torqueCurve;

        [Header("Transmission")]
        [SerializeField][Range(-10, 10)] List<float> gearRatios = new List<float>() { -5, 0, 4, 2.5f, 1.5f, 1.1f, 1, 0.85f };
        [SerializeField] float finalDriveRatio = 4f;
        [SerializeField][Range(0, 1)] float autoUpshiftRange = 0.9f;
        [SerializeField][Range(0, 1)] float autoDownshiftRange = 0.7f;

        public float Rpm { get; set; }
        public int Gear { get; private set; }
        public TransmissionType TransmissionType { get; set; }
        public bool Running { get; private set; }
        public bool Starting { get; private set; }

        bool _shiftingGear;
        float _shiftUpRpm;
        float _shiftDownRpm;
        float _lastShift;
        float _wheelTorque;
        float _lastCutoff;
        
        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            Gear = 2;

            if (autoCreateTorqueCurve)
            {
                torqueCurve = new AnimationCurve
                (
                    new Keyframe(0, 0),
                    new Keyframe(maxTorqueRPM * 0.25f, maximumEngineTorque * 0.5f),
                    new Keyframe(maxTorqueRPM * 0.5f, maximumEngineTorque * 0.75f),
                    new Keyframe(maxTorqueRPM, maximumEngineTorque),
                    new Keyframe(maxRpm, maximumEngineTorque * 0.9f)
                );

                torqueCurve.MakeLinear();
            }

            _shiftUpRpm = maxRpm * autoUpshiftRange;
            _shiftDownRpm = _shiftUpRpm * autoDownshiftRange;
        }

        public override void Update()
        {
            if (!Running)
            {
                Rpm = Mathf.Lerp(Rpm, 0, Time.deltaTime * 3f);
                Vehicle.dynamics?.ApplyMotorTorque(0);
                return;
            }

            if (Rpm >= maxRpm)
            {
                _lastCutoff = Time.time;
            }

            var targetRpm = 0f;
            var wheelRpm = Vehicle.dynamics?.GetWheelRPM() ?? 0;
            var throttle = Cutoff() ? 0 : Vehicle.ThrottleInput;

            if (Gear != 1)
            {
                targetRpm = idleRpm + (wheelRpm * gearRatios[Gear] * finalDriveRatio);
            }

            if (Gear == 1 || Vehicle.HandbrakeInput > 0 || Vehicle.Burnout)
            {
                targetRpm = idleRpm + (maxRpm * throttle);
            }

            Rpm = Mathf.Lerp(Rpm, targetRpm, Time.deltaTime * 3f);
            Rpm = Mathf.Abs(Rpm);

            _wheelTorque = GetTorque() * gearRatios[Gear] * finalDriveRatio * throttle * Vehicle.nitrous.Force;
            Vehicle.dynamics?.ApplyMotorTorque(_wheelTorque);
            AutomaticShifting();
        }

        void AutomaticShifting()
        {
            if (TransmissionType == TransmissionType.Manual)
                return;

            if (CanAutoShiftUp())
            {
                ShiftUp();
            }

            if (Gear > 2 && Rpm < _shiftDownRpm)
            {
                ShiftDown();
            }
        }

        public void ShiftUp()
        {
            if (!CanShiftToGear(Gear + 1))
                return;

            ShiftToGear(Gear + 1);

            if (Vehicle.ThrottleInput > 0.9f)
            {
                Vehicle.extra.Backfire();
            }
        }

        public void ShiftDown()
        {
            if (!CanShiftToGear(Gear - 1))
                return;

            ShiftToGear(Gear - 1);
            Vehicle.extra.Backfire();
        }

        public void ShiftToGear(int newGear)
        {
            if (Gear == newGear)
                return;

            _lastShift = Time.time + 0.5f;

            if (Gear != 1)
            {
                var oldRpm = Rpm;
                var oldGearRatio = gearRatios[Gear];
                var newGearRatio = gearRatios[newGear];
                Rpm = oldRpm * (newGearRatio / oldGearRatio);
            }

            Gear = newGear;
        }

        public void StartEngine(float delay = -1)
        {
            if (Starting || Running)
                return;

            Vehicle.StartCoroutine(StartEngineRoutine(delay == -1 ? startIgnitionDelay : delay));
        }

        IEnumerator StartEngineRoutine(float delay)
        {
            if (delay == 0)
            {
                Running = true;
                yield break;
            }

            Starting = true;

            yield return new WaitForSeconds(delay);

            Starting = false;
            Running = true;
        }

        public void StopEngine()
        {
            if (!Running)
                return;

            Running = false;
        }

        public bool Cutoff()
        {
            return Time.time < _lastCutoff + 0.01f;
        }

        bool CanAutoShiftUp()
        {
            return Rpm > _shiftUpRpm &&
                   Gear > 0 &&
                   Vehicle.CurrentSpeed > 5 &&
                   Vehicle.ThrottleInput > 0.9f && !Vehicle.IsAutoReverse &&
                   Vehicle.HandbrakeInput < 1 && !Vehicle.Burnout &&
                   Vehicle.dynamics.AllWheelsGrounded() &&
                   !Vehicle.dynamics.IsDrivenWheelSlipping(RGSKCore.Instance.VehicleSettings.autoShiftWheelSlipThreshold);
        }

        bool CanShiftToGear(int newGear)
        {
            if (Time.time < _lastShift || Gear == newGear ||
                newGear < 0 || newGear > gearRatios.Count - 1)
                return false;

            return true;
        }

        float GetTorque()
        {
            return torqueCurve.Evaluate(Rpm) * torqueMultiplier;
        }
    }
}