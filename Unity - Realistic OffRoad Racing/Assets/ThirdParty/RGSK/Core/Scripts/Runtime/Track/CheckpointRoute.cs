using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class CheckpointRoute : Route
    {
        public int TotalCheckpointCount => _checkpoints.Count;

        List<CheckpointNode> _checkpoints = new List<CheckpointNode>();
        CheckpointGate _checkpointGate;

        void OnEnable()
        {
            RGSKEvents.OnRaceDeInitialized.AddListener(OnRaceDeInitialized);
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceDeInitialized.RemoveListener(OnRaceDeInitialized);
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        public void SetupCheckpoints()
        {
            GetComponentsInChildren(_checkpoints);

            for (int i = 0; i < _checkpoints.Count; i++)
            {
                _checkpoints[i].Setup(RaceManager.Instance.Session);
            }

            if (RGSKCore.Instance.RaceSettings.showCheckpointGates &&
                RGSKCore.Instance.RaceSettings.checkpointGate != null)
            {
                _checkpointGate = Instantiate(RGSKCore.Instance.RaceSettings.checkpointGate, GeneralHelper.GetDynamicParent());
                HideCheckpointVisual();
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            switch (state)
            {
                case RaceState.PreRace:
                case RaceState.PostRace:
                    {
                        HideCheckpointVisual();
                        break;
                    }
            }
        }

        void OnRaceDeInitialized()
        {
            if (_checkpointGate != null)
            {
                Destroy(_checkpointGate.gameObject);
            }
        }

        void HideCheckpointVisual()
        {
            if (_checkpointGate == null)
                return;

            _checkpointGate.transform.position = Vector3.down * 10000;
        }

        public override void Reverse()
        {
            base.Reverse();

            foreach (var node in nodes)
            {
                node.transform.Rotate(0, 180, 0);
            }
        }
    }
}