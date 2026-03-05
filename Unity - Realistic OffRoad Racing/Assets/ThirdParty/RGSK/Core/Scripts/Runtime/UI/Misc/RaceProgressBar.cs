using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceProgressBar : MonoBehaviour
    {
        [SerializeField] DirectionAxis axis;
        [SerializeField] RectTransform bar;
        [SerializeField] Image marker;
        [SerializeField] Vector2 markerOffset;
        [SerializeField] Color playerColor;
        [SerializeField] Color opponentColor;

        Dictionary<RGSKEntity, Image> _markers = new Dictionary<RGSKEntity, Image>();
        Vector2 _size = new Vector2();

        void OnEnable()
        {
            RGSKEvents.OnEntityAdded.AddListener(OnEntityAdded);
            RGSKEvents.OnEntityRemoved.AddListener(OnEntityRemoved);
        }

        void OnDisable()
        {
            RGSKEvents.OnEntityAdded.RemoveListener(OnEntityAdded);
            RGSKEvents.OnEntityRemoved.RemoveListener(OnEntityRemoved);
        }

        void Start()
        {
            if (bar != null)
            {
                _size = bar.rect.size;
            }

            if (RaceManager.Instance != null)
            {
                foreach (var e in RaceManager.Instance.Entities.Items)
                {
                    if (!_markers.ContainsKey(e))
                    {
                        OnEntityAdded(e);
                    }
                }
            }
        }

        void Update()
        {
            foreach (var entry in _markers)
            {
                if (entry.Key.Competitor == null)
                    continue;

                var percent = entry.Key.Competitor.RacePercentage / 100;
                var focused = GeneralHelper.GetFocusedEntity();

                if (focused != null)
                {
                    entry.Value.color = focused == entry.Key ? playerColor : opponentColor;
                    if (entry.Key == focused)
                    {
                        entry.Value.transform.SetAsLastSibling();
                    }
                }

                if (axis == DirectionAxis.Vertical)
                {
                    var progress = Mathf.Lerp(-_size.y * 0.5f, _size.y * 0.5f, percent);
                    entry.Value.transform.localPosition = new Vector2(0, progress) + markerOffset;
                }
                else
                {
                    var progress = Mathf.Lerp(-_size.x * 0.5f, _size.x * 0.5f, percent);
                    entry.Value.transform.localPosition = new Vector2(progress, 0) + markerOffset;
                }
            }
        }

        void OnEntityAdded(RGSKEntity e)
        {
            if (_markers.ContainsKey(e) || (e.IsVirtual))
                return;

            if (marker != null && bar != null)
            {
                var m = Instantiate(marker, bar.transform);
                _markers.Add(e, m);
            }
        }

        void OnEntityRemoved(RGSKEntity data)
        {
            if (_markers.TryGetValue(data, out var m))
            {
                Destroy(m.gameObject);
                _markers.Remove(data);
            }
        }
    }
}