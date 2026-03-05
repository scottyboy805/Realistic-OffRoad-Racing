using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class SessionTimerUI : MonoBehaviour
    {
        public enum SessionTimerType
        {
            SessionTime,
            FastestLapInSession,
            PersonalBestLapTime,
            RaceTime
        }

        [SerializeField] SessionTimerType type;
        [SerializeField] TimeFormat format;
        [SerializeField] TMP_Text text;

        void OnEnable()
        {
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
        }

        void OnDisable()
        {
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        void Start()
        {
            text?.SetText(UIHelper.FormatTimeText(0, format));
            OnLapCompleted(null);
        }

        void Update()
        {
            if (!RaceManager.Instance.Initialized)
                return;

            switch (type)
            {
                case SessionTimerType.RaceTime:
                    {
                        text?.SetText(UIHelper.FormatTimeText(RaceManager.Instance.GetRaceTime(), format));
                        break;
                    }

                case SessionTimerType.SessionTime:
                    {
                        text?.SetText(UIHelper.FormatTimeText(RaceManager.Instance.GetSessionTime(), format));
                        break;
                    }
            }
        }

        void OnLapCompleted(Competitor c)
        {
            UpdateLapRecordTexts();
        }

        void OnRaceRestart()
        {
            UpdateLapRecordTexts();
        }

        void UpdateLapRecordTexts()
        {
            switch (type)
            {
                case SessionTimerType.PersonalBestLapTime:
                    {
                        text?.SetText(UIHelper.FormatTimeText(RaceManager.Instance.Track.GetBestLap(), format));
                        break;
                    }

                case SessionTimerType.FastestLapInSession:
                    {
                        var value = RaceManager.Instance.GetCompetitorWithBestLap()?.GetBestLapTime() ?? 0f;
                        text?.SetText(UIHelper.FormatTimeText(value, format));
                        break;
                    }
            }
        }
    }
}