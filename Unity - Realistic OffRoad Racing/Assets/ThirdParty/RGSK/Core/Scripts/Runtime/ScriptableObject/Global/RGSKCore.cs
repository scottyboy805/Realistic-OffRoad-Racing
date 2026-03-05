using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RuntimeData
    {
        public RaceSession SelectedSession;
        public TrackDefinition SelectedTrack;
        public VehicleDefinition SelectedVehicle;
        public Color SelectedVehicleColor;
        public Texture2D SelectedVehicleLivery;
    }

    //[CreateAssetMenu(menuName = "RGSK/RGSK Core", order = 100)]
    public class RGSKCore : ScriptableObjectSingleton<RGSKCore>
    {
        public string versionNumber = "2.0.0";
        public GameObject persistentManagers;

        public static RuntimeData runtimeData = new RuntimeData();

        [ExposedScriptableObject][SerializeField] RGSKGeneralSettings generalSettings;
        [ExposedScriptableObject][SerializeField] RGSKSceneSettings sceneSettings;
        [ExposedScriptableObject][SerializeField] RGSKContentSettings contentSettings;
        [ExposedScriptableObject][SerializeField] RGSKRaceSettings raceSettings;
        [ExposedScriptableObject][SerializeField] RGSKAISettings aiSettings;
        [ExposedScriptableObject][SerializeField] RGSKUISettings uiSettings;
        [ExposedScriptableObject][SerializeField] RGSKAudioSettings audioSettings;
        [ExposedScriptableObject][SerializeField] RGSKInputSettings inputSettings;
        [ExposedScriptableObject][SerializeField] RGSKVehicleSettings vehicleSettings;

        const string NullWarning = "{0} not assigned in RGSK Core! Using fallback settings.";

        public RGSKGeneralSettings GeneralSettings
        {
            get
            {
                if (generalSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "General Settings"));
                    return _fallbackGeneralSettings;
                }

                return generalSettings;
            }
            set
            {
                if (value != null)
                {
                    generalSettings = value;
                }
            }
        }

        public RGSKSceneSettings SceneSettings
        {
            get
            {
                if (sceneSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Scene Settings"));
                    return _fallbackSceneSettings;
                }

                return sceneSettings;
            }
            set
            {
                if (value != null)
                {
                    sceneSettings = value;
                }
            }
        }

        public RGSKContentSettings ContentSettings
        {
            get
            {
                if (contentSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Content Settings"));
                    return _fallbackContentSettings;
                }

                return contentSettings;
            }
            set
            {
                if (value != null)
                {
                    contentSettings = value;
                }
            }
        }

        public RGSKRaceSettings RaceSettings
        {
            get
            {
                if (raceSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Race Settings"));
                    return _fallbackRaceSettings;
                }

                return raceSettings;
            }
            set
            {
                if (value != null)
                {
                    raceSettings = value;
                }
            }
        }

        public RGSKAISettings AISettings
        {
            get
            {
                if (aiSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "AI Settings"));
                    return _fallbackAiSettings;
                }

                return aiSettings;
            }
            set
            {
                if (value != null)
                {
                    aiSettings = value;
                }
            }
        }

        public RGSKUISettings UISettings
        {
            get
            {
                if (uiSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "UI Settings"));
                    return _fallbackUiSettings;
                }

                return uiSettings;
            }
            set
            {
                if (value != null)
                {
                    uiSettings = value;
                }
            }
        }

        public RGSKAudioSettings AudioSettings
        {
            get
            {
                if (audioSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Audio Settings"));
                    return _fallbackAudioSettings;
                }

                return audioSettings;
            }
            set
            {
                if (value != null)
                {
                    audioSettings = value;
                }
            }
        }

        public RGSKInputSettings InputSettings
        {
            get
            {
                if (inputSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Input Settings"));
                    return _fallbackInputSettings;
                }

                return inputSettings;
            }
            set
            {
                if (value != null)
                {
                    inputSettings = value;
                }
            }
        }

        public RGSKVehicleSettings VehicleSettings
        {
            get
            {
                if (vehicleSettings == null)
                {
                    Logger.LogWarning(string.Format(NullWarning, "Vehicle Physics Settings"));
                    return _fallbackVehicleSettings;
                }

                return vehicleSettings;
            }
            set
            {
                if (value != null)
                {
                    vehicleSettings = value;
                }
            }
        }

        public RGSKGeneralSettings _fallbackGeneralSettings;
        public RGSKSceneSettings _fallbackSceneSettings;
        public RGSKContentSettings _fallbackContentSettings;
        public RGSKRaceSettings _fallbackRaceSettings;
        public RGSKAISettings _fallbackAiSettings;
        public RGSKUISettings _fallbackUiSettings;
        public RGSKAudioSettings _fallbackAudioSettings;
        public RGSKInputSettings _fallbackInputSettings;
        public RGSKVehicleSettings _fallbackVehicleSettings;
    }
}