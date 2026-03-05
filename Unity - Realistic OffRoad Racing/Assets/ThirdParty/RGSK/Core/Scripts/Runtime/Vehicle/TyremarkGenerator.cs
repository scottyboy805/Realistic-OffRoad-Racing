using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class TyremarkGenerator : MonoBehaviour
    {
        class MarkSection
        {
            public Vector3 Pos = Vector3.zero;
            public Vector3 Normal = Vector3.zero;
            public Vector4 Tangent = Vector4.zero;
            public Vector3 Posl = Vector3.zero;
            public Vector3 Posr = Vector3.zero;
            public byte Intensity;
            public int LastIndex;
        }

        [SerializeField] int maxMarks = 1024;
        [SerializeField] float groundOffset = 0.02f;
        [SerializeField] float minDistance = 0.1f;
        [SerializeField] Material material;

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;
        Mesh _markMesh;
        MarkSection[] _marks;
        Vector3[] _vertices;
        Vector3[] _normal;
        Vector4[] _tangents;
        Color32[] _colors;
        Vector2[] _uvs;
        int[] _tris;
        int _markIndex;
        bool _updated;
        bool _setBounds;

        void Start()
        {
            _marks = new MarkSection[maxMarks];

            for (int i = 0; i < maxMarks; i++)
            {
                _marks[i] = new MarkSection();
            }

            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            _markMesh = new Mesh();
            _markMesh.MarkDynamic();

            if (_meshFilter == null)
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            _meshFilter.sharedMesh = _markMesh;

            _vertices = new Vector3[maxMarks * 4];
            _normal = new Vector3[maxMarks * 4];
            _tangents = new Vector4[maxMarks * 4];
            _colors = new Color32[maxMarks * 4];
            _uvs = new Vector2[maxMarks * 4];
            _tris = new int[maxMarks * 6];

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.material = material;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            transform.position = Vector3.zero;
        }

        void LateUpdate()
        {
            if (!_updated)
                return;

            _updated = false;

            _markMesh.vertices = _vertices;
            _markMesh.normals = _normal;
            _markMesh.tangents = _tangents;
            _markMesh.triangles = _tris;
            _markMesh.colors32 = _colors;
            _markMesh.uv = _uvs;

            if (!_setBounds)
            {
                _markMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
                _setBounds = true;
            }

            _meshFilter.sharedMesh = _markMesh;
        }

        public int Add(Vector3 pos, Vector3 normal, float intensity, float width, int lastIndex)
        {
            if (intensity > 1)
            {
                intensity = 1.0f;
            }
            else if (intensity < 0) return -1; if (lastIndex > 0)
            {
                float sqrDistance = (pos - _marks[lastIndex].Pos).sqrMagnitude;

                if (sqrDistance < (minDistance * minDistance))
                    return lastIndex;
            }

            MarkSection curSection = _marks[_markIndex];

            curSection.Pos = pos + normal * groundOffset;
            curSection.Normal = normal;
            curSection.Intensity = (byte)(intensity * 255f);
            curSection.LastIndex = lastIndex;

            if (lastIndex != -1)
            {
                MarkSection lastSection = _marks[lastIndex];
                Vector3 dir = (curSection.Pos - lastSection.Pos);
                Vector3 xDir = Vector3.Cross(dir, normal).normalized;

                curSection.Posl = curSection.Pos + xDir * width * 0.5f;
                curSection.Posr = curSection.Pos - xDir * width * 0.5f;
                curSection.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

                if (lastSection.LastIndex == -1)
                {
                    lastSection.Tangent = curSection.Tangent;
                    lastSection.Posl = curSection.Pos + xDir * width * 0.5f;
                    lastSection.Posr = curSection.Pos - xDir * width * 0.5f;
                }
            }

            UpdateTyremarkMesh();

            int curIndex = _markIndex;

            _markIndex = ++_markIndex % maxMarks;

            return curIndex;
        }

        void UpdateTyremarkMesh()
        {
            if (!_meshRenderer.enabled)
                return;

            MarkSection curr = _marks[_markIndex];

            if (curr.LastIndex == -1)
                return;

            MarkSection last = _marks[curr.LastIndex];
            _vertices[_markIndex * 4 + 0] = last.Posl;
            _vertices[_markIndex * 4 + 1] = last.Posr;
            _vertices[_markIndex * 4 + 2] = curr.Posl;
            _vertices[_markIndex * 4 + 3] = curr.Posr;

            _normal[_markIndex * 4 + 0] = last.Normal;
            _normal[_markIndex * 4 + 1] = last.Normal;
            _normal[_markIndex * 4 + 2] = curr.Normal;
            _normal[_markIndex * 4 + 3] = curr.Normal;

            _tangents[_markIndex * 4 + 0] = last.Tangent;
            _tangents[_markIndex * 4 + 1] = last.Tangent;
            _tangents[_markIndex * 4 + 2] = curr.Tangent;
            _tangents[_markIndex * 4 + 3] = curr.Tangent;

            _colors[_markIndex * 4 + 0] = new Color32(0, 0, 0, last.Intensity);
            _colors[_markIndex * 4 + 1] = new Color32(0, 0, 0, last.Intensity);
            _colors[_markIndex * 4 + 2] = new Color32(0, 0, 0, curr.Intensity);
            _colors[_markIndex * 4 + 3] = new Color32(0, 0, 0, curr.Intensity);

            _uvs[_markIndex * 4 + 0] = new Vector2(0, 0);
            _uvs[_markIndex * 4 + 1] = new Vector2(1, 0);
            _uvs[_markIndex * 4 + 2] = new Vector2(0, 1);
            _uvs[_markIndex * 4 + 3] = new Vector2(1, 1);

            _tris[_markIndex * 6 + 0] = _markIndex * 4 + 0;
            _tris[_markIndex * 6 + 2] = _markIndex * 4 + 1;
            _tris[_markIndex * 6 + 1] = _markIndex * 4 + 2;

            _tris[_markIndex * 6 + 3] = _markIndex * 4 + 2;
            _tris[_markIndex * 6 + 5] = _markIndex * 4 + 1;
            _tris[_markIndex * 6 + 4] = _markIndex * 4 + 3;

            _updated = true;
        }

        public void Clear()
        {
            _vertices = new Vector3[maxMarks * 4];
            _normal = new Vector3[maxMarks * 4];
            _tangents = new Vector4[maxMarks * 4];
            _colors = new Color32[maxMarks * 4];
            _uvs = new Vector2[maxMarks * 4];
            _tris = new int[maxMarks * 6];

            _markMesh.vertices = _vertices;
            _markMesh.normals = _normal;
            _markMesh.tangents = _tangents;
            _markMesh.triangles = _tris;
            _markMesh.colors32 = _colors;
            _markMesh.uv = _uvs;

            _meshFilter.sharedMesh = _markMesh;
        }
    }
}