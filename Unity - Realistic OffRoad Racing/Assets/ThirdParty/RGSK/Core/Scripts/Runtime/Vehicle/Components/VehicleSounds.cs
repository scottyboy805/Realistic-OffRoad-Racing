using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    [System.Serializable]
    public class VehicleSounds : VehicleComponent
    {
        [Header("Engine")]
        [SerializeField] AnimationCurve enginePitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] AudioClip engineSound;
        [SerializeField] AudioClip engineStartSound;
        [SerializeField] Transform enginePosition;

        [Header("Horn")]
        [SerializeField] AudioClip hornSound;
        [SerializeField] float hornSoundPitch = 1;

        AudioSource _engineAudiosource;
        AudioSource _startAudiosource;
        AudioSource _windAudioSource;
        AudioSource _hornAudioSource;

        bool _playedStartSound;

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            _engineAudiosource = AudioHelper.CreateAudioSource(engineSound, false, true, true, 1, 1, AudioGroup.Vehicle.ToString(), Vehicle.transform);
            _startAudiosource = AudioHelper.CreateAudioSource(engineStartSound, false, false, false, 1, 1, AudioGroup.Vehicle.ToString(), Vehicle.transform);
            _windAudioSource = AudioHelper.CreateAudioSource(RGSKCore.Instance.VehicleSettings.windSound, false, true, true, 1, 1, AudioGroup.Vehicle.ToString(), Vehicle.transform);
            _hornAudioSource = AudioHelper.CreateAudioSource(hornSound, false, true, false, 1, hornSoundPitch, AudioGroup.Vehicle.ToString(), Vehicle.transform);

            if (enginePosition != null)
            {
                _engineAudiosource.transform.position = enginePosition.position;
            }
        }

        public override void Update()
        {
            var t = enginePitchCurve.Evaluate(Vehicle.engine.Rpm / Vehicle.engine.maxRpm);
            _engineAudiosource.volume = Mathf.Lerp(Vehicle.engine.Running ? 0.75f : 0, 1f, t);
            _engineAudiosource.pitch = Mathf.Lerp(1f, 2f, t);
            _windAudioSource.volume = Mathf.Lerp(0, 0.75f, Mathf.InverseLerp(50, 200, Vehicle.CurrentSpeed));

            if (Vehicle.engine.Starting && !_playedStartSound)
            {
                _playedStartSound = true;
                _startAudiosource.Play();
            }

            if (Vehicle.engine.Running)
            {
                _playedStartSound = false;
            }
        }

        public void Horn(bool play)
        {
            if (play)
            {
                if (!_hornAudioSource.isPlaying)
                    _hornAudioSource?.Play();
            }
            else
            {
                if (_hornAudioSource.isPlaying)
                    _hornAudioSource?.Stop();
            }
        }

        public void ToggleWindSound(bool toggle)
        {
            if(_windAudioSource == null)
                return;
            
            _windAudioSource.mute = !toggle;
        }
    }
}