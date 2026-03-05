using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class GameSettingsProfileEntryUI : GameSettingsEntryUI
    {
        public enum ProfileSettingType
        {
            FirstName,
            LastName,
            Nationality,
        }

        [SerializeField] ProfileSettingType type;
        [SerializeField] TMP_InputField nameField;
        [SerializeField] Image country;

        ProfileDefinition _playerProfile;
        CountrySettings _countries;
        TabController _tabController;
        int _countryIndex;

        protected override void Start()
        {
            _tabController = GetComponentInParent<TabController>();
            _playerProfile = RGSKCore.Instance.GeneralSettings.playerProfile;
            _countries = RGSKCore.Instance.GeneralSettings.countrySettings;

            if (_playerProfile != null)
            {
                if (nameField != null)
                {
                    switch (type)
                    {
                        case ProfileSettingType.FirstName:
                            {
                                nameField.text = UIHelper.FormatNameText(_playerProfile, NameDisplayMode.FirstName);
                                nameField.onValueChanged.AddListener(x => _playerProfile.firstName = nameField.text);
                                break;
                            }

                        case ProfileSettingType.LastName:
                            {
                                nameField.text = UIHelper.FormatNameText(_playerProfile, NameDisplayMode.LastName);
                                nameField.onValueChanged.AddListener(x => _playerProfile.lastName = nameField.text);
                                break;
                            }
                    }

                    nameField.onEndEdit.AddListener((x) => ToggleTabController(true));
                    nameField.onSelect.AddListener((x) => ToggleTabController(false));
                }

                if (_countries != null)
                {
                    _countryIndex = _countries.GetCountryIndex(_playerProfile.nationality);
                }
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(type.ToString());
            }

            base.Start();
        }

        public override void SelectOption(int direction)
        {
            switch (type)
            {
                case ProfileSettingType.Nationality:
                    {
                        SetNationality(direction);
                        break;
                    }
            }
        }

        void SetNationality(int direction)
        {
            if (_countries == null || _playerProfile == null)
                return;

            var count = _countries.countries.Count;
            _countryIndex = GeneralHelper.ValidateIndex(_countryIndex + direction, 0, count, loop);
            _playerProfile.nationality = _countries.GetCountryIndex(_countryIndex);

            if (country != null)
            {
                country.sprite = _playerProfile.nationality.flag;
            }

            valueText?.SetText(_playerProfile.nationality.name);
        }

        void ToggleTabController(bool enable)
        {
            if (_tabController != null)
            {
                _tabController.enabled = enable;
            }
        }
    }
}