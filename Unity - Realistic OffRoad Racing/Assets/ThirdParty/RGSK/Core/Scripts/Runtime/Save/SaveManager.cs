using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;
using RGSK.Helpers;

namespace RGSK
{
    public class SaveManager : Singleton<SaveManager>
    {
        string saveFileName => RGSKCore.Instance.GeneralSettings.saveFileName;
        bool enableSaveSystem => RGSKCore.Instance.GeneralSettings.enableSaveSystem;
        bool encryptData => RGSKCore.Instance.GeneralSettings.encryptSaveData;
        string SaveFilePath => Path.Combine(GeneralHelper.GetProfilesDirectory(), _filePath);
        string _filePath;

        public override void Awake()
        {
            base.Awake();
            Load(saveFileName);
        }

        void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //Load in Start() when running on WebGL
            Load(saveFileName);
#endif
            UpdateAudioMixerParameters();
        }

        public void Save()
        {
            if (!enableSaveSystem)
            {
                Logger.Log(this, "Save aborted! The save system is not enabled!");
                return;
            }

            if (string.IsNullOrWhiteSpace(_filePath))
                return;

            //gameplay
            SaveData.Instance.gameSettingsData.measureUnit = RGSKCore.Instance.UISettings.measureUnit;
            SaveData.Instance.gameSettingsData.transmission = RGSKCore.Instance.VehicleSettings.transmissionType;
            SaveData.Instance.gameSettingsData.autoThrottle = RGSKCore.Instance.VehicleSettings.autoThrottle;
            SaveData.Instance.gameSettingsData.autoBrake = RGSKCore.Instance.VehicleSettings.autoBrake;
            SaveData.Instance.gameSettingsData.showNameplates = RGSKCore.Instance.UISettings.showNameplates;
            SaveData.Instance.gameSettingsData.showProximityArrows = RGSKCore.Instance.UISettings.showProximityArrows;
            SaveData.Instance.gameSettingsData.mobileInput = RGSKCore.Instance.InputSettings.mobileControlType;
            SaveData.Instance.gameSettingsData.accelerometerSensitivity = RGSKCore.Instance.InputSettings.accelerometerSensitivity;
            SaveData.Instance.gameSettingsData.vibrate = RGSKCore.Instance.InputSettings.vibrate;
            SaveData.Instance.gameSettingsData.showFPS = RGSKCore.Instance.GeneralSettings.enableFpsReader;

            //profile
            if (RGSKCore.Instance.GeneralSettings.playerProfile != null)
            {
                SaveData.Instance.playerData.firstName = RGSKCore.Instance.GeneralSettings.playerProfile.firstName;
                SaveData.Instance.playerData.lastName = RGSKCore.Instance.GeneralSettings.playerProfile.lastName;

                if (RGSKCore.Instance.GeneralSettings.countrySettings != null)
                {
                    SaveData.Instance.playerData.countryIndex = RGSKCore.Instance.GeneralSettings.countrySettings.GetCountryIndex(RGSKCore.Instance.GeneralSettings.playerProfile.nationality);
                }
            }

            SerializationManager.Save(SaveData.Instance, SaveFilePath, encryptData);
        }

        public void Load(string path)
        {
            if (!enableSaveSystem)
            {
                Logger.Log(this, "Load aborted! The save system is not enabled!");
                return;
            }

            _filePath = path;
            SaveData.Instance = (SaveData)SerializationManager.Load(SaveFilePath, typeof(SaveData), encryptData);

            //graphics
            QualitySettings.SetQualityLevel(SaveData.Instance.gameSettingsData.qualityIndex, true);
            Application.targetFrameRate = SaveData.Instance.gameSettingsData.targetFPS;

            if (!GeneralHelper.IsMobilePlatform() && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                var resolutions = GeneralHelper.GetResolutionsList();
                SaveData.Instance.gameSettingsData.resolutionIndex = Mathf.Clamp(SaveData.Instance.gameSettingsData.resolutionIndex, 0, resolutions.Count - 1);
                Screen.SetResolution(resolutions[SaveData.Instance.gameSettingsData.resolutionIndex].width,
                                 resolutions[SaveData.Instance.gameSettingsData.resolutionIndex].height,
                                 SaveData.Instance.gameSettingsData.fullscreen);

                QualitySettings.vSyncCount = SaveData.Instance.gameSettingsData.vSyncCount;
            }

            //gameplay
            RGSKCore.Instance.UISettings.measureUnit = SaveData.Instance.gameSettingsData.measureUnit;
            RGSKCore.Instance.VehicleSettings.transmissionType = SaveData.Instance.gameSettingsData.transmission;
            RGSKCore.Instance.VehicleSettings.autoThrottle = SaveData.Instance.gameSettingsData.autoThrottle;
            RGSKCore.Instance.VehicleSettings.autoBrake = SaveData.Instance.gameSettingsData.autoBrake;
            RGSKCore.Instance.UISettings.showNameplates = SaveData.Instance.gameSettingsData.showNameplates;
            RGSKCore.Instance.UISettings.showProximityArrows = SaveData.Instance.gameSettingsData.showProximityArrows;
            RGSKCore.Instance.InputSettings.mobileControlType = SaveData.Instance.gameSettingsData.mobileInput;
            RGSKCore.Instance.InputSettings.accelerometerSensitivity = SaveData.Instance.gameSettingsData.accelerometerSensitivity;
            RGSKCore.Instance.InputSettings.vibrate = SaveData.Instance.gameSettingsData.vibrate;
            RGSKCore.Instance.GeneralSettings.enableFpsReader = SaveData.Instance.gameSettingsData.showFPS;

            //input
            InputManager.Instance?.LoadBindings();

            //profile
            if (RGSKCore.Instance.GeneralSettings.playerProfile != null)
            {
                RGSKCore.Instance.GeneralSettings.playerProfile.firstName = SaveData.Instance.playerData.firstName;
                RGSKCore.Instance.GeneralSettings.playerProfile.lastName = SaveData.Instance.playerData.lastName;

                if (RGSKCore.Instance.GeneralSettings.countrySettings != null)
                {
                    var country = RGSKCore.Instance.GeneralSettings.countrySettings.GetCountryIndex(SaveData.Instance.playerData.countryIndex);
                    RGSKCore.Instance.GeneralSettings.playerProfile.nationality = country;
                }
            }

            var vehicleID = SaveData.Instance.playerData.selectedVehicleID;
            if (!string.IsNullOrWhiteSpace(vehicleID))
            {
                RGSKCore.runtimeData.SelectedVehicle = RGSKCore.Instance.ContentSettings.vehicles.FirstOrDefault(x => x.ID == vehicleID);
            }
        }

        void UpdateAudioMixerParameters()
        {
            foreach (var vol in SaveData.Instance.gameSettingsData.volumes)
            {
                AudioHelper.SetAudioMixerVolume(AudioManager.Instance.Mixer, vol.Key);
            }
        }
        
        public void DeleteSaveFile() => StartCoroutine(DeleteSaveFileRoutine());
        
        IEnumerator DeleteSaveFileRoutine()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }

            SaveData.Instance = new SaveData();
            InputManager.Instance?.ResetBindings();
            MusicManager.Instance?.Stop(false);

            yield return null;

            _filePath = string.Empty;
            Load(saveFileName);
            
            UpdateAudioMixerParameters();
            SceneLoadManager.LoadMainScene();
        }

        void OnApplicationQuit()
        {
            if (RGSKCore.Instance.GeneralSettings.saveOnApplicationQuit)
            {
                Save();
            }
        }
    }
}