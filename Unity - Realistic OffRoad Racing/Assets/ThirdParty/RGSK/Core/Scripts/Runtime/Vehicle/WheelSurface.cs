using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Surface/Wheel Surface")]
    public class WheelSurface : ScriptableObject
    {
        [Header("ID")]
        public PhysicsMaterial physicMaterial;
        public List<Texture2D> terrainTextures = new List<Texture2D>();

        [Header("Properties")]
        public SurfaceEmissionType emissionType;
        public float forwardGrip = 1;
        public float sidewaysGrip = 1;
        public float dampingRate = 0.25f;
        public float minSlip = 0.2f;

        [Header("FX")]
        public AudioClip wheelslipSound;
        public ParticleSystem wheelslipParticles;
        public TyremarkGenerator tyremarksPrefab;
    }
}