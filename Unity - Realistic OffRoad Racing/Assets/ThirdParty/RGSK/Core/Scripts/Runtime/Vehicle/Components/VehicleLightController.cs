using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class VehicleLightController : VehicleComponent
    {
        public bool HeadlightsOn => _headlightsOn;

        VehicleLight[] _lights;
        bool _headlightsOn;

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            _lights = Vehicle.GetComponentsInChildren<VehicleLight>();

            foreach (var l in _lights)
            {
                l.SetIntensity(0);
            }
        }

        public override void Update()
        {
            foreach (var l in _lights)
            {
                if (l.type == VehicleLightType.TailLight)
                {
                    l.SetIntensity(Vehicle.BrakeInput >= 0.1f ? 1 : !_headlightsOn ? 0 : 0.5f);
                }

                if (l.type == VehicleLightType.ReverseLight)
                {
                    l.SetIntensity(Vehicle.engine.Gear == 0 ? 1 : 0);
                }
            }
        }

        public void ToggleHeadlights(bool on)
        {
            _headlightsOn = on;

            foreach (var l in _lights)
            {
                if (l.type == VehicleLightType.HeadLight)
                {
                    l.SetIntensity(_headlightsOn ? 1 : 0);
                }
            }
        }

        public void ToggleHeadlights()
        {
            _headlightsOn = !_headlightsOn;
            ToggleHeadlights(_headlightsOn);
        }
    }
}