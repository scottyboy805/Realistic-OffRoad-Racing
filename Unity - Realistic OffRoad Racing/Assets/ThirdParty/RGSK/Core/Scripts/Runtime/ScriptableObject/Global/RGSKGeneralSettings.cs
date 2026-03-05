using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/General")]
    public class RGSKGeneralSettings : ScriptableObject
    {
        [Header("Persistent")]
        public List<GameObject> persistentObjects = new List<GameObject>();

        [Header("Runtime Sets")]
        public EntityRuntimeSet entitySet;

        [Header("Settings")]
        public CameraSettings cameraSettings;
        public DriftSettings driftSettings;
        public SlipstreamSettings slipstreamSettings;
        public CatchupSettings catchupSettings;
        public RepositionSettings repositionSettings;
        public MinimapSettings minimapSettings;
        public ProximityArrowSettings proximityArrowSettings;
        public RecorderSettings recorderSettings;
        public CountrySettings countrySettings;

        [Header("Player")]
        public ProfileDefinition playerProfile;
        public XPCurve playerXPCurve;
        public PlayerData defaultPlayerData;

        [Header("Save")]
        public string saveFileName = "profile1";
        public bool enableSaveSystem = true;
        public bool encryptSaveData = true;
        public bool saveOnApplicationQuit = true;

        [Header("Layers")]
        public LayerMaskSingle vehicleLayerIndex;
        public LayerMaskSingle ghostLayerIndex;
        public LayerMaskSingle minimapLayerIndex;

        [Header("Util")]
        public GameObject terminal;
        public bool includeTerminalInBuild;
        public bool enableFpsReader = true;
        public bool enableLogs = true;
        public bool autoCheckForUpdates = true;
    }
}