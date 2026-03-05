using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RGSKCore))]
    public class RGSKCoreEditor : UnityEditor.Editor
    {
        RGSKCore _target;

        void OnEnable()
        {
            _target = (RGSKCore)target;
        }

        public override void OnInspectorGUI()
        {
            EditorHelper.DrawHeader(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth);
            EditorGUILayout.HelpBox("Please open the RGSK Menu to modify values.", MessageType.Info);

            if (GUILayout.Button("Open RGSK Menu"))
            {
                RGSKWindow.ShowWindow();
            }
        }
    }
}