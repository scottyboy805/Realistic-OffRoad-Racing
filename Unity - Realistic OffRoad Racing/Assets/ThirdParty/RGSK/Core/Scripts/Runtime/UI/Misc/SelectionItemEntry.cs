using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class SelectionItemEntry : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] GameObject lockedPanel;
        [SerializeField] TMP_Text unlockText;

        ItemDefinition _item;

        public void Setup(ItemDefinition item, Color col, Action onSelect, Action onClick)
        {
            _item = item;
            Setup("", null, col, onSelect, onClick);
            Refresh();
        }

        public void Setup(string text, Sprite img, Color col, Action onSelect, Action onClick)
        {
            nameText?.SetText(text);

            if (image != null)
            {
                image.sprite = img;
                image.color = col;
            }

            if (lockedPanel != null)
            {
                lockedPanel.SetActive(false);
            }

            if (TryGetComponent<UISelectionHandler>(out var _selectHandler))
            {
                _selectHandler.onSelect.AddListener(() =>
                {
                    onSelect?.Invoke();
                });

                _selectHandler.onClick.AddListener(() =>
                {
                    onClick?.Invoke();
                });
            }
            else
            {
                Logger.LogWarning(gameObject, "A UISelectionHandler component was not found on this entry! OnSelect and OnClick callbacks will not work.");
            }
        }

        public virtual void Refresh()
        {
            if (_item == null)
                return;

            if (image != null)
            {
                image.sprite = _item.previewPhoto;
            }

            if (icon != null)
            {
                icon.sprite = _item.icon;
                icon.DisableIfNullSprite();
            }

            nameText?.SetText(_item.objectName);
            descriptionText?.SetText(_item.description);
            unlockText?.SetText(UIHelper.FormatItemPriceText(_item));
        }

        public virtual void Update()
        {
            if (lockedPanel != null)
            {
                lockedPanel.SetActiveSafe(_item != null && !_item.IsUnlocked());
            }
        }
    }
}