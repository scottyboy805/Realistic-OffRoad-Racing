using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class ResetInputBindingsButton : MonoBehaviour
    {
        [SerializeField] BindingGroup group;
        [SerializeField] bool resetAll;

        void Start()
        {
            if (TryGetComponent<Button>(out var button))
            {
                button.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                {
                    header = RGSKCore.Instance.UISettings.resetInputBindingsModal.header,
                    message = RGSKCore.Instance.UISettings.resetInputBindingsModal.message,
                    confirmButtonText = RGSKCore.Instance.UISettings.resetInputBindingsModal.confirmButtonText,
                    declineButtonText = RGSKCore.Instance.UISettings.resetInputBindingsModal.declineButtonText,
                    confirmAction = () =>
                    {
                        if (resetAll)
                        {
                            InputManager.Instance?.ResetBindings();
                        }
                        else
                        {
                            InputManager.Instance?.ResetBindings(group);
                        }
                    },
                    declineAction = () => { },
                    startSelection = RGSKCore.Instance.UISettings.resetInputBindingsModal.startSelection,
                    prefabIndex = RGSKCore.Instance.UISettings.resetInputBindingsModal.prefabIndex
                }));
            }
        }
    }
}