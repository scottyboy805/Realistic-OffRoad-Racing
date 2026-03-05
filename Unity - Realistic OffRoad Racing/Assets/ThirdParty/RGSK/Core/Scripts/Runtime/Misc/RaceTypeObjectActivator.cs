using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RaceTypeObjectActivator : MonoBehaviour
    {
        [SerializeField] GameObject targetObject;
        [SerializeField] List<RaceType> activeRaceTypes = new List<RaceType>();

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
        }

        void Awake()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            if(RaceManager.Instance.Initialized)
            {
                OnRaceInitialized();
            }
        }

        void OnRaceInitialized()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(activeRaceTypes.Contains(RaceManager.Instance.Session.raceType));
            }
        }
    }
}
