using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceUIScreenManager : MonoBehaviour
    {
        UIScreenID preRaceScreen => RGSKCore.Instance.UISettings.screens.PreRaceScreen;
        UIScreenID raceScreen => RGSKCore.Instance.UISettings.screens.RaceScreen;
        UIScreenID postRaceScreen => RGSKCore.Instance.UISettings.screens.PostRaceScreen;
        UIScreenID replayScreeen => RGSKCore.Instance.UISettings.screens.ReplayScreeen;
        UIScreenID spectatorScreen => RGSKCore.Instance.UISettings.screens.SpectatorScreen;

        RaceWaypointArrow _arrowInstance;
        bool _spectatorMode;

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
            RGSKEvents.OnRaceDeInitialized.AddListener(OnRaceDeInitialized);
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
            RGSKEvents.OnGameUnpaused.AddListener(OnGameUnpaused);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
            RGSKEvents.OnRaceDeInitialized.RemoveListener(OnRaceDeInitialized);
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
            RGSKEvents.OnGameUnpaused.RemoveListener(OnGameUnpaused);
        }

        void OnRaceInitialized()
        {
            _spectatorMode = RaceManager.Instance.Session.spectatorMode;

            UIManager.Instance?.CreateScreen(preRaceScreen);
            UIManager.Instance?.CreateScreen(GetRacingStateScreen());
            UIManager.Instance?.CreateScreen(postRaceScreen);

            if (RGSKCore.Instance.RaceSettings.enableReplay)
            {
                UIManager.Instance.CreateScreen(replayScreeen);
            }

            if (RGSKCore.Instance.UISettings.showWaypointArrow)
            {
                if (RGSKCore.Instance.UISettings.waypointArrow != null)
                {
                    _arrowInstance = Instantiate(RGSKCore.Instance.UISettings.waypointArrow, GeneralHelper.GetDynamicParent());
                }
            }
        }

        void OnRaceDeInitialized()
        {
            UIManager.Instance?.DestroyScreen(preRaceScreen);
            UIManager.Instance?.DestroyScreen(GetRacingStateScreen());
            UIManager.Instance?.DestroyScreen(postRaceScreen);

            if (_arrowInstance != null)
            {
                Destroy(_arrowInstance.gameObject);
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            _spectatorMode = RaceManager.Instance.Session.spectatorMode;

            switch (state)
            {
                case RaceState.PreRace:
                    {
                        UIManager.Instance?.OpenScreen(preRaceScreen, false);
                        break;
                    }

                case RaceState.Countdown:
                case RaceState.RollingStart:
                case RaceState.Racing:
                    {
                        UIManager.Instance?.OpenScreen(GetRacingStateScreen(), false);
                        break;
                    }

                case RaceState.PostRace:
                    {
                        UIManager.Instance?.OpenScreen(postRaceScreen, false);
                        break;
                    }
            }
        }

        void OnGameUnpaused()
        {
            if (RaceManager.Instance.Initialized)
            {
                UIManager.Instance?.OpenScreen(GetRacingStateScreen(), false);
            }
        }

        UIScreenID GetRacingStateScreen() => !_spectatorMode ? raceScreen : spectatorScreen;
    }
}