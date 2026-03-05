using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Reposition Settings")]
    public class RepositionSettings : ScriptableObject
    {
        public RaceRepositionRotationOptions raceRepositionRotationOption;
        public Vector3 repositionOffset = new Vector3(0, 1, 0);
        public float ghostedDuration = 3;
        public float updsideDownWaitTime = 3.0f;
        public float updsideDownMinSpeed = 10;
        public bool repositionIfUpsideDown = true;
        public bool flickerMesh = true;
        public bool ghostMesh = false;
        [Range(5, 10)] public float flickerSpeed = 7;
        public LayerMask safeCheckLayers = ~0;
    }
}