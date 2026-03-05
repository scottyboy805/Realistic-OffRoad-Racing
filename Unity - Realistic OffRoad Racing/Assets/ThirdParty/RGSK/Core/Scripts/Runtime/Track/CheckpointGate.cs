using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class CheckpointGate : MonoBehaviour
    {
        public GameObject defaultVisual;
        public GameObject finishVisual;
        public GameObject timeExtendVisual;
        public GameObject speedtrapVisual;
        public ArrowPointer arrowPointer;

        void OnEnable()
        {
            RGSKEvents.OnHitCheckpoint.AddListener(OnHitCheckpoint);
            RGSKEvents.OnCompetitorStarted.AddListener(OnCompetitorStarted);
        }

        void OnDisable()
        {
            RGSKEvents.OnHitCheckpoint.RemoveListener(OnHitCheckpoint);
            RGSKEvents.OnCompetitorStarted.RemoveListener(OnCompetitorStarted);
        }

        void OnHitCheckpoint(CheckpointNode cp, Competitor c)
        {
            if (c == null || c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            UpdateVisual(c);
        }

        void OnCompetitorStarted(Competitor c)
        {
            if (c == null || c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            UpdateVisual(c);
        }

        void UpdateVisual(Competitor c)
        {
            if (!c.IsRacing())
                return;

            var next = c.NextCheckpoint;
            if (next != null)
            {
                transform.SetParent(next.transform, false);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                if (defaultVisual != null)
                {
                    defaultVisual.SetActive(!next.AllowTimeExtend() &&
                                !next.AllowSpeedtrap() &&
                                !c.IsFinalCheckpoint());
                }

                if (timeExtendVisual != null)
                {
                    timeExtendVisual.SetActive(next.AllowTimeExtend());
                }

                if (speedtrapVisual != null)
                {
                    speedtrapVisual.SetActive(next.AllowSpeedtrap());
                }

                if (finishVisual != null)
                {
                    finishVisual.SetActive(c.IsFinalCheckpoint());
                }

                if (arrowPointer != null)
                {
                    arrowPointer.SetTarget(next.NextCheckpoint?.transform);
                    arrowPointer.gameObject.SetActive(!c.IsFinalCheckpoint());
                }
            }
        }
    }
}