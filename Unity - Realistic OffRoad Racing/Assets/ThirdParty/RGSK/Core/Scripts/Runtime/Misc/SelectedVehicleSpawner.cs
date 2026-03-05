using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    public class SelectedVehicleSpawner : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] string minimapBlip = "player";

        void Start()
        {
            var vehicle = RGSKCore.runtimeData.SelectedVehicle;

            if(vehicle == null)
            {
                Logger.LogWarning("A selected vehicle has not been assigned!");
                return;
            }

            if(vehicle.prefab != null && spawnPoint != null)
            {
                var instance = Instantiate(vehicle.prefab, spawnPoint.position, spawnPoint.rotation, GeneralHelper.GetDynamicParent());
                var entity = instance.GetOrAddComponent<RGSKEntity>();

                if (instance.TryGetComponent<IVehicle>(out var v))
                {
                    v.Initialize();
                    v.StartEngine(0);
                }

                entity.Initialize(true, false);

                if (GeneralHelper.CanApplyColor(instance))
                {
                    GeneralHelper.SetColor(instance, RGSKCore.runtimeData.SelectedVehicleColor);
                }
                else if (GeneralHelper.CanApplyLivery(instance))
                {
                    var livery = RGSKCore.runtimeData.SelectedVehicleLivery;
                    
                    if(livery != null)
                    {
                        GeneralHelper.SetLivery(instance, livery);
                    }
                }

                MinimapManager.Instance?.CreateBlip(minimapBlip, instance.transform);
            }
        }
    }
}