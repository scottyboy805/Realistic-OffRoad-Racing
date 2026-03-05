using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceSession))]
    public class RaceSessionEditor : ItemDefinitionEditor
    {
        RaceSession _session;

        SerializedProperty sessionType;
        SerializedProperty raceType;
        SerializedProperty startMode;
        SerializedProperty lapCount;
        SerializedProperty opponentCount;
        SerializedProperty playerGridStartMode;
        SerializedProperty playerStartPosition;
        SerializedProperty sessionTimeLimit;
        SerializedProperty enableGhost;
        SerializedProperty ghostOffset;
        SerializedProperty enableSlipstream;
        SerializedProperty enableCatchup;
        SerializedProperty disableCollision;
        SerializedProperty spectatorMode;
        SerializedProperty targetScores;
        SerializedProperty entrants;
        SerializedProperty autoPopulateEntrantOptions;
        SerializedProperty opponentDifficulty;
        SerializedProperty raceRewards;
        SerializedProperty track;
        SerializedProperty saveRecords;

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

            _session = (RaceSession)_target;

            sessionType = serializedObject.FindProperty(nameof(sessionType));
            raceType = serializedObject.FindProperty(nameof(raceType));
            startMode = serializedObject.FindProperty(nameof(startMode));
            lapCount = serializedObject.FindProperty(nameof(lapCount));
            opponentCount = serializedObject.FindProperty(nameof(opponentCount));
            playerGridStartMode = serializedObject.FindProperty(nameof(playerGridStartMode));
            playerStartPosition = serializedObject.FindProperty(nameof(playerStartPosition));
            sessionTimeLimit = serializedObject.FindProperty(nameof(sessionTimeLimit));
            enableGhost = serializedObject.FindProperty(nameof(enableGhost));
            ghostOffset = serializedObject.FindProperty(nameof(ghostOffset));
            enableSlipstream = serializedObject.FindProperty(nameof(enableSlipstream));
            enableCatchup = serializedObject.FindProperty(nameof(enableCatchup));
            disableCollision = serializedObject.FindProperty(nameof(disableCollision));
            spectatorMode = serializedObject.FindProperty(nameof(spectatorMode));
            targetScores = serializedObject.FindProperty(nameof(targetScores));
            entrants = serializedObject.FindProperty(nameof(entrants));
            autoPopulateEntrantOptions = serializedObject.FindProperty(nameof(autoPopulateEntrantOptions));
            opponentDifficulty = serializedObject.FindProperty(nameof(opponentDifficulty));
            raceRewards = serializedObject.FindProperty(nameof(raceRewards));
            track = serializedObject.FindProperty(nameof(track));
            saveRecords = serializedObject.FindProperty(nameof(saveRecords));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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
                if (_session.entrants.Count(x => x.isPlayer) > 1)
                {
                    EditorGUILayout.HelpBox("More than 1 player is assigned! This is will result in undesired behavior.", MessageType.Error);
                }

                if (_session.entrants.Count(x => x.isPlayer) > 0 && _session.autoPopulateEntrantOptions.autoPopulatePlayer)
                {
                    EditorGUILayout.HelpBox("A player entrant is assigned and 'Auto Populate Player' is enabled! This may replace the assigned player entrant at runtime.", MessageType.Warning);
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(entrants, true);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(autoPopulateEntrantOptions);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(raceType);
                EditorGUILayout.PropertyField(sessionType);
                EditorGUILayout.PropertyField(startMode);

                EditorGUI.BeginDisabledGroup(HideLaps(_session.raceType));
                {
                    EditorGUILayout.PropertyField(lapCount);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.BeginDisabledGroup(!_session.IsTimedSession());
                {
                    EditorGUILayout.PropertyField(sessionTimeLimit);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.BeginDisabledGroup(!_session.UseGhostVehicle());
                {
                    EditorGUILayout.PropertyField(enableGhost);
                    EditorGUILayout.PropertyField(ghostOffset);
                    EditorGUI.EndDisabledGroup();
                }
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(opponentDifficulty);
                EditorGUILayout.PropertyField(playerGridStartMode);
                EditorGUI.BeginDisabledGroup(_session.playerGridStartMode != SelectionMode.Selected);
                {
                    EditorGUILayout.PropertyField(playerStartPosition);
                    EditorGUI.EndDisabledGroup();
                }

                if (_session.enableSlipstream && RGSKCore.Instance.GeneralSettings.slipstreamSettings == null)
                {
                    EditorGUILayout.HelpBox("Slipstream settings have not been assigned! Please open 'RGSK Menu > General tab' and assign the settings.", MessageType.Warning);
                }
                EditorGUILayout.PropertyField(enableSlipstream);

                if (_session.enableCatchup && RGSKCore.Instance.GeneralSettings.catchupSettings == null)
                {
                    EditorGUILayout.HelpBox("Catchup settings have not been assigned! Please open 'RGSK Menu > General tab' and assign the settings.", MessageType.Warning);
                }
                EditorGUILayout.PropertyField(enableCatchup);

                EditorGUILayout.PropertyField(disableCollision);

                if (_session.spectatorMode)
                {
                    EditorGUILayout.HelpBox("Spectator Mode is enabled! The player vehicle will be controlled by AI.", MessageType.Warning);
                }
                EditorGUILayout.PropertyField(spectatorMode);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                if (!_session.IsTargetScoreSession())
                {
                    EditorGUILayout.HelpBox("Target scores only apply for the \"Target Score\" session type.", MessageType.Info);
                }

                EditorGUI.BeginDisabledGroup(!_session.IsTargetScoreSession());
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(targetScores, true);
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup();
                }
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(raceRewards);
                EditorGUI.indentLevel--;
            }
        }

        protected override void DrawInfo()
        {
            base.DrawInfo();

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(track);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(saveRecords);
                EditorGUILayout.LabelField($"Best: {UIHelper.FormatOrdinalText(_session.LoadBestPosition())}");
            }
        }

        public static bool HideLaps(RaceType raceType)
        {
            if (raceType != null)
            {
                return RaceType.IsInfiniteLaps(raceType) ||
                    !EnumFlags.GetSelectedIndexes(raceType.allowedTrackLayouts).
                    Contains((int)TrackLayoutType.Circuit);
            }

            return false;
        }
    }
}