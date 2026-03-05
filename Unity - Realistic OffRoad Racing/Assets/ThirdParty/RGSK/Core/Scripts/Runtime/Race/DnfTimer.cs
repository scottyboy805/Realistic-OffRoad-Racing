using System;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class DnfTimer : MonoBehaviour
    {
        Timer _timer;
        bool _timerStarted;

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            RGSKEvents.OnRaceDeInitialized.AddListener(OnRaceDeInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
            RGSKEvents.OnRaceDeInitialized.RemoveListener(OnRaceDeInitialized);
        }

        void Start()
        {
            if (RGSKCore.Instance.RaceSettings.dnfTimerStartMode != DnfTimerStartMode.Off)
            {
                _timer = GeneralHelper.CreateTimer(RGSKCore.Instance.RaceSettings.dnfTimeLimit, true, false,
                                        () => RaceManager.Instance.ForceFinishRace(true),
                                        () => RGSKEvents.OnDnfTimerStart.Invoke(),
                                        () => RGSKEvents.OnDnfTimerStop.Invoke(),
                                        null,
                                        "Timer_DNF");

                _timer.transform.SetParent(GeneralHelper.GetDynamicParent());
            }
        }

        public float GetRemainingTime() => _timer?.Value ?? 0;

        void StartTimer()
        {
            if (_timer != null && _timerStarted)
                return;

            if (ShouldTimerStart())
            {
                _timerStarted = true;
                _timer?.ResetTimer();
                _timer?.StartTimer();
            }
        }

        void StopTimer()
        {
            if (_timer != null && !_timerStarted)
                return;

            _timerStarted = false;
            _timer?.StopTimer();
        }

        void OnRaceDeInitialized()
        {
            StopTimer();
        }

        void OnRaceRestart()
        {
            StopTimer();
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (RaceManager.Instance.AllCompetitorsFinished())
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }

        bool ShouldTimerStart()
        {
            var start = false;
            var finished = RaceManager.Instance.FinishedCompetitorCount();
            var active = RaceManager.Instance.ActiveCompetitorCount();

            switch (RGSKCore.Instance.RaceSettings.dnfTimerStartMode)
            {
                case DnfTimerStartMode.AfterFirstFinish:
                    {
                        start = finished >= 1;
                        break;
                    }

                case DnfTimerStartMode.AfterHalfFinish:
                    {
                        start = finished >= active / 2;
                        break;
                    }
            }

            return start;
        }

        void OnDestroy()
        {
            if (_timer != null)
            {
                Destroy(_timer.gameObject);
            }
        }
    }
}