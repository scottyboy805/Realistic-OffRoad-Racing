using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class ColliderGizmo : MonoBehaviour
    {
        public Color gizmoColor = new Color(0, 1, 0, 0.5f);
        public bool showGizmos = true;

        SphereCollider _sphere;
        BoxCollider _box;

        void OnDrawGizmos()
        {
            if(!showGizmos)
                return;
            
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            _box = GetComponent<BoxCollider>();
            _sphere = GetComponent<SphereCollider>();

            if (_box != null)
            {
                Gizmos.DrawCube(_box.center, _box.size);
            }

            if (_sphere != null)
            {
                Gizmos.DrawSphere(_sphere.center, _sphere.radius);
            }
        }
    }
}