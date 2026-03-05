using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class DigitalGauge : Gauge
    {
        [SerializeField] Image fillImage;

        public override void SetValue(float value)
        {
            if (fillImage == null)
                return;

            base.SetValue(value);
            fillImage.fillAmount = _reading;
        }
    }
}