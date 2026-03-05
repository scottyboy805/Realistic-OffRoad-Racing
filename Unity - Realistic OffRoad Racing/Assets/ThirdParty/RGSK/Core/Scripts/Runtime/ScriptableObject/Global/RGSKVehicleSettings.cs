using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Vehicle Physics")]
    public class RGSKVehicleSettings : ScriptableObject
    {
        [Header("Handling")]
        public VehicleHandlingData gripHandling;
        public VehicleHandlingData driftHandling;

        [Header("Assists")]
        public bool autoThrottle;
        public bool autoBrake;

        [Header("Engine & Transmission")]
        public TransmissionType transmissionType;
        public bool autoStartEngine = true;
        public bool autoReverseManual = true;
        public float autoShiftWheelSlipThreshold = 0.25f;

        [Header("Sounds")]
        public AudioClip windSound;
        public AudioClip nitroSound;
        public List<AudioClip> backfireSounds = new List<AudioClip>();

        [Header("Wheel Surfaces")]
        public WheelSurface fallbackWheelSurface;
        public List<WheelSurface> wheelSurfaces = new List<WheelSurface>();

        [Header("Collision Surfaces")]
        public CollisionSurface fallbackCollisionSurface;
        public List<CollisionSurface> collisionSurfaces = new List<CollisionSurface>();

        [Header("Ghost")]
        public Material ghostMaterial;
        public string ghostMaterialColorProperty = "_BaseColor";
        public string ghostMaterialTextureProperty = "_MainTex";

        [Header("Misc")]
        public ColorList vehicleColorList;
        public List<VehicleDriver> vehicleDrivers = new List<VehicleDriver>();
    }
}