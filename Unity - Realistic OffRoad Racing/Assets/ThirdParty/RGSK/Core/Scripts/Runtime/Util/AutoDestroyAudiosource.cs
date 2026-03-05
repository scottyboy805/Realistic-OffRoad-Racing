using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class AutoDestroyAudiosource : MonoBehaviour
    {
        AudioSource src;

        void Awake()
        {
            src = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (src == null)
                return;

            if (src.timeSamples == src.clip.samples || src.isPlaying == false)
            {
                Destroy(gameObject);
            }
        }
    }
}