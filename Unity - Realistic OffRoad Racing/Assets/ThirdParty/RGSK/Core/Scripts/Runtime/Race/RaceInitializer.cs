using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class RaceInitializer : MonoBehaviour
    {
        [ExposedScriptableObject] public RaceSession session;
        public Track track;
        public bool autoFindTrack = true;
        public bool autoInitialize = true;

        void Start()
        {
            if (autoFindTrack)
            {
                var tracks = FindObjectsByType<Track>(FindObjectsSortMode.InstanceID).ToList();

                if (tracks.Count > 0)
                {
                    var selected = tracks.FirstOrDefault(x => x.definition == RGSKCore.runtimeData.SelectedTrack);

                    if (selected != null)
                    {
                        track = selected;
                    }
                    else
                    {
                        track = tracks[0];
                    }
                }
            }

            if (autoInitialize)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            var selected = RGSKCore.runtimeData.SelectedSession;
            RaceManager.Instance?.InitializeRace(selected ?? session, track);
        }
    }
}