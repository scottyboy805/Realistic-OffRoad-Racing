using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    [System.Serializable]
    public class LiveryData
    {
        public Texture2D texture;
        public Sprite preview;
        public Color previewColor;
    }

    [CreateAssetMenu(menuName = "RGSK/Misc/Livery List")]
    public class LiveryList : ScriptableObject
    {
        public List<LiveryData> liveries = new List<LiveryData>();

        public LiveryData GetRandom()
        {
            if (liveries.Count > 0)
            {
                return liveries.GetRandom();
            }

            return null;
        }

        public int GetIndexOf(Texture2D tex)
        {
            for (int i = 0; i < liveries.Count; i++)
            {
                if (liveries[i].texture == tex)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}