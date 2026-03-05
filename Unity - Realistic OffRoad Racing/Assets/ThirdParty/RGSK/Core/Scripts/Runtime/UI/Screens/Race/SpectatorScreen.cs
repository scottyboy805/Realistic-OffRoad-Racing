using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class SpectatorScreen : RaceScreenBase
    {
        [SerializeField] GameObject sessionTimeContainer;

        [Header("Controls")]
        [SerializeField] Button changeCameraButton;
        [SerializeField] Button changeTargetNextButton;
        [SerializeField] Button changeTargetPreviousButton;
        [SerializeField] Button hideUIButton;

        AutoHideCanvasGroup _ui;
        TabController _tabController;
        TrackDefinitionUI _trackDefinitionUI;

        public override void Initialize()
        {
            base.Initialize();

            _ui = GetComponentInChildren<AutoHideCanvasGroup>();
            _tabController = GetComponentInChildren<TabController>();
            _trackDefinitionUI = GetComponentInChildren<TrackDefinitionUI>();

            _trackDefinitionUI?.UpdateUI(RaceManager.Instance?.Track?.definition);

            changeCameraButton?.onClick.AddListener(() => CameraManager.Instance?.ChangePerspective(1));
            changeTargetNextButton?.onClick.AddListener(() => CameraManager.Instance?.ChangeTarget(1));
            changeTargetPreviousButton?.onClick.AddListener(() => CameraManager.Instance?.ChangeTarget(-1));
            hideUIButton?.onClick.AddListener(() => _ui?.SetVisible(false));
        }
        
        protected override void Start()
        {
            base.Start();

            if (sessionTimeContainer != null)
            {
                sessionTimeContainer.SetActive(RaceManager.Instance?.Session?.IsTimedSession() ?? false);
            }
        }

        public override void Open()
        {
            base.Open();
            _tabController?.ChangeTab(0);
        }

        protected override void OnCompetitorFinished(Competitor c) { }
        protected override void OnRaceRestart() { }
        protected override void OnWrongwayStart(Competitor c) { }
        protected override void OnWrongwayStop(Competitor c) { }
    }
}
