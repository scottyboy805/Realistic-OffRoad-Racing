using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(UIScreenID))]
    public class UIScreenIDEditor : UnityEditor.Editor
    {
        UIScreenID _target;

        SerializedProperty screenPrefab;
        SerializedProperty screenPrefabMobile;
        SerializedProperty onOpenInputMode;
        SerializedProperty isPersistentScreen;

        void OnEnable()
        {
            _target = (UIScreenID)target;

            screenPrefab = serializedObject.FindProperty(nameof(screenPrefab));
            screenPrefabMobile = serializedObject.FindProperty(nameof(screenPrefabMobile));
            onOpenInputMode = serializedObject.FindProperty(nameof(onOpenInputMode));
            isPersistentScreen = serializedObject.FindProperty(nameof(isPersistentScreen));
        }

        public override void OnInspectorGUI()
        {
            CheckMismatchReference(_target.screenPrefab);
            CheckMismatchReference(_target.screenPrefabMobile);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(screenPrefab);
                    EditorGUILayout.PropertyField(screenPrefabMobile);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorHelper.SaveAssetReference(_target.screenPrefab, _target.name);
                        EditorHelper.SaveAssetReference(_target.screenPrefabMobile, $"{_target.name}_mobile");
                    }
                }

                EditorGUILayout.PropertyField(onOpenInputMode);
                EditorGUILayout.PropertyField(isPersistentScreen);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CheckMismatchReference(UIScreen screen)
        {
            if (screen == null)
                return;

            if (screen.screenID != _target)
            {
                EditorGUILayout.HelpBox($"This ID does not macth the one in the \"{screen.name}\" prefab!", MessageType.Warning);

                if (GUILayout.Button("Fix"))
                {
                    screen.screenID = _target;
                    EditorUtility.SetDirty(screen);
                }
            }
        }
    }
}