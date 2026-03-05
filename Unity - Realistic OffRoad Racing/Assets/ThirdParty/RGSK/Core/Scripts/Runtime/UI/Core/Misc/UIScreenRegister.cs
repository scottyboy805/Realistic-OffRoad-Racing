using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class UIScreenRegister : MonoBehaviour
    {
        void Awake()
        {
            foreach (var screen in GetComponentsInChildren<UIScreen>(true))
            {
                UIManager.Instance?.RegisterScreen(screen);
            }
        }
    }
}