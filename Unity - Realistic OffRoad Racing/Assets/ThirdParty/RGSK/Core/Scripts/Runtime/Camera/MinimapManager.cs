using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    public class MinimapManager : Singleton<MinimapManager>
    {
        MinimapSettings _settings => RGSKCore.Instance.GeneralSettings.minimapSettings;
        Dictionary<Transform, TransformFollower> _blips = new Dictionary<Transform, TransformFollower>();

        void OnEnable()
        {
            RGSKEvents.OnEntityRemoved.AddListener(OnEntityRemoved);
        }

        void OnDisable()
        {
            RGSKEvents.OnEntityRemoved.RemoveListener(OnEntityRemoved);
        }

        public void RemoveBlip(Transform target)
        {
            if (_blips.TryGetValue(target, out var blip))
            {
                _blips.Remove(target);
                if (blip != null)
                {
                    Destroy(blip.gameObject);
                }
            }
        }

        void OnEntityRemoved(RGSKEntity entity)
        {
            RemoveBlip(entity.transform);
        }

        public GameObject CreateBlip(string name, Transform target)
        {
            if (_settings == null)
                return null;

            var blip = _settings.GetBlip(name);

            if (blip != null)
            {
                return CreateBlip(target, blip.sprite, blip.color, blip.scale, blip.sortOrder);
            }

            return null;
        }

        GameObject CreateBlip(Transform target, Sprite iconSprite, Color color, int scale, int order)
        {
            if (_blips.ContainsKey(target) || _settings.blipPrefab == null)
            {
                return null;
            }

            var blip = Instantiate(_settings.blipPrefab, GeneralHelper.GetDynamicParent());
            var renderer = blip.GetComponentInChildren<SpriteRenderer>();

            if (renderer != null)
            {
                renderer.sprite = iconSprite;
                renderer.color = color;
                renderer.transform.localScale = Vector2.one * scale;
                renderer.sortingOrder = order;
                renderer.gameObject.layer = RGSKCore.Instance.GeneralSettings.minimapLayerIndex.Index;
            }

            blip.target = target;
            _blips[target] = blip;
            blip.gameObject.layer = RGSKCore.Instance.GeneralSettings.minimapLayerIndex.Index;

            return blip.gameObject;
        }
    }
}