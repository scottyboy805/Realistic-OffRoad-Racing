using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class VehicleHeadlightTrigger : MonoBehaviour
    {
        [SerializeField] bool on = true;

        void OnTriggerEnter(Collider other)
        {
            var vehicle = other.GetComponentInParent<IVehicle>();
            if (vehicle != null)
            {
                vehicle.HeadlightsOn = on;
            }
        }
    }
}