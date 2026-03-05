using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class GhostMaterialController : MonoBehaviour
    {
        Material _material => RGSKCore.Instance.VehicleSettings.ghostMaterial;
        string _colorProp => RGSKCore.Instance.VehicleSettings.ghostMaterialColorProperty;
        string _textureProp => RGSKCore.Instance.VehicleSettings.ghostMaterialTextureProperty;
        Dictionary<Renderer, Material[]> _defaultMaterials = new Dictionary<Renderer, Material[]>();
        Dictionary<Renderer, Material[]> _ghostMaterials = new Dictionary<Renderer, Material[]>();
        MeshColorizer[] _meshColorizers;

        void Initialize()
        {
            if (_material == null)
                return;

            var renderers = GetComponentsInChildren<Renderer>().Where(x =>
            !x.GetComponent<ParticleSystem>() && !x.GetComponent<TMPro.TMP_Text>()).ToList();

            if (renderers.Count == 0)
                return;

            foreach (var r in renderers)
            {
                if (!_defaultMaterials.ContainsKey(r))
                {
                    _defaultMaterials.Add(r, r.materials);
                }

                var materials = r.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    var m = new Material(_material);

                    if (materials[i].HasProperty(_colorProp))
                    {
                        m.SetColor(_colorProp, materials[i].GetColor(_colorProp));
                    }

                    if (materials[i].HasProperty(_textureProp))
                    {
                        m.SetTexture(_textureProp, materials[i].GetTexture(_textureProp));
                    }

                    materials[i] = m;
                }

                if (!_ghostMaterials.ContainsKey(r))
                {
                    _ghostMaterials.Add(r, materials);
                }
            }
        }

        public void ToggleGhostMaterials(bool toggle)
        {
            Initialize();

            foreach (var r in !toggle ? _defaultMaterials : _ghostMaterials)
            {
                r.Key.materials = r.Value;
            }
        }
    }
}