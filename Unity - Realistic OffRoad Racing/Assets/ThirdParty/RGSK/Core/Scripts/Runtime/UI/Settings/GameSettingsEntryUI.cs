using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;

namespace RGSK
{
    public abstract class GameSettingsEntryUI : MonoBehaviour
    {
        [SerializeField] protected TMP_Text titleText;
        [SerializeField] protected TMP_Text valueText;
        [SerializeField] protected Button next;
        [SerializeField] protected Button previous;
        [SerializeField] protected string title;
        [SerializeField] protected bool loop = true;

        protected string[] toggleOptions = new string[]
        {
            "On",
            "Off"
        };

        protected virtual void Start()
        {
            if (next != null)
            {
                next.onClick.AddListener(() => SelectOption(1));
            }

            if (previous != null)
            {
                previous.onClick.AddListener(() => SelectOption(-1));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(title);
            }

            SelectOption(0);
        }

        public virtual void SelectOption(int direction) { }
    }
}