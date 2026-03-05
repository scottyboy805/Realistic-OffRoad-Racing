using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    public class XPGauge : MonoBehaviour
    {
        [SerializeField] Gauge gauge;
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text totalXpText;
        [SerializeField] TMP_Text xpToNextLevelText;

        public void SetValue(int xp)
        {
            var level = GeneralHelper.GetLevelFromXP(xp);
            var previousLevelXP = (int)RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.Evaluate(level);
            var nextLevelXP = (int)RGSKCore.Instance.GeneralSettings.playerXPCurve.curve.Evaluate(level + 1);
            var xpToNextLevel = nextLevelXP - xp;
            var range = nextLevelXP - previousLevelXP;
            var cap = xp - previousLevelXP;
            var value = (float)cap / (float)range;

            gauge?.SetValue(value % 1);
            levelText?.SetText(level.ToString());
            totalXpText?.SetText(UIHelper.FormatPointsText(xp));
            xpToNextLevelText?.SetText(xpToNextLevel > 0 ? UIHelper.FormatPointsText(xpToNextLevel) : "-");
        }
    }
}