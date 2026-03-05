using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class RaceEntrant
    {
        public GameObject prefab;
        public ProfileDefinition profile;
        public Color color = Color.white;
        public Texture2D livery;
        public ColorSelectionMode colorSelectMode;
        public bool isPlayer;
        public int instanceID;
        public float championshipPoints;
    }

    [System.Serializable]
    public class RaceReward
    {
        public int currency;
        public int xp;
        public List<ItemDefinition> items = new List<ItemDefinition>();
    }

    [System.Serializable]
    public class OpponentDifficultyIndex
    {
        public int index;
    }

    [System.Serializable]
    public class AutoPopulateEntrantOptions
    {
        [Tooltip("Should the selected player vehicle be used.")]
        public bool autoPopulatePlayer;

        [Tooltip("Should the opponents be added automatically.")]
        public bool autoPopulateOpponents;

        public int opponentCount = 3;
        public OpponentClassOptions opponentClassOptions = OpponentClassOptions.SameAsPlayer;
        public VehicleClass opponentVehicleClass;
    }

    [CreateAssetMenu(menuName = "RGSK/Race/Race Session")]
    public class RaceSession : ItemDefinition
    {
        public RaceSessionType sessionType;

        [Tooltip("The race type used for this session.")]
        public RaceType raceType;

        [Tooltip("The way the race should start.")]
        public RaceStartMode startMode;

        [Tooltip("The number of laps in the race.\nThis is automatically set to 1 for point to point tracks.")]
        public int lapCount = 3;

        [Tooltip("The difficulty index of the AI opponents.")]
        public OpponentDifficultyIndex opponentDifficulty = new OpponentDifficultyIndex();

        public SelectionMode playerGridStartMode;
        public int playerStartPosition = 0;

        [Tooltip("The value that the session timer(s) will start at.\nOnly available in race types that are timed.")]
        public float sessionTimeLimit = 60;

        [Tooltip("Whether a ghost vehicle will be used.\nOnly available in race types that allow ghosts.")]
        public bool enableGhost = true;

        [Tooltip("How far ahead (in seconds) that the ghost will start infront of the player.")]
        public float ghostOffset = 0;

        [Tooltip("Whether slipstream should be enabled for the race.")]
        public bool enableSlipstream = true;

        [Tooltip("Whether catchup (rubberbanding) should be enabled for the race.")]
        public bool enableCatchup;

        [Tooltip("Disables collision between competitors in the race.")]
        public bool disableCollision;

        [Tooltip("Whether the race should be in spectator mode.")]
        public bool spectatorMode;

        [Tooltip("The race entrants")]
        public List<RaceEntrant> entrants = new List<RaceEntrant>();
        public AutoPopulateEntrantOptions autoPopulateEntrantOptions = new AutoPopulateEntrantOptions();

        [Tooltip("The scores that the player has to achieve in the race session.")]
        public List<float> targetScores = new List<float>();

        public List<RaceReward> raceRewards = new List<RaceReward>();
        public TrackDefinition track;

        public bool saveRecords = true;

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        public void Setup()
        {
            if (raceType == null)
            {
                Logger.LogWarning("A race type has not been assigned to this session! Default race type will be used.");
                raceType = ScriptableObject.CreateInstance<RaceType>();
            }

            if (raceType.raceDurationMode == RaceDurationMode.TimeBased)
            {
                raceType.infiniteLaps = true;
            }

            if (!IsInfiniteLaps())
            {
                lapCount = Mathf.Clamp(lapCount, raceType.minLaps, lapCount);
            }

            if (autoPopulateEntrantOptions.autoPopulatePlayer)
            {
                GeneralHelper.PopulatePlayerEntrant(ref entrants);
            }

            PopulateOpponents();
        }

        public void PopulateOpponents()
        {
            if (IsSoloSession())
            {
                //remove all opponnents in a solo session
                var opponents = entrants.Where(x => !x.isPlayer).ToList();
                opponents.ForEach(x => entrants.Remove(x));
                return;
            }

            entrants.AddRange(GeneralHelper.PopulateOpponentEntrants
            (
                autoPopulateEntrantOptions,
                GeneralHelper.GetPlayerEntrantVehicleClass(entrants)
            ));
        }

        public AIBehaviourSettings GetAiDifficulty()
        {
            if (opponentDifficulty.index < 0 || opponentDifficulty.index >= RGSKCore.Instance.AISettings.difficulties.Count)
                return RGSKCore.Instance.AISettings.defaultBehaviour ?? AIBehaviourSettings.CreateDefault();

            return RGSKCore.Instance.AISettings.difficulties[opponentDifficulty.index];
        }

        public int LoadBestPosition()
        {
            if (SaveData.Instance.bestSessionPosition.TryGetValue(ID, out var value))
            {
                return value;
            }

            return 0;
        }

        public void SaveBestPosition(int value)
        {
            SaveData.Instance.bestSessionPosition[ID] = value;
        }

        public RacePositioningMode GetPositioningMode() => raceType?.positioningMode ?? RacePositioningMode.Distance;
        public bool IsTargetScoreSession() => sessionType == RaceSessionType.TargetScore;
        public bool IsSoloSession() => RaceType.IsSolo(raceType);
        public bool IsTimedSession() => RaceType.IsTimed(raceType);
        public bool IsInfiniteLaps() => RaceType.IsInfiniteLaps(raceType);
        public bool IsDriftSession() => RaceType.IsDrift(raceType);
        public bool UseGlobalTimer() => RaceType.UseGlobalTimer(raceType);
        public bool UseSeparateTimers() => RaceType.UseSeparateTimers(raceType);
        public bool UseGhostVehicle() => RaceType.UseGhostVehicle(raceType);
        public bool UseVirtualCompetitors() => IsTargetScoreSession() && (entrants.Count == 1 || IsSoloSession());
    }
}