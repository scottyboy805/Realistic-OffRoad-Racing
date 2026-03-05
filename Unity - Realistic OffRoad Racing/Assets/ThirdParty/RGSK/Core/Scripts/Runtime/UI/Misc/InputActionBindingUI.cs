using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class InputActionBindingUI : MonoBehaviour
    {
        [SerializeField] InputActionReference inputAction;
        [SerializeField] int bindIndex;

        [Header("UI")]
        [SerializeField] TMP_Text actionText;
        [SerializeField] TMP_Text bindingText;
        [SerializeField] string actionString = "{0}";
        [SerializeField] string bindingString = "{0}";

        [Header("Rebinding")]
        [SerializeField] bool rebindable = false;
        [SerializeField] string expectedControlType = "Button";
        [SerializeField] GameObject rebindWaitPanel;

        [Header("Binding Group")]
        [SerializeField] BindingGroup bindingGroup;
        [SerializeField] bool autoUpdateBindingGroup = true;

        [Header("Gamepad Icons")]
        [SerializeField] GamepadIconDefinition gamepadIcons;
        [SerializeField] Image gamepadIcon;
        [SerializeField] bool useGamepadIcons = true;

        InputActionRebindingExtensions.RebindingOperation _operation;
        InputMode _prevInputMode;

        void OnEnable()
        {
            RGSKEvents.OnInputDeviceChanged.AddListener(OnInputDeviceChanged);
            RGSKEvents.OnInputBindingsChanged.AddListener(OnInputBindingsChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnInputDeviceChanged.RemoveListener(OnInputDeviceChanged);
            RGSKEvents.OnInputBindingsChanged.RemoveListener(OnInputBindingsChanged);
        }

        void Awake()
        {
            ToggleRebindWaitPanel(false);
            OnInputDeviceChanged(InputManager.Instance.ActiveInputDevice);

            if(rebindable)
            {
                if (TryGetComponent<Button>(out var btn))
                {
                    btn.onClick.AddListener(StartRebind);
                }
            }
        }

        void Update()
        {
            if (_operation != null && _operation.started)
            {
                switch (bindingGroup)
                {
                    case BindingGroup.Keyboard:
                        {
                            if (InputHelper.GamepadButtonOrAxisPressed())
                            {
                                _operation.Cancel();
                            }
                            break;
                        }

                    case BindingGroup.Gamepad:
                        {
                            if (InputHelper.KeyboardPressed())
                            {
                                _operation.Cancel();
                            }
                            break;
                        }
                }
            }
        }

        void OnInputDeviceChanged(InputDevice device)
        {
            if (inputAction == null)
                return;

            var action = InputManager.Instance.FindInputActionFromReference(inputAction);
            SetActionText(action.name.InsertSpacesBeforeCapitals());
            SetIcon(null);

            if (GeneralHelper.IsMobilePlatform())
            {
                SetBindingText("");
                return;
            }

            if (autoUpdateBindingGroup)
            {
                bindingGroup = InputManager.Instance.ActiveController == InputController.MouseAndKeyboard ?
                                BindingGroup.Keyboard :
                                BindingGroup.Gamepad;

                SetBindingText($"{GetBindingText(action, bindingGroup.ToString())}");

                if (bindingGroup == BindingGroup.Gamepad && useGamepadIcons)
                {
                    var icon = GetGamepadIcon(action, InputManager.Instance.ActiveController);
                    if(icon != null)
                    {
                        SetBindingText("");
                        SetIcon(icon);
                    }
                }
            }
            else
            {
                SetBindingText($"{GetBindingText(action, bindingGroup.ToString())}");

                switch (bindingGroup)
                {
                    case BindingGroup.Keyboard:
                        {
                            break;
                        }

                    case BindingGroup.Gamepad:
                        {
                            if (useGamepadIcons)
                            {
                                var controller = InputManager.Instance.ActiveController;

                                if (controller == InputController.MouseAndKeyboard)
                                {
                                    controller = InputController.Xbox;
                                }

                                var icon = GetGamepadIcon(action, controller);
                                if(icon != null)
                                {
                                    SetBindingText("");
                                    SetIcon(icon);
                                }
                            }
                            break;
                        }
                }
            }
        }

        void OnInputBindingsChanged() => OnInputDeviceChanged(InputManager.Instance.ActiveInputDevice);

        public void SetActionText(string value)
        {
            actionText?.SetText(string.Format(actionString, value));
        }

        public void SetBindingText(string value)
        {
            bindingText?.SetText(!string.IsNullOrWhiteSpace(value) ? string.Format(bindingString, value) : value);
        }
        
        public void StartRebind()
        {
            var action = InputManager.Instance.FindInputActionFromReference(inputAction);
            PerformRebind(action);
        }

        void PerformRebind(InputAction action)
        {
            _operation?.Cancel();
            _prevInputMode = InputManager.Instance.ActiveInputMode;

            InputManager.Instance.SetInputMode(InputMode.Disabled);
            InputManager.Instance.ToggleUIInputModule(false);

            var index = action.GetBindingIndex(InputBinding.MaskByGroup(bindingGroup.ToString()));

            if (index != -1)
            {
                if (action.bindings[index].isPartOfComposite)
                {
                    index += bindIndex;
                }
            }

            _operation = action.PerformInteractiveRebinding(index).
                OnMatchWaitForAnother(0.1f).
                WithExpectedControlType(expectedControlType).
                WithCancelingThrough(bindingGroup == BindingGroup.Keyboard ? "<Keyboard>/escape" : "<Gamepad>/start").
                WithControlsExcluding("<Mouse>").
                WithControlsExcluding("<Touch>").
                WithControlsExcluding(bindingGroup == BindingGroup.Keyboard ? "<Gamepad>" : "<Keyboard>").
                WithBindingGroup(bindingGroup.ToString()).
                OnCancel(x => CompleteRebind()).
                OnComplete(x => 
                {
                    InputManager.Instance.RemoveDuplicateBindings(action, index);
                    InputManager.Instance.SaveBindings();
                    CompleteRebind();
                });

            ToggleRebindWaitPanel(true);
            _operation.Start();
        }

        void CompleteRebind()
        {
            InputManager.Instance.SetInputMode(_prevInputMode);
            InputManager.Instance.ToggleUIInputModule(true);
            ToggleRebindWaitPanel(false);
            CleanUp();
        }

        void CleanUp()
        {
            _operation?.Dispose();
            _operation = null;
        }

        void ToggleRebindWaitPanel(bool toggle)
        {
            if (rebindWaitPanel == null)
                return;

            rebindWaitPanel.SetActive(toggle);
        }

        public void SetIcon(Sprite icon)
        {
            if (gamepadIcons == null || gamepadIcon == null)
                return;

            gamepadIcon.sprite = icon;
            gamepadIcon.DisableIfNullSprite();
        }

        string GetBindingText(InputAction a, string mask)
        {
            var index = a.GetBindingIndex(InputBinding.MaskByGroup(mask));

            if (index == -1)
            {
                return null;
            }

            if (a.bindings[index].isPartOfComposite)
            {
                index += bindIndex;
            }

            return a.GetBindingDisplayString(index, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }

        Sprite GetGamepadIcon(InputAction a, InputController controller)
        {
            var index = a.GetBindingIndex(InputBinding.MaskByGroup("Gamepad"));

            if (index == -1)
            {
                return null;
            }

            if (a.bindings[index].isPartOfComposite)
            {
                index += bindIndex;
            }

            var bind = a.GetBindingDisplayString(index, out var deviceLayout, out var controlPath);
            var icons = gamepadIcons.GetIconSet(controller);

            if (icons != null)
            {
                return icons.GetSprite(controlPath);
            }

            return null;
        }
    }
}