using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class WheelFriction
    {
        public float extremumSlip = 0.4f;
        public float extremumValue = 1f;
        public float asymptoteSlip = 0.8f;
        public float asymptoteValue = 0.5f;
    }

    [CreateAssetMenu(menuName = "RGSK/Vehicle/Vehicle Handling Data")]
    public class VehicleHandlingData : ScriptableObject
    {
        public AnimationCurve steeringCurve = new AnimationCurve(
           new Keyframe(0, 40),
           new Keyframe(200, 5));

        [Range(0, 360)] public float steeringSpeed = 180;
        public float angularDrag = 5f;
        public WheelFriction forwardFriction;
        public WheelFriction sidewaysFriction;
    }
}