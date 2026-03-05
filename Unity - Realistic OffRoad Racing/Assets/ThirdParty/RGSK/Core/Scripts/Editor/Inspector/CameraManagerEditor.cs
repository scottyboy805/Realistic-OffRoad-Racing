using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CameraManager))]
    public class CameraManagerEditor : UnityEditor.Editor 
    {
        SerializedProperty settings;
        SerializedProperty cinemachineBrain;

        void OnEnable() 
        {
            settings = serializedObject.FindProperty(nameof(settings));
            cinemachineBrain = serializedObject.FindProperty(nameof(cinemachineBrain));
        }

        public override void OnInspectorGUI() 
        {
            EditorGUILayout.PropertyField(settings);
            EditorGUILayout.PropertyField(cinemachineBrain);
            serializedObject.ApplyModifiedProperties();
        }
    }
}