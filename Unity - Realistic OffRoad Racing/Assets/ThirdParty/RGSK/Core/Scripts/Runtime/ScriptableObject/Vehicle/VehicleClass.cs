using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Vehicle/Vehicle Class")]
    public class VehicleClass : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        [Range(0, 1)] public float performanceRating;

        [TextArea(5, 5)]
        public string description;
    }
}