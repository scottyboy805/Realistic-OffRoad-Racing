using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK.Editor
{
    public class EditorHelper : UnityEditor.Editor
    {
        public const string SceneSetupPath = "GameObject/RGSK/Scene Setup";
        public const string VehicleSetupPath = "GameObject/RGSK/Vehicle Setup";
        public const string DriverSetupPath = "GameObject/RGSK/Driver Setup";
        public const string UnityPackagesPath = "Assets/RGSK/Core/Editor/UnityPackages/";

        public const string LastSaveDirectoryKey = "RGSK_LastSaveDir_{0}";
        public const string SelectedSettingsKey = "RGSK_SelectedSettings_{0}_{1}";
        public const string AssetPathKey = "RGSK_AssetPath_{0}_{1}";
        public const string SessionLoadedKey = "RGSK_ProjectLoaded";
        public const string UpdateCheckedKey = "RGSK_UpdateChecked";

        public const string DocumentationLink = "https://docs.google.com/document/d/10h544-O4qXrb_FkADzpSOxFbYAK4oESpVGzk8DvMunQ";
        public const string DiscordLink = "https://discord.com/invite/VJWTDBm8H3";
        public const string YouTubeLink = "https://www.youtube.com/@rgskunity";
        public const string UnityForumsLink = "https://discussions.unity.com/t/racing-game-starter-kit-2-0-easily-create-racing-games/587347";
        public const string AssetStoreLink = "https://assetstore.unity.com/packages/tools/game-toolkits/racing-game-starter-kit-22615";

        [MenuItem(SceneSetupPath, false, 100)]
        public static void CreateSceneSetupWizard()
        {
            var setup = FindAnyObjectByType<RGSKSceneSetup>();
            if (setup != null)
            {
                SelectObject(setup.gameObject);
                return;
            }

            var go = new GameObject("[RGSK]");
            go.AddComponent<RGSKSceneSetup>();
            go.transform.SetAsLastSibling();
            Undo.RegisterCreatedObjectUndo(go, "created_scene_setup");
            SelectObject(go);
        }

        [MenuItem(VehicleSetupPath, false, 101)]
        public static void CreateVehicleSetupWizard()
        {
            var selection = Selection.activeGameObject;

            if (selection != null)
            {
                if (!selection.TryGetComponent<VehicleSetup>(out var vs))
                {
                    Undo.AddComponent(selection, typeof(VehicleSetup));
                    EditorHelper.SelectObject(selection);
                }
            }
            else
            {
                Logger.Log("Please select the vehicle in the hierarchy!");
            }
        }

        [MenuItem(DriverSetupPath, false, 102)]
        public static void CreateDriverSetupWizard()
        {
            var selection = Selection.activeGameObject;

            if (selection != null)
            {
                var rigBuilder = selection.GetOrAddComponent<UnityEngine.Animations.Rigging.RigBuilder>();
                var rig = CreatePrefab(RGSKEditorSettings.Instance.vehicleDriverRigTemplate, selection.transform).GetComponent<UnityEngine.Animations.Rigging.Rig>();
                rigBuilder.layers.Add(new UnityEngine.Animations.Rigging.RigLayer(rig));

                var driver = selection.GetOrAddComponent<VehicleDriver>();
                driver.leftHand = rig.transform.Find("LeftHandIK").GetComponent<UnityEngine.Animations.Rigging.TwoBoneIKConstraint>();
                driver.rightHand = rig.transform.Find("RightHandIK").GetComponent<UnityEngine.Animations.Rigging.TwoBoneIKConstraint>();
                driver.leftFoot = rig.transform.Find("LeftFootIK").GetComponent<UnityEngine.Animations.Rigging.TwoBoneIKConstraint>();
                driver.rightFoot = rig.transform.Find("RightFootIK").GetComponent<UnityEngine.Animations.Rigging.TwoBoneIKConstraint>();
                driver.head = rig.transform.Find("HeadAim").GetComponent<UnityEngine.Animations.Rigging.MultiAimConstraint>();
            }
            else
            {
                Logger.Log("Please select the driver in the hierarchy!");
            }
        }

        public static void AddProjectSettingsEssentials()
        {
            var file = "ProjectSettings_Essentials.unitypackage";
            var path = $"{UnityPackagesPath}{file}";

            if (File.Exists(path))
            {
                AssetDatabase.ImportPackage(path, true);
            }
            else
            {
                Logger.LogWarning($"Failed to find the package! Please locate \"{file}\" in the project and import it manually via Assets > Import Package > Custom Package.");
            }
        }

        public static void AddMobileURPAssets()
        {
            var file = "URPSettings_Mobile.unitypackage";
            var path = $"{UnityPackagesPath}{file}";

            if (File.Exists(path))
            {
                AssetDatabase.ImportPackage(path, true);
            }
            else
            {
                Logger.LogWarning($"Failed to find the package! Please locate \"{file}\" in the project and import it manually via Assets > Import Package > Custom Package.");
            }
        }

        public static void AddTMPEssentialResources()
        {
            var path = "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage";

#if UNITY_6000_0_OR_NEWER
            path = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";
#endif

            if (File.Exists(path))
            {
                AssetDatabase.ImportPackage(path, true);
            }
            else
            {
                Logger.LogWarning("Failed to find the TMP Essential Resources package! Please import it manually via Window > TextMeshPro > Import TMP Essential Resources.");
            }
        }

        public static void AddDemoScenesToBuilds()
        {
            if (!DisplayDialog("", $"Proceeding will add all demo scenes to the build settings.",
                        "Continue", "Cancel"))
            {
                return;
            }

            var pathList = new List<string>();

            foreach (var p in RGSKEditorSettings.Instance.demoScenes)
            {
                pathList.Add(p.ScenePath);
            }

            if (pathList.Count > 0)
            {
                var sceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

                foreach (var path in pathList)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        var index = SceneUtility.GetBuildIndexByScenePath(path);

                        if (index < 0)
                        {
                            sceneList.Add(new EditorBuildSettingsScene(path, true));
                        }
                    }
                }

                EditorBuildSettings.scenes = sceneList.ToArray();

                if (!DisplayDialog("", $"Demo scenes were added to your project's build settings.", "OK", "Open Demo Hub"))
                {
                    EditorSceneManager.OpenScene(pathList[0]);
                }
            }
            else
            {
                DisplayDialog("", "Demo scenes could not be found! Please consider reimporting the asset.", "OK");
            }
        }

        public static GameObject CreatePrefab(GameObject template, Transform parent,
                                            Vector3? pos = null, Quaternion? rot = null,
                                            bool prefab = false, bool ping = true)
        {
            var t = !prefab ? Instantiate(template) : (GameObject)PrefabUtility.InstantiatePrefab(template);
            t.name = t.name.Replace("(Clone)", "").Trim();
            t.transform.SetParent(parent, false);

            if (pos.HasValue)
            {
                t.transform.position = pos.Value;
            }

            if (rot.HasValue)
            {
                t.transform.rotation = rot.Value;
            }

            if (rot != null)
            {

            }

            if (ping)
            {
                EditorHelper.SelectObject(t, true);
            }

            return t;
        }

        public static void SelectObject(Object obj, bool ping = true)
        {
            if (obj != null)
            {
                Selection.activeObject = obj;

                if (ping)
                {
                    EditorGUIUtility.PingObject(obj);
                }
            }
        }

        public static void DrawAddToListUI<T>(List<T> list, T obj)
        {
            var added = list.Contains(obj);
            var status = added ? $"In Global [Index: {list.IndexOf(obj)}]" : "Not In Global";

            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    var content = new GUIContent();
                    content.text = status;
                    content.image = !added ? CustomEditorStyles.RedIconContent.image :
                                             CustomEditorStyles.GreenIconContent.image;

                    EditorGUILayout.LabelField(content);

                    if (added)
                    {
                        if (GUILayout.Button("Remove"))
                        {
                            list.Remove(obj);
                            MarkSettingsAsDirty();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Add"))
                        {
                            list.Add(obj);
                            MarkSettingsAsDirty();
                        }
                    }
                }
            }
        }

        public static void DrawIntegrationUI(string name, string symbol, bool added)
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    var content = new GUIContent();
                    content.text = name;
                    content.image = !added ? CustomEditorStyles.RedIconContent.image :
                                             CustomEditorStyles.GreenIconContent.image;

                    EditorGUILayout.LabelField(content);

                    EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                    {
                        if (!added)
                        {
                            if (GUILayout.Button("Add", GUILayout.MinWidth(200), GUILayout.MaxWidth(200)))
                            {
                                if (DisplayDialog("", $"Add support for {name}?",
                        "Yes", "No"))
                                {
                                    AddDefineSymbol(symbol);
                                }
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Remove", GUILayout.MinWidth(200), GUILayout.MaxWidth(200)))
                            {
                                if (DisplayDialog("", $"Remove support for {name}?",
                        "Yes", "No"))
                                {
                                    RemoveDefineSymbol(symbol);
                                }
                            }
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
        }

        public static void DrawItemUI(Sprite tex, string title, string description, string button1 = "", string button2 = "", System.Action onButton1Click = null, System.Action onButton2Click = null)
        {
            using (new GUILayout.VerticalScope("Box", GUILayout.Width(300), GUILayout.MinHeight(200)))
            {
                GUILayout.Label(tex ? tex.texture : CustomEditorStyles.NullIconContent.image, CustomEditorStyles.MenuLabelCenter, GUILayout.MinWidth(150), GUILayout.MinHeight(150));
                EditorGUILayout.LabelField($"<b>{title}</b>", CustomEditorStyles.MenuLabelLeft);
                EditorGUILayout.LabelField(description, CustomEditorStyles.MenuDescription);

                using (new GUILayout.HorizontalScope())
                {
                    if (!string.IsNullOrWhiteSpace(button1))
                    {
                        if (GUILayout.Button(button1))
                        {
                            onButton1Click?.Invoke();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(button2))
                    {
                        if (GUILayout.Button(button2))
                        {
                            onButton2Click?.Invoke();
                        }
                    }
                }
            }
        }

        public static void DrawHeader(float width1, float width2, bool showVersion = true)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                var header = RGSKEditorSettings.Instance.welcomeHeader;

                if (header != null)
                {
                    var aspect = (float)header.width / header.height;
                    var texWidth = width1;
                    var texHeight = texWidth / aspect;
                    GUILayout.Label(header, CustomEditorStyles.MenuLabelCenter,
                                    GUILayout.Width(texWidth),
                                    GUILayout.Height(texHeight),
                                    GUILayout.Width(width2),
                                    GUILayout.MaxHeight(200));
                }

                if (showVersion)
                {
                    EditorGUILayout.LabelField($"v{RGSKCore.Instance.versionNumber}", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        public static bool DisplayDialog(string title, string message, string ok, string cancel = "")
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "RGSK";
            }

            return string.IsNullOrWhiteSpace(cancel) ?
             EditorUtility.DisplayDialog(title, message, ok) :
             EditorUtility.DisplayDialog(title, message, ok, cancel);
        }

        public static void MarkPrefabAsDirty()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }

        public static void MarkSettingsAsDirty()
        {
            EditorUtility.SetDirty(RGSKCore.Instance);
            EditorUtility.SetDirty(RGSKCore.Instance.GeneralSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.SceneSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.ContentSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.RaceSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.AISettings);
            EditorUtility.SetDirty(RGSKCore.Instance.UISettings);
            EditorUtility.SetDirty(RGSKCore.Instance.AudioSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.InputSettings);
            EditorUtility.SetDirty(RGSKCore.Instance.VehicleSettings);
        }

        public static void SaveRGSKCoreSettings()
        {
            SaveScriptableObject((RGSKGeneralSettings)RGSKCore.Instance.GeneralSettings, "GeneralSettings");
            SaveScriptableObject((RGSKSceneSettings)RGSKCore.Instance.SceneSettings, "SceneSettings");
            SaveScriptableObject((RGSKContentSettings)RGSKCore.Instance.ContentSettings, "ContentSettings");
            SaveScriptableObject((RGSKRaceSettings)RGSKCore.Instance.RaceSettings, "RaceSettings");
            SaveScriptableObject((RGSKAISettings)RGSKCore.Instance.AISettings, "AISettings");
            SaveScriptableObject((RGSKUISettings)RGSKCore.Instance.UISettings, "UISettings");
            SaveScriptableObject((RGSKAudioSettings)RGSKCore.Instance.AudioSettings, "AudioSettings");
            SaveScriptableObject((RGSKInputSettings)RGSKCore.Instance.InputSettings, "InputSettings");
            SaveScriptableObject((RGSKVehicleSettings)RGSKCore.Instance.VehicleSettings, "VehicleSettings");
            MarkSettingsAsDirty();
        }

        public static void LoadRGSKCoreSettings()
        {
            RGSKCore.Instance.GeneralSettings = (RGSKGeneralSettings)LoadScriptableObject("GeneralSettings");
            RGSKCore.Instance.SceneSettings = (RGSKSceneSettings)LoadScriptableObject("SceneSettings");
            RGSKCore.Instance.ContentSettings = (RGSKContentSettings)LoadScriptableObject("ContentSettings");
            RGSKCore.Instance.RaceSettings = (RGSKRaceSettings)LoadScriptableObject("RaceSettings");
            RGSKCore.Instance.AISettings = (RGSKAISettings)LoadScriptableObject("AISettings");
            RGSKCore.Instance.UISettings = (RGSKUISettings)LoadScriptableObject("UISettings");
            RGSKCore.Instance.AudioSettings = (RGSKAudioSettings)LoadScriptableObject("AudioSettings");
            RGSKCore.Instance.InputSettings = (RGSKInputSettings)LoadScriptableObject("InputSettings");
            RGSKCore.Instance.VehicleSettings = (RGSKVehicleSettings)LoadScriptableObject("VehicleSettings");
            MarkSettingsAsDirty();
        }

        public static void LoadUIScreenIDReferences()
        {
            var guids = AssetDatabase.FindAssets("t:UIScreenID");

            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var screenID = AssetDatabase.LoadAssetAtPath<UIScreenID>(path);

                if (screenID != null)
                {
                    var screenPrefab = LoadAssetReference<UIScreen>(screenID.name);
                    var screenPrefabMobile = LoadAssetReference<UIScreen>($"{screenID.name}_mobile");

                    if (screenPrefab != null)
                    {
                        screenID.screenPrefab = screenPrefab;
                        EditorUtility.SetDirty(screenID);
                    }

                    if (screenPrefabMobile != null)
                    {
                        screenID.screenPrefabMobile = screenPrefabMobile;
                        EditorUtility.SetDirty(screenID);
                    }
                }
            }
        }

        public static void OpenSaveFileDirectory()
        {
            if(Directory.Exists(GeneralHelper.GetProfilesDirectory()))
            {
                Application.OpenURL(GeneralHelper.GetProfilesDirectory());
            }
        }

        public static void CheckForDuplicatePersistentManagers()
        {
            if (RGSKCore.Instance.GeneralSettings.persistentObjects.Contains(RGSKCore.Instance.persistentManagers))
            {
                Logger.LogWarning("Persistent Managers were found in the \"Persistent Objects\" list in RGSKGeneralSettings! Persistent Managers are no longer loaded from this list and will therefore be removed.");
                RGSKCore.Instance.GeneralSettings.persistentObjects.Remove(RGSKCore.Instance.persistentManagers);
                EditorUtility.SetDirty(RGSKCore.Instance.GeneralSettings);
            }
        }

        public static void CheckIfSetupRequired()
        {
            var layer = LayerMask.LayerToName(6);
            
            if(string.IsNullOrEmpty(layer) || layer != "RGSK_Vehicle")
            {
                if (DisplayDialog("Project Setup", 
                    $"Welcome to RGSK!\n\nPlease open the RGSK Menu > Setup tab and use the \"Update Project Settings\" and \"Add TMP Essential Resources\" buttons.",
                    "Ok"))
                    {
                        RGSKWindow.ShowTab("Setup");
                    }
            }
        }

        public static void DrawLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static void DrawLabelWithinDistance(Vector3 position, string text, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text);
            }
        }

        public static void DrawLabelWithinDistance(Vector3 position, Texture2D text, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text);
            }
        }

        public static void DrawLabelWithinDistance(Vector3 position, string text, GUIStyle style, float distance = 250)
        {
            if (IsSceneViewCameraInRange(position, distance))
            {
                Handles.Label(position, text, style);
            }
        }

        public static bool IsSceneViewCameraInRange(Vector3 position, float distance)
        {
            Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
            return ((cameraPos.x >= 0) &&
            (cameraPos.x <= Camera.current.pixelWidth) &&
            (cameraPos.y >= 0) &&
            (cameraPos.y <= Camera.current.pixelHeight) &&
            (cameraPos.z > 0) &&
            (cameraPos.z < distance));
        }

        public static string SaveStartPath()
        {
            var dir = EditorPrefs.GetString(string.Format(LastSaveDirectoryKey, GetProjectName()), "Assets");

            if (!Directory.Exists(dir))
            {
                dir = "Assets";
            }

            return dir;
        }

        public static bool IsValidSavePath(string path, out string newPath)
        {
            newPath = path;

            if (newPath.Length == 0)
                return false;

            try
            {
                newPath = newPath.Substring(newPath.IndexOf("Assets/"));
            }
            catch
            {
                Logger.LogWarning("Please save the file under the 'Assets' directory!");
                return false;
            }

            var fileInfo = new FileInfo(path);
            EditorPrefs.SetString(string.Format(LastSaveDirectoryKey, GetProjectName()), fileInfo.DirectoryName);

            return true;
        }

        public static string GetProjectName()
        {
            var path = Application.dataPath;
            var dir = Directory.GetParent(path).FullName;
            return new DirectoryInfo(dir).Name;
        }

        public static ScriptableObject CloneScriptableObject(ScriptableObject so)
        {
            if (so == null)
                return null;

            var path = EditorUtility.SaveFilePanel("Save", EditorHelper.SaveStartPath(), $"{so.name}_Clone", "asset");

            if (EditorHelper.IsValidSavePath(path, out path))
            {
                var clone = ScriptableObject.Instantiate(so);

                AssetDatabase.CreateAsset(clone, path);
                AssetDatabase.SaveAssets();
                EditorHelper.SelectObject(AssetDatabase.LoadMainAssetAtPath(path));

                return clone;
            }

            return null;
        }

        public static void SaveScriptableObject(ScriptableObject so, string key)
        {
            var path = AssetDatabase.GetAssetPath(so);
            var guid = AssetDatabase.AssetPathToGUID(path);
            EditorPrefs.SetString(string.Format(SelectedSettingsKey, key, GetProjectName()), guid);
        }

        public static ScriptableObject LoadScriptableObject(string key)
        {
            var guid = EditorPrefs.GetString(string.Format(SelectedSettingsKey, key, GetProjectName()), "");

            if (!string.IsNullOrWhiteSpace(guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }

            return null;
        }

        public static void SaveAssetReference(Object obj, string key)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            EditorPrefs.SetString(string.Format(AssetPathKey, key, GetProjectName()), guid);
        }

        public static T LoadAssetReference<T>(string key) where T : Object
        {
            var guid = EditorPrefs.GetString(string.Format(AssetPathKey, key, GetProjectName()), "");

            if (!string.IsNullOrWhiteSpace(guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return null;
        }

        public static Texture CreateEmptyTexture(Color col, int width = 25, int height = 25)
        {
            var tex = new Texture2D(width, height);
            var pixels = tex.GetPixels();

            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = col;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        public static void RenameChildren(GameObject parent)
        {
            if (parent.TryGetComponent<ChildRename>(out var r))
            {
                r.Rename();
            }
        }

        public static bool CheckNamespacesExists(string namepsace)
        {
            HashSet<string> existingIdentifiers = new HashSet<string>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                System.Reflection.Assembly assembly = assemblies[i];

                System.Type[] types = assembly.GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    System.Type type = types[j];
                    var typeNamespace = type.Namespace;

                    if (namepsace == typeNamespace)
                    {
                        if (!existingIdentifiers.Contains(typeNamespace))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsPackageInManifest(string packageName)
        {
            var path = Path.Combine(Application.dataPath, "../Packages/manifest.json");

            if (File.Exists(path))
            {
                return File.ReadAllText(path).Contains($"\"{packageName}\"");
            }

            return false;
        }

        public static void AddDefineSymbol(string symbol)
        {
            var group = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var definesString = PlayerSettings.GetScriptingDefineSymbols(group);
            var allDefines = definesString.Split(';').ToList();

            if (allDefines.Contains(symbol))
                return;

            allDefines.Add(symbol);
            PlayerSettings.SetScriptingDefineSymbols(group, string.Join(";", allDefines.ToArray()));
        }

        public static void RemoveDefineSymbol(string symbol)
        {
            var group = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var definesString = PlayerSettings.GetScriptingDefineSymbols(group);
            var allDefines = definesString.Split(';').ToList();

            if (!allDefines.Contains(symbol))
                return;

            allDefines.Remove(symbol);
            PlayerSettings.SetScriptingDefineSymbols(group, string.Join(";", allDefines.ToArray()));
        }

        public static bool EVPSupport()
        {
#if EVP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool RCCSupport()
        {
#if RCC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool RCCProSupport()
        {
#if RCC_PRO_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool VPPSupport()
        {
#if VPP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool NWH2Support()
        {
#if NWH2_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool HSCSupport()
        {
#if HSC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool ASHVPSupport()
        {
#if ASHVP_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool UVCSupport()
        {
#if UVC_SUPPORT
            return true;
#else
            return false;
#endif
        }

        public static bool SCVPSupport()
        {
#if SCVP_SUPPORT
            return true;
#else
            return false;
#endif
        }
    }
}