using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    public class RaceRewardEntryUI : MonoBehaviour
    {
        [SerializeField] TMP_Text posText;
        [SerializeField] TMP_Text currencyText;
        [SerializeField] TMP_Text xpText;
        [SerializeField] Image trophy;

        public void UpdateUI(int pos, RaceReward reward)
        {
            posText?.SetText(UIHelper.FormatOrdinalText(pos));
            currencyText?.SetText(UIHelper.FormatCurrencyText(reward.currency, true));
            xpText?.SetText(UIHelper.FormatPointsText(reward.xp));

            if (trophy != null)
            {
                var icon = TargetScoreIcon.GetIcon(pos - 1);

                if (icon != null)
                {
                    trophy.sprite = icon.icon;
                    trophy.color = icon.color;
                }

                trophy.DisableIfNullSprite();
            }
        }
    }
}