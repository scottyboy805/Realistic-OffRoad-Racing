using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class ProfileDefinitionUI : MonoBehaviour
    {
        public ProfileDefinition profile;
        public NameDisplayMode nameDisplayMode;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public Image country;
        public Image avatar;

        void Start()
        {
            if (profile != null)
            {
                UpdateUI(profile);
            }
        }

        public void UpdateUI(ProfileDefinition definition)
        {
            if (definition == null)
                return;

            nameText?.SetText(UIHelper.FormatNameText(definition, nameDisplayMode));
            descriptionText?.SetText(definition.description);

            country?.SetSprite(definition?.nationality.flag ?? null);
            avatar?.SetSprite(definition.avatar);

            country?.DisableIfNullSprite();
            avatar?.DisableIfNullSprite();

            profile = definition;
        }
    }
}