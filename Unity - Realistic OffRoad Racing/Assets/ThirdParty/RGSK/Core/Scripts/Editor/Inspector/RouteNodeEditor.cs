using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RouteNode))]
    [CanEditMultipleObjects]
    public class RouteNodeEditor : UnityEditor.Editor
    {
        RouteNode _target;
        SerializedProperty leftWidth;
        SerializedProperty rightWidth;
        SerializedProperty combinedWidth;
        SerializedProperty combineWidth;

        protected virtual void OnEnable()
        {
            _target = (RouteNode)target;

            leftWidth = serializedObject.FindProperty(nameof(leftWidth));
            rightWidth = serializedObject.FindProperty(nameof(rightWidth));
            combinedWidth = serializedObject.FindProperty(nameof(combinedWidth));
            combineWidth = serializedObject.FindProperty(nameof(combineWidth));
            _target.combinedWidth = _target.leftWidth + _target.rightWidth;
        }

        public override void OnInspectorGUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(combineWidth);

                if (_target.combineWidth)
                {
                    EditorGUILayout.PropertyField(combinedWidth);
                }
                else
                {
                    EditorGUILayout.PropertyField(leftWidth);
                    EditorGUILayout.PropertyField(rightWidth);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}