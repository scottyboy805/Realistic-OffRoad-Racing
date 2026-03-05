using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceWaypointArrow : ArrowPointer
    {
        void OnEnable()
        {
            RGSKEvents.OnHitCheckpoint.AddListener(OnHitCheckpoint);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnHitCheckpoint.RemoveListener(OnHitCheckpoint);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        void Start()
        {
            if (TryGetComponent<Canvas>(out var c))
            {
                c.worldCamera = CameraManager.Instance?.Camera?.OutputCamera;
            }
        }

        void OnHitCheckpoint(CheckpointNode cp, Competitor c) => UpdateTarget();
        void OnRaceRestart() => UpdateTarget();

        void OnRaceStateChanged(RaceState state)
        {
            ToggleVisible(state == RaceState.Racing);

            if (state == RaceState.Racing)
            {
                UpdateTarget();
            }
        }

        void UpdateTarget()
        {
            var e = GeneralHelper.GetFocusedEntity();
            SetTarget(e?.Competitor?.NextCheckpoint?.transform ?? null);
        }
    }
}