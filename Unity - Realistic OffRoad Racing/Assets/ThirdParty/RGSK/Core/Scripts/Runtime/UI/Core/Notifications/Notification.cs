using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    [System.Serializable]
    public class NotificationProperties
    {
        public string message;
        public float duration;
        public Sprite sprite;
        public string sound;
        public Color? messageColor;
        public bool prioritize;
    }

    public class Notification : MonoBehaviour
    {
        [SerializeField] TMP_Text messageText;
        [SerializeField] Image icon;
        [SerializeField] string defaultSound;
        [SerializeField] float visibleDuration = 2;
        [SerializeField] float fadeInAndOutDuration = 0.3f;

        [Header("Animation")]
        [SerializeField] Animator animator;
        [SerializeField] string enterAnimTrigger = "Enter";
        [SerializeField] string exitAnimTrigger = "Exit";

        Color _defaultTextColor;

        void Awake()
        {
            if (TryGetComponent<CanvasGroup>(out var group))
            {
                group.SetAlpha(0);
            }

            if (messageText != null)
            {
                _defaultTextColor = messageText.color;
            }
        }

        public void Show(NotificationProperties properties)
        {
            if (messageText != null)
            {
                messageText.text = properties.message;
                messageText.color = properties.messageColor ?? _defaultTextColor;
            }

            if (icon != null)
            {
                icon.gameObject.SetActive(properties.sprite != null);
                icon.sprite = properties.sprite;
            }

            var sound = string.IsNullOrWhiteSpace(properties.sound) ? defaultSound : properties.sound;
            AudioManager.Instance?.Play(sound, AudioGroup.UI);

            gameObject.SetActive(true);
            StartCoroutine(ShowRoutine(properties.duration > 0 ? properties.duration : visibleDuration));
        }

        IEnumerator ShowRoutine(float duration)
        {
            if (animator != null)
            {
                animator.SetTrigger(enterAnimTrigger);
            }

            yield return GeneralHelper.GetCachedWaitForSeconds(fadeInAndOutDuration);

            yield return GeneralHelper.GetCachedWaitForSeconds(duration);

            if (animator != null)
            {
                animator.SetTrigger(exitAnimTrigger);
            }

            yield return GeneralHelper.GetCachedWaitForSeconds(fadeInAndOutDuration);
            gameObject.SetActive(false);
        }
    }
}