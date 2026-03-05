using UnityEngine;

namespace RGSK
{
    public class RecordableSceneObject : MonoBehaviour
    {
        void OnEnable()
        {
            RecorderManager.Instance?.ReplayRecorder?.AddRecordableObject(gameObject);
        }

        void OnDestroy()
        {
            RecorderManager.Instance?.ReplayRecorder?.RemoveRecordableObject(gameObject);
        }
    }
}