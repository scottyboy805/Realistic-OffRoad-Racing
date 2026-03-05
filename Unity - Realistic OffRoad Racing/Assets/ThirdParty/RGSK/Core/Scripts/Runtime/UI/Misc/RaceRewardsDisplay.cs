using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceRewardsDisplay : MonoBehaviour
    {
        [SerializeField] RaceRewardEntryUI entryPrefab;
        [SerializeField] LayoutGroup layoutGroup;

        public void Populate(List<RaceReward> rewards)
        {
            if (entryPrefab == null || layoutGroup == null)
                return;

            layoutGroup.gameObject.DestroyAllChildren();

            for (int i = 0; i < rewards.Count; i++)
            {
                var entry = Instantiate(entryPrefab, layoutGroup.transform);
                entry.UpdateUI(i + 1, rewards[i]);
            }
        }
    }
}