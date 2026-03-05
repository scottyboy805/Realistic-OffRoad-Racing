using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceInitializer))]
    public class RaceInitializerEditor : UnityEditor.Editor
    {
        RaceInitializer _target;

        SerializedProperty session;
        SerializedProperty track;
        SerializedProperty autoFindTrack;
        SerializedProperty autoInitialize;

        void OnEnable()
        {
            _target = (RaceInitializer)target;
            session = serializedObject.FindProperty(nameof(session));
            track = serializedObject.FindProperty(nameof(track));
            autoFindTrack = serializedObject.FindProperty(nameof(autoFindTrack));
            autoInitialize = serializedObject.FindProperty(nameof(autoInitialize));
        }

        public override void OnInspectorGUI()
        {
            if (_target.session == null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(session);

                    if (_target.session == null)
                    {
                        if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(100)))
                        {
                            CreateSession();
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(session);
            }

            EditorHelper.DrawLine();
            
            if (!_target.autoFindTrack)
            {
                EditorGUILayout.PropertyField(track);
            }

            EditorGUILayout.PropertyField(autoFindTrack);
            EditorGUILayout.PropertyField(autoInitialize);

            serializedObject.ApplyModifiedProperties();
        }

        void CreateSession()
        {
            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"NewRaceSession", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var data = ScriptableObject.CreateInstance<RaceSession>();

                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();

                Undo.RecordObject(_target, "added_session");
                _target.session = data;
            }
        }
    }
}