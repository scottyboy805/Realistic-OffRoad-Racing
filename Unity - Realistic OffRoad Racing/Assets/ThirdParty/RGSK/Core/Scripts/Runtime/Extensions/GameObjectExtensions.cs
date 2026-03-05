using UnityEngine;

namespace RGSK.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets all the game object's colliders to the given layer name.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetColliderLayer(this GameObject go, string layer, bool ignoreTriggers = true)
        {
            var colliders = go.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
                return;

            foreach (var col in colliders)
            {
                if (col.isTrigger && ignoreTriggers)
                    continue;

                col.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }

        /// <summary>
        /// Sets all the game object's colliders to the given layer name.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetColliderLayer(this GameObject go, int layer, bool ignoreTriggers = true)
        {
            var colliders = go.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
                return;

            foreach (var col in colliders)
            {
                if (col.isTrigger && ignoreTriggers)
                    continue;

                col.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Sets all the game object's children (ignoring colliders) to the given layer name.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetChildLayers(this GameObject go, int layer, bool ignoreColliders = true)
        {
            var children = go.GetComponentsInChildren<Transform>();

            foreach (var c in children)
            {
                if (ignoreColliders && c.GetComponent<Collider>())
                    continue;

                c.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Sets all the materials of the game object's renderers to the given material.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="material"></param>
        public static void SetMaterials(this GameObject gameObject, Material material)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return;

            foreach (var r in renderers)
            {
                var rendererMaterials = r.materials;

                for (int i = 0; i < rendererMaterials.Length; i++)
                {
                    r.material = material;
                }
            }
        }

        /// <summary>
        /// Sets the active state only when required
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="active"></param>
        public static void SetActiveSafe(this GameObject gameObject, bool active)
        {
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }

        public static void DestroyAllChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static bool IsUpsideDown(this Transform transform)
        {
            return transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280;
        }

        public static bool IsFacingTowards(this Transform transform, Transform other, float threshold = 0.7f)
        {
            var dot = Vector3.Dot(transform.forward, (other.position - transform.position).normalized);
            return dot > threshold;
        }

        public static bool IsAhead(this Transform transform, Transform other)
        {
            var dot = Vector3.Dot((transform.position - other.position), transform.forward);
            return dot >= 1;
        }

        public static bool IsBehind(this Transform transform, Transform other)
        {
            var dot = Vector3.Dot((transform.position - other.position), transform.forward);
            return dot <= 0;
        }
    }
}