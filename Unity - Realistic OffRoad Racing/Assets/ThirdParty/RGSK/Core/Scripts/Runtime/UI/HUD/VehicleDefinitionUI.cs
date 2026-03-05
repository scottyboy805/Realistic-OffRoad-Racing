using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class VehicleDefinitionUI : MonoBehaviour
    {
        public VehicleDefinition vehicle;
        public VehicleNameDisplayMode nameDisplayMode;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public Image photo;
        public TMP_Text priceText;
        public TMP_Text classText;
        public Image classIcon;
        public TMP_Text manufacturerText;
        public Image manufacturerIcon;
        public TMP_Text countryText;
        public Image country;
        public Gauge speedStat;
        public Gauge accelerationStat;
        public Gauge brakeStat;
        public Gauge handlingStat;

        void Start()
        {
            if (vehicle != null)
            {
                UpdateUI(vehicle);
            }
        }

        public void UpdateUI(VehicleDefinition definition)
        {
            if (definition == null)
                return;

            nameText?.SetText(UIHelper.FormatVehicleNameText(definition, nameDisplayMode));
            descriptionText?.SetText(definition.description);
            photo?.SetSprite(definition.previewPhoto);

            priceText?.SetText(UIHelper.FormatItemPriceText(definition));

            manufacturerText?.SetText(definition.manufacturer?.displayName ?? "");
            manufacturerIcon?.SetSprite(definition.manufacturer?.icon ?? null);

            countryText?.SetText(definition.manufacturer?.country.name ?? "");
            country?.SetSprite(definition.manufacturer?.country.flag ?? null);

            classText?.SetText(definition.vehicleClass?.displayName ?? "");
            classIcon?.SetSprite(definition.vehicleClass?.icon ?? null);

            manufacturerIcon?.DisableIfNullSprite();
            country?.DisableIfNullSprite();
            classIcon?.DisableIfNullSprite();

            speedStat?.SetValue(definition.defaultStats.speed);
            accelerationStat?.SetValue(definition.defaultStats.acceleration);
            brakeStat?.SetValue(definition.defaultStats.braking);
            handlingStat?.SetValue(definition.defaultStats.handling);
        }
    }
}