using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
using RGSK.Extensions;

namespace RGSK.Editor
{
    [CustomEditor(typeof(Route))]
    public class RouteEditor : SceneViewRaycaster
    {
        protected Route _target;

        SerializedProperty nodes;
        SerializedProperty loop;
        SerializedProperty defaultNodeWidth;
        SerializedProperty gizmoColor;
        SerializedProperty showRoute;
        SerializedProperty showNodeInfo;
        SerializedProperty routeNodeData;

        protected virtual void OnEnable()
        {
            _target = (Route)target;

            nodes = serializedObject.FindProperty(nameof(nodes));
            loop = serializedObject.FindProperty(nameof(loop));
            defaultNodeWidth = serializedObject.FindProperty(nameof(defaultNodeWidth));
            gizmoColor = serializedObject.FindProperty(nameof(gizmoColor));
            showRoute = serializedObject.FindProperty(nameof(showRoute));
            showNodeInfo = serializedObject.FindProperty(nameof(showNodeInfo));
            routeNodeData = serializedObject.FindProperty(nameof(routeNodeData));
        }

        public override void OnSceneGUI()
        {
            Handles.color = _target.gizmoColor;

            if (_target.showRoute)
            {
                DrawNodeHandles();
            }

            InsertNodes();

            base.OnSceneGUI();
        }

        public override void OnInspectorGUI()
        {
            if (_target.nodes.Count < 2)
            {
                EditorGUILayout.HelpBox("Please assign 2 or more nodes to this route!", MessageType.Error);
            }

            EditorGUILayout.HelpBox("Use Shift + Left Mouse Button to place a node\n\nUse CTRL + Left Mouse Button to instert nodes inbetween other nodes\n\nUse Shift + Left Mouse Button over a node to delete it", MessageType.Info);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Nodes", EditorStyles.boldLabel);

                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(nodes);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(loop);
                EditorGUILayout.PropertyField(defaultNodeWidth);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(routeNodeData);

                    if (GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        SaveRouteData();
                    }

                    if (_target.routeNodeData != null)
                    {
                        if (GUILayout.Button("Load", EditorStyles.miniButton, GUILayout.Width(100)))
                        {
                            LoadRouteData();
                        }
                    }
                }

                if (GUILayout.Button("Add Children"))
                {
                    if (EditorHelper.DisplayDialog("", $"Would you like to add all child transforms as nodes?",
                        "Yes", "No"))
                    {
                        AddChildren();
                    }
                }

                if (GUILayout.Button("Update Node Rotation"))
                {
                    _target.UpdateNodeRotation();
                    EditorUtility.SetDirty(_target);
                }

                if (GUILayout.Button("Update Node Width"))
                {
                    if (EditorHelper.DisplayDialog("", $"This will set the width of every node to the \" Default Node Width\" value.",
                       "Continue", "Cancel"))
                    {
                        _target.UpdateNodeWidth();
                        EditorUtility.SetDirty(_target);
                    }
                }

                if (GUILayout.Button("Reverse"))
                {
                    if (EditorHelper.DisplayDialog("", $"Would you like to reverse the route?",
                        "Yes", "No"))
                    {
                        _target.Reverse();
                        EditorUtility.SetDirty(_target);
                    }
                }

                if (GUILayout.Button("Delete All"))
                {
                    DeleteNodes();
                }

                EditorGUILayout.LabelField($"Distance: {_target.Distance}m");
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Gizmos", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(showRoute);

                using (new EditorGUI.DisabledGroupScope(!_target.showRoute))
                {
                    EditorGUILayout.PropertyField(showNodeInfo);
                }

                EditorGUILayout.PropertyField(gizmoColor);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void InsertNodes()
        {
            if (_target.nodes.Count < 2)
                return;

            var e = Event.current;

            if (e.control)
            {
                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000, placeableLayers, triggerInteraction))
                {
                    var node = _target.GetClosestNode(hit.point);
                    var normalizedDistance = _target.GetPercentageAtPosition(hit.point);

                    Handles.DrawLine(_target.GetPositionAtPercentage(normalizedDistance), hit.point);

                    if (e.type == EventType.MouseDown)
                    {
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        OnRouteClicked(normalizedDistance, hit);
                        e.Use();
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                }
            }
        }

