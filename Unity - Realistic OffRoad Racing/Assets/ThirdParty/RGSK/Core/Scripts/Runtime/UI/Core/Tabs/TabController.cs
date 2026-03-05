using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class TabController : MonoBehaviour
    {
        public bool Initialized => _initialized;

        List<TabButton> _buttons = new List<TabButton>();
        TabButton _selected;
        UIScreen _screen;
        bool _initialized;
        
        void OnEnable()
        {
            InputManager.TabChangedEvent += ChangeTab;
        }

        void OnDisable()
        {
            InputManager.TabChangedEvent -= ChangeTab;
        }

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if(_initialized)
                return;
            
            _screen = GetComponentInParent<UIScreen>();

            foreach (var btn in GetComponentsInChildren<TabButton>())
            {
                RegisterTabButton(btn);
                btn.Setup(this);
            }

            _initialized = true;
        }

        public void RegisterTabButton(TabButton button)
        {
            if (!_buttons.Contains(button))
            {
                _buttons.Add(button);
            }

            _buttons = _buttons.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        }

        public void UnregisterTabButton(TabButton button)
        {
            if (_buttons.Contains(button))
            {
                _buttons.Remove(button);
                ChangeTab(0);
            }

            _buttons = _buttons.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        }

        public void SelectTab(TabButton button)
        {
            if(!_initialized)
            {
                Initialize();
            }
            
            _selected = button;

            foreach (var btn in _buttons)
            {
                btn.ToggleActive(btn == _selected);
            }
        }

        public void ChangeTab(int direction)
        {
            if (_screen != null && !_screen.IsOpen() || ModalWindowManager.Instance.IsOpen())
                return;

            if(!_initialized)
            {
                Initialize();
            }

            var index = 0;
            if (_selected != null)
            {
                index = _buttons.IndexOf(_selected);
            }

            index += direction;
            index = Mathf.Clamp(index, 0, _buttons.Count - 1);

            _buttons[index].OnSubmit(null);
        }
    }
}