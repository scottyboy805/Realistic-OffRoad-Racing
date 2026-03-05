using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class GizmoDrawer : MonoBehaviour
    {
        public enum GizmoShape
        {
            Cube,
            Sphere
        }

        public bool showGizmos = true;
        public GizmoShape gizmoShape;
        public Color gizmoColor = new Color(0, 1, 0, 0.5f);
        public bool visualizeDirection;
        public float directionLineLength = 1.5f;

        void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (gizmoShape == GizmoShape.Cube)
            {
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
            else
            {
                Gizmos.DrawSphere(Vector3.zero, 1);
            }

            if (visualizeDirection)
            {
                DrawArrow.ForGizmo(Vector3.zero, Vector3.forward * directionLineLength, 0.5f, 30);
            }
        }
    }
}