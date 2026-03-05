using UnityEngine;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class DriftUI : EntityUIComponent
    {
        public TMP_Text totalPointsText;

        [Header("Current")]
        public CanvasGroup currentPanel;
        public TMP_Text currentPointsText;
        public TMP_Text currentMultiplierText;
        public TMP_Text currentTimeText;
        public TMP_Text currentAngleText;
        public TMP_Text currentDistanceText;
        public Gauge currentMultiplierGauge;
        public Gauge currentCompletionGauge;

        [Header("Best")]
        public TMP_Text bestPointsText;
        public TMP_Text bestDistanceText;
        public TMP_Text bestTimeText;
        public TMP_Text bestAngleText;
        public TMP_Text bestMultiplierText;

        TextChangeSoundPlayer _textChangeSoundPlayer;
        float _lastCurrentPointsUpdateTime;

        void OnEnable()
        {
            RGSKEvents.OnDriftStart.AddListener(OnDriftStart);
            RGSKEvents.OnDriftEnd.AddListener(OnDriftEnd);
            RGSKEvents.OnDriftPointMultiplierReached.AddListener(OnDriftPointMultiplierReached);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
        }

        void OnDisable()
        {
            RGSKEvents.OnDriftStart.RemoveListener(OnDriftStart);
            RGSKEvents.OnDriftEnd.RemoveListener(OnDriftEnd);
            RGSKEvents.OnDriftPointMultiplierReached.RemoveListener(OnDriftPointMultiplierReached);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
        }

        public override void Bind(RGSKEntity e)
        {
            base.Bind(e);
            _textChangeSoundPlayer = GetComponentInChildren<TextChangeSoundPlayer>();
            ToggleCanvasGroup(false);
        }

        public override void Update()
        {
            if (Entity == null || Entity.DriftController == null)
                return;

            if (Entity.DriftController != null && Entity.DriftController.HasDriftStarted)
            {
                if (Time.time > _lastCurrentPointsUpdateTime)
                {
                    _lastCurrentPointsUpdateTime = Time.time + 0.05f;
                    currentPointsText?.SetText(UIHelper.FormatPointsText(Entity.DriftController.CurrentPoints));
                }

                currentAngleText?.SetText(UIHelper.FormatAngleText(Entity.DriftController.CurrentAngle));
                currentDistanceText?.SetText(UIHelper.FormatDistanceText(Entity.DriftController.CurrentDistance));
                currentTimeText?.SetText(UIHelper.FormatTimeText(Entity.DriftController.CurrentTimer, RGSKCore.Instance.UISettings.raceTimerFormat));
                currentMultiplierGauge?.SetValue(Entity.DriftController.MultiplierProgress);
                currentCompletionGauge?.SetValue(Entity.DriftController.CompleteProgress);
            }
        }

        public override void Refresh()
        {
            if (Entity == null || Entity.DriftController == null)
                return;

            OnDriftStart(Entity.DriftController);
            OnDriftEnd(Entity.DriftController);
            OnDriftPointMultiplierReached(Entity.DriftController);
        }

        public override void Destroy()
        {

        }

        void OnDriftStart(DriftController dc)
        {
            if (dc.Entity != Entity)
                return;

            ToggleCanvasGroup(true);
        }

        void OnDriftPointMultiplierReached(DriftController dc)
        {
            if (dc.Entity != Entity)
                return;

            currentMultiplierText?.SetText($"x{Entity.DriftController.CurrentMultiplier.ToString("F1")}");
        }

        void OnDriftEnd(DriftController dc)
        {
            if (dc.Entity != Entity)
                return;

            totalPointsText?.SetText(UIHelper.FormatPointsText(Entity.DriftController.TotalPoints));
            bestPointsText?.SetText(UIHelper.FormatPointsText(Entity.DriftController.BestPoints));
            bestDistanceText?.SetText(UIHelper.FormatDistanceText(Entity.DriftController.BestDistance));
            bestTimeText?.SetText(UIHelper.FormatTimeText(Entity.DriftController.BestTime, RGSKCore.Instance.UISettings.raceTimerFormat));
            bestAngleText?.SetText(UIHelper.FormatAngleText(Entity.DriftController.BestAngle));
            bestMultiplierText?.SetText($"x{Entity.DriftController.BestMultiplier.ToString("F1")}");
            OnDriftPointMultiplierReached(Entity.DriftController);
            ToggleCanvasGroup(false);
        }

        void Clear()
        {
            currentPointsText?.SetText(UIHelper.FormatPointsText(0));
            currentAngleText?.SetText(UIHelper.FormatAngleText(0));
            currentDistanceText?.SetText(UIHelper.FormatDistanceText(0));
            currentTimeText?.SetText(UIHelper.FormatTimeText(0, RGSKCore.Instance.UISettings.raceTimerFormat));
            currentMultiplierGauge?.SetValue(0);
            currentCompletionGauge?.SetValue(0);
        }

        void ToggleCanvasGroup(bool enable)
        {
            if (currentPanel == null)
                return;

            if(_textChangeSoundPlayer != null)
            {
                _textChangeSoundPlayer.enabled = enable;
            }

            Clear();
            currentPanel.SetAlpha(enable ? 1 : 0);
        }

        void OnRaceRestart()
        {
            Refresh();
        }
    }
}