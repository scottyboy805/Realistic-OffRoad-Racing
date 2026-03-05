using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class UIScreenFader : Singleton<UIScreenFader>
    {
        [SerializeField] Image fadeImage;
        [SerializeField] Color startColor = new Color(0, 0, 0, 1);
        [SerializeField] Color endColor = new Color(0, 0, 0, 0);
        [SerializeField] bool ignoreTimeScale = true;

        bool _isFading;

        public void FadeIn(float duration = 2)
        {
            if (_isFading)
                return;

            StartCoroutine(FadeRoutine(endColor, startColor, duration));
        }

        public void FadeOut(float duration = 2)
        {
            if (_isFading)
                return;

            StartCoroutine(FadeRoutine(startColor, endColor, duration));
        }

        IEnumerator FadeRoutine(Color start, Color end, float duration)
        {
            if (fadeImage == null)
            {
                Logger.LogWarning("Cannot perform fade because the fade image has not been assigned!");
                yield break;
            }

            _isFading = true;

            var timer = 0f;
            while (timer < duration)
            {
                timer += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                fadeImage.color = Color.Lerp(start, end, timer / duration);
                yield return null;
            }

            _isFading = false;
        }
    }
}