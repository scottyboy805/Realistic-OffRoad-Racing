using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Input")]
    public class RGSKInputSettings : ScriptableObject
    {
        public MobileControlType mobileControlType;
        [Range(1, 2)] public float accelerometerSensitivity = 1f;
        public bool vibrate = true;
    }
}