//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All RP packages.
/// </summary>
public class RCCP_RPPackages : ScriptableObject {

    #region singleton
    private static RCCP_RPPackages instance;
    public static RCCP_RPPackages Instance { get { if (instance == null) instance = Resources.Load("RCCP_RPPackages") as RCCP_RPPackages; return instance; } }
    #endregion

    public Object URP;
    public Object HDRP;
    public Object BUILTIN;

    public bool dontWarnAgain = false;

    public string GetAssetPath(Object pathObject) {

        string path = UnityEditor.AssetDatabase.GetAssetPath(pathObject);
        return path;

    }

}
