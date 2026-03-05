using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace RGSK.Helpers
{
    public static class AudioHelper
    {
        public const string MasterVolumeParam = "master_volume";
        public const string MusicVolumeParam = "music_volume";
        public const string SFXMasterVolumeParam = "sfx_volume";
        public const string UiVolumeParam = "ui_volume";
        public const string VehicleVolumeParam = "vehicle_sfx_volume";
        public const string VehicleLowPassParam = "vehicle_sfx_lowpass_frequency";

        public static AudioSource CreateAudioSource(AudioClip clip,
                                        bool is2D,
                                        bool loop,
                                        bool playOnAwake,
                                        float volume = 1,
                                        float pitch = 1,
                                        string audioGroupName = "",
                                        Transform parent = null,
                                        string name = "")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "runtime_audiosource";
            }

            var audioSource = new GameObject(name).AddComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.spatialBlend = is2D ? 0 : 1;
            audioSource.loop = loop;
            audioSource.dopplerLevel = 0;

            if (parent != null)
            {
                audioSource.transform.SetParent(parent, false);
            }

            if (!string.IsNullOrWhiteSpace(audioGroupName))
            {
                audioSource.outputAudioMixerGroup = GetAudioMixerGroup(audioGroupName);
                audioSource.ignoreListenerPause = audioGroupName == AudioGroup.UI.ToString();
            }

            if (playOnAwake)
            {
                audioSource.Play();
            }

            return audioSource;
        }

        public static IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
        {
            var time = 0f;
            var start = audioSource.volume;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, time / duration);
                yield return null;
            }

            yield break;
        }

        public static void SetAudioMixerVolume(AudioMixer mixer, AudioGroup group)
        {
            var param = AudioHelper.GetMixerParameterName(group);
            var value = ConversionHelper.LinearToDecibel(SaveData.Instance.gameSettingsData.volumes[group]);
            mixer?.SetFloat(param, value);
        }

        public static AudioMixerGroup GetAudioMixerGroup(string name)
        {
            var mixer = AudioManager.Instance.Mixer;

            if (mixer != null)
            {
                var groups = mixer.FindMatchingGroups(name);

                foreach (var group in groups)
                {
                    if (group.name == name)
                    {
                        return group;
                    }
                }

                Logger.LogWarning($"The group: '{name}' does not exist in the Audio Mixer!");
            }

            return null;
        }

        public static string GetMixerParameterName(AudioGroup group)
        {
            switch (group)
            {
                case AudioGroup.Master:
                default:
                    {
                        return MasterVolumeParam;
                    }

                case AudioGroup.Music:
                    {
                        return MusicVolumeParam;
                    }

                case AudioGroup.SFX:
                    {
                        return SFXMasterVolumeParam;
                    }

                case AudioGroup.Vehicle:
                    {
                        return VehicleVolumeParam;
                    }

                case AudioGroup.UI:
                    {
                        return UiVolumeParam;
                    }
            }
        }
    }
}