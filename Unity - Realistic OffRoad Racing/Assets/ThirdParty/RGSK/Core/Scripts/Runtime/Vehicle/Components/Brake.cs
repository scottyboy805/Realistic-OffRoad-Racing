using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [System.Serializable]
    public class Brake : VehicleComponent
    {
        [SerializeField][Range(0, 1000)] float brakeTorque = 1000;
        [SerializeField][Range(1000, 3000)] float handbrakeTorque = 2000;
        [SerializeField][Range(0, 30)] float brakeForce = 10f;
        [SerializeField][Range(0, 30)] float decelerationForce = 5f;

        public override void Update()
        {
            var dir = Vehicle.Entity.Rigid.TravelDirection();

            if (Vehicle.Burnout)
            {
                Vehicle.dynamics?.ApplyBrakeTorque(0, WheelAxle.Front);
                Vehicle.dynamics?.ApplyBrakeTorque(0, WheelAxle.Rear);
                Vehicle.Entity.Rigid.AddRelativeForce(Vector3.forward * brakeForce * -dir, ForceMode.Acceleration);
                return;
            }

            if (Vehicle.HandbrakeInput < 1)
            {
                //footbrake
                Vehicle.dynamics?.ApplyBrakeTorque(Vehicle.BrakeInput * brakeTorque, WheelAxle.Front);
                Vehicle.dynamics?.ApplyBrakeTorque(Vehicle.BrakeInput * brakeTorque, WheelAxle.Rear);
            }
            else
            {
                //handbrake
                if (Vehicle.BrakeInput < 1)
                {
                    Vehicle.dynamics?.ApplyBrakeTorque(handbrakeTorque * Vehicle.HandbrakeInput, WheelAxle.Rear);
                }
            }

            //forward brake force
            if (Vehicle.BrakeInput > 0)
            {
                Vehicle.Entity.Rigid.AddRelativeForce(Vector3.forward * brakeForce * -dir * Vehicle.BrakeInput, ForceMode.Acceleration);
            }

            //coasting deceleration
            if (Vehicle.ThrottleInput == 0 && Vehicle.BrakeInput == 0 || Vehicle.engine.Gear == 1)
            {
                Vehicle.Entity.Rigid.AddRelativeForce(Vector3.forward * decelerationForce * -dir, ForceMode.Acceleration);
            }
        }
    }
}