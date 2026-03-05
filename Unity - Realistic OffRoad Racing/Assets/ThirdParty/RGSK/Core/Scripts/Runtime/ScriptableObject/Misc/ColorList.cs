using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Misc/Color List")]
    public class ColorList : ScriptableObject
    {
        public List<Color> colors = new List<Color>();

        public Color GetRandom()
        {
            if (colors.Count > 0)
            {
                return colors.GetRandom();
            }

            return Color.white;
        }
    }
}