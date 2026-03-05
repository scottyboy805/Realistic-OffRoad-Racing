using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Race")]
    public class RGSKRaceSettings : ScriptableObject
    {
        [Header("Race Types")]
        public List<RaceType> raceTypes = new List<RaceType>();

        [Header("Championship")]
        public List<int> defaultChampionshipPoints = new List<int>()
        {
            25,
            18,
            15,
            12,
            10,
            8,
            6,
            4,
            2,
            1,
        };

        [Header("Replay")]
        public bool enableReplay = true;

        [Header("Checkpoints")]
        public CheckpointGate checkpointGate;
        public bool showCheckpointGates;

        [Header("Audio")]
        public SoundList preRaceMusic;
        public SoundList raceMusic;
        public SoundList postRaceMusic;
        public string checkpointHitSound = "checkpoint";
        public string raceFinishSound = "race_finish";
        public string raceDisqualifySound = "race_dq";

        [Header("DNF")]
        [Tooltip("How the DNF timer start should be handled." +
                 "\n\nAfter First Finish - start the timer after 1 competitor finishes the race." +
                 "\n\nAfter Half Finish - start the timer after half the competitors finish the race.")]
        public DnfTimerStartMode dnfTimerStartMode;

        [Tooltip("The value that the DNF timer starts at.")]
        public float dnfTimeLimit = 60;

        [Header("Misc")]
        [Tooltip("The race states that the route cameras should activate in.")]
        [EnumFlags] public RaceState cinematicCameraStates;

        [Tooltip("The race states that the route cameras should activate in when in Spectator Mode.")]
        [EnumFlags] public RaceState spectatorModeCinematicCameraStates = (RaceState)~1;

        [Tooltip("The speed limit (in KM/h) that the vehicles will aim to go during rolling starts where there is only 1 competitor. Set to -1 for no limit")]
        public float rollingStartSpeedLimitSolo = -1;

        [Tooltip("The speed limit (in KM/h) that the vehicles will aim to go during rolling starts where there are multiple competitors. Set to -1 for no limit")]
        public float rollingStartSpeedLimitMultiple = 80;

        public bool ghostDisqualifiedCompetitors = true;
        public bool skipPreRaceState;
        public bool wrongwayTracking = true;
    }
}