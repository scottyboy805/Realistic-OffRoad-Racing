using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RGSK
{
    public class OnEnableEventTrigger : MonoBehaviour
    {
        public Action OnEnableEvent;
        public Action OnDisableEvent;

        void OnEnable()
        {
            OnEnableEvent?.Invoke();
        }

        void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }
    }
}