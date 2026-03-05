using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class InputModeSetter : MonoBehaviour
    {
        [SerializeField] InputMode inputMode = InputMode.Gameplay;

        void Start()
        {
            InputManager.Instance?.SetInputMode(inputMode);
        }
    }
}