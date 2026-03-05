using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(ScriptableObjectURL))]
    public class ScriptableObjectUrlEditor : UnityEditor.Editor
    {
        ScriptableObjectURL _target;

        void OnEnable()
        {
            _target = (ScriptableObjectURL)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(_target.helpText, CustomEditorStyles.MenuLabelCenter);
            if (GUILayout.Button(_target.buttonText))
            {
                Application.OpenURL(_target.url);
            }
        }
    }
}