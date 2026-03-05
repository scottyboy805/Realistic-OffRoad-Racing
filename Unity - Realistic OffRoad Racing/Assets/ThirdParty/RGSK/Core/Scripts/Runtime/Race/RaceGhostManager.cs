using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RaceGhostManager : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        void Update()
        {
            if (RaceManager.Instance.Initialized && RecorderManager.Instance.GhostRecorder != null)
            {
                if (RaceManager.Instance.CurrentState != RaceState.Racing &&
                    RecorderManager.Instance.GhostRecorder.IsPlayback)
                {
                    StopGhost();
                }
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            if (!IsEnabled())
                return;

            switch (state)
            {
                case RaceState.Racing:
                    {
                        RecorderManager.Instance?.GhostRecorder?.StartRecording();
                        break;
                    }
            }
        }

        void OnLapCompleted(Competitor c)
        {
            if (!IsEnabled() || !c.Entity.IsPlayer)
                return;

            if (c.GetLastLapTime() <= c.GetBestLapTime())
            {
                RecorderManager.Instance?.GhostRecorder?.CacheReplayData();
            }

            RecorderManager.Instance?.GhostRecorder?.StartPlayback();
            RecorderManager.Instance?.GhostRecorder?.StartRecording();

            var offset = RaceManager.Instance.Session.ghostOffset;
            if (offset > 0)
            {
                var startFrame = Mathf.RoundToInt(offset / Time.fixedDeltaTime);
                RecorderManager.Instance?.GhostRecorder?.SkipToFrame(startFrame);
            }
        }

        void OnRaceRestart()
        {
            if (!IsEnabled())
                return;

            StopGhost();
        }

        void StopGhost()
        {
            RecorderManager.Instance?.GhostRecorder?.StopRecording();
            RecorderManager.Instance?.GhostRecorder?.StopPlayback();
            RecorderManager.Instance?.GhostRecorder?.DeleteRecordedData();
        }

        bool IsEnabled()
        {
            return RaceManager.Instance.Session.UseGhostVehicle() && RaceManager.Instance.Session.enableGhost;
        }
    }
}