using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Slipstream Settings")]
    public class SlipstreamSettings : ScriptableObject
    {
        public bool enabled = true;
        [Range(0, 5)] public float strength = 1f;
        public float maxRange = 100;
        public float minSpeed = 100;
        public LayerMask layers;
    }
}