using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System;
using System.Linq;

namespace RGSK
{
    public class RaceRewardManager : Singleton<RaceRewardManager>
    {
        public RaceReward ActiveReward => _reward;
        RaceReward _reward;

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceDeInitialized.AddListener(OnRaceDeInitialized);
            RGSKEvents.OnChampionshipFinished.AddListener(OnChampionshipFinished);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceDeInitialized.RemoveListener(OnRaceDeInitialized);
            RGSKEvents.OnChampionshipFinished.RemoveListener(OnChampionshipFinished);
        }

        public void AssignReward(RaceReward reward) => _reward = reward;

        public void AssignReward(List<RaceReward> rewards, int index)
        {
            if (index < 0 || index >= rewards.Count)
            {
                AssignReward(null);
                return;
            }

            AssignReward(rewards[index]);
        }

        void OnCompetitorFinished(Competitor c)
        {
            var player = RaceManager.Instance.Entities.Items.FirstOrDefault(x => x.IsPlayer);

            if (player != null && player.Competitor != null)
            {
                if (!player.Competitor.IsFinished() || player.Competitor.IsDisqualified)
                    return;

                var session = RaceManager.Instance.Session;
                AssignReward(session.raceRewards, player.Competitor.FinalPosition - 1);
            }
        }

        void OnChampionshipFinished()
        {
            var player = ChampionshipManager.Instance.PlayerEntrant;

            if (player != null)
            {
                var index = ChampionshipManager.Instance.Championship.entrants.IndexOf(player);
                AssignReward(ChampionshipManager.Instance.Championship.rewards, index);
            }
        }

        void OnRaceDeInitialized()
        {
            if (_reward == null)
                return;

            _reward.items.ForEach(x => x?.Unlock());
            SaveData.Instance.playerData.AddCurrency(_reward.currency);
            SaveData.Instance.playerData.AddXP(_reward.xp);
            SaveManager.Instance?.Save();

            _reward = null;
        }
    }
}