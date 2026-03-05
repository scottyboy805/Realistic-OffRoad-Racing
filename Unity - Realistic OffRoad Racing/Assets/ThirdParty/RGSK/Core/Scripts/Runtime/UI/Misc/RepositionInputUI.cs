using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RepositionInputUI : HoldInputUI
    {
        void OnEnable()
        {
            InputManager.RepositionStartEvent += OnInputStart;
            InputManager.RepositionCancelEvent += OnInputCancel;
            InputManager.RepositionPerformedEvent += OnInputPerformed;
        }

        void OnDisable()
        {
            InputManager.RepositionStartEvent -= OnInputStart;
            InputManager.RepositionCancelEvent -= OnInputCancel;
            InputManager.RepositionPerformedEvent -= OnInputPerformed;
        }
    }
}