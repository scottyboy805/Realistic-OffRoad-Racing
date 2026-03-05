using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomPropertyDrawer(typeof(ExposedScriptableObjectAttribute))]
    public class ExposedScriptableObjectAttributeDrawer : PropertyDrawer
    {
        UnityEditor.Editor _editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.objectReferenceValue != null)
            {
                EditorGUI.indentLevel = 0;
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            if (property.isExpanded)
            {
                if (_editor == null)
                {
                    UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
                }

                EditorGUI.indentLevel++;

                _editor?.OnInspectorGUI();

                EditorGUI.indentLevel--;
            }
        }
    }
}