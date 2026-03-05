using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/AI")]
    public class RGSKAISettings : ScriptableObject
    {
        [Header("Common")]
        public AICommonSettings commonSettings;
        public AIBehaviourSettings defaultBehaviour;

        [Header("Race")]
        public List<AIBehaviourSettings> difficulties = new List<AIBehaviourSettings>();
        public List<RaceStateAIBehaviourComposite> raceStateBehaviours = new List<RaceStateAIBehaviourComposite>();

        [Header("Profiles")]
        public List<ProfileDefinition> opponentProfiles = new List<ProfileDefinition>();
    }
}