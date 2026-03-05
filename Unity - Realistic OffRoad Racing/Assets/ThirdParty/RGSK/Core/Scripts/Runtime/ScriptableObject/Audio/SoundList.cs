using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip clip;
    }

    [CreateAssetMenu(menuName = "RGSK/Audio/Sound List")]
    public class SoundList : ScriptableObject
    {
        public List<SoundData> sounds = new List<SoundData>();

        public AudioClip GetSound(string key)
        {
            var sound = sounds.FirstOrDefault(x => x.name == key);

            if (sound == null)
            {
                Logger.LogWarning(this, $"The sound '{key}' could not be found!");
                return null;
            }
            
            return sound.clip;
        }
    }
}