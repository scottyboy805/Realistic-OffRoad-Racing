using UnityEngine;
using Cinemachine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Camera/Camera Perspective")]
    public class CameraPerspective : ScriptableObject
    {
        public string perspectiveName;
        public CinemachineVirtualCamera virtualCamera;

        [Header("Audio")]
        [Range(0, 1)] public float volume = 0.8f;
        public float lowPassFrequency = 5000;

        [Header("Reverse Camera")]
        public bool reverseCameraOnLookBack;
        public bool reverseCameraOnUI;

        [HideInInspector]
        public CinemachineVirtualCamera runtimeCamera;
    }
}