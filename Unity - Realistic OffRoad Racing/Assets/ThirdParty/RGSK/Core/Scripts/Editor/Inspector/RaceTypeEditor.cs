using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceType))]
    public class RaceTypeEditor : UnityEditor.Editor
    {
        RaceType _target;

        SerializedProperty raceDurationMode;
        SerializedProperty positioningMode;
        SerializedProperty positionSortMode;
        SerializedProperty timerMode;
        SerializedProperty globalTimerElapsedAction;
        SerializedProperty minLaps;
        SerializedProperty minMaxCompetitorCount;
        SerializedProperty infiniteLaps;
        SerializedProperty disqualifyLastPlaceEachLap;
        SerializedProperty allowGhost;
        SerializedProperty vehicleHandlingMode;
        SerializedProperty selectableFromMenu;
        SerializedProperty allowedTrackLayouts;

        SerializedProperty displayName;
        SerializedProperty description;

        string[] tabs = new string[]
        {
            "Settings",
            "Info"
        };

        static int tabIndex;

        void OnEnable()
        {
            _target = (RaceType)target;

            raceDurationMode = serializedObject.FindProperty(nameof(raceDurationMode));
            positioningMode = serializedObject.FindProperty(nameof(positioningMode));
            positionSortMode = serializedObject.FindProperty(nameof(positionSortMode));
            timerMode = serializedObject.FindProperty(nameof(timerMode));
            globalTimerElapsedAction = serializedObject.FindProperty(nameof(globalTimerElapsedAction));
            minLaps = serializedObject.FindProperty(nameof(minLaps));
            minMaxCompetitorCount = serializedObject.FindProperty(nameof(minMaxCompetitorCount));
            infiniteLaps = serializedObject.FindProperty(nameof(infiniteLaps));
            disqualifyLastPlaceEachLap = serializedObject.FindProperty(nameof(disqualifyLastPlaceEachLap));
            allowGhost = serializedObject.FindProperty(nameof(allowGhost));
            vehicleHandlingMode = serializedObject.FindProperty(nameof(vehicleHandlingMode));
            selectableFromMenu = serializedObject.FindProperty(nameof(selectableFromMenu));
            allowedTrackLayouts = serializedObject.FindProperty(nameof(allowedTrackLayouts));

            description = serializedObject.FindProperty(nameof(description));
            displayName = serializedObject.FindProperty(nameof(displayName));
        }

        public override void OnInspectorGUI()
        {
            EditorHelper.DrawAddToListUI(RGSKCore.Instance.RaceSettings.raceTypes, _target);

            serializedObject.Update();

            tabIndex = GUILayout.Toolbar(tabIndex, tabs);

            switch (tabs[tabIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "settings":
                    {
                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.PropertyField(positioningMode);
                            EditorGUILayout.PropertyField(positionSortMode);
                            EditorGUILayout.PropertyField(raceDurationMode);

                            EditorGUI.BeginDisabledGroup(_target.raceDurationMode == RaceDurationMode.LapBased);
                            {
                                EditorGUILayout.PropertyField(timerMode);
                                EditorGUI.EndDisabledGroup();
                            }

                            EditorGUI.BeginDisabledGroup(_target.raceDurationMode == RaceDurationMode.LapBased || _target.timerMode == RaceTimerMode.PerCompetitor);
                            {
                                EditorGUILayout.PropertyField(globalTimerElapsedAction, new GUIContent("Timer Elapsed Action"));
                                EditorGUI.EndDisabledGroup();
                            }

                            EditorGUILayout.PropertyField(vehicleHandlingMode);
                            EditorGUILayout.PropertyField(minLaps);
                            EditorGUILayout.PropertyField(minMaxCompetitorCount);
                            EditorGUILayout.PropertyField(infiniteLaps);
                            EditorGUILayout.PropertyField(disqualifyLastPlaceEachLap);
                            EditorGUILayout.PropertyField(allowGhost);
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.PropertyField(allowedTrackLayouts);
                            EditorGUILayout.PropertyField(selectableFromMenu);
                        }
                        break;
                    }

                case "info":
                    {
                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.PropertyField(displayName);
                            EditorGUILayout.PropertyField(description);
                        }
                        break;
                    }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}