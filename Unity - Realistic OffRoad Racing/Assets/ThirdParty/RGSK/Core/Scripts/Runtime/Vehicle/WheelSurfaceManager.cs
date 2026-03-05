using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class WheelSurfaceManager : Singleton<WheelSurfaceManager>
    {
        public List<WheelSurface> surfaces => RGSKCore.Instance.VehicleSettings.wheelSurfaces;
        Dictionary<WheelSurface, TyremarkGenerator> _tyremarks = new Dictionary<WheelSurface, TyremarkGenerator>();

        void OnEnable()
        {
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        void OnDisable()
        {
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);
        }

        public override void Awake()
        {
            base.Awake();

            foreach (var s in surfaces)
            {
                if (s.tyremarksPrefab != null)
                {
                    var tm = Instantiate(s.tyremarksPrefab, transform);
                    _tyremarks.Add(s, tm);
                }
            }
        }

        public WheelSurface GetSurface(PhysicsMaterial mat)
        {
            var surface = surfaces.FirstOrDefault(x => x.physicMaterial == mat);

            if (surface == null)
            {
                surface = RGSKCore.Instance.VehicleSettings.fallbackWheelSurface;
            }

            return surface;
        }

        public WheelSurface GetSurface(Texture2D tex)
        {
            var surface = surfaces.FirstOrDefault(x => x.terrainTextures.FirstOrDefault(y => y == tex));

            if (surface == null)
            {
                surface = RGSKCore.Instance.VehicleSettings.fallbackWheelSurface;
            }

            return surface;
        }

        public TyremarkGenerator GetTyremarksForSurface(WheelSurface surface)
        {
            if (_tyremarks.TryGetValue(surface, out var v))
            {
                return v;
            }

            return null;
        }

        [ContextMenu("Clear Tyremarks")]
        public void ClearTyremarks()
        {
            foreach (var m in _tyremarks.Values)
            {
                m.Clear();
            }
        }

        public void ToggleTyremarks(bool toggle)
        {
            foreach (var m in _tyremarks.Values)
            {
                m.Clear();
                m.enabled = toggle;
            }
        }

        void OnReplayStart()
        {
            ToggleTyremarks(false);
        }

        void OnReplayStop()
        {
            ToggleTyremarks(true);
        }
    }
}