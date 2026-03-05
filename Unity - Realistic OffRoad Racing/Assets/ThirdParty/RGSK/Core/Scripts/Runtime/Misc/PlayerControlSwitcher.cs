using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class PlayerControlSwitcher : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void OnCameraTargetChanged(Transform t)
        {
            foreach (var entity in RGSKCore.Instance.GeneralSettings.entitySet.Items)
            {
                GeneralHelper.TogglePlayerInput(entity.gameObject, entity.gameObject == t.gameObject);
                GeneralHelper.ToggleAIInput(entity.gameObject, false);
                GeneralHelper.ToggleInputControl(entity.gameObject, entity.gameObject == t.gameObject);
            }
        }
    }
}