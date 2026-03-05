using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Settings/Camera Settings", order = 0)]
    public class CameraSettings : ScriptableObject
    {
        public List<CameraPerspective> perspectives = new List<CameraPerspective>();

        [Header("Route Cameras")]
        public bool cycleToRouteCameras;
        
        [Header("Fallback Audio Properties")]
        [Range(0, 1)] public float fallbackVolume = 0.8f;
        public float fallbackLowPassFrequency = 5000;
    }
}