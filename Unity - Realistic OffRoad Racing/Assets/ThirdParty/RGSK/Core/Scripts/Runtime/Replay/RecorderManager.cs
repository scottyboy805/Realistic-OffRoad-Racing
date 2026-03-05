using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class RecorderManager : Singleton<RecorderManager>
    {
        public ReplayRecorder ReplayRecorder
        {
            get
            {
                if (_replayRecorder == null)
                {
                    _replayRecorder = gameObject.GetOrAddComponent<ReplayRecorder>();
                }

                return _replayRecorder;
            }
        }

        public GhostRecorder GhostRecorder
        {
            get
            {
                if (_ghostRecorder == null)
                {
                    _ghostRecorder = gameObject.GetOrAddComponent<GhostRecorder>();
                }

                return _ghostRecorder;
            }
        }

        ReplayRecorder _replayRecorder;
        GhostRecorder _ghostRecorder;

        public void Clear()
        {
            _replayRecorder?.RemoveRecordableObjects();
            _ghostRecorder?.RemoveRecordableObjects();
            _ghostRecorder?.DeleteGhostData();
        }
    }
}