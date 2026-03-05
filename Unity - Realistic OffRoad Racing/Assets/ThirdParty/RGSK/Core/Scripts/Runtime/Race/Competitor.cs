using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class Competitor : RGSKEntityComponent
    {
        public RaceState RaceState { get; private set; }
        public RouteProgressTracker LapTracker { get; private set; }
        public WrongwayTracker WrongwayTracker { get; private set; }
        public CheckpointRoute CheckpointRoute { get; private set; }
        public CheckpointNode NextCheckpoint { get; private set; }
        public CheckpointNode PreviousCheckpoint { get; private set; }

        public List<float> LapTimes { get; private set; } = new List<float>();
        public Dictionary<CheckpointNode, float> SplitTimes { get; private set; } = new Dictionary<CheckpointNode, float>();
        public Dictionary<CheckpointNode, float> Speedtraps { get; private set; } = new Dictionary<CheckpointNode, float>();

        public int Position { get; set; }
        public int StartingPosition { get; set; }
        public int FinalPosition { get; set; }
        public int CurrentLap { get; set; }
        public int CurrentCheckpoint { get; set; }
        public int CheckpointsPerLap { get; set; }
        public int TotalLaps { get; set; }
        public int TotalCheckpointsPassed { get; set; }
        public int Overtakes { get; set; }
        public int CurrentSector { get; set; }
        public float TotalRaceTime { get; set; }
        public float TotalSpeedtrapSpeed { get; set; }
        public float LapPercentage { get; set; }
        public float RacePercentage { get; set; }
        public float DistanceTravelled { get; set; }
        public float FurthestDistanceTravelled { get; set; }
        public float LapStartTime { get; set; }
        public float AverageSpeed { get; set; }
        public float TopSpeed { get; set; }
        public float Score { get; set; }
        public bool IsDisqualified { get; set; }
        public int TotalRaceCheckpoints => CheckpointsPerLap * TotalLaps;
        public float TotalRaceDistance => CheckpointRoute ? CheckpointRoute.Distance * TotalLaps : 0;

        void Update()
        {
            if (LapTracker != null)
            {
                if (!IsFinished() && !IsDisqualified)
                {
                    TotalRaceTime = RaceManager.Instance.GetRaceTime();
                    DistanceTravelled = LapTracker.GetTotalDistance();
                    LapPercentage = LapTracker.LapPercentage * 100;
                    RacePercentage = (TotalLaps > 0 ? LapTracker.TotalPercentage / TotalLaps : 0) * 100;

                    if (TotalRaceTime > 1 && DistanceTravelled > 0)
                    {
                        AverageSpeed = ConversionHelper.ConvertSpeed(DistanceTravelled / TotalRaceTime, SpeedUnit.KMH);

                        if (TopSpeed < Entity.CurrentSpeed)
                        {
                            TopSpeed = Entity.CurrentSpeed;
                        }
                    }

                    UpdateFurthestDistance();
                }
            }
        }

        public void Setup(CheckpointRoute route, int totalLaps)
        {
            if (route == null || route.nodes.Count < 2)
                return;

            LapTracker = gameObject.AddComponent<RouteProgressTracker>();
            WrongwayTracker = gameObject.GetOrAddComponent<WrongwayTracker>();
            CheckpointRoute = route;

            TotalLaps = totalLaps;
            CheckpointsPerLap = route.TotalCheckpointCount;

            LapTracker.Setup(route);
            WrongwayTracker.Setup(this);
            ResetValues();
        }

        public void HitCheckpoint(CheckpointNode cp)
        {
            if (IsPreRace() || IsRacing())
            {
                if (NextCheckpoint == cp)
                {
                    CurrentCheckpoint++;
                    TotalCheckpointsPassed++;
                    PreviousCheckpoint = cp;
                    NextCheckpoint = cp.NextCheckpoint;

                    SectorCP(cp);
                    SpeedtrapCP(cp);
                    TimeExtendCP(cp);

                    if (CurrentCheckpoint >= CheckpointsPerLap)
                    {
                        CompleteLap();
                    }

                    if (Entity == GeneralHelper.GetFocusedEntity())
                    {
                        AudioManager.Instance?.Play(RGSKCore.Instance.RaceSettings.checkpointHitSound, AudioGroup.UI);
                    }

                    LapTracker?.SetNextAndPreviousNode(NextCheckpoint, PreviousCheckpoint);
                    LapTracker?.UpdateTotalPercentage(CurrentLap - 1);
                    RGSKEvents.OnHitCheckpoint.Invoke(cp, this);
                }

                if (cp.index == 0)
                {
                    if(RaceManager.Instance.CurrentState == RaceState.RollingStart)
                    {
                        RaceManager.Instance.StartCountdown();
                    }

                    WrongwayTracker?.ResetFurthestDistance();
                }
            }
        }

        void SectorCP(CheckpointNode cp)
        {
            if (!cp.IsSector)
                return;

            if (!SplitTimes.ContainsKey(cp))
            {
                SplitTimes.Add(cp, 0);
            }

            var bestTime = SplitTimes[cp];
            var lapTime = GetLapTime();
            var difference = lapTime - bestTime;

            if (bestTime == 0 || difference < 0)
            {
                SplitTimes[cp] = lapTime;
            }

            CurrentSector++;
            RGSKEvents.OnHitSector.Invoke(this, difference);
        }

        void SpeedtrapCP(CheckpointNode cp)
        {
            var speed = Entity.CurrentSpeed;

            if (!Speedtraps.ContainsKey(cp))
            {
                Speedtraps.Add(cp, speed);
            }
            else
            {
                if (speed > Speedtraps[cp])
                {
                    Speedtraps[cp] = speed;
                }
            }

            if (cp.AllowSpeedtrap())
            {
                TotalSpeedtrapSpeed += speed;
                RGSKEvents.OnHitSpeedtrap.Invoke(this, speed);
            }
        }

        void TimeExtendCP(CheckpointNode cp)
        {
            if (cp.AllowTimeExtend())
            {
                RaceManager.Instance?.ExtendCompetitorTimer(this, cp.TimeExtend);
            }
        }

        void CompleteLap()
        {
            LapTimes.Add(GetLapTime());
            CurrentLap++;
            CurrentCheckpoint = 0;
            CurrentSector = 0;
            UpdateLapStartTime();
            RGSKEvents.OnLapCompleted.Invoke(this);
        }

        public void SetPosition(int newPosition)
        {
            if (IsRacing())
            {
                if (newPosition < Position)
                {
                    Overtakes++;
                    RGSKEvents.OnPositionGained.Invoke(this);
                }
                else if (newPosition > Position)
                {
                    RGSKEvents.OnPositionLost.Invoke(this);
                }
            }

            Position = newPosition;
            RGSKEvents.OnRacePositionsChanged.Invoke();
        }

        public void SetState(RaceState state)
        {
            if (Entity != null && Entity.IsVirtual)
            {
                RaceState = RaceState.Racing;
                return;
            }

            RaceState = state;

            switch (state)
            {
                case RaceState.RollingStart:
                    {
                        //Set an initial offset for the AI to avoid to maintain before the race starts
                        if (Entity.TryGetComponent<AIController>(out var ai))
                        {
                            ai.SetTravelOffset(ai.GetLateralPositionFromCenter(ai.transform.position).x, 1000);
                        }
                        break;
                    }

                case RaceState.Racing:
                    {
                        if (Entity.DriftController != null)
                        {
                            Entity.DriftController.enabled = RaceManager.Instance.Session.IsDriftSession();
                        }

                        //Set an initial offset for the AI to avoid moving to the racing line once the race starts
                        if (Entity.TryGetComponent<AIController>(out var ai))
                        {
                            ai.SetTravelOffset(ai.GetLateralPositionFromCenter(ai.transform.position).x, RaceManager.Instance.Track?.raceStartOffsetDuration ?? -1);
                        }
                        break;
                    }

                case RaceState.PostRace:
                    {
                        if (Entity.DriftController != null)
                        {
                            Entity.DriftController.CompleteDrift();
                            Entity.DriftController.enabled = false;
                        }

                        if (!IsDisqualified)
                        {
                            RacePercentage = 100;
                        }
                        break;
                    }
            }

            var aiBehaviour = RGSKCore.Instance.AISettings.raceStateBehaviours.FirstOrDefault(x => x.state == state);
            if (aiBehaviour != null)
            {
                GeneralHelper.SetAIBehaviour(gameObject, aiBehaviour.behaviour);
            }

            RGSKEvents.OnCompetitorStateChanged.Invoke(this, state);
        }

        public override void ResetValues()
        {
            NextCheckpoint = CheckpointRoute?.GetNode(CheckpointRoute.loop ? 1 : 0) as CheckpointNode;
            PreviousCheckpoint = NextCheckpoint?.previousNode as CheckpointNode;

            LapTracker?.SetNextAndPreviousNode(NextCheckpoint, PreviousCheckpoint);
            LapTracker?.ResetTotalPercentage();
            WrongwayTracker?.ResetFurthestDistance();

            FinalPosition = -1;
            CurrentLap = 1;
            CurrentCheckpoint = 0;
            CurrentSector = 0;
            TotalSpeedtrapSpeed = 0;
            TotalCheckpointsPassed = 0;
            Score = 0;
            AverageSpeed = 0;
            TopSpeed = 0;
            Overtakes = 0;
            IsDisqualified = false;
            FurthestDistanceTravelled = int.MinValue;

            LapTimes.Clear();
            SplitTimes.Clear();
            Speedtraps.Clear();
        }

        void UpdateFurthestDistance()
        {
            if (TotalCheckpointsPassed == 0)
            {
                if (NextCheckpoint != null)
                {
                    var dist = -Vector3.Distance(transform.position, NextCheckpoint.transform.position);

                    if (dist > FurthestDistanceTravelled)
                    {
                        FurthestDistanceTravelled = dist;
                    }
                }
            }
            else
            {
                if (DistanceTravelled > FurthestDistanceTravelled)
                {
                    FurthestDistanceTravelled = DistanceTravelled;
                }
            }
        }

        public float GetLapTime() => RaceManager.Instance?.GetRaceTime() - LapStartTime ?? 0;
        public float GetBestLapTime() => LapTimes.Count == 0 ? float.PositiveInfinity : LapTimes.Min();
        public float GetLastLapTime() => LapTimes.Count == 0 ? 0 : LapTimes[LapTimes.Count - 1];
        public float GetRemainingDistance() => TotalLaps < 0 ? 0 : TotalRaceDistance - DistanceTravelled;
        public void UpdateLapStartTime() => LapStartTime = RaceManager.Instance.GetRaceTime();
        public bool IsPreRace() => RaceState == RaceState.PreRace || RaceState == RaceState.RollingStart;
        public bool IsRacing() => RaceState == RaceState.Racing;
        public bool IsFinished() => RaceState == RaceState.PostRace;
        public bool IsFinalLap() => CurrentLap == TotalLaps;
        public bool IsFinalCheckpoint() => TotalLaps > 0 && TotalCheckpointsPassed == TotalRaceCheckpoints - 1;
    }
}