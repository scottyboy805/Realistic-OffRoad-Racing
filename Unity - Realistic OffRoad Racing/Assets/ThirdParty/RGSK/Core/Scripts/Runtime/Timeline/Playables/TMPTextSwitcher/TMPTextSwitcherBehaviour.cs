using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RGSK
{
    [Serializable]
    public class TMPTextSwitcherBehaviour : PlayableBehaviour
    {
        public Color color = Color.white;
        public float fontSize = 14;
        public string text;
    }
}