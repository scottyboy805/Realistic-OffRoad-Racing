using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Race/Track Definition")]
    public class TrackDefinition : ItemDefinition
    {
        public SceneReference scene = new SceneReference();
        public CountryDefinition country;
        public Sprite minimapPreview;
        public TrackLayoutType layoutType;
        public float length;
        public int gridSlots;
        public bool allowRollingStarts;
        [Tooltip("A list of the selectable race types when this track is selected. Leave empty to allow for all race types.")]
        public List<RaceType> allowedRaceTypes = new List<RaceType>();

        public float LoadBestLap()
        {
            if (SaveData.Instance.bestLaps.TryGetValue(ID, out var value))
            {
                return value;
            }

            return 0;
        }

        public void SaveBestLap(float value)
        {
            SaveData.Instance.bestLaps[ID] = value;
        }
    }
}