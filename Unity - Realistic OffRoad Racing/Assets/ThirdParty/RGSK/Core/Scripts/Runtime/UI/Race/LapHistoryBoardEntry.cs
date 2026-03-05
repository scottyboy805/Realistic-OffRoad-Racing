using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class LapHistoryBoardEntry : MonoBehaviour
    {
        [SerializeField] TMP_Text lapText;
        [SerializeField] TMP_Text timeText;

        public void Setup(int lap, float time)
        {
            lapText?.SetText(UIHelper.FormatLapText(lap, NumberDisplayMode.Single));
            timeText?.SetText(UIHelper.FormatTimeText(time));
        }
    }
}