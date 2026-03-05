using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Surface/Collision Surface")]
    public class CollisionSurface : ScriptableObject
    {
        [Header("ID")]
        public PhysicsMaterial physicMaterial;

        [Header("FX")]
        public List<AudioClip> hitSounds;
        public AudioClip scrapeSound;
        public ParticleSystem hitParticles;
    }
}