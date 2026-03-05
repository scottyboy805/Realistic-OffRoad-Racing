using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoundingBox : MonoBehaviour
    {
        public BoxCollider Box
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<BoxCollider>();
                }

                return _collider;
            }
        }

        public Vector3 Size => Box.size;
        public Vector3 Center => Box.center;

        BoxCollider _collider;

        [ContextMenu("Auto Calculate Bounds")]
        public void AutoCalculateBounds()
        {
            var parent = transform.parent;

            if(parent == null)
                return;

            var oldRotation = parent.rotation;
            parent.rotation = Quaternion.identity;

            var bounds = new Bounds(parent.transform.position, Vector3.zero);
            var renderers = parent.transform.GetComponentsInChildren<MeshRenderer>();

            foreach (var r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }

            Box.size = bounds.size;
            Box.center = bounds.center - parent.transform.position;

            parent.rotation = oldRotation;
        }
    }
}