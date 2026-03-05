using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RouteCamera))]
    public class RouteCameraEditor : SceneViewRaycaster
    {
        RouteCamera _target;
        SerializedProperty routeDistance;
        Route route;

        void OnEnable()
        {
            _target = (RouteCamera)target;

            routeDistance = serializedObject.FindProperty(nameof(routeDistance));

            var root = _target.GetComponentInParent<CameraRoute>();
            if (root != null)
            {
                route = root.route;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use Shift + Left Mouse Button to set the camera's route distance", MessageType.Info);
            EditorGUILayout.PropertyField(routeDistance);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnSceneGUI()
        {
            if (route == null)
                return;

            base.OnSceneGUI();

            if (route.Distance > 0)
            {
                var dist = _target.routeDistance;
                var pos = route.GetPositionAtPercentage(dist / route.Distance);
                Handles.DrawLine(_target.transform.position, pos);
                EditorHelper.DrawLabelWithinDistance(pos, $"{dist.ToString("0.0")}m");
            }
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            Undo.RecordObject(_target, "set_camera_route_distance");
            _target.routeDistance = route.GetDistanceAtPosition(position);
        }
    }
}