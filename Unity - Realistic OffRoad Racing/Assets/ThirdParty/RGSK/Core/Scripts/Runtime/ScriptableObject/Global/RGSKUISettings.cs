using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/UI")]
    public class RGSKUISettings : ScriptableObject
    {
        [System.Serializable]
        public class Screens
        {
            [Header("Common")]
            [SerializeField] UIScreenID loadingScreen;
            [SerializeField] UIScreenID pauseScreen;
            [SerializeField] UIScreenID replayScreeen;

            [Header("Race")]
            [SerializeField] UIScreenID preRaceScreen;
            [SerializeField] UIScreenID raceScreen;
            [SerializeField] UIScreenID postRaceScreen;
            [SerializeField] UIScreenID spectatorScreen;

            [Header("Menu")]
            [SerializeField] UIScreenID vehicleSelectScreen;
            [SerializeField] UIScreenID trackSelectScreen;
            [SerializeField] UIScreenID raceSettingsScreen;

            public UIScreenID LoadingScreen => GetScreen(loadingScreen, "Loading Screen", RGSKCore.Instance._fallbackUiSettings.screens.loadingScreen);
            public UIScreenID PauseScreen => GetScreen(pauseScreen, "Pause Screen", RGSKCore.Instance._fallbackUiSettings.screens.pauseScreen);
            public UIScreenID ReplayScreeen => GetScreen(replayScreeen, "Replay Screen", RGSKCore.Instance._fallbackUiSettings.screens.replayScreeen);

            public UIScreenID PreRaceScreen => GetScreen(preRaceScreen, "Pre Race Screen", RGSKCore.Instance._fallbackUiSettings.screens.preRaceScreen);
            public UIScreenID RaceScreen => GetScreen(raceScreen, "Race Screen", RGSKCore.Instance._fallbackUiSettings.screens.raceScreen);
            public UIScreenID PostRaceScreen => GetScreen(postRaceScreen, "Post Race Screen", RGSKCore.Instance._fallbackUiSettings.screens.postRaceScreen);
            public UIScreenID SpectatorScreen => GetScreen(spectatorScreen, "Spectator Screen", RGSKCore.Instance._fallbackUiSettings.screens.spectatorScreen);

            public UIScreenID VehicleSelectScreen => GetScreen(vehicleSelectScreen, "Vehicle Select Screen", RGSKCore.Instance._fallbackUiSettings.screens.vehicleSelectScreen);
            public UIScreenID TrackSelectScreen => GetScreen(trackSelectScreen, "Track Select Screen", RGSKCore.Instance._fallbackUiSettings.screens.trackSelectScreen);
            public UIScreenID RaceSettingsScreen => GetScreen(raceSettingsScreen, "Race Settings Screen", RGSKCore.Instance._fallbackUiSettings.screens.raceSettingsScreen);

            UIScreenID GetScreen(UIScreenID screen, string name, UIScreenID fallback)
            {
                if (screen == null)
                {
                    Logger.LogWarning($"\"{name}\" is missing from the screens in the RGSK Menu > UI tab! Using fallback screen.");
                    return fallback;
                }

                return screen;
            }
        }

        [Header("Screens")]
        public Screens screens;

        [Header("Race Type UI")]
        public RaceUILayout defaultRaceUILayout;
        public List<RaceUILayout> raceUILayouts = new List<RaceUILayout>();

        [Header("Race Boards")]
        public RaceBoardLayout defaultRaceBoardLayout;
        public List<RaceBoardLayout> raceBoardLayouts = new List<RaceBoardLayout>();

        [Header("Worldspace")]
        public Nameplate nameplate;
        public RaceWaypointArrow waypointArrow;
        public bool showNameplates = true;
        public bool showWaypointArrow;

        [Header("Minimap")]
        public string playerMinimapBlip = "player";
        public string opponentMinimapBlip = "opponent";

        [Header("Proximity Arrows")]
        public bool showProximityArrows;

        [Header("Target Score Icons")]
        public List<TargetScoreIcon> targetScoreIcons = new List<TargetScoreIcon>();

        [Header("Formats")]
        public TimeFormat raceTimerFormat = TimeFormat.MM_SS_FFF;
        public TimeFormat realtimeGapTimerFormat = TimeFormat.S_FFF;
        public MeasureUnit measureUnit = MeasureUnit.Metric;
        public SpeedUnit speedUnit => measureUnit == MeasureUnit.Metric ? SpeedUnit.KMH : SpeedUnit.MPH;
        public DistanceUnit distanceUnit => measureUnit == MeasureUnit.Metric ? DistanceUnit.Meters : DistanceUnit.Feet;
        public NumberDisplayMode raceBoardPositionFormat = NumberDisplayMode.Single;
        public NumberDisplayMode raceBoardLapFormat = NumberDisplayMode.Single;
        public VehicleNameDisplayMode raceBoardVehicleNameFormat = VehicleNameDisplayMode.FullName;
        public NameDisplayMode raceBoardNameFormat = NameDisplayMode.FullName;
        public string dnfString = "DNF";
        public string currencyFormat = "Cr. {0}";
        public string lockedItemFormat = "Unlocks at level {0}";

        [Header("Audio")]
        public string buttonHoverSound = "button_hover";
        public string buttonClickSound = "button_click";

        [Header("Modal Windows")]
        public List<ModalWindow> modalWindowPrefabs = new List<ModalWindow>();
        public ModalWindowProperties restartModal = new ModalWindowProperties
        {
            header = "Restart",
            message = "Are you sure you want to restart?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties exitModal = new ModalWindowProperties
        {
            header = "Exit",
            message = "Are you sure you want to exit?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties quitModal = new ModalWindowProperties
        {
            header = "Quit",
            message = "Are you sure you want to quit?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties purchasePromptModal = new ModalWindowProperties
        {
            header = "Purchase",
            message = "Do you want to purchase {0} for {1}?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties purchaseFailModal = new ModalWindowProperties
        {
            header = "Purchase",
            message = "You do not have enough currency!",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties lockedItemModal = new ModalWindowProperties
        {
            header = "Locked",
            message = "This item will unlock at level {0}!",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties lockedItemConditionalModal = new ModalWindowProperties
        {
            header = "Locked",
            message = "{0}",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties deleteSaveModal = new ModalWindowProperties
        {
            header = "Delete Save Data",
            message = "Are you sure you want to delete the save data? This cannot be undone.",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties resetInputBindingsModal = new ModalWindowProperties
        {
            header = "Reset Input Bindings",
            message = "Are you sure you want to reset all input bindings?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties endRaceSessionModal = new ModalWindowProperties
        {
            header = "End Session",
            message = "Are you sure you want to end the session?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };
    }
}