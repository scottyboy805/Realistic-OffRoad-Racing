using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/AI/AI Common Settings")]
    public class AICommonSettings : ScriptableObject
    {
        public LayerMask obstacleLayers;

        [Tooltip("How far from the target to register that it has been reached.")]
        public float targetReachedDistance = 5;
        public float forwardSensorRange = 100;

        [Tooltip("The distance to travel before boost can be used.")]
        public float minBoostDistance = 100;

        [Tooltip("The min and max angles (degrees) at which the AI will be cautious.")]
        public Vector2 minMaxCautionAngle = new Vector2(10, 90);

        [Tooltip("The min and max distances (meters) from the target that the AI will be cautious.")]
        public Vector2 minMaxCautionDistance = new Vector2(10, 100);

        [Tooltip("The min and max time (seconds) it takes to reset the travel offset.")]
        public Vector2 minMaxTravelResetDuration = new Vector2(5, 10);

        [Tooltip("How long the AI will maintain their route offset when activated before returning to the racing line.")]
        public float travelResetDurationOnActivate = 10;

        [Tooltip("The speed (KM/h) to consider the AI is stuck.")]
        public float stuckSpeed = 3;

        [Tooltip("How long to wait before reversing when stuck.")]
        public float stuckReverseTime = 2;

        [Tooltip("How long to reverse for.")]
        public float reverseDuration = 2;

        [Tooltip("How many reverse attempts before resetting.")]
        public int maxReverseAttempts = 3;
    }
}
