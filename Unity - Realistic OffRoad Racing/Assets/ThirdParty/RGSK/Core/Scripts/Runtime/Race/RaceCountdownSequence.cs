using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RGSK
{
    public class RaceCountdownSequence : MonoBehaviour
    {
        [SerializeField] PlayableDirector sequence;

        [Tooltip("The time in the sequence that the 'Race Start' signal is triggered.\n\nThis is used during rolling starts to skip the countdown wait.")]
        [SerializeField] float raceStartSignalTime = 4;

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
            if (RaceManager.Instance != null)
            {
                OnRaceStateChanged(RaceManager.Instance.CurrentState);
            }
        }

        void OnRaceStateChanged(RaceState state)
        {
            if (sequence == null)
                return;

            switch (state)
            {
                case RaceState.PreRace:
                default:
                    {
                        sequence.gameObject.SetActive(false);
                        sequence.Stop();
                        break;
                    }

                case RaceState.RollingStart:
                    {
                        sequence.initialTime = raceStartSignalTime;
                        break;
                    }

                case RaceState.Countdown:
                    {
                        sequence.gameObject.SetActive(true);
                        sequence.Play();
                        break;
                    }

                case RaceState.Racing:
                    {
                        break;
                    }
            }
        }
    }
}