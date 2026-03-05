using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class MeshDeformation : MonoBehaviour
    {
        [SerializeField] bool deformable = true;
        [SerializeField] float deformationAmount = 1f;
        [SerializeField] float deformationRadius = 0.5f;
        [SerializeField] float maxDeformation = 0.25f;
        [SerializeField] float repairSpeed = 3f;
        [SerializeField] MeshFilter[] deformableMeshes;
        [SerializeField] MeshCollider[] deformableColliders;

        public bool IsDeformable
        {
            get => deformable;
            set => deformable = value;
        }

        Vector3[][] _orginalVerts;
        float _minVelocity = 2f;
        float _delayTimeDeform = 0.1f;
        float _minVertsDistanceToRestore = 0.002f;
        float _nextTimeDeform = 0f;
        bool _isRepairing = false;
        bool _isRepaired = false;
        bool _initialized;

        void Start()
        {
            _orginalVerts = new Vector3[deformableMeshes.Length][];

            for (int i = 0; i < deformableMeshes.Length; i++)
            {
                _orginalVerts[i] = deformableMeshes[i].mesh.vertices;
                deformableMeshes[i].mesh.MarkDynamic();
            }

            _initialized = true;
        }

        void Update()
        {
            RepairMesh();
        }

        public void Repair()
        {
            if(!_initialized)
                return;
                
            _isRepairing = true;
            RepairMesh();
        }

        void OnCollisionEnter(Collision collision)
        {
            if(!_initialized)
                return;

            if (deformable && Time.time > _nextTimeDeform)
            {
                if (collision.relativeVelocity.magnitude > _minVelocity)
                {
                    _isRepaired = false;

                    var contactPoint = collision.contacts[0].point;
                    var contactVelocity = collision.relativeVelocity * 0.02f;

                    for (int i = 0; i < deformableMeshes.Length; i++)
                    {
                        if (deformableMeshes[i] != null)
                        {
                            DeformationMesh(deformableMeshes[i].mesh, deformableMeshes[i].transform, contactPoint, contactVelocity, i);
                        }
                    }

                    _nextTimeDeform = Time.time + _delayTimeDeform;
                }
            }
        }

        void DeformationMesh(Mesh mesh, Transform localTransform, Vector3 contactPoint, Vector3 contactVelocity, int i)
        {
            if(!_initialized)
                return;

            var deformed = false;
            var localContactPoint = localTransform.InverseTransformPoint(contactPoint);
            var localContactForce = localTransform.InverseTransformDirection(contactVelocity);
            var vertices = mesh.vertices;

            for (int j = 0; j < vertices.Length; j++)
            {
                var distance = (localContactPoint - vertices[j]).magnitude;

                if (distance <= deformationRadius)
                {
                    vertices[j] += localContactForce * (deformationRadius - distance) * deformationAmount;
                    var deformation = vertices[j] - _orginalVerts[i][j];

                    if (deformation.magnitude > maxDeformation)
                    {
                        vertices[j] = _orginalVerts[i][j] + deformation.normalized * maxDeformation;
                    }

                    deformed = true;
                }
            }

            if (deformed)
            {
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                if (deformableColliders.Length > 0)
                {
                    if (deformableColliders[i] != null)
                    {
                        deformableColliders[i].sharedMesh = mesh;
                    }
                }
            }
        }

        void RepairMesh()
        {
            if(!_initialized)
                return;

            if (!_isRepaired && _isRepairing)
            {
                _isRepaired = true;

                for (int i = 0; i < deformableMeshes.Length; i++)
                {
                    var mesh = deformableMeshes[i].mesh;
                    var vertices = mesh.vertices;
                    var origVerts = _orginalVerts[i];

                    for (int j = 0; j < vertices.Length; j++)
                    {
                        vertices[j] += (origVerts[j] - vertices[j]) * Time.deltaTime * repairSpeed;

                        if ((origVerts[j] - vertices[j]).magnitude > _minVertsDistanceToRestore)
                        {
                            _isRepaired = false;
                        }
                    }

                    mesh.vertices = vertices;
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    if (deformableColliders.Length > 0)
                    {
                        if (deformableColliders[i] != null)
                        {
                            deformableColliders[i].sharedMesh = mesh;
                        }
                    }
                }

                if (_isRepaired)
                {
                    _isRepairing = false;

                    for (int i = 0; i < deformableMeshes.Length; i++)
                    {
                        if (deformableColliders.Length > 0)
                        {
                            if (deformableColliders[i] != null)
                            {
                                deformableColliders[i].sharedMesh = deformableMeshes[i].mesh;
                            }
                        }
                    }
                }
            }
        }
    }
}