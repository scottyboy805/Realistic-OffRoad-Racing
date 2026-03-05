using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;

namespace RGSK
{
    [System.Serializable]
    public class ChampionshipRound
    {
        public RaceStartMode startMode;
        public int lapCount = 3;
        public float sessionTimeLimit = 60;
        public TrackDefinition track;
    }

    [CreateAssetMenu(menuName = "RGSK/Race/Championship")]
    public class Championship : ItemDefinition
    {
        public List<ChampionshipRound> rounds = new List<ChampionshipRound>();
        public List<int> points = new List<int>();
        public List<RaceEntrant> entrants = new List<RaceEntrant>();
        public AutoPopulateEntrantOptions autoPopulateEntrantOptions = new AutoPopulateEntrantOptions();
        public List<RaceReward> rewards = new List<RaceReward>();
        public RaceType raceType;
        public ChampionshipSpawnMode spawnMode;
        public bool reverseSpawnOrder;
        public OpponentDifficultyIndex opponentDifficulty = new OpponentDifficultyIndex();
        public bool enableSlipstream = true;
        public bool enableCatchup;
        public bool disableCollision;
        public bool spectatorMode;

        public int TotalRounds => rounds.Count;

        public bool AllTracksAssigned()
        {
            foreach (var r in rounds)
            {
                if (r == null)
                    continue;

                if (r.track == null)
                    return false;
            }

            return true;
        }

        public bool ValidGridSlots()
        {
            var entrantCount = entrants.Count;

            if (autoPopulateEntrantOptions.autoPopulateOpponents)
            {
                entrantCount += autoPopulateEntrantOptions.opponentCount;
            }

            foreach (var r in rounds)
            {
                if (r == null || r.track == null)
                    continue;

                if (r.track.gridSlots < entrantCount)
                {
                    return false;
                }
            }

            return true;
        }

        public int LoadBestPosition()
        {
            if (SaveData.Instance.bestChampionshipPosition.TryGetValue(ID, out var value))
            {
                return value;
            }

            return 0;
        }

        public void SaveBestPosition(int value)
        {
            SaveData.Instance.bestChampionshipPosition[ID] = value;
        }
    }
}