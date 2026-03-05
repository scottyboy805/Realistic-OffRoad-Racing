using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class MenuVehicleSpawner : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] float rotateSpeed = 0;
        [SerializeField] Material lockedMaterial;
        [SerializeField] bool spawnAsKinematic;

        GameObject _instancedVehicle;
        VehicleDefinition _definitionVehicle;

        void Update()
        {
            if (_instancedVehicle != null)
            {
                _instancedVehicle.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
        }

        public void Spawn(VehicleDefinition vehicle)
        {            
            DestroyActiveVehicle();
            
            StopAllCoroutines();
            StartCoroutine(InstantiateRoutine(vehicle));
        }

        public void UpdateColor(Color c)
        {
            if (_instancedVehicle == null || c == null)
                return;

            GeneralHelper.SetColor(_instancedVehicle, c);
            RGSKCore.runtimeData.SelectedVehicleColor = c;
            RGSKCore.runtimeData.SelectedVehicleLivery = null;
        }

        public void UpdateLivery(Texture2D l)
        {
            if (_instancedVehicle == null)
                return;

            GeneralHelper.SetLivery(_instancedVehicle, l);
            RGSKCore.runtimeData.SelectedVehicleLivery = l;
        }

        public void DestroyActiveVehicle()
        {
            if (_instancedVehicle != null)
            {
                Destroy(_instancedVehicle);
            }
        }

        IEnumerator InstantiateRoutine(VehicleDefinition vehicle)
        {
            if (vehicle.prefab == null || spawnPoint == null)
                yield break;

#if UNITY_6000_2_OR_NEWER
            yield return null;
#endif

            _instancedVehicle = Instantiate(vehicle.prefab, spawnPoint.position * 100, Quaternion.identity, GeneralHelper.GetDynamicParent());
            _definitionVehicle = vehicle;

            if (!vehicle.IsUnlocked() && lockedMaterial != null)
            {
                var renderers = _instancedVehicle.GetComponentsInChildren<Renderer>();

                foreach (var r in renderers)
                {
                    var mats = new Material[r.materials.Length];

                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = lockedMaterial;
                    }

                    r.materials = mats;
                }
            }
            else
            {
                if (GeneralHelper.CanApplyColor(_instancedVehicle))
                {
                    GeneralHelper.SetColor(_instancedVehicle, vehicle.defaultStats.color);
                }
                else if (GeneralHelper.CanApplyLivery(_instancedVehicle))
                {
                    GeneralHelper.SetLivery(_instancedVehicle, vehicle.defaultStats.color);
                }
            }

            yield return null;

            if (_instancedVehicle.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.ResetVelocity();
                rb.isKinematic = spawnAsKinematic;
            }

            if (_instancedVehicle.TryGetComponent<IVehicle>(out var v))
            {
                v.StopEngine();
            }

            if (_instancedVehicle.TryGetComponent<VehicleDriverPlacer>(out var d))
            {
                d.ToggleDriverVisibility(false);
            }

            yield return null;

            _instancedVehicle.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }

        public GameObject VehicleInstance() => _instancedVehicle;
        public VehicleDefinition VehicleDefinition() => _definitionVehicle;
    }
}