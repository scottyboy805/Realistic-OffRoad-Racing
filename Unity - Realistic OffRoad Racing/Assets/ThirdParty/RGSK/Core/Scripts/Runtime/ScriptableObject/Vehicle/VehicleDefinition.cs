using UnityEngine;

namespace RGSK
{
    [System.Serializable]
    public class VehicleStats
    {
        [Header("Performance")]
        [Range(0, 1)] public float speed;
        [Range(0, 1)] public float acceleration;
        [Range(0, 1)] public float braking;
        [Range(0, 1)] public float handling;

        [Header("Color")]
        public int color;
    }

    [CreateAssetMenu(menuName = "RGSK/Vehicle/Vehicle Definition")]
    public class VehicleDefinition : ItemDefinition
    {
        public GameObject prefab;
        public VehicleManufacturer manufacturer;
        public VehicleClass vehicleClass;
        public VehicleStats defaultStats;
        public string year;

        public string GetVehicleClassName()
        {
            if (vehicleClass == null)
                return "";

            if (string.IsNullOrWhiteSpace(vehicleClass.displayName))
                return vehicleClass.name;

            return vehicleClass.displayName;
        }

        public string GetVehicleManufacturerName()
        {
            if (manufacturer == null)
                return "";

            if (string.IsNullOrWhiteSpace(manufacturer.displayName))
                return manufacturer.name;

            return manufacturer.displayName;
        }

        public float GetPerformanceRating()
        {
            if (vehicleClass == null)
                return -1;

            return vehicleClass.performanceRating;
        }
    }
}