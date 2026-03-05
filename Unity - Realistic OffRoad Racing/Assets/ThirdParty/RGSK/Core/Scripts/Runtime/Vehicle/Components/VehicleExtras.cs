using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    [System.Serializable]
    public class VehicleExtras : VehicleComponent
    {
        [Header("Wheel Alignment")]
        public VisualWheelAlignment frontAlignment;
        public VisualWheelAlignment rearAlignment;

        [Header("Steering Wheel")]
        [SerializeField] Transform steeringWheel;
        [SerializeField] Axis rotateAxis = Axis.Z;
        [SerializeField][Range(45, 90)] float maxAngle = 90;
        [SerializeField] float rotateSpeed = 5;
        [SerializeField] bool invertRotation;

        [Header("Misc")]
        [SerializeField][Range(0, 1)] float backfireProbability = 0.5f;
        [SerializeField][Range(0, 1f)] float chassisShakeAmount = 0.5f;

        List<ParticleSystem> _exhaustSmoke = new List<ParticleSystem>();
        List<ParticleSystem> _exhaustBackfire = new List<ParticleSystem>();
        AudioSource _backfireAudiosource;
        Quaternion _defaultSteeringAngle;
        float _lastBackfire;

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);
            _backfireAudiosource = AudioHelper.CreateAudioSource(null, false, false, false, 1, 1, AudioGroup.Vehicle.ToString(), Vehicle.transform);

            if (steeringWheel != null)
            {
                _defaultSteeringAngle = steeringWheel.localRotation;
            }

            foreach (var e in vc.GetComponentsInChildren<VehicleExhaustEffect>())
            {
                switch (e.type)
                {
                    case ExhaustEffectType.Smoke:
                        {
                            _exhaustSmoke.Add(e.Particles);
                            break;
                        }

                    case ExhaustEffectType.Backfire:
                        {
                            _exhaustBackfire.Add(e.Particles);
                            break;
                        }
                }
            }
        }

        public override void Update()
        {
            if (Vehicle.engine.Cutoff() && Vehicle.ThrottleInput > 0.9f)
            {
                Backfire();
            }

            if (Vehicle.CurrentSpeed < 2)
            {
                var force = (Vehicle.engine.Rpm * chassisShakeAmount * Vehicle.ThrottleInput) * Random.Range(-1.0f, 1.0f);
                Vehicle.Entity.Rigid.AddRelativeTorque(Vector3.forward * force);
            }

            foreach (var w in Vehicle.dynamics.AllWheels)
            {
                if (w.axle == WheelAxle.Front)
                {
                    w.UpdateVisualAlignment(frontAlignment);
                }
                else
                {
                    w.UpdateVisualAlignment(rearAlignment);
                }
            }

            SteeringWheel();
            ExhaustSmoke();
        }

        void SteeringWheel()
        {
            if (steeringWheel == null)
                return;

            var currentRotation = steeringWheel.localRotation;
            var targetAngle = Vehicle.SteerInput * maxAngle;

            if (invertRotation)
            {
                targetAngle *= -1;
            }

            switch (rotateAxis)
            {
                case Axis.X:
                    currentRotation = _defaultSteeringAngle * Quaternion.AngleAxis(targetAngle, Vector3.right);
                    break;

                case Axis.Y:
                    currentRotation = _defaultSteeringAngle * Quaternion.AngleAxis(targetAngle, Vector3.up);
                    break;

                case Axis.Z:
                    currentRotation = _defaultSteeringAngle * Quaternion.AngleAxis(targetAngle, Vector3.forward);
                    break;
            }

            steeringWheel.localRotation = Quaternion.Slerp(steeringWheel.localRotation, currentRotation, Time.deltaTime * rotateSpeed);
        }

        void ExhaustSmoke()
        {
            if (_exhaustSmoke.Count > 0)
            {
                var amount = 0f;

                if (Vehicle.CurrentSpeed <= 2 && Vehicle.IsEngineOn)
                {
                    amount = Mathf.InverseLerp(Vehicle.engine.maxRpm * 0.9f, Vehicle.engine.idleRpm, Vehicle.engine.Rpm);
                }

                for (int i = 0; i < _exhaustSmoke.Count; i++)
                {
                    var emissionModule = _exhaustSmoke[i].emission;
                    emissionModule.rateOverTime = amount * 100;

                    if (!_exhaustSmoke[i].isPlaying)
                    {
                        _exhaustSmoke[i].Play();
                    }
                }
            }
        }

        public void Backfire()
        {
            if (_exhaustBackfire.Count == 0 || Vehicle.nitrous.Active)
                return;

            if (Time.time > _lastBackfire && Random.value <= backfireProbability)
            {
                _lastBackfire = Time.time + 1f;

                foreach (var p in _exhaustBackfire)
                {
                    p.Play();
                    _backfireAudiosource.transform.position = p.transform.position;
                }

                _backfireAudiosource.clip = RGSKCore.Instance.VehicleSettings.backfireSounds.GetRandom();
                _backfireAudiosource.volume = Random.Range(0.8f, 1.2f);
                _backfireAudiosource.pitch = Random.Range(0.8f, 1.2f);
                _backfireAudiosource.Play();
            }
        }
    }
}