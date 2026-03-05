using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using UnityEngine.EventSystems;
using TMPro;

namespace RGSK
{
    public class ReleaseInputFieldOnGamepad : MonoBehaviour
    {
        TMP_InputField _inputField;
        UISelectionHandler _selectionHandler;

        void Start()
        {
            _inputField = GetComponent<TMP_InputField>();
            _selectionHandler = GetComponentInParent<UISelectionHandler>();
        }

        void Update()
        {
            if (_inputField != null &&
                _selectionHandler != null &&
                _inputField.isFocused &&
                InputHelper.GamepadButtonOrAxisPressed())
            {
                _inputField.ReleaseSelection();
                EventSystem.current.SetSelectedGameObject(_selectionHandler.gameObject);
            }
        }
    }
}