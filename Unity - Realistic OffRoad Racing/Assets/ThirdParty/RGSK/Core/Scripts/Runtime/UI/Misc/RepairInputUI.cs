using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RepairInputUI : HoldInputUI
    {
        void OnEnable()
        {
            InputManager.RepairStartEvent += OnInputStart;
            InputManager.RepairCancelEvent += OnInputCancel;
            InputManager.RepairPerformedEvent += OnInputPerformed;
        }

        void OnDisable()
        {
            InputManager.RepairStartEvent -= OnInputStart;
            InputManager.RepairCancelEvent -= OnInputCancel;
            InputManager.RepairPerformedEvent -= OnInputPerformed;
        }
    }
}