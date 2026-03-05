using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class Dynamics : VehicleComponent
    {
        public VehicleHandlingMode handlingMode;
        public Drivetrain drivetrain;
        public float antiRollAmount = 2500;

        public AnimationCurve downForceCurve = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(200, 25));

        public List<Wheel> AllWheels { get; private set; } = new List<Wheel>();
        public List<Wheel> FrontAxleWheels { get; private set; } = new List<Wheel>();
        public List<Wheel> RearAxleWheels { get; private set; } = new List<Wheel>();

        List<Wheel> _drivenWheels = new List<Wheel>();
        VehicleHandlingData _handling;
        int _driveWheelCount;
        float _targetSteerAngle;
        float _currentSteerAngle;
        float _oldRotation;

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            Vehicle.GetComponentsInChildren(AllWheels);
            FrontAxleWheels = AllWheels.Where(x => x.axle == WheelAxle.Front).ToList();
            RearAxleWheels = AllWheels.Where(x => x.axle == WheelAxle.Rear).ToList();

            SetDrivetrain(drivetrain);
        }

        public void SetDrivetrain(Drivetrain drivetrain)
        {
            if (AllWheels.Count == 0)
                return;

            _drivenWheels.Clear();
            _driveWheelCount = 0;

            FrontAxleWheels.ForEach(x =>
            {
                x.steer = true;
                x.drive = drivetrain == Drivetrain.FWD || drivetrain == Drivetrain.AWD;
            });

            RearAxleWheels.ForEach(x =>
            {
                x.steer = false;
                x.drive = drivetrain == Drivetrain.RWD || drivetrain == Drivetrain.AWD;
            });

            AllWheels.ForEach(x =>
            {
                if (x.drive)
                {
                    _drivenWheels.Add(x);
                    _driveWheelCount++;
                }
            });
        }

        public void SetHandling(VehicleHandlingData data)
        {
            _handling = data;
            Vehicle.Entity.Rigid.angularDamping = data.angularDrag;

            foreach (var w in AllWheels)
            {
                w.UpdateForwardFriction(data.forwardFriction);
                w.UpdateSidewaysFriction(data.sidewaysFriction);
            }
        }

        public override void Update()
        {
            var h = GetHandlingData(handlingMode);

            if (_handling != h)
            {
                SetHandling(h);
            }

            if (Vehicle.Entity.Rigid == null || _handling == null)
                return;

            _targetSteerAngle = _handling.steeringCurve.Evaluate(Vehicle.CurrentSpeed) * Vehicle.SteerInput;
            _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, _targetSteerAngle, _handling.steeringSpeed * Time.deltaTime);

            ApplySteer(_currentSteerAngle);
            ApplyDownforce();
            Antiroll(FrontAxleWheels);
            Antiroll(RearAxleWheels);
        }

        void ApplyDownforce()
        {
            Vehicle.Entity.Rigid.AddForce(
                                        -Vehicle.transform.up *
                                        downForceCurve.Evaluate(Vehicle.CurrentSpeed) *
                                        Vehicle.Entity.Rigid.linearVelocity.magnitude);
        }

        void Antiroll(List<Wheel> wheels)
        {
            if (wheels.Count < 2)
                return;

            WheelHit hit;
            var travelL = 1.0f;
            var travelR = 1.0f;

            var groundedL = wheels[0].WheelCollider.GetGroundHit(out hit);
            if (groundedL)
            {
                travelL = (-wheels[0].transform.InverseTransformPoint(hit.point).y - wheels[0].WheelCollider.radius) / wheels[0].WheelCollider.suspensionDistance;
            }

            var groundedR = wheels[1].WheelCollider.GetGroundHit(out hit);
            if (groundedR)
            {
                travelR = (-wheels[1].transform.InverseTransformPoint(hit.point).y - wheels[1].WheelCollider.radius) / wheels[1].WheelCollider.suspensionDistance;
            }

            var antiRollForce = (travelL - travelR) * antiRollAmount;

            if (groundedL)
                Vehicle.Entity.Rigid.AddForceAtPosition(wheels[0].transform.up * -antiRollForce, wheels[0].transform.position);

            if (groundedR)
                Vehicle.Entity.Rigid.AddForceAtPosition(wheels[1].transform.up * antiRollForce, wheels[1].transform.position);
        }

        public void ApplyMotorTorque(float value)
        {
            value /= _driveWheelCount;

            foreach (var w in AllWheels)
            {
                w.SetMotorTorque(value);
            }
        }

        public void ApplyBrakeTorque(float value, WheelAxle axle)
        {
            foreach (var w in AllWheels)
            {
                if (w.axle == axle)
                {
                    w.SetBrakeTorque(value);
                }
            }
        }

        public void ApplySteer(float value)
        {
            foreach (var w in AllWheels)
            {
                w.SetSteerAngle(value);
            }
        }

        public float GetWheelRPM()
        {
            var rpm = 0f;

            foreach (var w in AllWheels)
            {
                if (w.drive)
                {
                    rpm += w.Rpm;
                }
            }

            if (_driveWheelCount > 0)
            {
                return rpm / _driveWheelCount;
            }

            return 0;
        }

        public bool AllWheelsGrounded() => AllWheels.All(x => x.IsGrounded);

        public bool IsDrivenWheelSlipping(float value)
        {
            var result = false;

            foreach (var w in _drivenWheels)
            {
                result = (w.ForwardSlip() + w.SidewaysSlip()) >= value;
            }

            return result;
        }

        VehicleHandlingData GetHandlingData(VehicleHandlingMode mode)
        {
            return mode == VehicleHandlingMode.Grip ?
                RGSKCore.Instance.VehicleSettings.gripHandling :
                RGSKCore.Instance.VehicleSettings.driftHandling;
        }

        public void ResetTyremarks()
        {
            AllWheels.ForEach(x => x.ResetTyremarkIndex());
        }
    }
}