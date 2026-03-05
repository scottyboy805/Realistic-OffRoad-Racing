using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using RGSK.Helpers;

namespace RGSK.Editor
{
    public class ProfileDefinitionCreatorWindow : EditorWindow
    {
        TextAsset names;

        [MenuItem("Window/RGSK/Tools/Profile Definition Creator", false, 2600)]
        public static void ShowWindow()
        {
            var window = GetWindow<ProfileDefinitionCreatorWindow>("Profile Creator");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.HelpBox("Create ProfileDefinitions from a text file of names.\nEnsure first and last names are seperated by a space e.g \"John Doe\".", MessageType.Info);
            names = EditorGUILayout.ObjectField("Names", names, typeof(TextAsset), false) as TextAsset;

            if (GUILayout.Button("Create"))
            {
                if (names == null)
                    return;

                var path = EditorUtility.SaveFolderPanel("Save", "Assets", "Profiles");

                if (EditorHelper.IsValidSavePath(path, out path))
                {
                    using (var reader = new StringReader(names.text))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var p = GeneralHelper.CreateProfileDefinition(line);
                            AssetDatabase.CreateAsset(p, $"{path}/{p.firstName}_{p.lastName}.asset");
                        }
                    }
                }

                AssetDatabase.SaveAssets();
            }
        }
    }
}