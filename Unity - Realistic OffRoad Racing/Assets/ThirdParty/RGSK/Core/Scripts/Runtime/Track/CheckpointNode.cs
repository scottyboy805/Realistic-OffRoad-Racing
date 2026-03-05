using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class CheckpointNode : RouteNode
    {
        [EnumFlags][SerializeField] CheckpointType type;
        [SerializeField] float height = 20;
        [SerializeField] float timeExtend = 30;

        public CheckpointNode NextCheckpoint => (CheckpointNode)nextNode;
        public CheckpointNode PreviousCheckpoint => (CheckpointNode)previousNode;

        public bool IsSector => EnumFlags.GetSelectedIndexes(type).Contains((int)CheckpointType.Sector);
        public bool IsTimeExtend => EnumFlags.GetSelectedIndexes(type).Contains((int)CheckpointType.TimeExtend);
        public bool IsSpeedtrap => EnumFlags.GetSelectedIndexes(type).Contains((int)CheckpointType.Speedtrap);
        public float TimeExtend => timeExtend;

        public void Setup(RaceSession session)
        {
            var trigger = gameObject.GetOrAddComponent<BoxCollider>();
            var totalWidth = leftWidth + rightWidth;
            var centerX = leftWidth - (totalWidth / 2);
            trigger.isTrigger = true;
            trigger.size = new Vector3(totalWidth, height, 1);
            trigger.center = new Vector3(-centerX, 0, 0);
        }

        void OnTriggerEnter(Collider other)
        {
            var entity = other.GetComponentInParent<RGSKEntity>();
            if (entity != null && entity.Competitor != null)
            {
                entity.Competitor.HitCheckpoint(this);
            }
        }

        public bool AllowTimeExtend()
        {
            if (RaceManager.Instance.Initialized)
            {
                return IsTimeExtend && RaceManager.Instance.Session.UseSeparateTimers();
            }

            return IsTimeExtend;
        }

        public bool AllowSpeedtrap()
        {
            if (RaceManager.Instance.Initialized)
            {
                return IsSpeedtrap && RaceManager.Instance.Session.GetPositioningMode() == RacePositioningMode.TotalSpeed;
            }

            return IsSpeedtrap;
        }
    }
}