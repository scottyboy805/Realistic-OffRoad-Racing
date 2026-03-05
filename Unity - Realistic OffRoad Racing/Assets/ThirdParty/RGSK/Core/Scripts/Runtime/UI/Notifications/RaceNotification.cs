using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceNotification : MonoBehaviour
    {
        [SerializeField] NotificationDisplay primaryDisplay;
        [SerializeField] NotificationDisplay secondaryDisplay;

        [TextArea]
        [SerializeField] string competitorFinishMessage = "{0} finished {1}";
        [TextArea]
        [SerializeField] string competitorDisqualifiedMessage = "{0} has been disqualified";
        [TextArea]
        [SerializeField] string timeExtendedMessage = "{0}";
        [TextArea]
        [SerializeField] string scoreMessage = "{0}";
        [TextArea]
        [SerializeField] string speedtrapMessage = "+ {0}";
        [TextArea]
        [SerializeField] string finalLapMessage = "Final Lap!";
        [TextArea]
        [SerializeField] string bestLapMessage = "Best Lap {0}";
        [TextArea]
        [SerializeField] string rollingStartMessage = "Get ready to race";
        [SerializeField] float rollingStartMessageDuration = 10;

        [Header("Sector")]
        [SerializeField] NotificationDisplay sectorDisplay;
        [SerializeField] Color fastColor = Color.green;
        [SerializeField] Color slowColor = Color.red;

        [Header("Icons")]
        [SerializeField] Sprite timeExtendIcon;
        [SerializeField] Sprite speedtrapIcon;
        [SerializeField] Sprite scoreIcon;

        [Header("Sounds")]
        [SerializeField] string finalLapSound;
        [SerializeField] string newBestLapSound;
        [SerializeField] string timecheckpointSound;
        [SerializeField] string scoreSound;
        [SerializeField] string speedtrapSound;

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnTimerExtended.AddListener(OnTimerExtended);
            RGSKEvents.OnScoreAdded.AddListener(OnScoreAdded);
            RGSKEvents.OnHitSpeedtrap.AddListener(OnHitSpeedtrap);
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
            RGSKEvents.OnHitSector.AddListener(OnHitSector);
            RGSKEvents.OnSetNewBestLapTime.AddListener(OnSetNewBestLapTime);
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnTimerExtended.RemoveListener(OnTimerExtended);
            RGSKEvents.OnScoreAdded.RemoveListener(OnScoreAdded);
            RGSKEvents.OnHitSpeedtrap.RemoveListener(OnHitSpeedtrap);
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
            RGSKEvents.OnHitSector.RemoveListener(OnHitSector);
            RGSKEvents.OnSetNewBestLapTime.RemoveListener(OnSetNewBestLapTime);
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        void OnLapCompleted(Competitor c)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            if (c.IsFinalLap())
            {
                primaryDisplay?.Show(new NotificationProperties
                {
                    message = finalLapMessage,
                    sound = finalLapSound
                });
            }
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (!c.IsDisqualified)
            {
                secondaryDisplay?.Show(new NotificationProperties
                {
                    message = string.Format(competitorFinishMessage, UIHelper.FormatNameText(c.Entity?.ProfileDefiner?.definition),
                                UIHelper.FormatOrdinalText(c.FinalPosition)),
                });
            }
            else
            {
                secondaryDisplay?.Show(new NotificationProperties
                {
                    message = string.Format(competitorDisqualifiedMessage, UIHelper.FormatNameText(c.Entity?.ProfileDefiner?.definition)),
                });
            }
        }

        void OnTimerExtended(Competitor c, float value)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            var symbol = value >= 0 ? "+" : "-";
            primaryDisplay?.Show(new NotificationProperties
            {
                message = string.Format(timeExtendedMessage, $"{symbol}{Mathf.Abs(value)}"),
                sprite = timeExtendIcon,
                sound = timecheckpointSound,
                prioritize = true
            });
        }

        void OnScoreAdded(Competitor c, float value)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            var symbol = value >= 0 ? "+" : "-";
            primaryDisplay?.Show(new NotificationProperties
            {
                message = string.Format(scoreMessage, $"{symbol}{Mathf.Abs(value)}"),
                sprite = scoreIcon,
                sound = scoreSound,
                prioritize = true
            });
        }

        void OnHitSpeedtrap(Competitor c, float value)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            primaryDisplay?.Show(new NotificationProperties
            {
                message = string.Format(speedtrapMessage, UIHelper.FormatSpeedText(value, true)),
                sprite = speedtrapIcon,
                sound = speedtrapSound
            });
        }

        void OnSetNewBestLapTime(Competitor c)
        {
            if (!c.IsRacing())
                return;

            primaryDisplay?.Show(new NotificationProperties
            {
                message = string.Format(bestLapMessage, UIHelper.FormatTimeText(c.GetBestLapTime())),
                sound = newBestLapSound
            });
        }

        void OnHitSector(Competitor c, float time)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            if (c.CurrentLap > 1)
            {
                sectorDisplay?.Show(new NotificationProperties
                {
                    message = UIHelper.FormatTimeText(time, TimeFormat.MM_SS_FFF, true),
                    messageColor = time < 0 ? fastColor : slowColor,
                });
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            primaryDisplay?.Clear();

            switch (state)
            {
                case RaceState.RollingStart:
                    {
                        if (!string.IsNullOrWhiteSpace(rollingStartMessage))
                        {
                            primaryDisplay?.Show(new NotificationProperties
                            {
                                message = string.Format(rollingStartMessage),
                                duration = rollingStartMessageDuration,
                                prioritize = true
                            });
                        }
                        break;
                    }
            }
        }
    }
}