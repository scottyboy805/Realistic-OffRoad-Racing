using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RGSK.Extensions;

namespace RGSK
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        public UIScreenID screenID;
        public Selectable startSelectable;
        public float fadeDuration = 0f;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        public UIScreen PreviousScreen { get; set; }

        CanvasGroup _canvasGroup;
        OnEnableEventTrigger _seletableEventTrigger;

        protected virtual void OnEnable()
        {
            UIManager.Instance?.RegisterScreen(this);
        }

        protected virtual void OnDisable()
        {
            UIManager.Instance?.UnregisterScreen(this);
        }

        public virtual void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.SetAlpha(0);

            if (startSelectable != null)
            {
                _seletableEventTrigger = startSelectable.gameObject.AddComponent<OnEnableEventTrigger>();
            }
        }

        public virtual void Open()
        {
            if (IsOpen())
                return;

            StartCoroutine(_canvasGroup.Fade(fadeDuration, 1));

            EventSystem.current?.SetSelectedGameObject(null);
            SelectStartElement();

            if (_seletableEventTrigger != null)
            {
                _seletableEventTrigger.OnEnableEvent += SelectStartElement;
            }

            if (screenID != null)
            {
                InputManager.Instance?.SetInputMode(screenID.onOpenInputMode);
            }

            OnOpen.Invoke();
        }

        public virtual void Close()
        {
            StopAllCoroutines();
            _canvasGroup.SetAlpha(0);
            EventSystem.current?.SetSelectedGameObject(null);

            if (_seletableEventTrigger != null)
            {
                _seletableEventTrigger.OnEnableEvent -= SelectStartElement;
            }

            OnClose.Invoke();
        }

        public virtual void Back()
        {
            if (PreviousScreen != null)
            {
                UIManager.Instance.OpenScreen(PreviousScreen.screenID, false);
            }
        }

        public bool IsOpen()
        {
            if (_canvasGroup == null || gameObject == null)
                return false;

            return _canvasGroup.alpha > 0 && gameObject.activeInHierarchy;
        }

        public void SelectStartElement()
        {
            if (startSelectable != null)
            {
                startSelectable.Select();
                startSelectable.OnSelect(null);
            }
        }
    }
}