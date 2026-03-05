using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Recorder Settings")]
    public class RecorderSettings : ScriptableObject
    {
        [Tooltip("The maximum time (seconds) that a recording can last. Leave at -1 for no limit.")]
        public float recordingTimeLimit = -1;
        public int maxPlaybackSpeed = 5;
        public float slowMotionTimeScale = 0.5f;
    }
}