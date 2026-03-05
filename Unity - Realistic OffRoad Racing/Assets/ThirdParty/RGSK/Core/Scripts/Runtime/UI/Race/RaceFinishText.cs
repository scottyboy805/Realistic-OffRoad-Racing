using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class RaceFinishText : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [TextArea][SerializeField] string finishMessage = "Finished {0}\n{1}";
        [TextArea][SerializeField] string disqualifyMessage = "Disqualified";

        void OnEnable()
        {
            RGSKEvents.OnCompetitorFinished.AddListener(OnCompetitorFinished);
        }

        void OnDisable()
        {
            RGSKEvents.OnCompetitorFinished.RemoveListener(OnCompetitorFinished);
        }

        void OnCompetitorFinished(Competitor c)
        {
            if (!c.Entity.IsPlayer)
                return;

            if (c.IsDisqualified)
            {
                text?.SetText(disqualifyMessage);
            }
            else
            {
                var pos = !RaceManager.Instance.Session.IsTargetScoreSession() ?
                          UIHelper.FormatPositionText(c.FinalPosition, NumberDisplayMode.Ordinal) : "";

                text?.SetText(string.Format(finishMessage, pos, UIHelper.FormatFinishText(c.Entity)));
            }
        }
    }
}