using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RecordableGhostObject : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;

            var clone = gameObject;
            var vehicle = GetComponent<IVehicle>();

            if (vehicle != null)
            {
                clone = vehicle?.VehicleDefinition?.prefab;

                if (clone == null)
                {
                    Logger.LogWarning(this, "Failed to create ghost! Please assign a prefab to the vehicle's VehicleDefinition asset.");
                    yield break;
                }
            }

            RecorderManager.Instance?.GhostRecorder?.CreateGhost(gameObject, clone);
        }
    }
}