using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceScreen : RaceScreenBase
    {
        [SerializeField] GameObject wrongwayContainer;
        [SerializeField] GameObject sessionTimeContainer;

        protected override void OnEnable()
        {
            base.OnEnable();
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        protected override void Start()
        {
            base.Start();

            if (RaceManager.Instance.Initialized)
            {
                var layout = RGSKCore.Instance.UISettings.raceUILayouts.FirstOrDefault(x => x.raceTypes.Contains(RaceManager.Instance.Session.raceType));
                var prefab = layout ?? RGSKCore.Instance.UISettings.defaultRaceUILayout;

                if (prefab != null)
                {
                    Instantiate(prefab, transform);
                }
            }

            if (wrongwayContainer != null)
            {
                wrongwayContainer.SetActive(false);
            }

            if (sessionTimeContainer != null)
            {
                sessionTimeContainer.SetActive(RaceManager.Instance?.Session?.IsTimedSession() ?? false);
            }
        }

        protected override void OnWrongwayStart(Competitor c)
        {
            if (c != GeneralHelper.GetFocusedEntity().Competitor)
                return;

            if (wrongwayContainer != null)
            {
                wrongwayContainer.SetActive(true);
            }
        }

        protected override void OnWrongwayStop(Competitor c)
        {
            if (c != GeneralHelper.GetFocusedEntity().Competitor)
                return;

            if (wrongwayContainer != null)
            {
                wrongwayContainer.SetActive(false);
            }
        }

        void OnCameraTargetChanged(Transform target)
        {
            if (wrongwayContainer != null)
            {
                wrongwayContainer.SetActive(false);
            }
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnRaceRestart() { }
    }
}