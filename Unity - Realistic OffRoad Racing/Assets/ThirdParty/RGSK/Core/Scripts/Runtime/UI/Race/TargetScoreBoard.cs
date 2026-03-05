using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;
using TMPro;

namespace RGSK
{
    [System.Serializable]
    public class TargetScoreIcon
    {
        public string name;
        public Sprite icon;
        public Color color = Color.white;

        public static TargetScoreIcon GetIcon(int index)
        {
            var icons = RGSKCore.Instance.UISettings.targetScoreIcons;

            if (index >= 0 && index < icons.Count)
            {
                return icons[index];
            }

            return null;
        }
    }

    public class TargetScoreBoard : MonoBehaviour
    {
        [SerializeField] TargetScoreEntry entryPrefab;
        [SerializeField] LayoutGroup entryLayoutGroup;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] bool highlight;

        List<TargetScoreEntry> _entries = new List<TargetScoreEntry>();

        void OnEnable()
        {
            RGSKEvents.OnRacePositionsChanged.AddListener(OnRacePositionChanged);
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);

        }

        void OnDisable()
        {
            RGSKEvents.OnRacePositionsChanged.RemoveListener(OnRacePositionChanged);
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
        }

        public void Initialize()
        {
            for (int i = 0; i < RaceManager.Instance.Session.targetScores.Count; i++)
            {
                CreateEntry(i);
            }

            OnRacePositionChanged();
        }

        void CreateEntry(int index)
        {
            if (entryPrefab == null || entryLayoutGroup == null)
                return;

            var entry = Instantiate(entryPrefab, entryLayoutGroup.transform);
            entry.Setup(TargetScoreIcon.GetIcon(index), UIHelper.FormatTargetScoreText(index));
            _entries.Add(entry);
        }

        public void Refresh()
        {
            var c = GeneralHelper.GetFocusedEntity().Competitor;

            if (c != null)
            {
                scoreText?.SetText(UIHelper.FormatFinishText(c.Entity));

                if (highlight)
                {
                    var index = c.Position - 1;

                    if (index > _entries.Count)
                        return;

                    for (int i = 0; i < _entries.Count; i++)
                    {
                        _entries[i].Highlight(i == index);
                    }
                }
            }
        }

        void OnRacePositionChanged() => Refresh();
        void OnCompetitorFinished(Competitor c) => Refresh();
    }
}