using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace RGSK
{
    public class RacePositioningSystem : MonoBehaviour
    {
        public List<RGSKEntity> RacingList { get; private set; } = new List<RGSKEntity>();
        public List<RGSKEntity> FinishedList { get; private set; } = new List<RGSKEntity>();
        public List<RGSKEntity> DisqualifiedList { get; private set; } = new List<RGSKEntity>();
        public List<RGSKEntity> CombinedList { get; private set; } = new List<RGSKEntity>();

        RacePositioningMode _positioningMode;
        SortOrder _sortOrder;
        float _updatesPerSecond = 10;
        float _lastUpdate;

        public void Setup(RacePositioningMode positioningMode, SortOrder sortOrder)
        {
            _positioningMode = positioningMode;
            _sortOrder = sortOrder;
        }

        void Update()
        {
            if (Time.time > _lastUpdate)
            {
                UpdatePositions();
            }
        }

        public void UpdatePositions()
        {
            if (!RaceManager.Instance.Session.UseVirtualCompetitors())
            {
                RaceManager.Instance.Entities.Items.ForEach(x =>
                {
                    if (x.Competitor != null && x.Competitor.IsFinished())
                    {
                        if (!x.Competitor.IsDisqualified)
                        {
                            if (!FinishedList.Contains(x))
                            {
                                FinishedList.Add(x);

                                if (LockFinishingPositions())
                                {
                                    if (x.Competitor.FinalPosition <= 0)
                                    {
                                        x.Competitor.FinalPosition = FinishedList.IndexOf(x) + 1;
                                    }
                                }
                                else
                                {
                                    switch (_sortOrder)
                                    {
                                        case SortOrder.Ascending:
                                            {
                                                FinishedList = FinishedList.OrderBy(x => GetRankableValue(x)).ToList();
                                                break;
                                            }

                                        case SortOrder.Descending:
                                            {
                                                FinishedList = FinishedList.OrderByDescending(x => GetRankableValue(x)).ToList();
                                                break;
                                            }
                                    }

                                    FinishedList.ForEach(y => y.Competitor.FinalPosition = FinishedList.IndexOf(y) + 1);
                                }
                            }
                        }
                        else
                        {
                            if (!DisqualifiedList.Contains(x))
                            {
                                DisqualifiedList.Insert(0, x);
                                x.Competitor.FinalPosition = CombinedList.IndexOf(x) + 1;
                            }
                        }
                    }
                });

                switch (_sortOrder)
                {
                    case SortOrder.Ascending:
                        {
                            RacingList = RaceManager.Instance.Entities.Items.Where(x => x.Competitor != null && !x.Competitor.IsFinished()).OrderBy(x => GetRankableValue(x)).ToList();
                            break;
                        }

                    case SortOrder.Descending:
                        {
                            RacingList = RaceManager.Instance.Entities.Items.Where(x => x.Competitor != null && !x.Competitor.IsFinished()).OrderByDescending(x => GetRankableValue(x)).ToList();
                            break;
                        }
                }
            }
            else
            {
                switch (_sortOrder)
                {
                    case SortOrder.Ascending:
                        {
                            RacingList = RaceManager.Instance.Entities.Items.OrderBy(x => GetRankableValue(x)).ToList();
                            break;
                        }

                    case SortOrder.Descending:
                        {
                            RacingList = RaceManager.Instance.Entities.Items.OrderByDescending(x => GetRankableValue(x)).ToList();
                            break;
                        }
                }

                RaceManager.Instance.Entities.Items.ForEach(x =>
                {
                    if (x.Competitor != null && x.Competitor.IsFinished() && !x.Competitor.IsDisqualified && x.Competitor.FinalPosition <= 0)
                    {
                        x.Competitor.FinalPosition = RacingList.IndexOf(x) + 1;
                    }
                });
            }

            CombinedList.Clear();
            CombinedList.AddRange(FinishedList);
            CombinedList.AddRange(RacingList);
            CombinedList.AddRange(DisqualifiedList);

            for (int i = 0; i < CombinedList.Count; i++)
            {
                var c = CombinedList[i].Competitor;
                var pos = (i + 1);

                if (c != null)
                {
                    if (c.Position != pos)
                    {
                        c.SetPosition(pos);
                    }
                }
            }

            _lastUpdate = Time.time + (1 / _updatesPerSecond);
        }

        float GetRankableValue(RGSKEntity e)
        {
            if (e.Competitor == null)
                return 0;

            switch (_positioningMode)
            {
                case RacePositioningMode.Distance:
                default:
                    {
                        return e.Competitor.TotalCheckpointsPassed > 0 ?
                               e.Competitor.DistanceTravelled :
                               e.Competitor.FurthestDistanceTravelled;
                    }

                case RacePositioningMode.DriftPoints:
                    {
                        if (e.DriftController == null)
                            return 0;

                        return e.DriftController.TotalPoints;
                    }

                case RacePositioningMode.TotalSpeed:
                    {
                        return e.Competitor.TotalSpeedtrapSpeed;
                    }

                case RacePositioningMode.AverageSpeed:
                    {
                        return e.Competitor.AverageSpeed;
                    }

                case RacePositioningMode.Score:
                    {
                        return e.Competitor.Score;
                    }

                case RacePositioningMode.TotalTime:
                    {
                        return e.Competitor.TotalRaceTime;
                    }

                case RacePositioningMode.BestLapTime:
                    {
                        return e.Competitor.GetBestLapTime();
                    }
            }
        }

        bool LockFinishingPositions()
        {
            return _positioningMode == RacePositioningMode.Distance;
        }
    }
}