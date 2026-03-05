using UnityEngine;

namespace RGSK
{
    public class RouteNode : MonoBehaviour
    {
        public Route route { get; set; }
        public RouteNode nextNode { get; set; }
        public RouteNode previousNode { get; set; }
        public int index { get; set; }
        public float distance { get; set; }
        public float normalizedDistance { get; set; }

        public float leftWidth = 10;
        public float rightWidth = 10;
        public float combinedWidth = 20;
        public bool combineWidth = true;

        void OnDrawGizmos()
        {
            if (route != null && route.showRoute)
            {
                if (combineWidth)
                {
                    leftWidth = rightWidth = combinedWidth / 2;
                }

                Gizmos.color = route.gizmoColor;
                DrawGizmos();
            }
        }

        protected virtual void DrawGizmos()
        {
            var pos = transform.position;
            Gizmos.DrawSphere(pos, 0.5f);
            Gizmos.DrawLine(pos, pos + -transform.right * leftWidth);
            Gizmos.DrawLine(pos, pos + transform.right * rightWidth);
        }
    }
}