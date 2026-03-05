using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class MusicManager : Singleton<MusicManager>
    {
        float _fadeDuration => RGSKCore.Instance.AudioSettings.musicFadeDuration;
        List<SoundData> _playlist = new List<SoundData>();
        AudioSource _audioSource;
        int _currentTrackIndex;
        float _playbackTime;
        bool _isPlaying;
        bool _repeat;

        public override void Awake()
        {
            base.Awake();

            _audioSource = AudioHelper.CreateAudioSource(
                                                        null,
                                                        true,
                                                        false,
                                                        false,
                                                        1,
                                                        1,
                                                        AudioGroup.Music.ToString(),
                                                        transform,
                                                        "AudioSource_Music");

            _audioSource.bypassEffects = _audioSource.bypassReverbZones = true;
        }

        void OnEnable()
        {
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        void OnDisable()
        {
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);
        }

        void Update()
        {
            AutoPlayNext();
        }

        void AutoPlayNext()
        {
            if (_audioSource.loop)
                return;

            if (_isPlaying)
            {
                _playbackTime += Time.deltaTime;

                if (_playbackTime >= _audioSource.clip.length)
                {
                    _isPlaying = false;
                    PlayNext();
                }
            }
        }

        public void UpdatePlaylist(SoundList playlist, bool shuffle, bool play, bool repeat = true)
        {
            StopAllCoroutines();
            Stop();

            if (playlist != null)
            {
                _playlist.Clear();
                playlist.sounds.ForEach(x => _playlist.Add(x));
                _repeat = repeat;
                _currentTrackIndex = -1;
                _audioSource.loop = _playlist.Count == 1 && _repeat;

                if (shuffle)
                {
                    _playlist.Shuffle();
                }

                if (play)
                {
                    PlayNext();
                }
            }
        }

        public void Play(int index)
        {
            if (index < 0 || index >= _playlist.Count || _playlist[index] == null)
                return;

            StopAllCoroutines();
            _currentTrackIndex = index;
            StartCoroutine(PlayRoutuine());
        }

        public void PlayNext()
        {
            if (_playlist.Count == 0)
                return;

            var nextIndex = _currentTrackIndex + 1;

            if (nextIndex >= _playlist.Count)
            {
                if (!_repeat)
                {
                    Stop();
                    return;
                }
            }

            Play(nextIndex % _playlist.Count);
        }

        public void PlayRandom()
        {
            if (_playlist.Count == 0)
                return;

            if (_playlist.Count == 1)
            {
                Play(0);
            }

            var newIndex = _currentTrackIndex;

            while (newIndex == _currentTrackIndex)
            {
                newIndex = Random.Range(0, _playlist.Count);
            }

            Play(newIndex);
        }

        public void Stop(bool fadeOut = true)
        {
            if(fadeOut)
            {
                StopCoroutine(StopRoutine());
                StartCoroutine(StopRoutine());
            }
            else
            {   
                _audioSource.Stop();
                _isPlaying = false;
                _playbackTime = 0;
            }
        }

        public void Pause()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            _audioSource.Pause();
        }

        public void Resume()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;
            _audioSource.Play();
        }

        IEnumerator PlayRoutuine()
        {
            yield return StartCoroutine(StopRoutine());

            _audioSource.clip = _playlist[_currentTrackIndex].clip;
            _audioSource.volume = 1;
            _playbackTime = 0;
            _isPlaying = true;
            _audioSource.Play();
            RGSKEvents.OnMusicPlay.Invoke(_playlist[_currentTrackIndex]);
        }

        IEnumerator StopRoutine()
        {
            if (!_isPlaying)
                yield break;

            yield return StartCoroutine(AudioHelper.FadeAudio(_audioSource, _fadeDuration, 0));

            _audioSource.Stop();
            _isPlaying = false;
            _playbackTime = 0;
        }

        void OnReplayStart()
        {
            _audioSource.ignoreListenerPause = true;
        }

        void OnReplayStop()
        {
            _audioSource.ignoreListenerPause = false;
        }
    }
}