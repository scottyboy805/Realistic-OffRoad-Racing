using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceGrid))]
    public class RaceGridEditor : SceneViewRaycaster
    {
        RaceGrid _target;

        SerializedProperty gridType;
        SerializedProperty autoPlacement;
        SerializedProperty gridSize;
        SerializedProperty columnCount;
        SerializedProperty columnSpacing;
        SerializedProperty rowSpacing;
        SerializedProperty zOffset;

        GizmoDrawer[] _gizmoDrawers;

        void OnEnable()
        {
            _target = (RaceGrid)target;

            gridType = serializedObject.FindProperty(nameof(gridType));
            autoPlacement = serializedObject.FindProperty(nameof(autoPlacement));
            gridSize = serializedObject.FindProperty(nameof(gridSize));
            columnCount = serializedObject.FindProperty(nameof(columnCount));
            columnSpacing = serializedObject.FindProperty(nameof(columnSpacing));
            rowSpacing = serializedObject.FindProperty(nameof(rowSpacing));
            zOffset = serializedObject.FindProperty(nameof(zOffset));

            _gizmoDrawers = _target.GetComponentsInChildren<GizmoDrawer>();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use Shift + Left Mouse Button to place a grid position\n\nUse Shift + Left Mouse Button over a grid position to delete it.", MessageType.Info);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(gridType);

                EditorHelper.DrawLine();

                EditorGUILayout.PropertyField(autoPlacement);

                if (_target.autoPlacement && _target.transform.childCount == 0)
                {
                    EditorGUILayout.HelpBox("1 grid position is required to begin auto placement!", MessageType.Warning);
                }

                EditorGUI.BeginDisabledGroup(!_target.autoPlacement || _target.transform.childCount == 0);
                {
                    var oldValue = GetValueSum();

                    EditorGUILayout.PropertyField(gridSize);
                    EditorGUILayout.PropertyField(columnCount);
                    EditorGUILayout.PropertyField(columnSpacing);
                    EditorGUILayout.PropertyField(rowSpacing);
                    EditorGUILayout.PropertyField(zOffset);

                    serializedObject.ApplyModifiedProperties();

                    if (oldValue != GetValueSum() || GUILayout.Button("Update Layout"))
                    {
                        UpdateGridLayout();
                    }

                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        void UpdateGridLayout()
        {
            if (_target.transform.childCount == 0)
                return;

            var first = _target.transform.GetChild(0);

            for (int i = _target.transform.childCount - 1; i > 0; i--)
            {
                Undo.DestroyObjectImmediate(_target.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < _target.gridSize; i++)
            {
                if (i == 0)
                    continue;

                var row = i / _target.columnCount;
                var col = i % _target.columnCount;

                var offset = new Vector3(col * _target.columnSpacing,
                                        0,
                                        -row * _target.rowSpacing - (i * _target.zOffset));

                var position = first.position + first.rotation * offset;

                var clone = Instantiate(first, position, first.rotation, _target.transform);
                Undo.RegisterCreatedObjectUndo(clone.gameObject, "created_auto_grid_position");

                EditorHelper.RenameChildren(_target.gameObject);
            }
        }

        public override void OnSceneGUI()
        {
            var e = Event.current;

            for (int i = 0; i < _target.transform.childCount; ++i)
            {
                var node = _target.transform.GetChild(i);
                var pos = node.transform.position;

                EditorHelper.DrawLabelWithinDistance(pos + Vector3.up * 3, $"P{(i + 1).ToString()}", CustomEditorStyles.NodeLabel);

                if (Handles.Button(pos, Quaternion.identity, 1, 1, Handles.SphereHandleCap))
                {
                    if (e.shift)
                    {
                        Undo.RecordObject(_target, "deleted_node");
                        Undo.DestroyObjectImmediate(node.gameObject);
                        e.Use();
                    }
                    else
                    {
                        Selection.activeObject = node.gameObject;
                    }
                }
            }

            if (_gizmoDrawers.Length > 0)
            {
                foreach (var g in _gizmoDrawers)
                {
                    if (g != null)
                    {
                        g.gizmoColor = _target.gridType == RaceStartMode.StandingStart ?
                                        RGSKEditorSettings.Instance.standingStartGridColor :
                                        RGSKEditorSettings.Instance.rollingStartGridColor;
                    }
                }
            }

            base.OnSceneGUI();
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            var go = new GameObject();
            go.transform.position = position + new Vector3(0, 0.1f, 0);
            go.transform.SetParent(_target.transform);
            go.name = $"GridPosition";

            var gizmo = go.AddComponent<GizmoDrawer>();
            gizmo.gizmoShape = GizmoDrawer.GizmoShape.Cube;
            gizmo.visualizeDirection = true;
            gizmo.transform.localScale = new Vector3(2, 0.5f, 4);

            Undo.RegisterCreatedObjectUndo(go, "created_grid_position");
            Undo.RecordObject(_target, "added_grid_position");

            _gizmoDrawers = _target.GetComponentsInChildren<GizmoDrawer>();
            EditorHelper.RenameChildren(_target.gameObject);
        }

        float GetValueSum()
        {
            return _target.gridSize +
                    _target.columnCount +
                    _target.columnSpacing +
                    _target.rowSpacing +
                    _target.zOffset;
        }
    }
}