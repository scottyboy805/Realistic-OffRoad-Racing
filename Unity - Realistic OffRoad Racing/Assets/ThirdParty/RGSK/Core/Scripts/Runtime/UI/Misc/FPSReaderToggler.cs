using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class FPSReaderToggler : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        void Update()
        {
            if (panel == null)
                return;

            panel.SetActiveSafe(RGSKCore.Instance.GeneralSettings.enableFpsReader);
        }
    }
}