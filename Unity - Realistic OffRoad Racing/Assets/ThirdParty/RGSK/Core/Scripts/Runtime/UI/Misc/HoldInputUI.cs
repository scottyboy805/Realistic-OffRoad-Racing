using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RGSK.Extensions;

namespace RGSK
{
    public class HoldInputUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup group;
        [SerializeField] Gauge dial;

        float _holdTime;

        public virtual void Start()
        {
            _holdTime = InputSystem.settings.defaultHoldTime;
            group?.SetAlpha(0);
        }

        protected virtual void OnInputStart()
        {
            StartCoroutine(InputProgressUIRoutine());
            group?.SetAlpha(1);
        }

        protected virtual void OnInputCancel()
        {
            StopAllCoroutines();
            group?.SetAlpha(0);
        }

        protected virtual void OnInputPerformed()
        {
            StopAllCoroutines();
            group?.SetAlpha(0);
        }

        IEnumerator InputProgressUIRoutine()
        {
            var timer = 0f;

            while (timer < _holdTime)
            {
                timer += Time.deltaTime;
                dial.SetValue(Mathf.Lerp(0, 1, timer / _holdTime));
                yield return null;
            }
        }
    }
}
