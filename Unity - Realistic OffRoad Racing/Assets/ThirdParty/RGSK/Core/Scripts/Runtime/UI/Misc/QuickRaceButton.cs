using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;

namespace RGSK
{
    public class QuickRaceButton : MonoBehaviour
    {
        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() =>
                    {
                        StartCoroutine(GeneralHelper.OpenTrackSelectScreenWithCallback(() =>
                        {
                            var screen = RGSKCore.Instance.UISettings.screens.RaceSettingsScreen;

                            if (screen == null)
                            {
                                Logger.LogWarning("Unable to open the race settings screen! Please assign it to the RGSK Menu under the 'UI' tab.");
                            }

                            screen?.Open();
                        }));
                    }));
                });
            }
        }
    }
}