using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace RGSK
{
    public class ModalWindowDisplayer : MonoBehaviour
    {
        [SerializeField] ModalWindowProperties properties;
        [SerializeField] UnityEvent confirmAction, declineAction;
        [SerializeField] InputMode inputModeOnClose = InputMode.Gameplay;

        IEnumerator Start()
        {
            yield return null;

            properties.confirmAction = () =>
            {
                confirmAction?.Invoke();
                InputManager.Instance?.SetInputMode(inputModeOnClose);
            };

            properties.declineAction = () =>
            {
                declineAction?.Invoke();
                InputManager.Instance?.SetInputMode(inputModeOnClose);
            };

            ModalWindowManager.Instance?.Show(properties);
        }
    }
}