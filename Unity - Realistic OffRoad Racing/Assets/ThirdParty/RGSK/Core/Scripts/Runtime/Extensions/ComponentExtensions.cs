using System.Collections.Generic;
using UnityEngine;

namespace RGSK.Extensions
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent(out T component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static T[] GetComponentsInChildrenWithoutParent<T>(this GameObject gameObject) where T : Component
        {
            var result = new List<T>(gameObject.GetComponentsInChildren<T>());

            if (result.Count > 0)
            {
                if (result[0].gameObject == gameObject)
                {
                    result.RemoveAt(0);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Sets the enabled state only when required
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="active"></param>
        public static void SetActiveSafe(this Behaviour behaviour, bool active)
        {
            if (behaviour.enabled != active)
            {
                behaviour.enabled = active;
            }
        }
    }
}
