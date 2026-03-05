using UnityEngine;
using UnityEngine.Audio;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Audio")]
    public class RGSKAudioSettings : ScriptableObject
    {
        public AudioMixer mixer;
        public SoundList sounds;
        public float musicFadeDuration = 1;
    }
}