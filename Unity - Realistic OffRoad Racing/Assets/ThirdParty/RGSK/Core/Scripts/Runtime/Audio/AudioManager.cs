using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using RGSK.Helpers;

namespace RGSK
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer Mixer => RGSKCore.Instance.AudioSettings.mixer;
        SoundList sounds => RGSKCore.Instance.AudioSettings.sounds;

        AudioSource _uiNavigationAudioSource;
        float _lastUIHoverSoundTime;

        public override void Awake()
        {
            base.Awake();

            _uiNavigationAudioSource = AudioHelper.CreateAudioSource(
                                                        null,
                                                        true,
                                                        false,
                                                        false,
                                                        1,
                                                        1,
                                                        AudioGroup.UI.ToString(),
                                                        transform,
                                                        "AudioSource_UINavigation");
            
            _uiNavigationAudioSource.bypassEffects = _uiNavigationAudioSource.bypassReverbZones = true;
        }

        public void Play(string sound, AudioGroup group, Vector3? position = null, bool destroyOnSceneLoad = true)
        {
            if (string.IsNullOrWhiteSpace(sound))
                return;

            Play(sound, group.ToString(), position, destroyOnSceneLoad);
        }

        void Play(string key, string group, Vector3? position, bool destroyOnSceneLoad)
        {
            if (sounds == null)
            {
                Logger.Log("Sounds have not been assigned!");
                return;
            }

            var clip = sounds.GetSound(key);

            if (clip != null)
            {
                var src = AudioHelper.CreateAudioSource(
                                                        clip,
                                                        !position.HasValue,
                                                        false,
                                                        true,
                                                        1,
                                                        1,
                                                        group,
                                                        null,
                                                        key);

                if (position.HasValue)
                {
                    src.transform.position = position.Value;
                    src.rolloffMode = AudioRolloffMode.Linear;
                    src.maxDistance = 50;
                    src.dopplerLevel = 0;
                }

                if (!destroyOnSceneLoad)
                {
                    src.gameObject.AddComponent<DontDestroyOnLoad>();
                }

                src.gameObject.AddComponent<AutoDestroyAudiosource>();
            }
        }

        public void PlayUIClickSound()
        {
            if(_uiNavigationAudioSource == null)
                return;
            
            var clip = sounds.GetSound(RGSKCore.Instance.UISettings.buttonClickSound);

            if (clip != null)
            {
                _uiNavigationAudioSource.Stop();
                _uiNavigationAudioSource.clip = clip;
                _uiNavigationAudioSource.Play();
            }
        }

        public void PlayUIHoverSound()
        {
            if(_uiNavigationAudioSource == null)
                return;

            if(Time.unscaledTime > _lastUIHoverSoundTime + 0.1f)
            {
                var hoverClip = sounds.GetSound(RGSKCore.Instance.UISettings.buttonHoverSound);
                var clickClip = sounds.GetSound(RGSKCore.Instance.UISettings.buttonClickSound);

                if (_uiNavigationAudioSource.isPlaying && _uiNavigationAudioSource.clip == clickClip)
                    return;

                if(hoverClip != null)
                {
                    _uiNavigationAudioSource.Stop();
                    _uiNavigationAudioSource.clip = hoverClip;
                    _uiNavigationAudioSource.Play();
                    _lastUIHoverSoundTime = Time.unscaledTime;
                }
            }
        }
    }
}