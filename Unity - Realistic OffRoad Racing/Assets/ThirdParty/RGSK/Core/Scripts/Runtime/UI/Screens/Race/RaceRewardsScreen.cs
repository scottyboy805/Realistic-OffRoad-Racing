using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RGSK.Helpers;
using RGSK.Extensions;
using TMPro;

namespace RGSK
{
    public class RaceRewardsScreen : UIScreen
    {
        [SerializeField] Button continueButton;

        [Header("Currency & XP")]
        [SerializeField] GameObject currencyRewardsPanel;
        [SerializeField] TMP_Text currencyText;
        [SerializeField] TMP_Text xpText;
        [SerializeField] XPGauge xpGauge;

        [Header("Items")]
        [SerializeField] GameObject itemsRewardsPanel;
        [SerializeField] SelectionItemEntry itemEntryPrefab;
        [SerializeField] ScrollRect itemScrollView;

        List<ItemDefinition> _items = new List<ItemDefinition>();

        public override void Initialize()
        {
            base.Initialize();
            continueButton?.onClick.AddListener(Continue);
        }

        public override void Open()
        {
            base.Open();

            if (currencyRewardsPanel != null && itemsRewardsPanel != null)
            {
                currencyRewardsPanel.gameObject.SetActive(true);
                itemsRewardsPanel.gameObject.SetActive(false);
            }

            if (RaceRewardManager.Instance.ActiveReward != null)
            {
                var maxXP = RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.GetMaxValue();
                var currentXP = SaveData.Instance.playerData.xp;
                var newXP = currentXP + RaceRewardManager.Instance.ActiveReward.xp;
                _items = RaceRewardManager.Instance.ActiveReward.items.Where(x => !x.IsUnlocked()).ToList();

                newXP = Mathf.Clamp(newXP, newXP, (int)maxXP);

                currencyText?.SetText($"+ {UIHelper.FormatCurrencyText(RaceRewardManager.Instance.ActiveReward.currency)}");
                xpText?.SetText($"+ {UIHelper.FormatPointsText(RaceRewardManager.Instance.ActiveReward.xp)}");

                PopulateItems();

                StopAllCoroutines();
                StartCoroutine(XPGaugeFillRoutine(currentXP, newXP));
            }
        }

        void PopulateItems()
        {
            if (_items.Count == 0 || itemScrollView == null || itemEntryPrefab == null)
                return;

            itemScrollView.content.gameObject.DestroyAllChildren();

            foreach (var item in _items)
            {
                var entry = Instantiate(itemEntryPrefab, itemScrollView.content);

                entry.Setup
                (
                    item: item,
                    col: Color.white,
                    onSelect: () => { },
                    onClick: () => { }
                );
            }
        }

        IEnumerator XPGaugeFillRoutine(int oldXP, int newXP)
        {
            var timer = 0f;
            var xp = 0f;

            while (timer < 5)
            {
                timer += Time.deltaTime;
                xp = Mathf.Lerp(oldXP, newXP, timer / 5);
                xpGauge?.SetValue((int)xp);
                yield return null;
            }
        }

        void Continue()
        {
            if (currencyRewardsPanel != null && itemsRewardsPanel != null)
            {
                if (_items.Count > 0 && !itemsRewardsPanel.gameObject.activeSelf)
                {
                    currencyRewardsPanel.gameObject.SetActive(false);
                    itemsRewardsPanel.gameObject.SetActive(true);
                    return;
                }
            }

            SceneLoadManager.LoadMainScene();
        }
    }
}