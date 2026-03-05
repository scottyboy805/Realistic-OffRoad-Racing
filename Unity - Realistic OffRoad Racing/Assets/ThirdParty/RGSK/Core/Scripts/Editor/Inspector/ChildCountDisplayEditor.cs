using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(ChildCountDisplay))]
    public class ChildCountDisplayEditor : UnityEditor.Editor
    {
        ChildCountDisplay _target;

        void OnEnable()
        {
            _target = (ChildCountDisplay)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField($"Child Count: {_target.transform.childCount}");
                EditorGUILayout.EndVertical();
            }
        }
    }
}