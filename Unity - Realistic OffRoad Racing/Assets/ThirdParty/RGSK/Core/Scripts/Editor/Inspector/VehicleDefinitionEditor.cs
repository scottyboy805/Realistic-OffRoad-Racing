using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(VehicleDefinition))]
    public class VehicleDefinitionEditor : ItemDefinitionEditor
    {
        VehicleDefinition _definition;

        SerializedProperty prefab;
        SerializedProperty manufacturer;
        SerializedProperty vehicleClass;
        SerializedProperty defaultStats;
        SerializedProperty year;
        string[] tabs;
        static int tabIndex;

        protected override void OnEnable()
        {
            base.OnEnable();

            tabs = new string[]
            {
                "Settings",
                "Info"
            };

            _definition = (VehicleDefinition)_target;

            prefab = serializedObject.FindProperty(nameof(prefab));
            manufacturer = serializedObject.FindProperty(nameof(manufacturer));
            vehicleClass = serializedObject.FindProperty(nameof(vehicleClass));
            defaultStats = serializedObject.FindProperty(nameof(defaultStats));
            year = serializedObject.FindProperty(nameof(year));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorHelper.DrawAddToListUI(RGSKCore.Instance.ContentSettings.vehicles, _definition);

            tabIndex = GUILayout.Toolbar(tabIndex, tabs);

            switch (tabs[tabIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "settings":
                    {
                        DrawSettings();
                        break;
                    }

                case "info":
                    {
                        DrawInfo();
                        break;
                    }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawSettings()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(prefab);
                EditorGUILayout.PropertyField(manufacturer);
                EditorGUILayout.PropertyField(vehicleClass);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(defaultStats);
                EditorGUI.indentLevel--;
            }
        }

        protected override void DrawInfo()
        {
            base.DrawInfo();

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(year);
            }
        }
    }
}