using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class RecorderControlPanel : MonoBehaviour
    {
        public enum RecorderType
        {
            Replay = 0,
            Ghost = 1,
        }

        [SerializeField] RecorderType recorderType;
        [SerializeField] Button startRecordingButton;
        [SerializeField] Button stopRecordingButton;
        [SerializeField] Button startPlaybackButton;
        [SerializeField] Button deleteRecordingButton;
        [SerializeField] TMP_Text statusText;
        [SerializeField] TMP_Text recordTimeText;
        [SerializeField] Image statusImage;
        [SerializeField] Color statusNotRecording = Color.gray;
        [SerializeField] Color statusRecording = Color.red;
        [SerializeField] Color statusPlayback = Color.green;

        void Start()
        {
            startRecordingButton?.onClick?.AddListener(() => GetRecorder()?.StartRecording());
            stopRecordingButton?.onClick?.AddListener(() => GetRecorder()?.StopRecording());
            startPlaybackButton?.onClick?.AddListener(() => StartPlayback());
            deleteRecordingButton?.onClick?.AddListener(() => GetRecorder()?.DeleteRecordedData());
        }

        void Update()
        {
            if (GetRecorder() == null)
                return;

            statusText?.SetText(GetRecorder().IsRecording ? "Recording" : GetRecorder().IsPlayback ? "Playing" : "Not Recording");
            recordTimeText?.SetText(UIHelper.FormatTimeText(GetRecorder().GetTotalTime, TimeFormat.MM_SS));

            if (statusImage != null)
            {
                statusImage.color = GetRecorder().IsRecording ? statusRecording : GetRecorder().IsPlayback ? statusPlayback : statusNotRecording;
            }
        }

        void StartPlayback()
        {
            if (recorderType == RecorderType.Ghost)
            {
                RecorderManager.Instance.GhostRecorder?.CacheReplayData();
            }

            GetRecorder()?.StartPlayback();
        }

        Recorder GetRecorder()
        {
            switch (recorderType)
            {
                case RecorderType.Replay:
                    {
                        return RecorderManager.Instance.ReplayRecorder;
                    }

                case RecorderType.Ghost:
                    {
                        return RecorderManager.Instance.GhostRecorder;
                    }
            }

            return null;
        }
    }
}