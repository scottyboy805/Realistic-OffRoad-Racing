using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Proximity Arrow Settings")]
    public class ProximityArrowSettings : ScriptableObject
    {
        public RectTransform arrowPrefab;
        public int maxArrows = 5;
        public float radius = 6;
        public LayerMask layers;
    }
}