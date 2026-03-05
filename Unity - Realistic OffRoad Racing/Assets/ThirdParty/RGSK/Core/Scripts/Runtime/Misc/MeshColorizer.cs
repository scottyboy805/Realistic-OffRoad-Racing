using UnityEngine;

namespace RGSK
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshColorizer : MonoBehaviour
    {
        [SerializeField] int materialIndex = 0;
        [SerializeField] string property = "_BaseColor";

        [Header("Color Index")]
        public int colorIndex = 0;

        MeshRenderer _mesh;
        MaterialPropertyBlock _block;

        public void SetColor(Color col, int index)
        {
            if (colorIndex != index)
                return;

            _block = new MaterialPropertyBlock();
            _block.SetColor(property, col);

            if (TryGetComponent<MeshRenderer>(out _mesh))
            {
                _mesh.SetPropertyBlock(_block, materialIndex);
            }
        }

        public Color GetColor() => _block?.GetColor(property) ?? Color.black;
    }
}