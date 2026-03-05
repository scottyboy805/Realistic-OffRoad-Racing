using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] ProfileDefinitionUI profileDefinitionUI;

        [Header("Vehicle")]
        [SerializeField] VehicleDefinitionUI vehicleDefinitionUI;

        [Header("Currency/XP")]
        [SerializeField] TMP_Text currencyText;
        [SerializeField] XPGauge xpGauge;

        [Header("Stats")]
        [SerializeField] TMP_Text totalCurrencyEarnedText;
        [SerializeField] TMP_Text totalWinsText;
        [SerializeField] TMP_Text totalRacesText;
        [SerializeField] TMP_Text winRatioText;
        [SerializeField] TMP_Text totalDistanceText;

        UIScreen _parentScreen;

        void Start()
        {
            _parentScreen = GetComponentInParent<UIScreen>();
        }

        void Update()
        {
            if (_parentScreen != null && !_parentScreen.IsOpen())
                return;

            UpdateUI();
        }

        public void UpdateUI()
        {
            profileDefinitionUI?.UpdateUI(RGSKCore.Instance.GeneralSettings.playerProfile);
            vehicleDefinitionUI?.UpdateUI(RGSKCore.runtimeData.SelectedVehicle);

            currencyText?.SetText(UIHelper.FormatCurrencyText(SaveData.Instance.playerData.currency));
            xpGauge?.SetValue(SaveData.Instance.playerData.xp);

            var totalWins = SaveData.Instance.playerData.totalWins;
            var totalRaces = SaveData.Instance.playerData.totalRaces;

            totalCurrencyEarnedText?.SetText(UIHelper.FormatCurrencyText(SaveData.Instance.playerData.totalCurrencyEarned));
            totalWinsText?.SetText(totalWins.ToString());
            totalRacesText?.SetText(totalRaces.ToString());
            winRatioText?.SetText(UIHelper.FormatPercentageText(((float)totalWins / (float)Mathf.Clamp(totalRaces, 1, totalRaces)) * 100));
            totalDistanceText?.SetText(UIHelper.FormatDistanceText(SaveData.Instance.playerData.totalDistance));
        }
    }
}