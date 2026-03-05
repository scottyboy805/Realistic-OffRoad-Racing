using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class MeshLivery : MonoBehaviour
    {
        [SerializeField] LiveryList liveries;
        [SerializeField] int materialIndex = 0;
        [SerializeField] string property = "_BaseMap";

        public LiveryList Liveries => liveries;
        public int CurrentLiveryIndex { get; set; }

        MeshRenderer _mesh;
        MaterialPropertyBlock _block;

        public void SetLivery(Texture2D livery)
        {
            if (livery == null)
            {
                Logger.LogWarning(this, "The livery you are trying to assign is null!");
                return;
            }

            _block = new MaterialPropertyBlock();
            _block.SetTexture(property, livery);

            if (TryGetComponent<MeshRenderer>(out _mesh))
            {
                _mesh.SetPropertyBlock(_block, materialIndex);
                CurrentLiveryIndex = liveries.GetIndexOf(livery);
            }
        }
    }
}