using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CameraRTSaver))]
    public class CameraRTSaverEditor : UnityEditor.Editor
    {
        CameraRTSaver _target;
        Camera cam;

        void OnEnable()
        {
            _target = (CameraRTSaver)target;
            cam = _target.GetComponent<Camera>();
        }

        public override void OnInspectorGUI()
        {
            if (cam.targetTexture != null)
            {
                if (GUILayout.Button("Save Render Texture as PNG"))
                {
                    SaveAsPNG();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please assign a render texture to the camera inorder to save it as a PNG!", MessageType.Info);
            }
        }

        void SaveAsPNG()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), sceneName, "png");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
                RenderTexture.active = cam.targetTexture;
                tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
                RenderTexture.active = null;
                File.WriteAllBytes(path, tex.EncodeToPNG());

                AssetDatabase.Refresh();

                AssetDatabase.ImportAsset(path);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.WriteImportSettingsIfDirty(path);

                AssetDatabase.Refresh();
                EditorHelper.SelectObject(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
        }
    }
}