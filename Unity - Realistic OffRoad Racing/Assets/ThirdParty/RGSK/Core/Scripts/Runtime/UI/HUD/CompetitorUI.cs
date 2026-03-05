using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class CompetitorUI : EntityUIComponent
    {
        public NumberDisplayMode positionDisplayMode;
        public NumberDisplayMode lapDisplayMode;
        public TMP_Text positionText;
        public TMP_Text lapText;
        public TMP_Text lapTimeText;
        public TMP_Text lastLapTimeText;
        public TMP_Text bestLapTimeText;
        public TMP_Text totalTimeText;
        public TMP_Text racePercentageText;
        public TMP_Text lapPercentageText;
        public TMP_Text distanceTravelledText;
        public TMP_Text distanceRemainingText;
        public TMP_Text checkpointText;
        public TMP_Text lapsCompletedText;
        public TMP_Text totalSpeedText;
        public TMP_Text finishTimeText;
        public TMP_Text finishTimeGapText;
        public TMP_Text leaderDistanceGapText;
        public TMP_Text leaderTimeGapText;
        public TMP_Text bestTimeGapText;
        public TMP_Text overtakesText;
        public TMP_Text averageSpeedText;
        public TMP_Text topSpeedText;
        public TMP_Text scoreText;
        public TMP_Text championshipPositionText;
        public TMP_Text championshipRoundPointsText;
        public TMP_Text championshipTotalPointsText;

        float _nextGapUpdate;

        void OnEnable()
        {
            RGSKEvents.OnRacePositionsChanged.AddListener(OnRacePositionChanged);
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
            RGSKEvents.OnHitCheckpoint.AddListener(OnHitCheckpoint);
            RGSKEvents.OnHitSpeedtrap.AddListener(OnHitSpeedtrap);
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            RGSKEvents.OnScoreAdded.AddListener(OnScoreAdded);
        }

        void OnDisable()
        {
            RGSKEvents.OnRacePositionsChanged.RemoveListener(OnRacePositionChanged);
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
            RGSKEvents.OnHitCheckpoint.RemoveListener(OnHitCheckpoint);
            RGSKEvents.OnHitSpeedtrap.RemoveListener(OnHitSpeedtrap);
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
            RGSKEvents.OnScoreAdded.RemoveListener(OnScoreAdded);
        }

        public override void Update()
        {
            if (Entity == null || Entity.Competitor == null)
                return;

            lapTimeText?.SetText(UIHelper.FormatTimeText(Entity.Competitor.GetLapTime(), RGSKCore.Instance.UISettings.raceTimerFormat));
            totalTimeText?.SetText(UIHelper.FormatTimeText(Entity.Competitor.TotalRaceTime, RGSKCore.Instance.UISettings.raceTimerFormat));
            racePercentageText?.SetText(UIHelper.FormatPercentageText(Entity.Competitor.RacePercentage));
            lapPercentageText?.SetText(UIHelper.FormatPercentageText(Entity.Competitor.LapPercentage));
            distanceTravelledText?.SetText(DistanceText());
            distanceRemainingText?.SetText(UIHelper.FormatDistanceText(Entity.Competitor.GetRemainingDistance()));
            averageSpeedText?.SetText(UIHelper.FormatSpeedText(Entity.Competitor.AverageSpeed, true));
            topSpeedText?.SetText(UIHelper.FormatSpeedText(Entity.Competitor.TopSpeed, true));

            if (Time.time > _nextGapUpdate)
            {
                _nextGapUpdate = Time.time + 1;

                if (leaderDistanceGapText != null)
                {
                    leaderDistanceGapText.SetText(RealTimeGap(true));
                }

                if (leaderTimeGapText != null)
                {
                    leaderTimeGapText.SetText(RealTimeGap(false));
                }

                if (bestTimeGapText != null)
                {
                    bestTimeGapText.SetText(BestLapGap());
                }
            }
        }

        public override void Refresh()
        {
            if (Entity == null || Entity.Competitor == null)
                return;

            OnRacePositionChanged();
            OnLapCompleted(Entity.Competitor);
            OnCompetitorFinished(Entity.Competitor);
            OnHitCheckpoint(null, Entity.Competitor);
            OnHitSpeedtrap(Entity.Competitor, 0);
            OnScoreAdded(Entity.Competitor, 0);
        }

        public override void Destroy()
        {

        }

        void OnRacePositionChanged()
        {
            if (Entity == null || Entity.Competitor == null)
                return;

            positionText?.SetText(UIHelper.FormatPositionText(Entity.Competitor.Position, positionDisplayMode, positionText.fontSize / 2));
            overtakesText?.SetText(Entity.Competitor.Overtakes.ToString());
        }

        void OnLapCompleted(Competitor c)
        {
            if (c.Entity != Entity)
                return;

            lapText?.SetText(UIHelper.FormatLapText(Entity.Competitor.CurrentLap, lapDisplayMode, lapText.fontSize / 2));
            lapsCompletedText?.SetText(UIHelper.FormatLapText(Entity.Competitor.CurrentLap - 1, lapDisplayMode, lapsCompletedText.fontSize / 2));
            finishTimeText?.SetText(FinishTime());
            bestLapTimeText?.SetText(UIHelper.FormatTimeText(Entity.Competitor.GetBestLapTime(), RGSKCore.Instance.UISettings.raceTimerFormat));
            lastLapTimeText?.SetText(UIHelper.FormatTimeText(Entity.Competitor.GetLastLapTime(), RGSKCore.Instance.UISettings.raceTimerFormat));
        }

        void OnHitCheckpoint(CheckpointNode cp, Competitor c)
        {
            if (c.Entity != Entity)
                return;

            if (checkpointText != null)
            {
                var total = Entity.Competitor.TotalRaceCheckpoints > 0 ? Entity.Competitor.TotalRaceCheckpoints.ToString() : "-";
                checkpointText?.SetText($"{Entity.Competitor.TotalCheckpointsPassed}/{total}");
            }
        }

        void OnHitSpeedtrap(Competitor c, float s)
        {
            if (c.Entity != Entity)
                return;

            totalSpeedText?.SetText($"{UIHelper.FormatSpeedText(Entity.Competitor.TotalSpeedtrapSpeed, true)}");
        }

        void OnScoreAdded(Competitor c, float value)
        {
            if (c.Entity != Entity)
                return;

            scoreText?.SetText($"{UIHelper.FormatPointsText(Entity.Competitor.Score)}");
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (c.Entity != Entity)
                return;

            finishTimeGapText?.SetText(FinishTimeGap());

            if (ChampionshipManager.Instance.Initialized)
            {
                championshipPositionText?.SetText(UIHelper.FormatPositionText(ChampionshipManager.Instance.GetEntrantPosition(c.Entity), positionDisplayMode));
                championshipRoundPointsText?.SetText($"+{ChampionshipManager.Instance.GetPointsForPosition(c.FinalPosition - 1).ToString()}");
                championshipTotalPointsText?.SetText(ChampionshipManager.Instance.GetEntrantPoints(c.Entity).ToString());
            }
        }

        void OnRaceRestart()
        {
            Refresh();
        }

        string DistanceText()
        {
            var distance = Entity.Competitor.DistanceTravelled;
            distance = Mathf.Clamp(distance, 0, distance);

            return Entity.Competitor.IsDisqualified ?
                    RGSKCore.Instance.UISettings.dnfString :
                    UIHelper.FormatDistanceText(distance);
        }

        string FinishTime()
        {
            var time = 0f;

            if (Entity.Competitor.IsFinished())
            {
                time = Entity.Competitor.TotalRaceTime;
            }

            return Entity.Competitor.IsDisqualified ?
                    RGSKCore.Instance.UISettings.dnfString :
                    UIHelper.FormatTimeText(time, RGSKCore.Instance.UISettings.raceTimerFormat);
        }

        string FinishTimeGap()
        {
            var leader = RaceManager.Instance.GetCompetitorInPosition(1);

            if (leader == null || !Entity.Competitor.IsFinished() || Entity.Competitor.IsDisqualified)
            {
                return "";
            }

            var gap = Entity.Competitor.TotalRaceTime - leader.TotalRaceTime;
            return UIHelper.FormatTimeText(gap, RGSKCore.Instance.UISettings.raceTimerFormat, true);
        }

        string RealTimeGap(bool distance)
        {
            var gap = 0f;
            var focused = GeneralHelper.GetFocusedEntity();

            if (Entity == focused)
            {
                return "";
            }

            if (Entity.Competitor.IsFinished())
            {
                return FinishTime();
            }

            if (focused != null)
            {
                var lapGap = GeneralHelper.GetLapsAhead(focused.Competitor, Entity.Competitor);
                if (lapGap != 0)
                {
                    return $"{lapGap} Lap";
                }

                if (distance)
                {
                    gap = GeneralHelper.GetDistanceGapBetween(Entity.Competitor, focused.Competitor);
                    return $"{UIHelper.FormatDistanceText(gap, true)}";
                }
                else
                {
                    gap = GeneralHelper.GetTimeGapBetween(focused.Competitor, Entity.Competitor);
                    return UIHelper.FormatTimeText(gap, RGSKCore.Instance.UISettings.realtimeGapTimerFormat, true);
                }
            }

            return "";
        }

        string BestLapGap()
        {
            var leader = RaceManager.Instance.GetCompetitorInPosition(1);
            var gap = 0f;

            if (Entity.Competitor == leader)
            {
                gap = leader.GetBestLapTime();
            }
            else
            {
                gap = Entity.Competitor.GetBestLapTime() - leader.GetBestLapTime();
            }

            return UIHelper.FormatTimeText(gap, RGSKCore.Instance.UISettings.raceTimerFormat, Entity.Competitor != leader);
        }
    }
}