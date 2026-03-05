using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public abstract class RaceScreenBase : UIScreen
    {
        [SerializeField] protected Transform boardParent;
        [SerializeField] protected RaceBoard raceBoard;
        [SerializeField] protected RaceBoardLayout raceBoardLayout;
        [SerializeField] protected TargetScoreBoard targetScoreBoard;

        protected RaceBoard _raceBoard;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Open()
        {
            base.Open();
            _raceBoard?.Refresh();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            RGSKEvents.OnWrongwayStart.AddListener(OnWrongwayStart);
            RGSKEvents.OnWrongwayStop.AddListener(OnWrongwayStop);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
            RGSKEvents.OnWrongwayStart.RemoveListener(OnWrongwayStart);
            RGSKEvents.OnWrongwayStop.RemoveListener(OnWrongwayStop);
        }

        protected virtual void Start()
        {
            if (!RaceManager.Instance.Initialized)
                return;

            if (boardParent != null)
            {
                if (!RaceManager.Instance.Session.IsTargetScoreSession())
                {
                    if (raceBoardLayout == null)
                    {
                        var layout = RGSKCore.Instance.UISettings.raceBoardLayouts.FirstOrDefault(x => x.raceTypes.Contains(RaceManager.Instance.Session.raceType));
                        raceBoardLayout = layout ?? RGSKCore.Instance.UISettings.defaultRaceBoardLayout;
                    }

                    if (raceBoard != null && raceBoardLayout != null)
                    {
                        _raceBoard = Instantiate(raceBoard, boardParent, false);
                        _raceBoard.Initialize(raceBoardLayout);
                    }
                }
                else
                {
                    if (targetScoreBoard != null)
                    {
                        var board = Instantiate(targetScoreBoard, boardParent, false);
                        board.Initialize();
                    }
                }
            }
        }

        protected abstract void OnCompetitorFinished(Competitor c);
        protected abstract void OnRaceRestart();
        protected abstract void OnWrongwayStart(Competitor c);
        protected abstract void OnWrongwayStop(Competitor c);
    }
}