using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class UIOpenScreenButton : MonoBehaviour
    {
        [SerializeField] UIScreenID screen;

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => UIManager.Instance?.OpenScreen(screen));
            }
        }
    }
}