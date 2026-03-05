using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

namespace RGSK.Editor
{
    [CustomEditor(typeof(Track))]
    public class TrackEditor : UnityEditor.Editor
    {
        Track _target;

        SerializedProperty definition;
        SerializedProperty raceStartOffsetDuration;

        string[] tabs = new string[]
        {
            "Routes",
            "Grid",
        };

        static int tabIndex;
        int _btnSize = 25;

        void OnEnable()
        {
            _target = (Track)target;
            definition = serializedObject.FindProperty(nameof(definition));
            raceStartOffsetDuration = serializedObject.FindProperty(nameof(raceStartOffsetDuration));
        }

        public override void OnInspectorGUI()
        {
            if (_target.definition == null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(definition);

                    if (_target.definition == null)
                    {
                        if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(100)))
                        {
                            CreateDefinition();
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(definition);
            }

            EditorGUI.BeginDisabledGroup(_target.definition == null);
            {
                if (GUILayout.Button("Update Definition Properties"))
                {
                    AutoUpdateProperties();
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorHelper.DrawLine();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Route", EditorStyles.boldLabel);

                if (GUILayout.Button("Checkpoint Route", GUILayout.Height(_btnSize)))
                {
                    CreateCheckpointRoute();
                }

                if (GUILayout.Button("AI Route", GUILayout.Height(_btnSize)))
                {
                    CreateAIRoute();
                }

                if (GUILayout.Button("Camera Route", GUILayout.Height(_btnSize)))
                {
                    CreateRouteCameras();
                }
    
                EditorHelper.DrawLine();
                GUILayout.Label("Grid", EditorStyles.boldLabel);

                if (GUILayout.Button("Grid (Standing Start)", GUILayout.Height(_btnSize)))
                {
                    CreateGrid(RaceStartMode.StandingStart);
                }

                if (GUILayout.Button("Grid (Rolling Start)", GUILayout.Height(_btnSize)))
                {
                    CreateGrid(RaceStartMode.RollingStart);
                }
            
                EditorHelper.DrawLine();
                GUILayout.Label("Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(raceStartOffsetDuration);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void AutoUpdateProperties()
        {
            if (_target != null && _target.definition != null)
            {
                if (_target.CheckpointRoute != null)
                {
                    _target.definition.layoutType = _target.CheckpointRoute.loop ? TrackLayoutType.Circuit : TrackLayoutType.PointToPoint;
                    _target.definition.length = _target.CheckpointRoute.Distance;
                }

                var ssGrid = _target.GetRaceGrid(RaceStartMode.StandingStart);
                var rsGrid = _target.GetRaceGrid(RaceStartMode.RollingStart);

                if (ssGrid != null)
                {
                    _target.definition.gridSlots = ssGrid.GetPositions().Count;
                }

                _target.definition.allowRollingStarts = rsGrid != null && rsGrid.GetPositions().Count > 0; 

                EditorUtility.SetDirty(_target.definition);
            }
        }

        void CreateCheckpointRoute()
        {
            var route = _target.GetComponentInChildren<CheckpointRoute>();
            if (route != null)
            {
                EditorHelper.SelectObject(route);
                return;
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.checkpointRoute, _target.transform),
                "created_cpRoute");
        }

        void CreateAIRoute()
        {
            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.aiRoute, _target.transform),
                "created_aiRoute");
        }

        /* void CreateSequences()
        {
            var seq = _target.GetComponentInChildren<RaceStateSequencePlayer>();
            if (seq != null)
            {
                EditorHelper.SelectObject(seq);
                return;
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.trackSequenceTemplate, _target.transform),
                "created_racesequence");
        } */

        void CreateRouteCameras()
        {
            var cams = _target.GetComponentInChildren<CameraRoute>();
            if (cams != null)
            {
                EditorHelper.SelectObject(cams);
                return;
            }

            var instance = EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.cinematicCameraTemplate, _target.transform);
            Undo.RegisterCreatedObjectUndo(instance, "created_cinematiccamera");
        }

        void CreateGrid(RaceStartMode type)
        {
            var grid = _target.GetRaceGrid(type);
            if (grid != null)
            {
                EditorHelper.SelectObject(grid);
                return;
            }

            var template = type == RaceStartMode.StandingStart ?
                            RGSKEditorSettings.Instance.standingStartGridTemplate :
                            RGSKEditorSettings.Instance.rollingStartGridTemplate;

            Undo.RegisterCreatedObjectUndo(
            EditorHelper.CreatePrefab(template, _target.transform),
            $"created_{type}_grid");
        }

        void CreateDefinition()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"TrackDefinition_{sceneName}", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var data = ScriptableObject.CreateInstance<TrackDefinition>();
                data.scene.ScenePath = SceneManager.GetActiveScene().path;
                data.objectName = SceneManager.GetActiveScene().name;

                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();

                Undo.RecordObject(_target, "added_track_definition");
                _target.definition = data;
            }
        }
    }
}