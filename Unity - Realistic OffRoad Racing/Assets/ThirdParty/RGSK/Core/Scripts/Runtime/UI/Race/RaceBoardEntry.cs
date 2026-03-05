using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Helpers;
using RGSK.Extensions;

namespace RGSK
{
    [System.Serializable]
    public class EntryColor
    {
        public Color backgroundColor;
        public Color textColor;

        public void SetColor(Graphic graphic, TMP_Text[] texts)
        {
            if (graphic != null)
            {
                graphic.color = backgroundColor;
            }

            foreach (var text in texts)
            {
                text.color = textColor;
            }
        }
    }

    public class RaceBoardEntry : EntityUIController
    {
        [SerializeField]
        EntryColor playerColor = new EntryColor
        {
            backgroundColor = new Color(1, 1, 1, 0.5f),
            textColor = Color.black,
        };

        [SerializeField]
        EntryColor opponentColor = new EntryColor
        {
            backgroundColor = new Color(0, 0, 0, 0.5f),
            textColor = Color.white,
        };

        [SerializeField] Graphic backgroundImage;
        TMP_Text[] _texts;
        LayoutElement _layoutElement;
        CanvasGroup _canvasGroup;

        public override void BindElements(RGSKEntity e)
        {
            base.BindElements(e);
            _texts = GetComponentsInChildren<TMP_Text>();
            _layoutElement = GetComponent<LayoutElement>();
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        public void UpdateEntry()
        {
            RefreshUI();
            UpdateEntryColor();
        }

        void UpdateEntryColor()
        {
            if (entity == GeneralHelper.GetFocusedEntity())
            {
                playerColor.SetColor(backgroundImage, _texts);
            }
            else
            {
                opponentColor.SetColor(backgroundImage, _texts);
            }
        }

        public void ToggleVisible(bool visible)
        {
            if (_layoutElement == null)
            {
                gameObject.SetActiveSafe(visible);
                return;
            }

            _layoutElement.ignoreLayout = !visible;
            _canvasGroup.SetAlpha(visible ? 1 : 0);
        }
    }
}