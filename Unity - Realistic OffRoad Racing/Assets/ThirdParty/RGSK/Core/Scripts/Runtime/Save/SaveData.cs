using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    [Serializable]
    public class SaveData
    {
        public static SaveData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveData();
                }

                return _instance;
            }
            set
            {
                if (value != null)
                {
                    _instance = value;
                }
            }
        }

        static SaveData _instance;
        public PlayerData playerData = new PlayerData(RGSKCore.Instance.GeneralSettings.defaultPlayerData);
        public GameSettingsData gameSettingsData = new GameSettingsData();
        public Dictionary<string, float> bestLaps = new Dictionary<string, float>();
        public Dictionary<string, int> bestSessionPosition = new Dictionary<string, int>();
        public Dictionary<string, int> bestChampionshipPosition = new Dictionary<string, int>();
        public List<string> unlockedItems = new List<string>();
        public List<string> ownedItems = new List<string>();

        //public Dictionary<string, CustomizerPreset> customizations = new();
    }

    [Serializable]
    public class PlayerData
    {
        public string firstName = "Player";
        public string lastName = "One";
        [HideInInspector] public int countryIndex = 0;
        [HideInInspector] public string selectedVehicleID = "";
        public int currency = 0;
        [HideInInspector] public int xp = 0;
        [HideInInspector] public int totalCurrencyEarned;
        [HideInInspector] public int totalWins = 0;
        [HideInInspector] public int totalRaces = 0;
        [HideInInspector] public int totalDistance = 0;

        public PlayerData(PlayerData data)
        {
            firstName = data.firstName;
            lastName = data.lastName;
            countryIndex = data.countryIndex;
            selectedVehicleID = data.selectedVehicleID;
            currency = data.currency;
            xp = data.xp;
            totalCurrencyEarned = data.totalCurrencyEarned;
            totalWins = data.totalWins;
            totalRaces = data.totalRaces;
            totalDistance = data.totalDistance;
        }

        public void AddCurrency(int amount)
        {
            currency += amount;
            totalCurrencyEarned += amount;
        }

        public void AddXP(int amount)
        {
            xp += amount;
            xp = Mathf.Clamp(xp, xp, (int)RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.GetMaxValue());
        }
    }

    [Serializable]
    public class GameSettingsData
    {
        //graphics
        public int qualityIndex = QualitySettings.GetQualityLevel();
        public int resolutionIndex = GeneralHelper.GetCurrentResolutionIndex();
        public bool fullscreen = true;
        public int vSyncCount = 0;
        public int targetFPS = -1;

        //audio
        public Dictionary<AudioGroup, float> volumes = new Dictionary<AudioGroup, float>
        {
            {AudioGroup.Master, 1},
            {AudioGroup.Music, 0.5f},
            {AudioGroup.SFX, 0.5f},
        };

        //input
        public string inputBindings = string.Empty;
        public MobileControlType mobileInput = MobileControlType.Touch;
        public float accelerometerSensitivity = 1f;

        //gameplay
        public int cameraPerspectiveIndex = 0;
        public MeasureUnit measureUnit = MeasureUnit.Metric;
        public TransmissionType transmission = TransmissionType.Automatic;
        public bool autoThrottle = false;
        public bool autoBrake = false;
        public bool showNameplates = true;
        public bool showProximityArrows = true;
        public bool vibrate = true;
        public bool showFPS = false;
    }
}