using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RGSK
{
    public class RaceStateObjectActivator : MonoBehaviour
    {
        [SerializeField] RaceState activeState;
        [SerializeField] GameObject targetObject;

        void OnEnable()
        {
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        void Awake()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(activeState == state);
            }
        }
    }
}