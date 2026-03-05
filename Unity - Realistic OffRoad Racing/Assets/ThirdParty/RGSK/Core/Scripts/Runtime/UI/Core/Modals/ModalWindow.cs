using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace RGSK
{
    [System.Serializable]
    public class ModalWindowProperties
    {
        public string header;
        [TextArea(10, 10)] public string message;
        public string confirmButtonText;
        public string declineButtonText;
        public Action confirmAction;
        public Action declineAction;
        public int startSelection;

        [Tooltip("The index of the prefab to use for this modal. The prefab list can be found under RGSK Menu > UI tab.")]
        public int prefabIndex = 0;
    }

    public class ModalWindow : MonoBehaviour
    {
        [SerializeField] TMP_Text headerText;
        [SerializeField] TMP_Text contentText;
        [SerializeField] Button confirmButton;
        [SerializeField] Button declineButton;

        TMP_Text _confirmText, _declineText;
        Action _onConfirm, _onDecline;
        GameObject _lastSelected;

        void Awake()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(() =>
                {
                    ModalWindowManager.Instance.Close();
                    _onConfirm?.Invoke();
                });

                _confirmText = confirmButton.GetComponentInChildren<TMP_Text>();
            }

            if (declineButton != null)
            {
                declineButton.onClick.AddListener(() =>
                {
                    ModalWindowManager.Instance.Close();
                    _onDecline?.Invoke();
                });

                _declineText = declineButton.GetComponentInChildren<TMP_Text>();
            }
        }

        public void Show(ModalWindowProperties properties)
        {
            if (headerText == null || contentText == null || confirmButton == null || declineButton == null)
            {
                Logger.LogWarning(this, "Please assign all text and button references before using the modal window!");
                Close();
                return;
            }

            headerText.text = properties.header;
            contentText.text = properties.message;

            _confirmText.text = properties.confirmButtonText;
            _onConfirm = properties.confirmAction;

            bool canDecline = properties.declineAction != null && !string.IsNullOrWhiteSpace(properties.declineButtonText);
            declineButton.gameObject.SetActive(canDecline);
            _declineText.text = properties.declineButtonText;
            _onDecline = properties.declineAction;

            if (!canDecline)
            {
                properties.startSelection = 0;
            }

            _lastSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(properties.startSelection == 0 ? confirmButton.gameObject : declineButton.gameObject);
            InputManager.Instance?.SetInputMode(InputMode.UI);
        }

        public void Close()
        {
            if (_lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelected);
            }

            InputManager.Instance?.SetInputMode(InputManager.Instance.PreviousInputMode);
            Destroy(gameObject);
        }
    }
}