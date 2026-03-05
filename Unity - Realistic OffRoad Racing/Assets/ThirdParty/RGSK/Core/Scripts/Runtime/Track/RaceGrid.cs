using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class RaceGrid : MonoBehaviour
    {
        public RaceStartMode gridType;

        public bool autoPlacement;
        [Range(1, 50)] public int gridSize = 4;
        [Range(1, 10)] public int columnCount = 2;
        public float columnSpacing = 10;
        public float rowSpacing = 0;
        public float zOffset = 10;

        public List<Transform> GetPositions()
        {
            var children = GetComponentsInChildren<Transform>().ToList();
            children.RemoveAt(0);

            return children;
        }
    }
}