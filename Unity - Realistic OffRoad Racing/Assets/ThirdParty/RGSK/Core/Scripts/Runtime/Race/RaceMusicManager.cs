using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK;

namespace RGSK
{
    public class RaceMusicManager : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnRaceStateChanged.AddListener(OnRaceStateChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceStateChanged.RemoveListener(OnRaceStateChanged);
        }

        void OnRaceStateChanged(RaceState state)
        {
            switch (state)
            {
                case RaceState.PreRace:
                    {
                        MusicManager.Instance?.UpdatePlaylist(RGSKCore.Instance.RaceSettings.preRaceMusic, true, true, true);
                        break;
                    }

                case RaceState.Countdown:
                case RaceState.RollingStart:
                    {
                        MusicManager.Instance?.UpdatePlaylist(RGSKCore.Instance.RaceSettings.raceMusic, true, false, true);
                        break;
                    }

                case RaceState.Racing:
                    {
                        MusicManager.Instance?.PlayNext();
                        break;
                    }

                case RaceState.PostRace:
                    {
                        MusicManager.Instance?.UpdatePlaylist(RGSKCore.Instance.RaceSettings.postRaceMusic, true, true, true);
                        break;
                    }
            }
        }
    }
}