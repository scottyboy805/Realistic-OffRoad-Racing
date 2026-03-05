using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class DriftNotification : MonoBehaviour
    {
        [SerializeField] NotificationDisplay display;
        [TextArea]
        [SerializeField] string driftCompleteMessage = "Drift Complete +{0}";
        [TextArea]
        [SerializeField] string driftFailedMessage = "Drift Failed -{0}";
        [SerializeField] Color driftCompleteMessageColor = Color.green;
        [SerializeField] Color driftFailedMessageColor = Color.red;

        [Header("Sounds")]
        [SerializeField] string completeSound = "drift_complete";
        [SerializeField] string failSound = "drift_fail";
        [SerializeField] string messageSound = "drift_message";

        void OnEnable()
        {
            RGSKEvents.OnDriftMessage.AddListener(OnDriftMessage);
            RGSKEvents.OnDriftComplete.AddListener(OnDriftComplete);
            RGSKEvents.OnDriftFail.AddListener(OnDriftFail);
        }

        void OnDisable()
        {
            RGSKEvents.OnDriftMessage.RemoveListener(OnDriftMessage);
            RGSKEvents.OnDriftComplete.RemoveListener(OnDriftComplete);
            RGSKEvents.OnDriftFail.RemoveListener(OnDriftFail);
        }

        void OnDriftComplete(DriftController dc, int pts)
        {
            if (dc.Entity != GeneralHelper.GetFocusedEntity())
                return;

            display?.Show(new NotificationProperties
            {
                message = string.Format(driftCompleteMessage, UIHelper.FormatPointsText(pts, true)),
                messageColor = driftCompleteMessageColor,
                sound = completeSound,
                prioritize = true
            });
        }

        void OnDriftFail(DriftController dc, int pts)
        {
            if (dc.Entity != GeneralHelper.GetFocusedEntity())
                return;

            display?.Show(new NotificationProperties
            {
                message = string.Format(driftFailedMessage, UIHelper.FormatPointsText(pts, true)),
                messageColor = driftFailedMessageColor,
                sound = failSound,
                prioritize = true
            });
        }

        void OnDriftMessage(DriftController dc, string msg)
        {
            if (dc.Entity != GeneralHelper.GetFocusedEntity())
                return;

            display?.Show(new NotificationProperties
            {
                message = msg,
                sound = messageSound
            });
        }
    }
}