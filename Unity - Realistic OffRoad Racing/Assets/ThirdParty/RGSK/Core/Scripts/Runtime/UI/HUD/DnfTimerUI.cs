using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class DnfTimerUI : MonoBehaviour
    {
        [SerializeField] GameObject container;
        [SerializeField] TMP_Text text;
        [SerializeField] TimeFormat format = TimeFormat.MM_SS;

        void OnEnable()
        {
            RGSKEvents.OnDnfTimerStart.AddListener(OnDNFTimerStart);
            RGSKEvents.OnDnfTimerStop.AddListener(OnDNFTimerStop);
            RGSKEvents.OnRaceInitialized.AddListener(OnDNFTimerStop);
        }

        void OnDisable()
        {
            RGSKEvents.OnDnfTimerStart.RemoveListener(OnDNFTimerStart);
            RGSKEvents.OnDnfTimerStop.RemoveListener(OnDNFTimerStop);
            RGSKEvents.OnRaceInitialized.RemoveListener(OnDNFTimerStop);
        }

        void Awake() => Toggle(false);

        void Update()
        {
            if (container == null)
                return;

            if (container.activeSelf && RaceManager.Instance.Initialized)
            {
                text?.SetText(UIHelper.FormatTimeText(RaceManager.Instance.GetDnfTime(), format));
            }
        }

        void OnDNFTimerStart() => Toggle(true);

        void OnDNFTimerStop() => Toggle(false);

        void Toggle(bool toggle)
        {
            if (container == null)
                return;

            container.SetActive(toggle);
        }
    }
}