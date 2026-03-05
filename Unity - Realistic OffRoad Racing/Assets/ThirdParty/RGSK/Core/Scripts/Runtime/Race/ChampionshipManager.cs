using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;
using RGSK.Extensions;

namespace RGSK
{
    public class ChampionshipManager : Singleton<ChampionshipManager>
    {
        public Championship Championship => _championship;
        public RaceEntrant PlayerEntrant => Championship.entrants.FirstOrDefault(x => x.isPlayer);
        public bool Initialized { get; private set; }
        public bool IsFinished { get; private set; }
        public int CurrentRound { get; private set; }
        public bool IsFinalRound => CurrentRound == Championship.TotalRounds;

        Championship _championship;
        RaceSession _session;
        Dictionary<int, float> _previousPoints = new Dictionary<int, float>();
        List<ProfileDefinition> _opponentProfiles;
        int _opponentProfileCounter;

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        public void StartChampionship(Championship championship)
        {
            if (championship == null || championship.TotalRounds == 0 || Initialized)
                return;

            if (championship.raceType == null)
            {
                Logger.LogWarning("Failed to start championship! Please assign a race type!");
                return;
            }

            if (!championship.AllTracksAssigned() || !championship.ValidGridSlots())
            {
                Logger.LogWarning("Failed to start championship! Please ensure that each round has a track assigned and/or each track has enough grid slots for all entrants!");
                return;
            }

            _previousPoints.Clear();
            _championship = ScriptableObject.Instantiate(championship);

            if (Championship.autoPopulateEntrantOptions.autoPopulatePlayer)
            {
                GeneralHelper.PopulatePlayerEntrant(ref Championship.entrants);
            }

            Championship.entrants.AddRange(GeneralHelper.PopulateOpponentEntrants
            (
                Championship.autoPopulateEntrantOptions,
                GeneralHelper.GetPlayerEntrantVehicleClass(Championship.entrants)
            ));

            foreach (var e in Championship.entrants)
            {
                e.profile = GeneralHelper.GetEntrantProfile(e, ref _opponentProfiles, ref _opponentProfileCounter);
                e.championshipPoints = 0;

                switch (e.colorSelectMode)
                {
                    case ColorSelectionMode.Random:
                        {
                            if (GeneralHelper.CanApplyColor(e.prefab))
                            {
                                e.colorSelectMode = ColorSelectionMode.Color;
                                e.color = GeneralHelper.GetRandomVehicleColor();
                            }
                            else if (GeneralHelper.CanApplyLivery(e.prefab))
                            {
                                e.colorSelectMode = ColorSelectionMode.Livery;
                                e.livery = GeneralHelper.GetRandomLivery(e.prefab);
                            }

                            break;
                        }
                }
            }

            Championship.entrants.Shuffle();
            CurrentRound = 0;
            Initialized = true;
            IsFinished = false;

            RGSKEvents.OnChampionshipInitialized.Invoke();
            LoadNextRound();
        }

        public void LoadNextRound()
        {
            RaceManager.Instance.ForceFinishRace(true);

            if (!IsFinalRound)
            {
                CurrentRound++;
                UpdateSession(Championship.rounds[CurrentRound - 1]);
                RGSKCore.runtimeData.SelectedSession = _session;
                RGSKCore.runtimeData.SelectedTrack = _session.track;
                SceneLoadManager.LoadScene(_session.track.scene);
            }
            else
            {
                if (!IsFinished)
                {
                    IsFinished = true;
                    RGSKEvents.OnChampionshipFinished.Invoke();
                }
            }
        }

        public void EndChampionship()
        {
            SaveChampionshipRecord();
            _championship = null;
            _session = null;
            Initialized = false;
            IsFinished = false;
            RGSKEvents.OnChampionshipDeInitialized.Invoke();
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (!Initialized)
                return;

            if (RaceManager.Instance.AllCompetitorsFinished())
            {
                RaceManager.Instance.Entities.Items.ForEach(x =>
                {
                    if (x.Competitor != null)
                    {
                        GivePointsToEntrant(x.Competitor);
                    }
                });
            }

            void GivePointsToEntrant(Competitor c)
            {
                var entrant = Championship.entrants.FirstOrDefault(x => x.instanceID == c.Entity.ID);

                if (entrant != null)
                {
                    var pts = GetPointsForPosition(c.FinalPosition - 1);
                    _previousPoints.Add(entrant.instanceID, entrant.championshipPoints);
                    entrant.championshipPoints += pts;
                    Championship.entrants = Championship.entrants.OrderByDescending(x => x.championshipPoints).ToList();
                    RGSKEvents.OnChampionshipPositionsChanged.Invoke();
                }
            }
        }

