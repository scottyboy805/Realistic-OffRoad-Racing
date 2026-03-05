using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RGSK
{
    public class RaceReplayManager : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        void OnRaceStateChanged(RaceState state)
        {
            if (!IsEnabled())
                return;

            switch (state)
            {
                case RaceState.Racing:
                    {
                        RecorderManager.Instance?.ReplayRecorder?.StartRecording();
                        break;
                    }

                case RaceState.PostRace:
                    {
                        RecorderManager.Instance?.ReplayRecorder?.StopRecording();
                        break;
                    }
            }
        }

        void OnRaceRestart()
        {
            if (!IsEnabled())
                return;

            RecorderManager.Instance?.ReplayRecorder?.StopPlayback();
            RecorderManager.Instance?.ReplayRecorder?.DeleteRecordedData();
        }

        bool IsEnabled()
        {
            return RGSKCore.Instance.RaceSettings.enableReplay;
        }
    }
}