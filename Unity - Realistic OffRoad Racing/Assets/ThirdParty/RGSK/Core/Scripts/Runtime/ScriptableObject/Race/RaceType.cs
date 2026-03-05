using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Race/Race Type")]
    public class RaceType : ScriptableObject
    {
        public string displayName;

        [TextArea(5, 5)]
        public string description;

        [Tooltip("Select whether the race is lap based and/or time based.\n\nLap based - the race will end after completing a certain number of laps.\n\nTime based - the race will end after the timer has elapsed.\n\nBoth - the race will end after completing a certain number of laps OR after the timer has elapsed.")]
        public RaceDurationMode raceDurationMode = RaceDurationMode.LapBased;

        [Tooltip("The value used to determine competitor positions in the race.")]
        public RacePositioningMode positioningMode = RacePositioningMode.Distance;

        [Tooltip("How to sort the values from the 'Positioning Mode'.\n\nDescending - put the competitor with the highest value in 1st place.\n\nAscending - put the competitor with the highest value in last place.")]
        public SortOrder positionSortMode = SortOrder.Descending;

        [Tooltip("The type of timer to use for the race session.\n\nGlobal - affects all competitors.\n\nIndividual - affects individual competitors.")]
        public RaceTimerMode timerMode;

        [Tooltip("What happens when the global timer elapses.")]
        public GlobalTimerElapsedAction globalTimerElapsedAction;

        [Tooltip("The min/max amount of laps allowed.")]
        [Min(1)] public int minLaps = 1;

        [Tooltip("The maximum number of competitors allowed.")]
        public Vector2Int minMaxCompetitorCount = new Vector2Int(1, 20);

        [Tooltip("Force the race to have infinte laps.")]
        public bool infiniteLaps = false;

        [Tooltip("Disqualify last place after other competitors complete a lap.")]
        public bool disqualifyLastPlaceEachLap = false;

        [Tooltip("Whether ghost vehicles are allowed.")]
        public bool allowGhost = false;

        public VehicleHandlingMode vehicleHandlingMode;

        [Tooltip("Will this race type be selectable in the quick race menu.")]
        public bool selectableFromMenu = true;

        [EnumFlags][SerializeField] public TrackLayoutType allowedTrackLayouts;

        public static bool IsInfiniteLaps(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.infiniteLaps || raceType.raceDurationMode == RaceDurationMode.TimeBased;
        }

        public static bool IsTimed(RaceType raceType)
        {
            return UseGlobalTimer(raceType) || UseSeparateTimers(raceType);
        }

        public static bool IsSolo(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.minMaxCompetitorCount.y == 1;
        }

        public static bool IsDrift(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.vehicleHandlingMode == VehicleHandlingMode.Drift;
        }

        public static bool UseGlobalTimer(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.raceDurationMode != RaceDurationMode.LapBased && raceType.timerMode == RaceTimerMode.Global;
        }

        public static bool UseSeparateTimers(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.raceDurationMode != RaceDurationMode.LapBased && raceType.timerMode == RaceTimerMode.PerCompetitor;
        }

        public static bool UseGhostVehicle(RaceType raceType)
        {
            if (raceType == null)
                return false;

            return raceType.allowGhost;
        }
    }
}