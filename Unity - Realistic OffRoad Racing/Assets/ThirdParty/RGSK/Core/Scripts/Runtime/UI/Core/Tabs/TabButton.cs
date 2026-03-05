using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RGSK
{
    public class TabButton : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] Tab tab;
        [SerializeField] Graphic targetGraphic;
        [SerializeField] Color selectedColor = new Color(1, 0, 0, 1);
        [SerializeField] Color deselectedColor = new Color(0, 0, 0, 0);
        [SerializeField] Color hoverColor = new Color(1, 0, 0, 0.5f);

        TabController _controller;
        bool _active;
        bool _init;

        void OnEnable()
        {
            _controller?.RegisterTabButton(this);
        }

        void OnDisable()
        {
            _controller?.UnregisterTabButton(this);
            ToggleActive(false);
        }

        public void Setup(TabController controller)
        {
            if (_init)
                return;

            _controller = controller;

            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => _controller.SelectTab(this));
                btn.transition = Selectable.Transition.None;

                var navigation = btn.navigation;
                navigation.mode = Navigation.Mode.None;
                btn.navigation = navigation;
            }

            if (tab != null)
            {
                tab.gameObject.SetActive(true);
            }

            _init = true;
        }

        public void ToggleActive(bool active)
        {
            if (targetGraphic != null)
            {
                targetGraphic.color = active ? selectedColor : deselectedColor;
            }

            if (tab != null)
            {
                if (active)
                {
                    tab.Open();
                }
                else
                {
                    tab.Close();
                }
            }

            _active = active;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!_active)
            {
                if (targetGraphic != null)
                {
                    targetGraphic.color = hoverColor;
                }
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!gameObject.activeSelf)
                return;

            _controller?.SelectTab(this);
            AudioManager.Instance?.PlayUIHoverSound();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _controller?.SelectTab(this);
            AudioManager.Instance?.PlayUIHoverSound();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_active)
            {
                if (targetGraphic != null)
                {
                    targetGraphic.color = deselectedColor;
                }
            }
        }
    }
}