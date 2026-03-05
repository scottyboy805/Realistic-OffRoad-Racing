using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class PauseScreen : UIScreen
    {
        public override void Back()
        {
            base.Back();
            PauseManager.Instance?.Unpause();
        }
    }
}