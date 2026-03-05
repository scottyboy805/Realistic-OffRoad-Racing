using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class AnalogGauge : Gauge
    {
        [SerializeField] Transform needle;

        public override void SetValue(float value)
        {
            if (needle == null)
                return;

            base.SetValue(value);
            var rot = needle.localEulerAngles;
            rot.z = -_reading;
            needle.localEulerAngles = rot;
        }
    }
}