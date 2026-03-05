using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(ProfileDefiner))]
    public class ProfileDefinerEditor : UnityEditor.Editor
    {
        ProfileDefiner _target;
        SerializedProperty definition;

        void OnEnable()
        {
            _target = (ProfileDefiner)target;
            definition = serializedObject.FindProperty(nameof(definition));
        }

        public override void OnInspectorGUI()
        {
            if (_target.definition == null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(definition);

                    if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        CreateDefinition();
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(definition);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateDefinition()
        {
            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"NewProfileDefinition", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var data = ScriptableObject.CreateInstance<ProfileDefinition>();

                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();
                EditorHelper.SelectObject(AssetDatabase.LoadMainAssetAtPath(path));

                Undo.RecordObject(_target, "added_profile_definition");
                _target.definition = data;
            }
        }
    }
}