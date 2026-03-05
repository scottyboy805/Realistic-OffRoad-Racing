using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class VehicleLight : MonoBehaviour
    {
        public VehicleLightType type;

        [Header("Lights")]
        [SerializeField] Light[] lights;
        [SerializeField] float lightIntensity = 1.5f;

        [Header("Emission")]
        [SerializeField] MeshRenderer[] meshes;
        [SerializeField] int materialIndex = 0;
        [SerializeField] float emissionIntensity = 2;
        [SerializeField] string emissionProperty = "_Intensity";
        [SerializeField] bool toggleMeshVisibility;

        List<MaterialPropertyBlock> _blocks = new List<MaterialPropertyBlock>();
        float _currentValue = -1;

        void Start()
        {
            foreach (var mesh in meshes)
            {
                var block = new MaterialPropertyBlock();
                _blocks.Add(block);
            }
        }

        public void SetIntensity(float value)
        {
            if (_currentValue == value)
                return;

            for (int i = 0; i < lights.Length; i++)
            {
                if(lights[i] == null)
                    continue;
                
                lights[i].enabled = value > 0;
                lights[i].intensity = lightIntensity * value;
            }

            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].SetFloat(emissionProperty, value * emissionIntensity);
                meshes[i].SetPropertyBlock(_blocks[i], materialIndex);
            }

            if (toggleMeshVisibility)
            {
                foreach (var mesh in meshes)
                {
                    if (mesh != null)
                    {
                        mesh.enabled = value > 0;
                    }
                }
            }

            _currentValue = value;
        }
    }
}