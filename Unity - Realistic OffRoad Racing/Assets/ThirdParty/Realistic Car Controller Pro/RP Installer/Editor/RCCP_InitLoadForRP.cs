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

public class RCCP_InitLoadForRP {

    [InitializeOnLoadMethod]
    public static void InitOnLoad() {

#if !RCCP_RP
        EditorApplication.delayCall += EditorDelayedUpdate;
#endif

    }

    public static void EditorDelayedUpdate() {

        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        EditorApplication.projectChanged += OnProjectChanged;

        if (RCCP_RPPackages.Instance.dontWarnAgain)
            return;

        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        Check();

    }

    private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj) {

        if (RCCP_RPPackages.Instance.dontWarnAgain)
            return;

        if (obj == PlayModeStateChange.EnteredEditMode)
            Check();

    }

    public static void Check() {

        if (RCCP_RPPackages.Instance.dontWarnAgain)
            return;

        if (!EditorUtility.DisplayDialog("Realistic Car Controller Pro | Setup", "Realistic Car Controller Pro will be installed on the next step.", "Proceed", "Cancel Install")) {

            EditorUtility.DisplayDialog("Realistic Car Controller Pro | Not Installed Yet", "RCCP is not installed yet, because you've clicked the cancel button. You can manually import Builtin, URP or HDRP version of RCCP to your project. Packages are located in the Realistic Car Controller Pro/RP Installer.", "Dismiss");

            RCCP_RPPackages.Instance.dontWarnAgain = true;
            EditorUtility.SetDirty(RCCP_RPPackages.Instance);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SessionState.EraseBool("RCCP_INSTALLINGRP");

            return;

        }

        bool installedRP = false;

#if RCCP_RP
        installedRP = true;
#endif

        if (!installedRP) {

            int decision = EditorUtility.DisplayDialogComplex("Realistic Car Controller Pro | Select Render Pipeline", "Which render pipeline will be imported?\n\nThis process is irreversible, once you select the render pipeline, compatible version of RCCP will be imported. Switching between render pipelines is not supported.\n\nIf you want to change the render pipeline after this step, you'll need to delete ''Realistic Car Controller Pro'' folder from the project and import the new render pipeline. Be sure your project has proper configuration setup for the selected render pipeline. ", "Import [Universal Render Pipeline] (URP)", "Import [Builtin Render Pipeline] (Standard)", "Import [High Definition Render Pipeline] (HDRP)");

            if (decision == 0) {

                SessionState.SetBool("RCCP_INSTALLINGRP", true);
                AssetDatabase.ImportPackage(RCCP_RPPackages.Instance.GetAssetPath(RCCP_RPPackages.Instance.URP), true);

            }

            if (decision == 2) {

                SessionState.SetBool("RCCP_INSTALLINGRP", true);
                AssetDatabase.ImportPackage(RCCP_RPPackages.Instance.GetAssetPath(RCCP_RPPackages.Instance.HDRP), true);

            }

            if (decision == 1) {

                SessionState.SetBool("RCCP_INSTALLINGRP", true);
                AssetDatabase.ImportPackage(RCCP_RPPackages.Instance.GetAssetPath(RCCP_RPPackages.Instance.BUILTIN), true);

            }

            RCCP_RPPackages.Instance.dontWarnAgain = true;

            EditorUtility.SetDirty(RCCP_RPPackages.Instance);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

    }

    public static void OnProjectChanged() {

        if (SessionState.GetBool("RCCP_INSTALLINGRP", false)) {

            SessionState.EraseBool("RCCP_INSTALLINGRP");
            RCCP_SetScriptingSymbolForRP.SetEnabled("RCCP_RP", true);

        }

    }

}
