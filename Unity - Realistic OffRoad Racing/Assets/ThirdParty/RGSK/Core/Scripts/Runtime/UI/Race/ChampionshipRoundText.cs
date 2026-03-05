using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class ChampionshipRoundText : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [TextArea][SerializeField] string message = "Round {0}/{1}";

        void Start() => UpdateText();

        public void UpdateText()
        {
            if (!ChampionshipManager.Instance.Initialized)
            {
                text?.SetText("");
                return;
            }

            text?.SetText(string.Format(message,
                    ChampionshipManager.Instance.CurrentRound,
                    ChampionshipManager.Instance.Championship.TotalRounds));
        }
    }
}
