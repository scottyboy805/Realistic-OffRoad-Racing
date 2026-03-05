using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class RaceStateAIBehaviourComposite
    {
        public RaceState state;
        public AIBehaviourSettings behaviour;
    }

    [CreateAssetMenu(menuName = "RGSK/AI/AI Behaviour")]
    public class AIBehaviourSettings : ScriptableObject
    {
        public string displayName;

        [TextArea(5, 5)]
        public string description;

        public AICommonSettings commonSettings => RGSKCore.Instance.AISettings.commonSettings;

        [Header("Navigation")]
        public TargetApproachAction targetApproachAction;

        [Tooltip("How effectively the AI will use the throttle to reach the target speed.")]
        [Range(0, 1)]
        public float throttleSensitivity = 1f;

        [Tooltip("How effectively the AI will use the brake to reach the target speed.")]
        [Range(0, 1)]
        public float brakeSensitivity = 1f;

        [Tooltip("How effectively the AI steer towards the target.")]
        [Range(0, 1)]
        public float steerSensitivity = 0.5f;

        [Tooltip("The target speed override. Set to -1 to ignore.")]
        public float speedOverride = -1;

        public bool canUseBoost = true;

        [Header("Caution")]
        [Tooltip("The percentage of current speed that the AI will slow down to when fully cautious.\n\n0 =full slow down, 1 = no slow down")]
        [Range(0, 1)]
        public float cautiousSpeedFactor = 0.1f;

        [Tooltip("How cautious the AI will be in detecting a spinout.")]
        [Range(0, 1)]
        public float spinCautionAmount = 0.5f;

        [Header("Avoidance")]
        public bool canAvoid = true;
        public bool keepWithinRouteWidth = true;
        public float avoidImpactTime = 2f;
        public float obstacleSlowDownDistance = 10f;

        [Header("Recovery")]
        public bool canRecover = true;

        public static AIBehaviourSettings CreateDefault()
        {
            return ScriptableObject.CreateInstance<AIBehaviourSettings>();
        }
    }
}