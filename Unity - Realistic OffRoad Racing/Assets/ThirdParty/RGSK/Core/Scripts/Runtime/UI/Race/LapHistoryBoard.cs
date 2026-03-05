using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RGSK.Helpers;

namespace RGSK
{
    public class LapHistoryBoard : MonoBehaviour
    {
        [SerializeField] LapHistoryBoardEntry entryPrefab;
        [SerializeField] ScrollRect scrollView;

        List<LapHistoryBoardEntry> _entries = new List<LapHistoryBoardEntry>();

        void OnEnable()
        {
            RGSKEvents.OnLapCompleted.AddListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.AddListener(OnRaceRestart);
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnLapCompleted.RemoveListener(OnLapCompleted);
            RGSKEvents.OnRaceRestart.RemoveListener(OnRaceRestart);
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void OnLapCompleted(Competitor c)
        {
            if (c.Entity != GeneralHelper.GetFocusedEntity())
                return;

            CreateEntry(c, c.LapTimes.Count - 1);
        }

        void OnRaceRestart()
        {
            DeleteEntries();
        }

        void OnCameraTargetChanged(Transform target)
        {
            DeleteEntries();

            var c = GeneralHelper.GetFocusedEntity()?.Competitor;
            if (c != null)
            {
                for (int i = 0; i < c.LapTimes.Count; i++)
                {
                    CreateEntry(c, i);
                }
            }
        }

        void CreateEntry(Competitor c, int lapIndex)
        {
            var e = Instantiate(entryPrefab, scrollView.content);
            e.Setup(lapIndex + 1, c.LapTimes[lapIndex]);
            _entries.Add(e);
        }

        void DeleteEntries()
        {
            foreach (var e in _entries)
            {
                Destroy(e.gameObject);
            }

            _entries.Clear();
        }
    }
}