        protected virtual void OnRouteClicked(float normalizedDistance, RaycastHit hit)
        {
            if (normalizedDistance > _target.nodes.LastOrDefault().normalizedDistance)
            {
                Logger.LogWarning("Cannot insert here! Please create a node instead using Shift + Left Click.");
            }

            for (int i = 0; i < _target.nodes.Count - 1; i++)
            {
                if (normalizedDistance > _target.nodes[i].normalizedDistance &&
                    normalizedDistance < _target.nodes[i + 1].normalizedDistance)
                {
                    OnHit(hit.point, hit.normal);
                    var last = _target.nodes.LastOrDefault();
                    _target.nodes.Remove(last);
                    _target.nodes.Insert(i + 1, last);
                    last.transform.SetSiblingIndex(i + 1);
                    RenameNodes();
                    break;
                }
            }
        }

        protected virtual void DrawNodeHandles()
        {
            Handles.matrix = _target.transform.localToWorldMatrix;

            for (int i = 0; i < _target.nodes.Count; i++)
            {
                var node = _target.nodes[i];
                var pos = node.transform.position;
                DrawNodeSelectionButton(node, pos);
            }
        }

        protected void DrawNodeSelectionButton(RouteNode node, Vector3 pos)
        {
            var e = Event.current;

            if (Handles.Button(pos, Quaternion.identity, 1, 1, Handles.SphereHandleCap))
            {
                if (e.shift) 
                {
                    Undo.RecordObject(_target, "deleted_node");
                    _target.nodes.Remove(node);
                    Undo.DestroyObjectImmediate(node.gameObject);
                    e.Use();
                }
                else
                {
                    Selection.activeObject = node.gameObject;
                }
            }
        }

        protected virtual void AddChildren()
        {
            if (_target.transform.childCount == 0)
                return;

            Undo.RecordObject(_target, "added_child_nodes");

            _target.nodes.Clear();

            foreach (Transform child in _target.transform)
            {
                var node = CreateNode(child.gameObject);
                node.combinedWidth = _target.defaultNodeWidth;
                node.transform.SetPositionAndRotation(child.position, child.rotation);
                _target.nodes.Add(node);
            }
        }

        protected virtual void DeleteNodes()
        {
            if (!EditorHelper.DisplayDialog("", $"Delete all nodes?",
                        "Yes", "No"))
            {
                return;
            }

            Undo.RecordObject(_target, "delete_all_nodes");
            _target.nodes.Clear();

            foreach (var node in _target.GetComponentsInChildren<RouteNode>())
            {
                Undo.DestroyObjectImmediate(node.gameObject);
            }
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            var node = CreateNode();
            var yoffset = node.transform.localScale.y / 2;
            node.transform.SetPositionAndRotation(position + Vector3.up * yoffset, Quaternion.FromToRotation(Vector3.up, normal));
            node.transform.SetParent(_target.transform, true);

            node.combinedWidth = _target.defaultNodeWidth;

            Undo.RegisterCreatedObjectUndo(node.gameObject, "created_node");
            Undo.RecordObject(_target, "added_node");
            _target.nodes.Add(node);
            RenameNodes();
        }

        public virtual RouteNode CreateNode(GameObject node = null)
        {
            var n = node != null ? node.GetOrAddComponent<RouteNode>() : new GameObject("Node").AddComponent<RouteNode>();
            n.route = _target;
            return n;
        }

        public void RenameNodes() => EditorHelper.RenameChildren(_target.gameObject);

        void SaveRouteData()
        {
            if (_target.nodes.Count < 2)
            {
                Debug.LogWarning("Add 2 or more nodes to save!");
                return;
            }

            var sceneName = SceneManager.GetActiveScene().name;
            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"RouteData_{sceneName}", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var data = ScriptableObject.CreateInstance<RouteNodeData>();

                foreach (var node in _target.nodes)
                {
                    data.nodeData.Add(new RouteNodeData.NodeData
                    {
                        position = node.transform.position,
                        rotation = node.transform.rotation,
                        leftWidth = node.leftWidth,
                        rightWidth = node.rightWidth
                    });
                }

                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();

                _target.routeNodeData = data;
            }
        }

        void LoadRouteData()
        {
            if (_target.routeNodeData == null ||
                !EditorHelper.DisplayDialog("", $"Load route data?",
                        "Yes", "No"))
            {
                return;
            }

            _target.nodes.Clear();
            foreach (var node in _target.GetComponentsInChildren<RouteNode>())
            {
                DestroyImmediate(node.gameObject);
            }

            foreach (var node in _target.routeNodeData.nodeData)
            {
                var n = CreateNode();
                n.transform.SetParent(_target.transform, false);
                n.transform.position = node.position;
                n.transform.rotation = node.rotation;
                n.leftWidth = node.leftWidth;
                n.rightWidth = node.rightWidth;
                _target.nodes.Add(n);
            }

            RenameNodes();
        }
    }
}