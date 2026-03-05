using UnityEngine;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public class LoadingScreen : UIScreen
    {
        [SerializeField] Gauge progressBar;
        [SerializeField] TMP_Text progressText;

        void Update()
        {
            progressBar?.SetValue(SceneLoadManager.LoadingProgress);
            progressText?.SetText(UIHelper.FormatPercentageText(SceneLoadManager.LoadingProgress * 100));
        }
    }
}