using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RGSK.Extensions;
using UnityEngine.Serialization;

namespace RGSK
{
    public class RaceEventScreen : UIScreen
    {
        [SerializeField] RaceEventEntry entryPrefab;
        [SerializeField] ScrollRect scrollView;

        [FormerlySerializedAs("eventDefinitions")]
        [SerializeField] List<RaceSession> raceSessions = new List<RaceSession>();
        [SerializeField] List<Championship> championships = new List<Championship>();

        List<RaceEventEntry> _entries = new List<RaceEventEntry>();
        int _startSelectionIndex = 0;

        public override void Initialize()
        {
            if (entryPrefab != null && scrollView != null)
            {
                foreach (var e in raceSessions)
                {
                    if (e != null)
                    {
                        var entry = Instantiate(entryPrefab, scrollView.content);
                        entry.Setup(e);
                        _entries.Add(entry);
                    }
                }

                foreach (var c in championships)
                {
                    if (c != null)
                    {
                        var entry = Instantiate(entryPrefab, scrollView.content);
                        entry.Setup(c);
                        _entries.Add(entry);
                    }
                }
            }

            base.Initialize();
        }

        public override void Open()
        {
            base.Open();

            foreach (var e in _entries)
            {
                e.Refresh();
            }

            StartCoroutine(scrollView?.SelectChild(_startSelectionIndex));
        }

        public override void Close()
        {
            var lastSelected = EventSystem.current.currentSelectedGameObject;

            if (lastSelected != null)
            {
                if (lastSelected.TryGetComponent<RaceEventEntry>(out var entry))
                {
                    _startSelectionIndex = entry.transform.GetSiblingIndex();
                }
            }

            base.Close();
        }
    }
}