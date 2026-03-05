using UnityEngine;

namespace RGSK
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] SoundList playlist;
        [SerializeField] bool shuffle;
        [SerializeField] bool playOnAwake;
        [SerializeField] bool repeat;

        void Start() => MusicManager.Instance?.UpdatePlaylist(playlist, shuffle, playOnAwake, repeat);
    }
}