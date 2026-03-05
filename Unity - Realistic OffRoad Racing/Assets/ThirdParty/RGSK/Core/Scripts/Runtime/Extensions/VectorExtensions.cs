using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK.Extensions
{
    public static class VectorExtensions
    {
        public static float GetRandomValue(this Vector2 v)
        {
            return Random.Range(v.x, v.y);
        }

        public static float GetRandomValue(this Vector2Int v)
        {
            return Random.Range(v.x, v.y);
        }
    }
}