using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(AIRoute))]
    public class AIRouteEditor : RouteEditor
    {
        AIRoute _aiRoute;

        SerializedProperty speedZones;
        SerializedProperty speedLimit;
        SerializedProperty priority;
        SerializedProperty branchProbability;
        SerializedProperty lookAheadCurve;
        SerializedProperty nodeReachedDistance;

        SerializedProperty cornerSampleDistance;
        SerializedProperty minCornerAngle;
        SerializedProperty minCornerDistance;
        SerializedProperty minStraightDistance;
        SerializedProperty cornerSpeedRange;
        SerializedProperty cornerBrakeTimeRange;

        SerializedProperty showRacingLineGizmos;
        SerializedProperty showSpeedZoneGizmos;
        
        SceneViewGUIMode sceneViewGUIMode;
        int selectedSpeedZone = -1;

        enum SceneViewGUIMode
        {
            None,
            SpeedZones,
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _aiRoute = (AIRoute)_target;

            speedZones = serializedObject.FindProperty(nameof(speedZones));
            speedLimit = serializedObject.FindProperty(nameof(speedLimit));
            priority = serializedObject.FindProperty(nameof(priority));
            branchProbability = serializedObject.FindProperty(nameof(branchProbability));

            lookAheadCurve = serializedObject.FindProperty(nameof(lookAheadCurve));
            nodeReachedDistance = serializedObject.FindProperty(nameof(nodeReachedDistance));

            cornerSampleDistance = serializedObject.FindProperty(nameof(cornerSampleDistance));
            minCornerAngle = serializedObject.FindProperty(nameof(minCornerAngle));
            minCornerDistance = serializedObject.FindProperty(nameof(minCornerDistance));
            minStraightDistance = serializedObject.FindProperty(nameof(minStraightDistance));
            cornerSpeedRange = serializedObject.FindProperty(nameof(cornerSpeedRange));
            cornerBrakeTimeRange = serializedObject.FindProperty(nameof(cornerBrakeTimeRange));
            
            showRacingLineGizmos = serializedObject.FindProperty(nameof(showRacingLineGizmos));
            showSpeedZoneGizmos = serializedObject.FindProperty(nameof(showSpeedZoneGizmos));

            _aiRoute.SortSpeedzones();
            ResetSceneViewGUIMode();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (_aiRoute.showRacingLineGizmos)
            {
                DrawRacingLineHandles();
            }

            if (_aiRoute.showSpeedZoneGizmos)
            {
                DrawSpeedZoneHandles();
            }

            switch (sceneViewGUIMode)
            {
                case SceneViewGUIMode.SpeedZones:
                    {
                        DrawSpeedZoneUI();
                        break;
                    }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(priority);
                EditorGUILayout.PropertyField(speedLimit);
                EditorGUILayout.PropertyField(branchProbability);

                EditorGUILayout.PropertyField(showSpeedZoneGizmos, new GUIContent("Show Speed Zones"));
                EditorGUILayout.PropertyField(showRacingLineGizmos, new GUIContent("Show Racing Line")); 

                EditorHelper.DrawLine();
                GUILayout.Label("Navigation", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(lookAheadCurve);
                EditorGUILayout.PropertyField(nodeReachedDistance);

                EditorHelper.DrawLine();
                GUILayout.Label("Speed Zones", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("To insert a speed zone, select the \"Speed Zone\" tool in the scene view and use CTRL + Left Mouse Button on the route.", MessageType.Info);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(speedZones);
                EditorGUI.indentLevel--;
                
                EditorGUILayout.PropertyField(cornerSampleDistance);
                EditorGUILayout.PropertyField(minCornerAngle);
                EditorGUILayout.PropertyField(minCornerDistance);
                EditorGUILayout.PropertyField(minStraightDistance);
                EditorGUILayout.PropertyField(cornerSpeedRange);
                EditorGUILayout.PropertyField(cornerBrakeTimeRange);

                var hasSpeedZones = _aiRoute.speedZones.Count > 0;

                if (GUILayout.Button("Generate Speed Zones", EditorStyles.miniButton))
                {
                    if (hasSpeedZones && !EditorHelper.DisplayDialog("", $"This will recreate all speed zones on the route. Use the \"Update Speed Zones\" button below to update the speed zones instead.",
                       "Continue", "Cancel"))
                    {
                        return;
                    }

                    ResetSceneViewGUIMode();
                    Undo.RecordObject(_aiRoute, "detected_speedzones");
                    _aiRoute.CreateSpeedZones();
                }

                using (new EditorGUI.DisabledGroupScope(!hasSpeedZones))
                {
                    if (GUILayout.Button("Update Speed Zones", EditorStyles.miniButton))
                    {
                        Undo.RecordObject(_aiRoute, "updated_speedzones");
                        _aiRoute.UpdateSpeedZones();
                    }

                    if (GUILayout.Button("Delete Speed Zones", EditorStyles.miniButton))
                    {
                        ResetSceneViewGUIMode();
                        Undo.RecordObject(_aiRoute, "deleted_speedzones");
                        _aiRoute.speedZones.Clear();
                    }
                }

                EditorHelper.DrawLine();
                GUILayout.Label("Racing Line", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("To edit the racing line, select the \"Racing Line\" tool in the scene view and use CTRL + Left Mouse Button next to a node. Alternatively, position the mouse next to a node and hold ALT.", MessageType.Info);

                if (GUILayout.Button("Reset Racing Line", EditorStyles.miniButton))
                {
                    foreach (var node in _aiRoute.nodes)
                    {
                        Undo.RecordObject(node, "reset_racingline");
                    }

                    _aiRoute.ResetRacingLine();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override RouteNode CreateNode(GameObject node = null)
        {
            var n = node != null ? node.GetOrAddComponent<AINode>() : new GameObject("AINode").AddComponent<AINode>();
            n.route = _target;
            return n;
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            base.OnHit(position, normal);
            _aiRoute.UpdateNodeRotation();
        }

        protected override void OnRouteClicked(float normalizedDistance, RaycastHit hit)
        {
            switch (_aiRoute.toolMode)
            {
                case AIRouteToolMode.None:
                    {
                        base.OnRouteClicked(normalizedDistance, hit);
                        _aiRoute.UpdateNodeRotation();
                        break;
                    }

                case AIRouteToolMode.SpeedZones:
                    {
                        normalizedDistance *= _aiRoute.Distance;

                        var speedzone = new AISpeedZone
                        {
                            startDistance = normalizedDistance,
                            endDistance = normalizedDistance + 50,
                            speedLimit = -1
                        };

                        Undo.RecordObject(_aiRoute, "inserted_speedzone");
                        _aiRoute.speedZones.Add(speedzone);
                        _aiRoute.SortSpeedzones();
                        break;
                    }

                case AIRouteToolMode.RacingLine:
                    {
                        var closest = (AINode)_aiRoute.GetClosestNode(hit.point);
                        var hitVector = hit.point - closest.transform.position;

                        Undo.RecordObject(closest, "inserted_racingline");
                        closest.RacingLineOffset = Vector3.Dot(hitVector, closest.transform.right);
                        break;
                    }
            }
        }

        protected override void AddChildren()
        {
            base.AddChildren();

            ResetSceneViewGUIMode();
            _aiRoute.speedZones.Clear();
        }

        protected override void DeleteNodes()
        {
            base.DeleteNodes();

            ResetSceneViewGUIMode();
            _aiRoute.speedZones.Clear();
        }

        protected override void DrawNodeHandles()
        {
            Handles.matrix = _target.transform.localToWorldMatrix;

            for (int i = 0; i < _target.nodes.Count; i++)
            {
                var node = (AINode)_target.nodes[i];
                var pos = node.transform.position;
                var percent = $"\n{UIHelper.FormatDistanceText(node.normalizedDistance * _target.Distance)}({(node.normalizedDistance * 100).ToString("F1")}%)"; var branches = "";

                if (node.branchRoutes.Count > 0)
                {
                    branches = $"\n[Branches: {node.branchRoutes.Count}]";
                }

                DrawNodeSelectionButton(node, pos);

                if (_target.showNodeInfo)
                {
                    EditorHelper.DrawLabelWithinDistance(pos + Vector3.up * 3, $"{i.ToString()}{percent}{branches}", CustomEditorStyles.NodeLabel);
                }
            }
        }

        void DrawSpeedZoneHandles()
        {
            if (_aiRoute.speedZones.Count == 0)
                return;

            Handles.matrix = _target.transform.localToWorldMatrix;
            Handles.color = RGSKEditorSettings.Instance.aiSpeedzoneColor;

            for (int i = 0; i < _aiRoute.speedZones.Count; i++)
            {
                var startDistance = _aiRoute.speedZones[i].startDistance;
                var endDistance = _aiRoute.speedZones[i].endDistance;
                var factor = _aiRoute.speedZones[i].speedLimit;
                var brake = _aiRoute.speedZones[i].brakeTime;
                var boost = _aiRoute.speedZones[i].boostProbability;

                if (_aiRoute.Distance < startDistance)
                {
                    continue;
                }

                var next = (endDistance - startDistance) / 20;
                for (float j = startDistance; j < endDistance; j += next)
                {
                    Vector3 start = _target.GetPositionAtDistance(j);
                    Vector3 end = _target.GetPositionAtDistance(j + next);

                    start.y += 2;
                    end.y += 2;

                    Handles.DrawLine(start, end);
                }

                var speed = factor > -1 ? UIHelper.FormatSpeedText(factor, true) : $"Unlimited";
                var startPos = _target.GetPositionAtDistance(startDistance);
                var endPos = _target.GetPositionAtDistance(endDistance);

                startPos.y += 2;
                endPos.y += 2;

                if (i == selectedSpeedZone)
                {
                    EditorGUI.BeginChangeCheck();
                    var newStartPos = Handles.FreeMoveHandle(startPos, 0.8f, Vector3.zero, Handles.SphereHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_aiRoute, "moved_speedzone_handle");
                        _aiRoute.speedZones[i].startDistance = _aiRoute.GetDistanceAtPosition(newStartPos);
                    }

                    EditorGUI.BeginChangeCheck();
                    var newEndPos = Handles.FreeMoveHandle(endPos, 0.8f, Vector3.zero, Handles.SphereHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_aiRoute, "moved_speedzone_handle");
                        _aiRoute.speedZones[i].endDistance = _aiRoute.GetDistanceAtPosition(newEndPos);
                    }
                }
                else
                {
                    if (Handles.Button(startPos, Quaternion.identity, 1, 1, Handles.SphereHandleCap))
                    {
                        selectedSpeedZone = i;
                        sceneViewGUIMode = SceneViewGUIMode.SpeedZones;
                    }
                }

                EditorHelper.DrawLabelWithinDistance(startPos,
                        $"Speed Zone [{i}]\nMax Speed: {speed}\nBrake Time: {brake.ToString("F2")}s\nBoost Probability: {boost.ToString("F2")}",
                        CustomEditorStyles.NodeLabel);
            }
        }

        void DrawSpeedZoneUI()
        {
            if (selectedSpeedZone >= _aiRoute.speedZones.Count)
            {
                ResetSceneViewGUIMode();
                return;
            }

            Handles.BeginGUI();
            {
                var sceneView = SceneView.lastActiveSceneView;
                
                if(sceneView == null)
                {
                    Handles.EndGUI();
                    return;
                }

                var panelWidth = 300f;
                var panelHeight = 200f;
                var padding = 10f;
                var x = sceneView.position.width - panelWidth - padding;
                var y = sceneView.position.height - panelHeight - padding;

                GUILayout.BeginArea(new Rect(x, y, panelWidth, panelHeight));

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Speed Zone [{selectedSpeedZone}]", EditorStyles.boldLabel);

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                        {
                            ResetSceneViewGUIMode();
                            return;
                        }
                    }

                    GUILayout.Space(5);
                    var prop = speedZones.GetArrayElementAtIndex(selectedSpeedZone);
                    var startDistance = prop.FindPropertyRelative("startDistance");
                    var endDistance = prop.FindPropertyRelative("endDistance");
                    var speedLimit = prop.FindPropertyRelative("speedLimit");
                    var brakeTime = prop.FindPropertyRelative("brakeTime");
                    var boostProbability = prop.FindPropertyRelative("boostProbability");

                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(startDistance);
                        EditorGUILayout.PropertyField(endDistance);
                        EditorGUILayout.PropertyField(speedLimit);
                        EditorGUILayout.PropertyField(brakeTime);
                        EditorGUILayout.PropertyField(boostProbability);

                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            _aiRoute.SortSpeedzones();
                        }
                    }

                    if (GUILayout.Button("Delete"))
                    {
                        Undo.RecordObject(_aiRoute, "deleted_speedzone");
                        _aiRoute.speedZones.RemoveAt(selectedSpeedZone);
                        ResetSceneViewGUIMode();
                    }
                }

                GUILayout.EndArea();
                Handles.EndGUI();
            }
        }

        void DrawRacingLineHandles()
        {
            Handles.matrix = _target.transform.localToWorldMatrix;
            Handles.color = RGSKEditorSettings.Instance.racingLineColor;

            if (_aiRoute.toolMode == AIRouteToolMode.RacingLine)
            {
                var e = Event.current;

                if (e.alt)
                {
                    var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                    if (Physics.Raycast(ray, out var hit, 10000, placeableLayers, triggerInteraction))
                    {
                        var closest = (AINode)_aiRoute.GetClosestNode(hit.point);
                        var hitVector = hit.point - closest.transform.position;

                        closest.RacingLineOffset = Vector3.Dot(hitVector, closest.transform.right);
                        EditorUtility.SetDirty(_aiRoute);
                    }
                }

                for (int i = 0; i < _target.nodes.Count; i++)
                {
                    var node = (AINode)_target.nodes[i];
                    var pos = node.transform.position + (node.transform.right * node.RacingLineOffset);

                    EditorGUI.BeginChangeCheck();
                    var newPos = Handles.FreeMoveHandle(pos, 0.75f, Vector3.zero, Handles.SphereHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        var point = newPos - node.transform.position;
                        node.RacingLineOffset = Vector3.Dot(point, node.transform.right);
                        EditorUtility.SetDirty(_aiRoute);
                    }
                }
            }
        }

        void ResetSceneViewGUIMode()
        {
            sceneViewGUIMode = SceneViewGUIMode.None;
            selectedSpeedZone = -1;
        }
    }
}