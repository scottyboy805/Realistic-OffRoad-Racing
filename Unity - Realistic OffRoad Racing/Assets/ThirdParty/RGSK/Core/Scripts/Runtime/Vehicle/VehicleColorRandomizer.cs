using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class VehicleColorRandomizer : MonoBehaviour
    {
        [SerializeField] int colorIndex = 0;

        [ContextMenu("Randomize")]
        void Start() => GeneralHelper.SetColor(gameObject, GeneralHelper.GetRandomVehicleColor(), colorIndex);
    }
}