using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    [System.Serializable]
    public class Nitrous : VehicleComponent
    {
        public bool enabled;
        [Range(0, 1)] public float capacity = 1;

        [Tooltip("The amount of torque multiplied to the engine while active.")]
        [SerializeField] float strength = 2f;

        [Tooltip("How long (in seconds) it will take to deplete at full capacity.")]
        [SerializeField] float depletionRate = 3f;

        [Header("Regeneration")]
        [Tooltip("Will it regenerate automatically?")]
        [SerializeField] bool autoRegenerate = true;

        [Tooltip("How long (in seconds) it will take to start regeneration after using.")]
        [SerializeField] float regenerationWait = 2f;

        [Tooltip("How long (in seconds) it will take to regenerate to full capacity.")]
        [SerializeField] float regenerationRate = 5f;

        public bool Active => _active;
        public float Force => _active && capacity > 0 ? strength : 1;

        List<ParticleSystem> _nitroParticles = new List<ParticleSystem>();
        AudioSource _audiosource;
        float _activeTime;
        float _lastActiveTime;
        bool _active;
        bool _emitting;

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            _audiosource = AudioHelper.CreateAudioSource(
                            RGSKCore.Instance.VehicleSettings.nitroSound,
                            false,
                            true,
                            false,
                            1,
                            1,
                            AudioGroup.Vehicle.ToString(),
                            Vehicle.transform);

            foreach (var e in vc.GetComponentsInChildren<VehicleExhaustEffect>())
            {
                switch (e.type)
                {
                    case ExhaustEffectType.Nitrous:
                        {
                            _nitroParticles.Add(e.Particles);
                            break;
                        }
                }
            }
        }

        public override void Update()
        {
            if (!enabled)
                return;

            _active = Vehicle.IsEngineOn && Vehicle.ThrottleInput > 0 && Vehicle.CurrentGear > 1 && Vehicle.NitrousInput > 0;

            if (_active && capacity > 0)
            {
                _activeTime += Time.deltaTime;
                _lastActiveTime = Time.time;

                capacity = Mathf.MoveTowards(capacity, 0, (1 / depletionRate) * Time.deltaTime);

                if (!_audiosource.isPlaying)
                {
                    _audiosource.Play();
                }

                ToggleParticles(true);
            }
            else
            {
                if (autoRegenerate && !_active && Time.time > _lastActiveTime + regenerationWait)
                {
                    capacity = Mathf.MoveTowards(capacity, 1, (1 / regenerationRate) * Time.deltaTime);
                }

                _activeTime = 0;
                _audiosource.Stop();
                ToggleParticles(false);
            }
        }

        void ToggleParticles(bool toggle)
        {
            if (_emitting == toggle)
                return;

            _emitting = toggle;

            foreach (var p in _nitroParticles)
            {
                if (_emitting)
                {
                    p.Play();
                }
                else
                {
                    p.Stop();
                }
            }
        }
    }
}