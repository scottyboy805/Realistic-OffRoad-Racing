using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using RGSK.Extensions;

namespace RGSK
{
    public class RaceSettingsScreen : UIScreen
    {
        [SerializeField] Button startButton;

        [Header("Session Defaults")]
        [SerializeField] SelectionMode playerGridStartMode = SelectionMode.Selected;
        [SerializeField] OpponentClassOptions opponentClassOptions = OpponentClassOptions.SameAsPlayer;
        [SerializeField] int opponentDifficultyIndex = 1;
        [SerializeField] int playerStartPosition = 100; //defaults to last place

        [Header("Rewards")]
        [SerializeField] bool autoCalculateRewards = true;
        [SerializeField] int baseCurrencyReward = 200;
        [SerializeField] int baseXpReward = 100;

        [Header("Modal")]
        [SerializeField] bool showModalWindow = true;
        [SerializeField]
        ModalWindowProperties modalProperties = new ModalWindowProperties
        {
            header = "Start Race",
            message = "Are you sure you want to start the race?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        List<RaceSettingsEntryUI> _entries = new List<RaceSettingsEntryUI>();

        public override void Initialize()
        {
            GetComponentsInChildren<RaceSettingsEntryUI>(true, _entries);

            foreach (var e in _entries)
            {
                e.Setup(this);
            }

            if (showModalWindow)
            {
                startButton.onClick.AddListener(() =>
                {
                    ModalWindowManager.Instance.Show(new ModalWindowProperties
                    {
                        header = modalProperties.header,
                        message = modalProperties.message,
                        confirmButtonText = modalProperties.confirmButtonText,
                        declineButtonText = modalProperties.declineButtonText,
                        confirmAction = () => StartSession(),
                        declineAction = () => { },
                        startSelection = modalProperties.startSelection,
                        prefabIndex = modalProperties.prefabIndex
                    });
                });
            }
            else
            {
                startButton.onClick.AddListener(() => StartSession());
            }

            base.Initialize();
        }

        public override void Open()
        {
            base.Open();

            if (RGSKCore.runtimeData.SelectedSession != null)
            {
                RGSKCore.runtimeData.SelectedSession = null;
            }

            RGSKCore.runtimeData.SelectedSession = ScriptableObject.CreateInstance<RaceSession>();
            RGSKCore.runtimeData.SelectedSession.saveRecords = false;
            RGSKCore.runtimeData.SelectedSession.startMode = RaceStartMode.StandingStart;
            RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentClassOptions = opponentClassOptions;
            RGSKCore.runtimeData.SelectedSession.opponentDifficulty.index = Mathf.Clamp(opponentDifficultyIndex, 0, RGSKCore.Instance.AISettings.difficulties.Count - 1);
            RGSKCore.runtimeData.SelectedSession.playerGridStartMode = playerGridStartMode;
            RGSKCore.runtimeData.SelectedSession.playerStartPosition = playerStartPosition;

            foreach (var e in _entries)
            {
                e.UpdateSession(RGSKCore.runtimeData.SelectedSession);
            }

            RefreshEntries();
        }

        public void RefreshEntries()
        {
            foreach (var e in _entries)
            {
                e.SelectOption(0);
                e.ToggleActive();
            }
        }

        void StartSession()
        {
            if (RGSKCore.runtimeData.SelectedTrack == null || RGSKCore.runtimeData.SelectedVehicle == null)
                return;

            RGSKCore.runtimeData.SelectedSession.entrants.Clear();
            RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.autoPopulatePlayer = true;
            RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentClassOptions = OpponentClassOptions.SameAsPlayer;
            RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentVehicleClass = RGSKCore.runtimeData.SelectedVehicle.vehicleClass;
            RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.autoPopulateOpponents = true;

            CalculateRewards();

            SceneLoadManager.LoadScene(RGSKCore.runtimeData.SelectedTrack.scene);
        }

        void CalculateRewards()
        {
            if (!autoCalculateRewards || RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentCount == 0)
                return;

            var baseCurrency = baseCurrencyReward * RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentCount * RGSKCore.runtimeData.SelectedSession.lapCount;
            var baseXp = baseXpReward * RGSKCore.runtimeData.SelectedSession.autoPopulateEntrantOptions.opponentCount * RGSKCore.runtimeData.SelectedSession.lapCount;
            var rewardMultiplier = 1f;

            for (int i = 0; i < 3; i++)
            {
                RGSKCore.runtimeData.SelectedSession.raceRewards.Add(new RaceReward
                {
                    currency = (int)(baseCurrency * rewardMultiplier),
                    xp = (int)(baseXp * rewardMultiplier)
                });

                rewardMultiplier -= 0.25f;
            }
        }
    }
}