using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Extensions;

namespace RGSK.Editor
{
    [CustomEditor(typeof(AINode))]
    [CanEditMultipleObjects]
    public class AINodeEditor : RouteNodeEditor
    {
        AINode _target2;
        SerializedProperty racingLineOffset;
        SerializedProperty branchRoutes;

        protected override void OnEnable()
        {
            base.OnEnable();

            _target2 = (AINode)target;
            racingLineOffset = serializedObject.FindProperty(nameof(racingLineOffset));
            branchRoutes = serializedObject.FindProperty(nameof(branchRoutes));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Racing Line", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(racingLineOffset);
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Branches", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(branchRoutes);
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Create Branch Route"))
                {
                    var template = EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.aiRoute,
                                                            _target2.route.transform.parent);

                    template.name = $"{_target2.route.name}_Branch";

                    Undo.RegisterCreatedObjectUndo(template, "created_branch");
                    Undo.RecordObject(_target2, "added_branch");

                    if (template.TryGetComponent<AIRoute>(out var route))
                    {
                        _target2.branchRoutes.RemoveNullElements();
                        _target2.branchRoutes.Add(route);
                        route.priority = 0;
                        route.defaultNodeWidth = _target2.route.defaultNodeWidth;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}