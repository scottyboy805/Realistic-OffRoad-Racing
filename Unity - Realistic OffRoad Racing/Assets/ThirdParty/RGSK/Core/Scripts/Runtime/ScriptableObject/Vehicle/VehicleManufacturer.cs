using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Vehicle/Vehicle Manufacturer")]
    public class VehicleManufacturer : ScriptableObject
    {
        public string displayName;
        public CountryDefinition country;
        public Sprite icon;

        [TextArea(5, 5)]
        public string description;
    }
}