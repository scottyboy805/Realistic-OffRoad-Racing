using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class AIInput : AIComponent
    {
        public float ThrottleInput { get; private set; }
        public float BrakeInput { get; private set; }
        public float SteerInput { get; private set; }
        public float BoostInput { get; private set; }

        float _lastBoostInputCheck;

        public override void Update()
        {
            if (Controller.Entity.Vehicle == null)
                return;

            if (Controller.Entity.Vehicle.IsInitialized && !Controller.Entity.Vehicle.IsEngineOn)
            {
                Controller.Entity.Vehicle.StartEngine(0);
            }

            ThrottleInput = GetThrottleInput();
            BrakeInput = GetBrakeInput();
            SteerInput = GetSteerInput();
            BoostInput = GetBoostInput();

            if (Controller.RecoverController.IsStuck)
            {
                ThrottleInput = 0;
                BrakeInput = 1;
                SteerInput *= -1;
                BoostInput = 0;
            }

            Controller.Entity.Vehicle.ThrottleInput = ThrottleInput;
            Controller.Entity.Vehicle.SteerInput = SteerInput;
            Controller.Entity.Vehicle.BrakeInput = BrakeInput;
            Controller.Entity.Vehicle.NitrousInput = BoostInput;
            Controller.Entity.Vehicle.HandbrakeInput = 0;
        }

        float GetSteerInput()
        {
            if (!Controller.IsActive)
                return 0;

            var localTarget = Controller.transform.InverseTransformPoint(Controller.TargetPosition);
            var targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            
            return Mathf.Clamp(targetAngle * (Controller.Behaviour.steerSensitivity * 0.05f), -1, 1) * Mathf.Sign(Controller.Entity.Vehicle.CurrentSpeed);
        }

        float GetThrottleInput()
        {
            if (!Controller.IsActive)
                return 0;

            var sensitivity = 0f;

            if (Controller.Entity.Vehicle.CurrentSpeed < Controller.TargetSpeed)
            {
                sensitivity = Controller.Behaviour.throttleSensitivity * 0.1f;
            }

            var value = ((Controller.TargetSpeed - Controller.Entity.Vehicle.CurrentSpeed) * sensitivity);
            return Mathf.Clamp(value, 0, 1);
        }

        float GetBrakeInput()
        {
            if (!Controller.IsActive)
                return 0;

            var sensitivity = 0f;

            if (Controller.Entity.Vehicle.CurrentSpeed > Controller.TargetSpeed)
            {
                sensitivity = Controller.Behaviour.brakeSensitivity * 0.1f;
            }

            var value = ((Controller.TargetSpeed - Controller.Entity.Vehicle.CurrentSpeed) * sensitivity);
            return Mathf.Clamp(value, -1, 0) * -1;
        }

        float GetBoostInput()
        {
            var distance = Controller.Entity.Competitor?.LapTracker?.GetTotalDistance() ?? float.MaxValue;

            if (!Controller.Behaviour.canUseBoost ||
                distance < Controller.Behaviour.commonSettings.minBoostDistance ||
                Controller.Entity.Vehicle.CurrentSpeed > Controller.TargetSpeed * 0.95f)
            {
                return 0;
            }

            if (Controller.CurrentSpeedZone != null)
            {
                return GetInputChance(Controller.CurrentSpeedZone.boostProbability);
            }

            float GetInputChance(float probability)
            {
                var value = 0f;

                if (Time.time > _lastBoostInputCheck)
                {
                    _lastBoostInputCheck = Time.time + 5;
                    var val = UnityEngine.Random.value;
                    if (val <= probability)
                    {
                        value = 1f;
                    }
                }
                else
                {
                    value = BoostInput;
                }

                return value;
            }

            return 0f;
        }

        public override void Reset()
        {

        }
    }
}