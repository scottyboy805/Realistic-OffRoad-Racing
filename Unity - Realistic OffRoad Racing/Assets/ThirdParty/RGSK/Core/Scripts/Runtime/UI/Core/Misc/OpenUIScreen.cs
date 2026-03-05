using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class OpenUIScreen : MonoBehaviour
    {
        [SerializeField] UIScreenID screen;

        void Start()
        {
            UIManager.Instance?.OpenScreen(screen);
        }
    }
}