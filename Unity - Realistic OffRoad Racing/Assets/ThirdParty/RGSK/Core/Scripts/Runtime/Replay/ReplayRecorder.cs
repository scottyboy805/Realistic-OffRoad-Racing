using UnityEngine;
using System.Collections.Generic;
using RGSK.Helpers;

namespace RGSK
{
    public class ReplayRecorder : Recorder
    {
        UIScreenID replayScreeen => RGSKCore.Instance.UISettings.screens.ReplayScreeen;
        bool _damageEnabled;

        protected override void Playback()
        {
            var totalFrames = GetTotalFrames();

            if (totalFrames > 0)
            {
                if (CurrentFrame > totalFrames)
                {
                    RestartPlayback();
                }

                if (CurrentFrame < 0)
                {
                    CurrentFrame = 0;
                    SetPlaybackSpeed(1);
                }

                PlayFrame(CurrentFrame);
                CurrentFrame += PlaybackSpeed;
            }
        }

        public override void StartPlayback()
        {
            if (GetTotalFrames() == 0)
                return;

            if (IsPlayback)
            {
                UIManager.Instance?.OpenScreen(replayScreeen);
                return;
            }

            base.StartPlayback();

            ToggleInput();
            UIScreenFader.Instance?.FadeOut(0.5f);
            UIManager.Instance?.OpenScreen(replayScreeen);

            foreach (var obj in recordableObjects)
            {
                if (obj.gameObject != null)
                {
                    GeneralHelper.ToggleGhostedMesh(obj.gameObject, false);
                }

                if (obj.vehicle != null)
                {
                    _damageEnabled = obj.vehicle.Damageable;
                    obj.vehicle.Repair();
                    obj.vehicle.Damageable = false;
                    obj.vehicle.OdometerEnabled = false;
                }
            }

            RGSKEvents.OnReplayStart.Invoke();
        }

        public override void StopPlayback()
        {
            if (!IsPlayback)
                return;

            base.StopPlayback();
            ToggleInput();

            foreach (var obj in recordableObjects)
            {
                if (obj.vehicle != null)
                {
                    obj.vehicle.Damageable = _damageEnabled;
                    obj.vehicle.OdometerEnabled = true;
                    obj.vehicle.HornOn = false;
                }
            }

            RGSKEvents.OnReplayStop.Invoke();
        }

        public override void RestartPlayback()
        {
            base.RestartPlayback();
            UIScreenFader.Instance?.FadeOut(0.5f);
        }

        public override void SetPlaybackSpeed(int speed)
        {
            base.SetPlaybackSpeed(speed);

            if (!IsSlowMotion)
            {
                AudioListener.pause = PlaybackSpeed != 1 ? true : false;
                Time.timeScale = PlaybackSpeed == 0 ? 0 : 1;
            }
            else
            {
                AudioListener.pause = false;
                Time.timeScale = Settings != null ? Settings.slowMotionTimeScale : 0.5f;
            }

            CameraManager.Instance?.SetUpdateMethod(PlaybackSpeed == 0 ?
            Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate :
            Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);
        }

        void ToggleInput()
        {
            PlayerVehicleInput.Instance.enabled = !IsPlayback;

            foreach (var obj in recordableObjects)
            {
                if (obj.gameObject != null)
                {
                    if (obj.gameObject.TryGetComponent<AIController>(out var ai))
                    {
                        ai.enabled = !IsPlayback;
                    }
                }
            }
        }
    }
}