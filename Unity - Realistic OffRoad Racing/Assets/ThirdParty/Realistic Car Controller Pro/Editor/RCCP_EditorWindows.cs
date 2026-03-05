//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class RCCP_EditorWindows : Editor {

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Edit RCCP Settings", false, -100)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Edit RCCP Settings", false, -100)]
    public static void OpenRCCSettings() {
        Selection.activeObject = RCCP_Settings.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Add Main Controller To Vehicle", false, -85)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Add Main Controller To Vehicle", false, -85)]
    public static void AddMainControllerToVehicle() {

        if (Selection.activeGameObject != null) {

            if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.scene.name != null && !EditorUtility.IsPersistent(Selection.activeGameObject))
                RCCP_CreateNewVehicle.NewVehicle(Selection.activeGameObject);
            else
                EditorUtility.DisplayDialog("Realistic Car Controller Pro | Selection", "Please select only one vehicle in the scene. Be sure to select root of the vehicle gameobject before adding the main controller", "Care to try again?", "Yesn't");

        }

    }

    #region Configure

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Ground Materials", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Ground Materials", false, -65)]
    public static void OpenGroundMaterialsSettings() {
        Selection.activeObject = RCCP_GroundMaterials.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Changable Wheels", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Changable Wheels", false, -65)]
    public static void OpenChangableWheelSettings() {
        Selection.activeObject = RCCP_ChangableWheels.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Recorded Clips", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Recorded Clips", false, -65)]
    public static void OpenRecordSettings() {
        Selection.activeObject = RCCP_Records.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo/Demo Vehicles", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo/Demo Vehicles", false, -65)]
    public static void OpenDemoVehiclesSettings() {
        Selection.activeObject = RCCP_DemoVehicles.Instance;
    }

#if RCCP_PHOTON && PHOTON_UNITY_NETWORKING
    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo Vehicles (Photon)", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo Vehicles (Photon)", false, -65)]
    public static void OpenPhotonDemoVehiclesSettings() {
        Selection.activeObject = RCCP_DemoVehicles_Photon.Instance;
    }
#endif

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo/Demo Scenes", false, -65)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Configure/Demo/Demo Scenes", false, -65)]
    public static void OpenDemoScenesSettings() {
        Selection.activeObject = RCCP_DemoScenes.Instance;
    }

    #endregion

    #region Managers

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Scene Manager To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Scene Manager To Scene", false, -50)]
    public static void CreateRCCPSceneManager() {
        Selection.activeObject = RCCP_SceneManager.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Skidmarks Manager To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Skidmarks Manager To Scene", false, -50)]
    public static void CreateRCCPSkidmarksManager() {
        Selection.activeObject = RCCP_SkidmarksManager.Instance;
    }

    #endregion

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add RCCP Camera To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add RCCP Camera To Scene", false, -50)]
    public static void CreateRCCCamera() {

        if (FindFirstObjectByType<RCCP_Camera>(FindObjectsInactive.Include)) {

            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Scene has RCCP Camera already!", "Scene has RCCP Camera already!", "Close");
            Selection.activeGameObject = FindFirstObjectByType<RCCP_Camera>(FindObjectsInactive.Include).gameObject;

        } else {

            GameObject cam = Instantiate(RCCP_Settings.Instance.RCCPMainCamera.gameObject);
            cam.name = RCCP_Settings.Instance.RCCPMainCamera.name;
            Selection.activeGameObject = cam.gameObject;

        }

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add RCCP UI Canvas To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add RCCP UI Canvas To Scene", false, -50)]
    public static void CreateRCCUICanvas() {

        if (FindFirstObjectByType<RCCP_UIManager>(FindObjectsInactive.Include)) {

            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Scene has RCCP UI Canvas already!", "Scene has RCCP UI Canvas already!", "Close");
            Selection.activeGameObject = FindFirstObjectByType<RCCP_UIManager>(FindObjectsInactive.Include).gameObject;

        } else {

            GameObject cam = Instantiate(RCCP_Settings.Instance.RCCPCanvas.gameObject);
            cam.name = RCCP_Settings.Instance.RCCPCanvas.name;
            Selection.activeGameObject = cam.gameObject;

        }

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add AI Waypoints Container To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add AI Waypoints Container To Scene", false, -50)]
    public static void CreateRCCAIWaypointManager() {

        GameObject wpContainer = new GameObject("RCCP_AI_WaypointsContainer");
        wpContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        wpContainer.AddComponent<RCCP_AIWaypointsContainer>();
        Selection.activeGameObject = wpContainer;

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add AI Brake Zones Container To Scene", false, -50)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Scene/Add AI Brake Zones Container To Scene", false, -50)]
    public static void CreateRCCAIBrakeManager() {

        GameObject bzContainer = new GameObject("RCCP_AI_BrakeZonesContainer");
        bzContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        bzContainer.AddComponent<RCCP_AIBrakeZonesContainer>();
        Selection.activeGameObject = bzContainer;

    }

    #region Help
    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Help", false, 0)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Help", false, 0)]
    public static void Help() {

        EditorUtility.DisplayDialog("Realistic Car Controller Pro | Contact", "Please include your invoice number while sending a contact form. I usually respond within a business day.", "Close");

        string url = "http://www.bonecrackergames.com/contact/";
        Application.OpenURL(url);

    }
    #endregion Help

    //    #region Logitech
    //#if RCC_LOGITECH
    //	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/Create/Logitech/Logitech Manager", false, -50)]
    //	public static void CreateLogitech() {

    //		RCC_LogitechSteeringWheel logi = RCC_LogitechSteeringWheel.Instance;
    //		Selection.activeGameObject = logi.gameObject;

    //	}
    //#endif
    //    #endregion

    //[MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Export Project Settings", false, 10)]
    //public static void ExportProjectSettings() {

    //    string[] projectContent = new string[] { "ProjectSettings/InputManager.asset" };
    //    AssetDatabase.ExportPackage(projectContent, "RCCP_ProjectSettings.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
    //    Debug.Log("Project Exported");

    //}

}
