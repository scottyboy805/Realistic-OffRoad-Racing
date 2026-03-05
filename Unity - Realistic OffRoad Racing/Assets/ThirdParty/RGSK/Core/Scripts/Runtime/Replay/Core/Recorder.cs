using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public abstract class Recorder : MonoBehaviour
    {
        public RecorderSettings Settings => RGSKCore.Instance.GeneralSettings.recorderSettings;
        public float GetTotalTime => GetTotalFrames() * Time.fixedDeltaTime;
        public float GetCurrentTime => CurrentFrame * Time.fixedDeltaTime;
        public bool IsRecording => _record;
        public bool IsPlayback => _playback;
        public bool IsPaused => PlaybackSpeed == 0;
        public bool IsSlowMotion { get; private set; }
        public int PlaybackSpeed { get; private set; } = 1;
        public int CurrentFrame { get; protected set; }
        protected List<RecordableObject> recordableObjects = new List<RecordableObject>();

        bool _record;
        bool _playback;

        public void AddRecordableObject(GameObject go)
        {
            var obj = new RecordableObject
            {
                gameObject = go,
                rigidbody = go.GetComponent<Rigidbody>(),
                vehicle = go.GetComponent<IVehicle>(),
            };

            obj.AllocateFrames();
            recordableObjects.Add(obj);
        }

        public void RemoveRecordableObject(GameObject go)
        {
            foreach (var obj in recordableObjects)
            {
                if (obj.gameObject != null && obj.gameObject == go)
                {
                    recordableObjects.Remove(obj);
                    break;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_record)
            {
                Record();
            }

            if (_playback)
            {
                Playback();
            }
        }

        protected void Record()
        {
            if (Settings != null && Settings.recordingTimeLimit > 0)
            {
                if (GetTotalTime > Settings.recordingTimeLimit)
                {
                    Logger.Log(this, "Reached the recording time limit!");
                    StopRecording();
                    return;
                }
            }

            for (int i = 0; i < recordableObjects.Count; i++)
            {
                recordableObjects[i].CreateFrame();
            }
        }

        protected virtual void Playback() { }

        public void PlayFrame(int frame)
        {
            if (frame < 0 || frame > GetTotalFrames() || GetTotalFrames() == 0)
                return;

            CurrentFrame = frame;

            for (int i = 0; i < recordableObjects.Count; i++)
            {
                recordableObjects[i].PlayFrame(frame);
            }
        }

        public void GoToLastFrame() => PlayFrame(GetTotalFrames());

        public void SkipToFrame(int frame) => CurrentFrame = frame;

        public int GetTotalFrames()
        {
            if (recordableObjects.Count == 0)
                return 0;

            return GetTotalFrames(recordableObjects[0]);
        }

        public int GetTotalFrames(RecordableObject recordableObject)
        {
            var result = 0;

            if (recordableObject.FrameData.Count > 0)
            {
                result = recordableObject.FrameData.Count - 1;
            }

            return Mathf.Clamp(result, 0, result);
        }

        public virtual void SetPlaybackSpeed(int speed)
        {
            if (!IsPlayback)
                return;

            if (IsSlowMotion && speed != 1)
            {
                ActivateSlowMotionPlayback(false);
            }

            PlaybackSpeed = speed;
        }

        public virtual void StartRecording()
        {
            if (IsRecording)
                return;

            _record = true;
        }

        public virtual void StopRecording()
        {
            if (!IsRecording)
                return;

            _record = false;
        }

        public virtual void StartPlayback()
        {
            if (IsPlayback)
                return;

            StopRecording();
            ActivateSlowMotionPlayback(false);
            SetPlaybackSpeed(1);
            _playback = true;
            CurrentFrame = 0;
        }

        public virtual void StopPlayback()
        {
            if (!IsPlayback)
                return;

            _playback = false;
        }

        public virtual void RestartPlayback()
        {
            if (!IsPlayback)
                return;

            CurrentFrame = 0;
            ActivateSlowMotionPlayback(false);
            SetPlaybackSpeed(1);
        }

        public virtual void TogglePlayAndPause()
        {
            SetPlaybackSpeed(PlaybackSpeed != 0 ? 0 : 1);
        }

        public void FrameByFramePlayback()
        {
            if (!IsPlayback)
                return;

            if (PlaybackSpeed != 0)
            {
                SetPlaybackSpeed(0);
            }

            Playback();
            CurrentFrame += 1;
        }

        public void ToggleSlowMotion()
        {
            IsSlowMotion = !IsSlowMotion;
            ActivateSlowMotionPlayback(IsSlowMotion);
        }

        public void ActivateSlowMotionPlayback(bool active)
        {
            IsSlowMotion = active;
            SetPlaybackSpeed(1);
        }

        public void FastForward()
        {
            if (IsSlowMotion)
            {
                ActivateSlowMotionPlayback(false);
                SetPlaybackSpeed(1);
                return;
            }

            var speed = PlaybackSpeed;
            speed += 1;
            speed = Mathf.Clamp(speed, 1, Settings != null ? Settings.maxPlaybackSpeed : 5);
            SetPlaybackSpeed(speed);
        }

        public void Rewind()
        {
            var speed = PlaybackSpeed;
            speed -= 1;
            speed = Mathf.Clamp(speed, Settings != null ? -Settings.maxPlaybackSpeed : -5, -1);
            SetPlaybackSpeed(speed);
        }

        public void DeleteRecordedData()
        {
            StopRecording();
            StopPlayback();

            for (int i = 0; i < recordableObjects.Count; i++)
            {
                recordableObjects[i].DeleteFrameData();
            }
        }

        public void RemoveRecordableObjects()
        {
            DeleteRecordedData();
            recordableObjects.Clear();
        }
    }
}