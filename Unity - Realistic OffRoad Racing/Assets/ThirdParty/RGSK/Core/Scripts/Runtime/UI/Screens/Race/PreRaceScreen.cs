using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class PreRaceScreen : RaceScreenBase
    {
        [SerializeField] Button startRaceButton;

        [Tooltip("The board layout that will be used during championships.")]
        [SerializeField] RaceBoardLayout championshipBoardLayout;

        public override void Initialize()
        {
            base.Initialize();
            startRaceButton?.onClick?.AddListener(() => RaceManager.Instance?.StartRace());

            if (ChampionshipManager.Instance.Initialized && championshipBoardLayout != null)
            {
                raceBoardLayout = championshipBoardLayout;
            }
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnRaceRestart() { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}