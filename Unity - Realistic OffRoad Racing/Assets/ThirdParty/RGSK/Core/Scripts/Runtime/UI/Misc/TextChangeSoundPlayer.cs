using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class TextChangeSoundPlayer : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] AudioClip sound;

        AudioSource _audiosource;
        string _lastValue = "";
        float _lastChange;
        float _activeTime;

        void OnDisable() 
        {
            Pause();
        }

        void Start()
        {
            if (text == null)
                return;

            _audiosource = AudioHelper.CreateAudioSource(sound,
                            true,
                            true,
                            false,
                            0.5f,
                            1,
                            AudioGroup.UI.ToString(),
                            transform);

            _lastValue = text.text;
        }

        void LateUpdate()
        {
            if (text == null)
                return;

            if (!string.IsNullOrWhiteSpace(text.text) && text.text != _lastValue)
            {
                _lastChange = Time.unscaledTime;
                _lastValue = text.text;
            }

            HandleSound();
        }

        void HandleSound()
        {
            if (_lastChange < 1)
                return;

            if (_lastChange + 0.1f > Time.unscaledTime)
            {
                _activeTime += Time.deltaTime;
                Play();
            }
            else if (Time.unscaledTime > _lastChange + 0.1f)
            {
                _activeTime = 0;
                Pause();
            }
        }

        void Play()
        {
            if(_audiosource == null)
                return;

            if (!_audiosource.isPlaying)
            {
                _audiosource.Play();
            }
        }

        void Pause()
        {
            if(_audiosource == null)
                return;
                
            if (_audiosource.isPlaying)
            {
                _audiosource.Pause();
            }
        }
    }
}