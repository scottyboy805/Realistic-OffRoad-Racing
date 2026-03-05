using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class VehicleExhaustEffect : MonoBehaviour
    {
        public ExhaustEffectType type;

        public ParticleSystem Particles
        {
            get
            {
                if (_particles == null)
                {
                    _particles = GetComponent<ParticleSystem>();
                }

                return _particles;
            }
        }

        ParticleSystem _particles;
    }
}