using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Catchup Settings")]
    public class CatchupSettings : ScriptableObject
    {
        public bool enabled = true;

        [Tooltip("The distance range where the catchup effect applies. X = start distance, Y = full effect distance.")]
        public Vector2 effectRange = new Vector2(50, 100);

        [Tooltip("How much vehicles will be slowed down when catchup is at full effect.")]
        [Range(1, 5)] public float slowDownStrength = 1.5f;

        [Tooltip("How much vehicles will be sped up when catchup is at full effect.")]
        [Range(1, 5)] public float speedUpStrength = 2.0f;
    }
}