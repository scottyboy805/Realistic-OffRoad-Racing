using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(TrackDefinition))]
    public class TrackDefinitionEditor : ItemDefinitionEditor
    {
        TrackDefinition _definition;

        SerializedProperty scene;
        SerializedProperty country;
        SerializedProperty minimapPreview;
        SerializedProperty layoutType;
        SerializedProperty length;
        SerializedProperty gridSlots;
        SerializedProperty allowRollingStarts;
        SerializedProperty allowedRaceTypes;

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

            _definition = (TrackDefinition)_target;

            scene = serializedObject.FindProperty(nameof(scene));
            country = serializedObject.FindProperty(nameof(country));
            minimapPreview = serializedObject.FindProperty(nameof(minimapPreview));
            layoutType = serializedObject.FindProperty(nameof(layoutType));
            length = serializedObject.FindProperty(nameof(length));
            gridSlots = serializedObject.FindProperty(nameof(gridSlots));
            allowRollingStarts = serializedObject.FindProperty(nameof(allowRollingStarts));
            allowedRaceTypes = serializedObject.FindProperty(nameof(allowedRaceTypes));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorHelper.DrawAddToListUI(RGSKCore.Instance.ContentSettings.tracks, _definition);

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
                EditorGUILayout.PropertyField(layoutType);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(gridSlots);
                EditorGUILayout.PropertyField(allowRollingStarts);
            }


            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(allowedRaceTypes);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(scene);
            }
        }

        protected override void DrawInfo()
        {
            base.DrawInfo();

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(country);
                EditorGUILayout.PropertyField(minimapPreview);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField($"Lap Record: {UIHelper.FormatTimeText(_definition.LoadBestLap())}");
            }
        }
    }
}