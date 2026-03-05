using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class VehicleSelectScreen : UIScreen
    {
        [Header("Vehicle Select")]
        [SerializeField] SelectionItemEntry entryPrefab;
        [SerializeField] ScrollRect vehicleScrollView;
        [SerializeField] VehicleDefinitionUI vehicleDefinitionUI;
        [SerializeField] Button selectButton;
        [SerializeField] bool createLockedItemEntries = true;
        [SerializeField] bool destroyVehicleOnClose = true;

        [Header("Color Select")]
        [SerializeField] SelectionItemEntry colorEntryPrefab;
        [SerializeField] ScrollRect colorScrollView;

        public UIScreenID vehicleStoreScreen;

        public UnityEvent OnSelected;

        MenuVehicleSpawner _vehicleSpawner;
        [HideInInspector] public VehicleDefinition _selected;
        bool _inColorSelect;

        public override void Initialize()
        {
            PopulateVehicleSelection();

            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() => Select());
            }
            base.Initialize();
        }

        public override void Open()
        {
            base.Open();
            ToggleColorSelection(false);
        }

        public override void Close()
        {
            base.Close();

            // Restore previous vehicle
            if (_selected != null && _selected != RGSKCore.runtimeData.SelectedVehicle)
                UpdateVehicle(RGSKCore.runtimeData.SelectedVehicle);

            _selected = null;

            if (destroyVehicleOnClose)
            {
                _vehicleSpawner?.DestroyActiveVehicle();
            }
        }

        public override void Back()
        {
            if (!_inColorSelect)
            {
                base.Back();
                return;
            }

            ToggleColorSelection(false);
        }

        void Select()
        {
            if (!_selected.IsUnlocked())
            {
                ModalWindowManager.Instance.Show(new ModalWindowProperties
                {
                    header = "Visit Store",
                    message = "This vehicle must be purchased from the store. Do you want to go to the store now?",
                    confirmButtonText = "Yes",
                    declineButtonText = "No",
                    confirmAction = () => { vehicleStoreScreen.Open(); },
                    declineAction = () => { }
                });

                //GeneralHelper.PurchaseItem(
                //    item: _selected,
                //    OnSuccess: () =>
                //    {
                //        var current = _selected;
                //        _selected = null;
                //        UpdateVehicle(current);
                //    },
                //    OnFail: () => { }
                //    );

                return;
            }

            if (!_selected.IsUnlocked())
                return;

            if (_inColorSelect)
            {
                OnSelected.Invoke();
                return;
            }

            //RGSKCore.runtimeData.SelectedVehicle = _selected;
            //SaveData.Instance.playerData.selectedVehicleID = _selected.ID;

            if (CanSelectColors())
            {
                PopulateColorSelection();
                ToggleColorSelection(true);
            }
            else if (CanSelectLiveries())
            {
                PopulateLiverySelection();
                ToggleColorSelection(true);
            }
            else
            {
                OnSelected.Invoke();
            }
        }

        void UpdateVehicle(VehicleDefinition definition)
        {
            if (_selected == definition)
                return;

            if (_vehicleSpawner == null)
            {
                _vehicleSpawner = FindAnyObjectByType<MenuVehicleSpawner>();
            }

            _selected = definition;
            vehicleDefinitionUI?.UpdateUI(_selected);
            _vehicleSpawner?.Spawn(_selected);
        }

        void ToggleColorSelection(bool toggle)
        {
            if (RGSKCore.Instance.ContentSettings.vehicles.Count == 0 || vehicleScrollView == null || colorScrollView == null)
                return;

            vehicleScrollView.gameObject.SetActive(!toggle);
            colorScrollView.gameObject.SetActive(toggle);

            if (toggle)
            {
                var index = 0;

                if (_vehicleSpawner?.VehicleInstance() != null)
                {
                    index = GeneralHelper.GetVehicleColorIndex(_vehicleSpawner.VehicleInstance(), 0);
                }

                StartCoroutine(colorScrollView.SelectChild(index));
            }
            else
            {
                var index = 0;

                if (RGSKCore.runtimeData.SelectedVehicle != null)
                {
                    index = RGSKCore.Instance.ContentSettings.vehicles.IndexOf(RGSKCore.runtimeData.SelectedVehicle);

                    if (index < 0)
                    {
                        index = 0;
                    }
                }

                UpdateVehicle(RGSKCore.Instance.ContentSettings.vehicles[index]);
                StartCoroutine(vehicleScrollView.SelectChild(index));
                colorScrollView.content.gameObject.DestroyAllChildren();
            }

            _inColorSelect = toggle;
        }

        void PopulateVehicleSelection()
        {
            if (vehicleScrollView == null || entryPrefab == null)
                return;

            vehicleScrollView.content.gameObject.DestroyAllChildren();

            foreach (var item in RGSKCore.Instance.ContentSettings.vehicles)
            {
                if (!createLockedItemEntries && !item.IsUnlocked())
                    continue;

                if (item != null)
                {
                    var e = Instantiate(entryPrefab, vehicleScrollView.content);
                    e.Setup(
                    item: item,
                    col: Color.white,
                    onSelect: () =>
                    {
                        UpdateVehicle(item);
                    },
                    onClick: () =>
                    {
                        if (GeneralHelper.IsMobilePlatform())
                        {
                            UpdateVehicle(item);
                        }
                        else
                        {
                            Select();
                        }
                    });
                }
            }
        }

        void PopulateColorSelection()
        {
            var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;

            if (colorScrollView == null || colorEntryPrefab == null || colors == null)
                return;

            foreach (var c in colors.colors)
            {
                var e = Instantiate(colorEntryPrefab, colorScrollView.content);

                e.Setup(
                text: "",
                img: null,
                col: c,
                onSelect: () =>
                {
                    _vehicleSpawner?.UpdateColor(c);
                },
                onClick: () =>
                {
                    if (GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateColor(c);
                    }
                    else
                    {
                        Select();
                    }
                });
            }
        }

        void PopulateLiverySelection()
        {
            var liveries = GeneralHelper.GetVehicleLiveries(_vehicleSpawner.VehicleInstance());

            if (colorScrollView == null || colorEntryPrefab == null || liveries == null)
                return;

            foreach (var l in liveries.liveries)
            {
                var e = Instantiate(colorEntryPrefab, colorScrollView.content);

                e.Setup(
                text: "",
                img: l.preview,
                col: l.preview == null ? l.previewColor : Color.white,
                onSelect: () =>
                {
                    _vehicleSpawner?.UpdateLivery(l.texture);
                },
                onClick: () =>
                {
                    if (GeneralHelper.IsMobilePlatform())
                    {
                        _vehicleSpawner?.UpdateLivery(l.texture);
                    }
                    else
                    {
                        Select();
                    }
                });
            }
        }

        bool CanSelectColors()
        {
            if (_vehicleSpawner?.VehicleInstance() != null)
            {
                var colors = RGSKCore.Instance.VehicleSettings.vehicleColorList;
                return GeneralHelper.CanApplyColor(_vehicleSpawner.VehicleInstance()) && colors != null && colors.colors.Count > 0;
            }

            return false;
        }

        bool CanSelectLiveries()
        {
            if (_vehicleSpawner?.VehicleInstance() != null)
            {
                var liveries = GeneralHelper.GetVehicleLiveries(_vehicleSpawner.VehicleInstance());
                return liveries != null && liveries.liveries.Count > 0;
            }

            return false;
        }
    }
}