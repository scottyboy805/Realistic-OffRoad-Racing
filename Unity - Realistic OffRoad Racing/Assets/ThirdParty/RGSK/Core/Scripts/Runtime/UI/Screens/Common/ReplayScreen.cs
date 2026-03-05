using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class ReplayScreen : UIScreen
    {
        [SerializeField] TMP_Text currentTimeText;
        [SerializeField] TMP_Text totalTimeText;
        [SerializeField] TMP_Text playbackSpeedText;

        [Header("Slider Controls")]
        [SerializeField] Slider slider;
        [SerializeField] bool updateFrameOnSliderDrag;

        [Header("Button Controls")]
        [SerializeField] Button playAndPauseButton;
        [SerializeField] Button fastForwardButton;
        [SerializeField] Button rewindButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button framePlaybackButton;
        [SerializeField] Button slowMotionButton;
        [SerializeField] Button hideUiButton;
        [SerializeField] Button exitButton;
        [SerializeField] Button changeCameraButton;
        [SerializeField] Button changeTargetNextButton;
        [SerializeField] Button changeTargetPreviousButton;

        AutoHideCanvasGroup _ui;
        bool _sliderPressed;
        bool _playOnSliderUp;

        public override void Initialize()
        {
            base.Initialize();

            _ui = GetComponentInChildren<AutoHideCanvasGroup>();

            playAndPauseButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.TogglePlayAndPause());
            fastForwardButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.FastForward());
            rewindButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.Rewind());
            restartButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.RestartPlayback());
            framePlaybackButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.FrameByFramePlayback());
            slowMotionButton?.onClick.AddListener(() => RecorderManager.Instance?.ReplayRecorder?.ToggleSlowMotion());
            hideUiButton?.onClick.AddListener(() => _ui?.SetVisible(false));
            exitButton?.onClick.AddListener(() => Back());
            changeCameraButton?.onClick.AddListener(() => CameraManager.Instance?.ChangePerspective(1));
            changeTargetNextButton?.onClick.AddListener(() => ChangeTarget(1));
            changeTargetPreviousButton?.onClick.AddListener(() => ChangeTarget(-1));

            if (slider != null)
            {
                var eventTrigger = slider.gameObject.GetOrAddComponent<EventTrigger>();
                eventTrigger.AddListener(EventTriggerType.PointerDown, OnSliderDown);
                eventTrigger.AddListener(EventTriggerType.PointerUp, OnSliderUp);
                eventTrigger.AddListener(EventTriggerType.Drag, OnSliderDrag);
                slider.wholeNumbers = true;
            }
        }

        public override void Open()
        {
            base.Open();

            CameraManager.Instance?.ToggleRouteCameras(true);

            if (RaceManager.Instance.Initialized)
            {
                RaceManager.Instance.ForceFinishRace(true);
            }
        }

        public override void Close()
        {
            base.Close();

            RecorderManager.Instance?.ReplayRecorder.SetPlaybackSpeed(1);
            RecorderManager.Instance?.ReplayRecorder.ActivateSlowMotionPlayback(false);
            CameraManager.Instance?.ToggleRouteCameras(RaceManager.Instance.Initialized);
        }

        public override void Back()
        {
            base.Back();

            if (RaceManager.Instance.Initialized && RaceManager.Instance.CurrentState == RaceState.PostRace)
                return;

            RecorderManager.Instance?.ReplayRecorder?.StopPlayback();
        }

        void Update()
        {
            if (!IsOpen())
                return;

            UpdateUIElements();
        }

        void UpdateUIElements()
        {
            if (RecorderManager.Instance.ReplayRecorder == null || !RecorderManager.Instance.ReplayRecorder.IsPlayback)
                return;

            currentTimeText?.SetText(UIHelper.FormatTimeText(RecorderManager.Instance.ReplayRecorder.GetCurrentTime, TimeFormat.MM_SS));
            totalTimeText?.SetText(UIHelper.FormatTimeText(RecorderManager.Instance.ReplayRecorder.GetTotalTime, TimeFormat.MM_SS));

            if (!RecorderManager.Instance.ReplayRecorder.IsSlowMotion)
            {
                playbackSpeedText?.SetText($"{RecorderManager.Instance.ReplayRecorder.PlaybackSpeed}x");
            }
            else
            {
                playbackSpeedText?.SetText($"{Time.timeScale}x");
            }

            if (slider != null)
            {
                slider.maxValue = RecorderManager.Instance.ReplayRecorder.GetTotalFrames();

                if (!_sliderPressed)
                {
                    slider.value = RecorderManager.Instance.ReplayRecorder.CurrentFrame;
                }
            }
        }

        void OnSliderDown(PointerEventData data)
        {
            _sliderPressed = true;

            if (updateFrameOnSliderDrag)
            {
                _playOnSliderUp = RecorderManager.Instance.ReplayRecorder.PlaybackSpeed != 0;
            }
        }

        void OnSliderUp(PointerEventData data)
        {
            RecorderManager.Instance.ReplayRecorder.PlayFrame((int)slider.value);
            _sliderPressed = false;

            if (updateFrameOnSliderDrag)
            {
                RecorderManager.Instance.ReplayRecorder.SetPlaybackSpeed(_playOnSliderUp ? 1 : 0);
            }
        }

        void OnSliderDrag(PointerEventData data)
        {
            if (updateFrameOnSliderDrag)
            {
                RecorderManager.Instance.ReplayRecorder.SetPlaybackSpeed(0);
                RecorderManager.Instance.ReplayRecorder.PlayFrame((int)slider.value);
            }
        }

        void ChangeTarget(int dir)
        {
            //Fix for an issue that stops target changing when playback is paused
            StartCoroutine(ChangeTargetRoutine(dir));
        }

        IEnumerator ChangeTargetRoutine(int dir)
        {
            var oldTimeScale = Time.timeScale;

            Time.timeScale = 1;
            CameraManager.Instance?.ChangeTarget(dir);

            yield return null;

            Time.timeScale = oldTimeScale;
        }
    }
}