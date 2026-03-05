using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(ChildRename))]
    [CanEditMultipleObjects]
    public class ChildRenameEditor : UnityEditor.Editor
    {
        ChildRename _target;
        SerializedProperty prefix;
        SerializedProperty includeSiblingIndex;

        void OnEnable()
        {
            _target = (ChildRename)target;

            prefix = serializedObject.FindProperty(nameof(prefix));
            includeSiblingIndex = serializedObject.FindProperty(nameof(includeSiblingIndex));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Renames all the children of this gameObject.", MessageType.Info);

            EditorGUILayout.PropertyField(prefix);
            EditorGUILayout.PropertyField(includeSiblingIndex);

            if (GUILayout.Button("Rename"))
            {
                _target.Rename();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}