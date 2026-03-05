using UnityEngine;

namespace RGSK
{
    public class TrackObjectEnabler : MonoBehaviour
    {
        [SerializeField] Track track;
        [SerializeField] GameObject[] objects;

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
        }

        void OnRaceInitialized()
        {
            if (track == null)
                return;

            foreach (var obj in objects)
            {
                if(obj == null)
                    continue;
                
                obj.SetActive(RaceManager.Instance.Track == track);
            }
        }
    }
}