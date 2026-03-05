using UnityEngine;
using UnityEngine.Playables;

namespace RGSK
{
    [RequireComponent(typeof(PlayableDirector))]
    public class AutoDestroyTimeline : MonoBehaviour
    {
        PlayableDirector _director;

        void OnEnable()
        {
            if (TryGetComponent<PlayableDirector>(out _director))
            {
                _director.stopped += OnStopped;
            }
        }

        void OnDestroy()
        {
            _director.stopped -= OnStopped;
        }

        void OnStopped(PlayableDirector director)
        {
            Destroy(_director.gameObject);
        }
    }
}