        void OnRaceRestart()
        {
            if (!Initialized)
                return;

            foreach (var entrant in Championship.entrants)
            {
                if (_previousPoints.ContainsKey(entrant.instanceID))
                {
                    entrant.championshipPoints = _previousPoints[entrant.instanceID];
                    Championship.entrants = Championship.entrants.OrderByDescending(x => x.championshipPoints).ToList();
                    _previousPoints.Remove(entrant.instanceID);
                    RGSKEvents.OnChampionshipPositionsChanged.Invoke();
                }
            }

            IsFinished = false;
        }

        void UpdateSession(ChampionshipRound round)
        {
            if (_session == null)
            {
                _session = ScriptableObject.CreateInstance<RaceSession>();
            }

            _session.raceType = Championship.raceType;
            _session.sessionType = RaceSessionType.Race;
            _session.opponentDifficulty.index = Championship.opponentDifficulty.index;
            _session.enableSlipstream = Championship.enableSlipstream;
            _session.enableCatchup = Championship.enableCatchup;
            _session.disableCollision = Championship.disableCollision;
            _session.spectatorMode = Championship.spectatorMode;
            _session.startMode = round.startMode;
            _session.lapCount = round.lapCount;
            _session.sessionTimeLimit = round.sessionTimeLimit;
            _session.track = round.track;

            switch (Championship.spawnMode)
            {
                case ChampionshipSpawnMode.Random:
                    {
                        Championship.entrants.Shuffle();
                        _session.playerGridStartMode = SelectionMode.Random;
                        break;
                    }

                case ChampionshipSpawnMode.TotalPoints:
                    {
                        Championship.entrants = Championship.entrants.OrderByDescending(x => x.championshipPoints).ToList();
                        _session.playerGridStartMode = SelectionMode.Selected;

                        if (Championship.reverseSpawnOrder)
                        {
                            Championship.entrants.Reverse();
                        }

                        if (PlayerEntrant != null)
                        {
                            _session.playerStartPosition = Championship.entrants.IndexOf(PlayerEntrant) + 1;
                        }

                        break;
                    }

                case ChampionshipSpawnMode.PreviousRoundPosition:
                    {
                        if (_previousPoints.Count > 0)
                        {
                            Championship.entrants = Championship.entrants.OrderByDescending(x => (x.championshipPoints - _previousPoints[x.instanceID])).ToList();
                            _session.playerGridStartMode = SelectionMode.Selected;

                            if (Championship.reverseSpawnOrder)
                            {
                                Championship.entrants.Reverse();
                            }

                            if (PlayerEntrant != null)
                            {
                                _session.playerStartPosition = Championship.entrants.IndexOf(PlayerEntrant) + 1;
                            }
                        }

                        break;
                    }
            }
        }

        void SaveChampionshipRecord()
        {
            if (!Initialized || !IsFinished || PlayerEntrant == null)
                return;

            var record = Championship.LoadBestPosition();
            var pos = Championship.entrants.IndexOf(PlayerEntrant) + 1;

            if (record <= 0 || record > pos)
            {
                Championship.SaveBestPosition(pos);
            }
        }

        public float GetEntrantPoints(RGSKEntity entity)
        {
            var entrant = Championship.entrants.FirstOrDefault(x => x.instanceID == entity.ID);

            if (entrant != null)
            {
                return entrant.championshipPoints;
            }

            return 0;
        }

        public int GetEntrantPosition(RGSKEntity entity)
        {
            var entrant = Championship.entrants.FirstOrDefault(x => x.instanceID == entity.ID);

            if (entrant != null)
            {
                return Championship.entrants.IndexOf(entrant) + 1;
            }

            return -1;
        }

        public int GetPointsForPosition(int index)
        {
            var points = Championship.points.Count > 0 ?
                         Championship.points :
                         RGSKCore.Instance.RaceSettings.defaultChampionshipPoints;

            if (index < 0 || index >= points.Count)
                return 0;

            return points[index];
        }
    }
}