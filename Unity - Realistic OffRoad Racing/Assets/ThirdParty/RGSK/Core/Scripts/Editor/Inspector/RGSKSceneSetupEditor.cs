using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RGSKSceneSetup))]
    public class RGSKSceneSetupEditor : UnityEditor.Editor
    {
        RGSKSceneSetup _target;
        string[] tabs = new string[] { "General", "Misc" };
        static int tabIndex;
        int _btnSize = 25;
        const string duplicateMessage = "A {0} already exists in the scene. Would you like to create another one?";

        void OnEnable()
        {
            _target = (RGSKSceneSetup)target;
        }

        public override void OnInspectorGUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("General", EditorStyles.boldLabel);

                if (GUILayout.Button("Camera Manager", GUILayout.Height(_btnSize)))
                {
                    CreateCameraManager();
                }

                EditorHelper.DrawLine();
                GUILayout.Label("Race", EditorStyles.boldLabel);

                if (GUILayout.Button("Race Initializer", GUILayout.Height(_btnSize)))
                {
                    CreateRaceInitializer();
                }

                if (GUILayout.Button("Track", GUILayout.Height(_btnSize)))
                {
                    CreateTrack();
                }


                EditorHelper.DrawLine();
                GUILayout.Label("Misc", EditorStyles.boldLabel);

                if (GUILayout.Button("Music Player", GUILayout.Height(_btnSize)))
                {
                    CreateMusicPlayer();
                }
            }
        }

        void CreateRaceInitializer()
        {
            if (RGSKEditorSettings.Instance.raceInitTemplate == null)
                return;

            var initializer = FindAnyObjectByType<RaceInitializer>();
            if (initializer != null)
            {
                if (!EditorHelper.DisplayDialog("", string.Format(duplicateMessage, "Race Initializer"),
                        "Yes", "No"))
                {
                    return;
                }
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.raceInitTemplate,
                _target.transform, null, null, false),
                "created_race_init");
        }

        void CreateCameraManager()
        {
            if (RGSKEditorSettings.Instance.cameraManagerTemplate == null)
                return;

            if (CameraManager.Instance != null)
            {
                EditorHelper.SelectObject(CameraManager.Instance.gameObject);
                return;
            }

            var camera = Camera.allCameras;
            if (camera.Length > 0)
            {
                if (EditorHelper.DisplayDialog("Create Camera Manager",
                        $"Proceeding will deactivate {camera.Length} camera(s) in your scene.",
                        "OK", "Cancel"))
                {
                    foreach (var c in camera)
                    {
                        c.gameObject.SetActive(false);
                    }
                }
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.cameraManagerTemplate,
                _target.transform, null, null, true),
                "created_camera_manager");
        }

        void CreateTrack()
        {
            if (RGSKEditorSettings.Instance.trackTemplate == null)
                return;

            var track = FindAnyObjectByType<Track>();
            if (track != null)
            {
                if (!EditorHelper.DisplayDialog("", string.Format(duplicateMessage, "Track"),
                        "Yes", "No"))
                {
                    return;
                }
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.trackTemplate,
                _target.transform),
                "created_newtrack");
        }

        void CreateMusicPlayer()
        {
            var music = FindAnyObjectByType<MusicPlayer>();
            if (music != null)
            {
                EditorHelper.SelectObject(music);
                return;
            }

            Undo.RegisterCreatedObjectUndo(
                EditorHelper.CreatePrefab(RGSKEditorSettings.Instance.musicTemplate,
                 _target.transform),
                 "created_musicplayer");
        }
    }
}