using UnityEngine;

namespace RGSK
{
    public class NitrousCollectible : RaceCollectible
    {
        [SerializeField] float regenValue = 0.25f;

        protected override void Collect(RGSKEntity entity)
        {
            var v = entity.gameObject.GetComponent<VehicleController>();
            if (v != null)
            {
                v.nitrous.capacity += regenValue;
            }
        }
    }
}