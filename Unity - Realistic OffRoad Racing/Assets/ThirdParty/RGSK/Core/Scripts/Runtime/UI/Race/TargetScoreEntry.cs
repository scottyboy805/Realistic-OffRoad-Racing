using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RGSK
{
    public class TargetScoreEntry : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text valueText;
        [SerializeField] Graphic highlightGraphic;

        [SerializeField]
        EntryColor normalColor = new EntryColor
        {
            backgroundColor = new Color(0, 0, 0, 0.5f),
            textColor = Color.white,
        };

        [SerializeField]
        EntryColor highlightColor = new EntryColor
        {
            backgroundColor = new Color(0, 0, 0, 0.5f),
            textColor = Color.green,
        };

        TMP_Text[] _texts;

        public void Setup(TargetScoreIcon icon, string value)
        {
            _texts = GetComponentsInChildren<TMP_Text>();
            
            valueText?.SetText(value);
            nameText?.SetText("");

            if (this.icon != null)
            {
                this.icon.enabled = false;
            }

            if (icon != null)
            {
                nameText?.SetText(icon.name);

                if (this.icon != null)
                {
                    this.icon.sprite = icon.icon;
                    this.icon.color = icon.color;
                    this.icon.enabled = icon != null;
                }
            }

            Highlight(false);
        }

        public void Highlight(bool toggle)
        {
            if (toggle)
            {
                highlightColor.SetColor(highlightGraphic, _texts);
            }
            else
            {
                normalColor.SetColor(highlightGraphic, _texts);
            }
        }
    }
}