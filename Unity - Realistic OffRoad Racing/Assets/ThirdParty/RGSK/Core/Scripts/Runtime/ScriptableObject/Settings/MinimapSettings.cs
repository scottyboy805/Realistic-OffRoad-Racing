using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class MinimapBlip
    {
        public string name;
        public Sprite sprite;
        public Color color = Color.white;
        public int scale = 15;
        public int sortOrder;
    }

    [CreateAssetMenu(menuName = "RGSK/Settings/Minimap Settings")]
    public class MinimapSettings : ScriptableObject
    {
        public TransformFollower blipPrefab;
        public bool followPosition = true;
        public bool followRotation = true;
        public bool zoomWithSpeed = true;
        public float orthographicSize = 250;
        public AnimationCurve speedVsZoomCurve = new AnimationCurve(
            new Keyframe(0, 250),
            new Keyframe(50, 250),
            new Keyframe(100, 300));

        [SerializeField] List<MinimapBlip> blips = new List<MinimapBlip>();

        public MinimapBlip GetBlip(string name)
        {
            var blip = blips.FirstOrDefault(x => x.name.ToLower() == name.ToLower());

            if (blip == null)
            {
                Logger.LogWarning(this, $"The blip \"{name}\" was not found! Please ensure that it exists in the blip list.");
                return null;
            }

            return blip;
        }
    }
}