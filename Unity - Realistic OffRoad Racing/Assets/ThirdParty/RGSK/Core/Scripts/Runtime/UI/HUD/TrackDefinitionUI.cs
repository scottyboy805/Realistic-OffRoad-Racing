using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class TrackDefinitionUI : MonoBehaviour
    {
        public TrackDefinition track;
        public Image photo;
        public Image icon;
        public Image country;
        public Image minimap;
        public TMP_Text nameText;
        public TMP_Text countryText;
        public TMP_Text descriptionText;
        public TMP_Text priceText;
        public TMP_Text lengthText;
        public TMP_Text gridSizeText;
        public TMP_Text bestLapText;
        public GameObject locked;

        void Start()
        {
            if (track != null)
            {
                UpdateUI(track);
            }
        }

        public void UpdateUI(TrackDefinition definition)
        {
            if (definition == null)
                return;

            nameText?.SetText(definition.objectName);
            descriptionText?.SetText(definition.description);
            priceText?.SetText(UIHelper.FormatItemPriceText(definition));

            lengthText?.SetText(UIHelper.FormatDistanceText(definition.length));
            gridSizeText?.SetText(definition.gridSlots.ToString());
            bestLapText?.SetText(UIHelper.FormatTimeText(definition.LoadBestLap()));
            countryText?.SetText(definition.country?.name ?? "");

            photo?.SetSprite(definition.previewPhoto);
            icon?.SetSprite(definition.icon);
            country?.SetSprite(definition.country?.flag);
            minimap?.SetSprite(definition.minimapPreview);

            photo?.DisableIfNullSprite();
            icon?.DisableIfNullSprite();
            country?.DisableIfNullSprite();
            minimap?.DisableIfNullSprite();

            if (locked != null)
            {
                locked.SetActive(!definition.IsUnlocked());
            }
        }
    }
}