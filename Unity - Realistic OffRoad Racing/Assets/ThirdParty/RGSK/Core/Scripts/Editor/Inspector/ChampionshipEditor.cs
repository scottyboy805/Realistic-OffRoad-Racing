using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(Championship))]
    public class ChampionshipEditor : ItemDefinitionEditor
    {
        Championship _championship;

        SerializedProperty raceType;
        SerializedProperty points;
        SerializedProperty entrants;
        SerializedProperty autoPopulateEntrantOptions;
        SerializedProperty rounds;
        SerializedProperty rewards;
        SerializedProperty spawnMode;
        SerializedProperty reverseSpawnOrder;
        SerializedProperty enableSlipstream;
        SerializedProperty enableCatchup;
        SerializedProperty disableCollision;
        SerializedProperty spectatorMode;
        SerializedProperty opponentDifficulty;

        string[] tabs;
        static int tabIndex;

        protected override void OnEnable()
        {
            base.OnEnable();

            tabs = new string[]
            {
                "Settings",
                "Rounds",
                "Info"
            };

            _championship = (Championship)target;

            entrants = serializedObject.FindProperty(nameof(entrants));
            points = serializedObject.FindProperty(nameof(points));
            autoPopulateEntrantOptions = serializedObject.FindProperty(nameof(autoPopulateEntrantOptions));
            raceType = serializedObject.FindProperty(nameof(raceType));
            rounds = serializedObject.FindProperty(nameof(rounds));
            rewards = serializedObject.FindProperty(nameof(rewards));
            spawnMode = serializedObject.FindProperty(nameof(spawnMode));
            reverseSpawnOrder = serializedObject.FindProperty(nameof(reverseSpawnOrder));
            enableSlipstream = serializedObject.FindProperty(nameof(enableSlipstream));
            enableCatchup = serializedObject.FindProperty(nameof(enableCatchup));
            disableCollision = serializedObject.FindProperty(nameof(disableCollision));
            spectatorMode = serializedObject.FindProperty(nameof(spectatorMode));
            opponentDifficulty = serializedObject.FindProperty(nameof(opponentDifficulty));
        }

        public override void OnInspectorGUI()
        {
            if (_championship.rounds.Count == 0)
            {
                EditorGUILayout.HelpBox("No rounds have been added! Please add a round in the 'Rounds' tab.", MessageType.Warning);
            }

            if (_championship.raceType == null)
            {
                EditorGUILayout.HelpBox("A race type has not been assigned! Please assign a race type below.", MessageType.Error);
            }

            if (!_championship.AllTracksAssigned())
            {
                EditorGUILayout.HelpBox("One or more rounds do not have an assigned track! Please assign a track to each round.", MessageType.Error);
            }

            if (!_championship.ValidGridSlots())
            {
                EditorGUILayout.HelpBox("One of the tracks does not have enough grid slots to spawn all of the entrants! Please reduce the number of entrants.", MessageType.Error);
            }

            serializedObject.Update();

            tabIndex = GUILayout.Toolbar(tabIndex, tabs);

            switch (tabs[tabIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "settings":
                    {
                        DrawSettings();
                        break;
                    }

                case "rounds":
                    {
                        DrawRounds();
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
                if (_championship.entrants.Count(x => x.isPlayer) > 0 && _championship.autoPopulateEntrantOptions.autoPopulatePlayer)
                {
                    EditorGUILayout.HelpBox("A player entrant is assigned and 'Auto Populate Player' is enabled! This may replace the assigned player entrant at runtime.", MessageType.Warning);
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(entrants);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(autoPopulateEntrantOptions);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(raceType);
                EditorGUILayout.PropertyField(opponentDifficulty);
                EditorGUILayout.PropertyField(enableSlipstream);
                EditorGUILayout.PropertyField(enableCatchup);
                EditorGUILayout.PropertyField(disableCollision);

                if (_championship.spectatorMode)
                {
                    EditorGUILayout.HelpBox("Spectator Mode is enabled! The player vehicle will be controlled by AI.", MessageType.Warning);
                }
                EditorGUILayout.PropertyField(spectatorMode);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(points);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(spawnMode, GUILayout.MinWidth(500));

                EditorGUI.BeginDisabledGroup(_championship.spawnMode == ChampionshipSpawnMode.Random);
                {
                    EditorGUILayout.PropertyField(reverseSpawnOrder);
                    EditorGUI.EndDisabledGroup();
                }
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rewards);
                EditorGUI.indentLevel--;
            }
        }

        void DrawRounds()
        {
            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < _championship.rounds.Count; i++)
                {
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"Round {i + 1}", EditorStyles.boldLabel);

                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                            {
                                Undo.RecordObject(_championship, "removed_championship_round");
                                _championship.rounds.RemoveAt(i);
                                break;
                            }
                        }

                        EditorGUILayout.Space(5);

                        if (_championship.rounds[i].track == null)
                        {
                            EditorGUILayout.HelpBox("A track has not been assigned for this round!", MessageType.Warning);
                        }

                        _championship.rounds[i].startMode = (RaceStartMode)EditorGUILayout.EnumPopup("Start Mode", _championship.rounds[i].startMode);

                        EditorGUI.BeginDisabledGroup(RaceSessionEditor.HideLaps(_championship.raceType));
                        {
                            _championship.rounds[i].lapCount = EditorGUILayout.IntField("Lap Count", _championship.rounds[i].lapCount);
                            EditorGUI.EndDisabledGroup();
                        }

                        EditorGUI.BeginDisabledGroup(!RaceType.IsTimed(_championship.raceType));
                        {
                            _championship.rounds[i].sessionTimeLimit = EditorGUILayout.FloatField("Session Time Limit", _championship.rounds[i].sessionTimeLimit);
                            EditorGUI.EndDisabledGroup();
                        }

                        _championship.rounds[i].track = EditorGUILayout.ObjectField("Track", _championship.rounds[i].track, typeof(TrackDefinition), false) as TrackDefinition;
                    }
                }

                EditorHelper.DrawLine();

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Add Round", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        Undo.RecordObject(_championship, "added_championship_round");
                        _championship.rounds.Add(new ChampionshipRound());
                    }
                }
            }
        }

        protected override void DrawInfo()
        {
            base.DrawInfo();

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField($"Best: {UIHelper.FormatOrdinalText(_championship.LoadBestPosition())}");
            }
        }
    }
}