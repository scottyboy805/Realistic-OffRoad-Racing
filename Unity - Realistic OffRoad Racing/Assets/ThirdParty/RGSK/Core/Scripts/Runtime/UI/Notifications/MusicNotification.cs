using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class MusicNotification : MonoBehaviour
    {
        [SerializeField] NotificationDisplay display;

        void OnEnable()
        {
            RGSKEvents.OnMusicPlay.AddListener(OnMusicPlay);
        }

        void OnDisable()
        {
            RGSKEvents.OnMusicPlay.RemoveListener(OnMusicPlay);
        }

        void OnMusicPlay(SoundData track)
        {
            display?.Show(new NotificationProperties
            {
                message = track.name
            });
        }
    }
}