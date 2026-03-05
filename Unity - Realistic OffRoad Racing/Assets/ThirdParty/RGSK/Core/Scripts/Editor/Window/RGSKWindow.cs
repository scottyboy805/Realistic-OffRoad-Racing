using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK.Editor
{
    [InitializeOnLoad]
    public class RGSKWindow : EditorWindow, IHasCustomMenu
    {
        static string[] tabs = new string[]
        {
            "Welcome",
            "Setup",
            "General",
            "Scene",
            "Content",
            "Race",
            "AI",
            "UI",
            "Audio",
            "Input",
            "Vehicle",
            "Integrations"
        };

        static int tabIndex;
        int tabSize = 250;
        Vector2 scrollPosition;
        int supportButtonSize = 30;

        string[] contentTabs = new string[]
        {
            "Vehicles",
            "Tracks"
        };
        static int contentTabIndex;
        Vector2 contentTabScrollPosition;

        SerializedObject serializedCore;
        SerializedProperty generalSettings;
        SerializedProperty sceneSettings;
        SerializedProperty contentSettings;
        SerializedProperty raceSettings;
        SerializedProperty aiSettings;
        SerializedProperty uiSettings;
        SerializedProperty audioSettings;
        SerializedProperty inputSettings;
        SerializedProperty vehicleSettings;

        static RGSKWindow()
        {
            EditorApplication.delayCall += () =>
            {
                EditorHelper.LoadRGSKCoreSettings();
                EditorHelper.LoadUIScreenIDReferences();
                EditorHelper.CheckForDuplicatePersistentManagers();
                EditorHelper.CheckIfSetupRequired();
            };
        }

        void OnEnable()
        {
            if (serializedCore == null)
            {
                serializedCore = new SerializedObject(RGSKCore.Instance);
                generalSettings = serializedCore.FindProperty(nameof(generalSettings));
                sceneSettings = serializedCore.FindProperty(nameof(sceneSettings));
                contentSettings = serializedCore.FindProperty(nameof(contentSettings));
                raceSettings = serializedCore.FindProperty(nameof(raceSettings));
                aiSettings = serializedCore.FindProperty(nameof(aiSettings));
                uiSettings = serializedCore.FindProperty(nameof(uiSettings));
                audioSettings = serializedCore.FindProperty(nameof(audioSettings));
                inputSettings = serializedCore.FindProperty(nameof(inputSettings));
                vehicleSettings = serializedCore.FindProperty(nameof(vehicleSettings));
            }

            EditorHelper.LoadRGSKCoreSettings();
        }

        void OnFocus()
        {
            EditorHelper.LoadRGSKCoreSettings();
        }

        public void AddItemsToMenu(UnityEditor.GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh"), false, () => EditorHelper.LoadRGSKCoreSettings());
            menu.AddItem(new GUIContent("Open Save File Directory"), false, () => EditorHelper.OpenSaveFileDirectory());
        }

        [MenuItem("Window/RGSK/Menu", false, 2500)]
        public static void ShowWindow()
        {
            var window = GetWindow<RGSKWindow>();
            window.titleContent = new GUIContent("RGSK", CustomEditorStyles.MenuIconContent.image);
            window.Show();
        }

        public static void ShowTab(string tabName)
        {
            tabIndex = tabs.ToList().IndexOf(tabName);
            
            if(tabIndex < 0)
            {
                tabIndex = 0;
            }

            ShowWindow();
        }

        void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope("Box", GUILayout.MaxWidth(tabSize), GUILayout.ExpandHeight(true)))
                {
                    tabIndex = GUILayout.SelectionGrid(tabIndex, tabs, 1, CustomEditorStyles.VerticalToolbarButton);
                }

                serializedCore?.Update();

                using (new GUILayout.VerticalScope())
                {
                    using (var scope = new GUILayout.ScrollViewScope(scrollPosition, false, false))
                    {
                        scrollPosition = scope.scrollPosition;
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField(tabs[tabIndex], CustomEditorStyles.TitleLabel, GUILayout.Height(25));
                        EditorHelper.DrawLine();

                        switch (tabs[tabIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture))
                        {
                            case "welcome":
                                {
                                    DrawWelcomeUI();
                                    break;
                                }

                            case "setup":
                                {
                                    DrawSetupUI();
                                    break;
                                }

                            case "general":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: generalSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.GeneralSettings =
                                        (RGSKGeneralSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKGeneralSettings)generalSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.GeneralSettings = (RGSKGeneralSettings)generalSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "scene":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: sceneSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.SceneSettings =
                                        (RGSKSceneSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKSceneSettings)sceneSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.SceneSettings = (RGSKSceneSettings)sceneSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "content":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: contentSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.ContentSettings =
                                        (RGSKContentSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKContentSettings)contentSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.ContentSettings = (RGSKContentSettings)contentSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    GUILayout.Space(5);
                                    using (new GUILayout.VerticalScope())
                                    {
                                        using (var s = new GUILayout.ScrollViewScope(contentTabScrollPosition, false, false))
                                        {
                                            contentTabScrollPosition = s.scrollPosition;
                                            contentTabIndex = GUILayout.Toolbar(contentTabIndex, contentTabs, CustomEditorStyles.HorizontalToolbarButton);
                                            DrawContentItems(contentTabIndex == 0);
                                        }
                                    }

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "race":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: raceSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.RaceSettings =
                                        (RGSKRaceSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKRaceSettings)raceSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.RaceSettings = (RGSKRaceSettings)raceSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "ai":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: aiSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.AISettings =
                                        (RGSKAISettings)EditorHelper.CloneScriptableObject
                                        ((RGSKAISettings)aiSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.AISettings = (RGSKAISettings)aiSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "ui":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: uiSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.UISettings =
                                        (RGSKUISettings)EditorHelper.CloneScriptableObject
                                        ((RGSKUISettings)uiSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.UISettings = (RGSKUISettings)uiSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "audio":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: audioSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.AudioSettings =
                                        (RGSKAudioSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKAudioSettings)audioSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.AudioSettings = (RGSKAudioSettings)audioSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "input":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: inputSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.InputSettings =
                                        (RGSKInputSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKInputSettings)inputSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.InputSettings = (RGSKInputSettings)inputSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "vehicle":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: vehicleSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.VehicleSettings =
                                        (RGSKVehicleSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKVehicleSettings)vehicleSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.VehicleSettings = (RGSKVehicleSettings)vehicleSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "integrations":
                                {
                                    DrawIntegrationsUI();
                                    break;
                                }
                        }
                    }
                }
            }

            serializedCore?.ApplyModifiedProperties();
        }

        void DrawWelcomeUI()
        {
            EditorHelper.DrawHeader(position.width - 250, position.width - 300);
            GUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                if (GUILayout.Button("Online Documentation", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.DocumentationLink);
                }

                if (GUILayout.Button("Discord", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.DiscordLink);
                }

                if (GUILayout.Button("YouTube", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.YouTubeLink);
                }

                if (GUILayout.Button("Unity Forums", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.UnityForumsLink);
                }

                if (GUILayout.Button("Asset Store", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.AssetStoreLink);
                }
            }

            GUILayout.BeginArea(new Rect(position.width - (250 + 165), position.height - 25, 150, 50));
            {
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Check For Updates"))
                    {
                        UpdateCheckerWindow.ShowWindow();
                    }
                }

                GUILayout.EndArea();
            }
        }

        void DrawSetupUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Essential Project Settings", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add the required project settings to the project.", MessageType.Info);

                if (GUILayout.Button("Update Project Settings", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddProjectSettingsEssentials();
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("TextMesh Pro", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add the TMP Essential Resources to the project, which is crucial for UI to work.", MessageType.Info);

                if (GUILayout.Button("Add TMP Essential Resources", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddTMPEssentialResources();
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Demo Scenes", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add all demo scenes to the build settings.", MessageType.Info);

                if (GUILayout.Button("Add Demos to Build Settings", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddDemoScenesToBuilds();
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Mobile URP Assets", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add mobile-ready URP assets to the project quality settings, useful when targeting mobile platforms.", MessageType.Info);

                if (GUILayout.Button("Mobile URP Assets", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddMobileURPAssets();
                }
            }
        }

        void DrawIntegrationsUI()
        {
            EditorGUILayout.HelpBox("Please ensure that the asset is already imported before adding support for it!", MessageType.Info);
            EditorHelper.DrawIntegrationUI("Edy's Vehicle Physics", "EVP_SUPPORT", EditorHelper.EVPSupport());
            EditorHelper.DrawIntegrationUI("Realistic Car Controller", "RCC_SUPPORT", EditorHelper.RCCSupport());
            EditorHelper.DrawIntegrationUI("Realistic Car Controller Pro", "RCC_PRO_SUPPORT", EditorHelper.RCCProSupport());
            EditorHelper.DrawIntegrationUI("Vehicle Physics Pro - Community Edition", "VPP_SUPPORT", EditorHelper.VPPSupport());
            EditorHelper.DrawIntegrationUI("NWH Vehicle Physics 2", "NWH2_SUPPORT", EditorHelper.NWH2Support());
            EditorHelper.DrawIntegrationUI("Highroad Solid Controller", "HSC_SUPPORT", EditorHelper.HSCSupport());
            EditorHelper.DrawIntegrationUI("Ash Vehicle Physics", "ASHVP_SUPPORT", EditorHelper.ASHVPSupport());
            EditorHelper.DrawIntegrationUI("Universal Vehicle Controller", "UVC_SUPPORT", EditorHelper.UVCSupport());
            EditorHelper.DrawIntegrationUI("Sim-Cade Vehicle Physics", "SCVP_SUPPORT", EditorHelper.SCVPSupport());
        }

        void DrawSettingsLayout(SerializedProperty prop, Action onClone, Action onChange = null)
        {
            var referenceValue = prop.objectReferenceValue;
            var cloneButtonRect = new Rect((position.width - tabSize) - 110, EditorGUIUtility.singleLineHeight * 3.15f, 100, EditorGUIUtility.singleLineHeight);
            var settingsWidth = (position.width - tabSize) - 120;

            EditorGUILayout.PropertyField(prop, new GUIContent("Settings"), GUILayout.Width(settingsWidth));

            if (referenceValue != null)
            {
                if (GUI.Button(cloneButtonRect, "Clone", EditorStyles.miniButton))
                {
                    onClone?.Invoke();
                }
            }

            if (referenceValue != prop.objectReferenceValue)
            {
                onChange?.Invoke();
            }
        }

        void DrawContentItems(bool vehicles)
        {
            HandleContentSorting(vehicles);
            GUILayout.Space(30);

            var content = RGSKCore.Instance.ContentSettings;
            var max = vehicles ? content.vehicles.Count : content.tracks.Count;
            var columnCount = (position.width - (tabSize * 1.25f)) / 350;

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    var i = 0;
                    while (i < max)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            for (int j = 0; j < columnCount; j++)
                            {
                                if (i >= max)
                                    break;

                                if (vehicles)
                                {
                                    var item = content.vehicles[i];
                                    if (item != null)
                                    {
                                        EditorHelper.DrawItemUI(
                                            item.previewPhoto,
                                            UIHelper.FormatVehicleNameText(item),
                                            $"Unlock Mode: {item.unlockMode.ToString().InsertSpacesBeforeCapitals()}\n{GetUnlockText(item)}Class: {item.GetVehicleClassName()}\nManufacturer: {item.GetVehicleManufacturerName()}",
                                            "Select",
                                            "Remove",
                                            () => EditorHelper.SelectObject(item),
                                            () =>
                                            {
                                                if (EditorHelper.DisplayDialog("",
                                                    $"Are you sure you want to remove this vehicle from the list?",
                                                    "Yes", "No"))
                                                {

                                                    content.vehicles.RemoveAt(i);
                                                    EditorHelper.MarkSettingsAsDirty();
                                                    i = max;
                                                }
                                            });
                                    }
                                }
                                else
                                {
                                    var item = content.tracks[i];
                                    if (item != null)
                                    {
                                        EditorHelper.DrawItemUI(
                                            item.previewPhoto,
                                            item.objectName,
                                            $"Unlock Mode: {item.unlockMode.ToString().InsertSpacesBeforeCapitals()}\n{GetUnlockText(item)}Layout: {item.layoutType}\nLength: {UIHelper.FormatDistanceText(item.length)}\nGrid Size: {item.gridSlots}",
                                            "Select",
                                            "Remove",
                                            () => EditorHelper.SelectObject(item),
                                            () =>
                                            {
                                                if (EditorHelper.DisplayDialog("",
                                                    $"Are you sure you want to remove this track from the list?",
                                                    "Yes", "No"))
                                                {

                                                    content.tracks.RemoveAt(i);
                                                    EditorHelper.MarkSettingsAsDirty();
                                                    i = max;
                                                }
                                            });
                                    }
                                }

                                i++;
                            }

                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }

            string GetUnlockText(ItemDefinition item)
            {
                var price = item.unlockMode == ItemUnlockMode.Purchase ?
                            UIHelper.FormatCurrencyText(item.unlockPrice) :
                            "N/A";

                switch (item.unlockMode)
                {
                    default:
                        {
                            return $"Price: {price}\n";
                        }

                    case ItemUnlockMode.XPLevel:
                        {
                            return $"Unlock XP Level: {item.unlockXPLevel}\n";
                        }
                }
            }
        }

        void HandleContentSorting(bool vehicles)
        {
            GUILayout.BeginArea(new Rect(position.width - (300 + 225), 25, 250, 50));
            {
                using (new GUILayout.HorizontalScope())
                {
                    Undo.RecordObject(RGSKCore.Instance.ContentSettings, "changed_content_sorting_options");

                    EditorGUI.BeginChangeCheck();
                    {
                        if (vehicles)
                        {
                            RGSKCore.Instance.ContentSettings.vehicleSortOptions = (VehicleSortOptions)EditorGUILayout.EnumPopup(RGSKCore.Instance.ContentSettings.vehicleSortOptions);
                            RGSKCore.Instance.ContentSettings.vehicleSortOrder = (SortOrder)EditorGUILayout.EnumPopup(RGSKCore.Instance.ContentSettings.vehicleSortOrder);
                        }
                        else
                        {
                            RGSKCore.Instance.ContentSettings.trackSortOptions = (TrackSortOptions)EditorGUILayout.EnumPopup(RGSKCore.Instance.ContentSettings.trackSortOptions);
                            RGSKCore.Instance.ContentSettings.trackSortOrder = (SortOrder)EditorGUILayout.EnumPopup(RGSKCore.Instance.ContentSettings.trackSortOrder);
                        }

                        if (EditorGUI.EndChangeCheck() || GUILayout.Button("Refresh"))
                        {
                            if (vehicles)
                            {
                                //Vehicles
                                switch (RGSKCore.Instance.ContentSettings.vehicleSortOptions)
                                {
                                    case VehicleSortOptions.Alphabetical:
                                        {
                                            RGSKCore.Instance.ContentSettings.vehicles = RGSKCore.Instance.ContentSettings.vehicles.OrderBy(x => x.objectName).ToList();
                                            break;
                                        }

                                    case VehicleSortOptions.ClassPerformanceRating:
                                        {
                                            RGSKCore.Instance.ContentSettings.vehicles = RGSKCore.Instance.ContentSettings.vehicles.OrderBy(x => x.GetPerformanceRating()).ToList();
                                            break;
                                        }

                                    case VehicleSortOptions.TopSpeed:
                                        {
                                            RGSKCore.Instance.ContentSettings.vehicles = RGSKCore.Instance.ContentSettings.vehicles.OrderBy(x => x.defaultStats.speed).ToList();
                                            break;
                                        }

                                    case VehicleSortOptions.Price:
                                        {
                                            RGSKCore.Instance.ContentSettings.vehicles = RGSKCore.Instance.ContentSettings.vehicles.OrderBy(x => x.unlockPrice).ToList();
                                            break;
                                        }
                                }

                                if (RGSKCore.Instance.ContentSettings.vehicleSortOptions != VehicleSortOptions.None &&
                                    RGSKCore.Instance.ContentSettings.vehicleSortOrder == SortOrder.Descending)
                                {
                                    RGSKCore.Instance.ContentSettings.vehicles.Reverse();
                                }
                            }
                            else
                            {
                                //Tracks
                                switch (RGSKCore.Instance.ContentSettings.trackSortOptions)
                                {
                                    case TrackSortOptions.Alphabetical:
                                        {
                                            RGSKCore.Instance.ContentSettings.tracks = RGSKCore.Instance.ContentSettings.tracks.OrderBy(x => x.objectName).ToList();
                                            break;
                                        }

                                    case TrackSortOptions.Length:
                                        {
                                            RGSKCore.Instance.ContentSettings.tracks = RGSKCore.Instance.ContentSettings.tracks.OrderBy(x => x.length).ToList();
                                            break;
                                        }

                                    case TrackSortOptions.Price:
                                        {
                                            RGSKCore.Instance.ContentSettings.tracks = RGSKCore.Instance.ContentSettings.tracks.OrderBy(x => x.unlockPrice).ToList();
                                            break;
                                        }
                                }

                                if (RGSKCore.Instance.ContentSettings.trackSortOptions != TrackSortOptions.None &&
                                    RGSKCore.Instance.ContentSettings.trackSortOrder == SortOrder.Descending)
                                {
                                    RGSKCore.Instance.ContentSettings.tracks.Reverse();
                                }
                            }

                            EditorUtility.SetDirty(RGSKCore.Instance.ContentSettings);
                        }
                    }
                }

                GUILayout.EndArea();
            }
        }
    }
}