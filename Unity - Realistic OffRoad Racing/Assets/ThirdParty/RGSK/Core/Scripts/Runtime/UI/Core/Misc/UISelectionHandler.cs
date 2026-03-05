using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class UISelectionHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
    {
        public UnityEvent onSelect;
        public UnityEvent onClick;

        Button _button;
        ScrollRect _scroll;

        void Start()
        {
            if (TryGetComponent<Button>(out _button))
            {
                _button.onClick.AddListener(HandleClick);
            }

            _scroll = GetComponentInParent<ScrollRect>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button != null && _button.interactable)
            {
                _button.Select();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_button != null && _button.interactable)
            {
                _button.Select();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            HandleSelect();
        }

        void HandleSelect()
        {
            AudioManager.Instance?.PlayUIHoverSound();
            ScrollViewFocus();
            onSelect.Invoke();
        }

        void HandleClick()
        {
            AudioManager.Instance?.PlayUIClickSound();
            onClick.Invoke();
        }

        void ScrollViewFocus()
        {
            //Don't auto focus if a mouse or mobile is being used
            if (InputManager.Instance != null && InputManager.Instance.ActiveInputDevice != null)
            {
                if (InputManager.Instance.ActiveInputDevice.displayName.Contains("Mouse") || GeneralHelper.IsMobilePlatform())
                {
                    return;
                }
            }

            _scroll?.FocusOnItem(GetComponent<RectTransform>());
        }
    }
}