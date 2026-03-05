using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Extensions;

namespace RGSK
{
    public class UIBackButton : MonoBehaviour
    {
        [SerializeField] Button button;

        void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(() => UIManager.Instance?.OpenPreviousScreen());
            }
        }

        void Update()
        {
            if(button != null)
            {
                button.gameObject.SetActiveSafe(UIManager.Instance.HasScreenHistory());
            }
        }
    }
}