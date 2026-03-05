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

public class RCCP_DemoAssetsInitLoad {

    [InitializeOnLoadMethod]
    public static void InitOnLoad() {

        EditorApplication.delayCall += CheckSymbols;

    }

    public static void CheckSymbols() {

        bool hasKey = false;

#if RCCP_DEMO
        hasKey = true;
#endif

        if (!hasKey) {

            RCCP_SetScriptingSymbol.SetEnabled("RCCP_DEMO", true);

            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Demo Assets", "Demo assets have been imported successfully. You can add them to your build settings from welcome window (Tools --> BCG --> RCCP --> Welcome Window).\n\nRemember that, this will increase your build size even if you don't even use any of them. You can always remove demo assets from the project at welcome window.", "Close");
            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Demo Scenes", "Demo Scenes have been imported successfully. You can add them to your build settings from welcome window.", "Close");

            EditorApplication.delayCall += () => {

                RCCP_Installation.CheckAllLayers();

            };

        }

    }

}
