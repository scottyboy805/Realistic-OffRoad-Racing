using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class NotificationDisplay : MonoBehaviour
    {
        [SerializeField] Notification prefab;
        [SerializeField] RectTransform parent;
        [SerializeField] int max = 1;

        Queue<NotificationProperties> _messageQueue = new Queue<NotificationProperties>();
        List<Notification> _instances = new List<Notification>();

        void Start()
        {
            if (prefab != null && parent != null)
            {
                for (int i = 0; i < max; i++)
                {
                    var n = Instantiate(prefab, parent);
                    n.gameObject.SetActive(false);
                    _instances.Add(n);
                }
            }
        }

        void LateUpdate()
        {
            if (_messageQueue.Count == 0 || _instances.Count == 0)
                return;

            for (int i = 0; i < _instances.Count; i++)
            {
                if (_instances[i].gameObject.activeSelf)
                    continue;

                var props = _messageQueue.Dequeue();
                _instances[i].Show(props);

                break;
            }
        }

        public void Show(NotificationProperties props)
        {
            if (string.IsNullOrWhiteSpace(props.message))
                return;

            if (props.prioritize)
            {
                Clear();
            }

            _messageQueue.Enqueue(props);
        }

        public void Clear()
        {
            if (_instances.Count == 1)
            {
                _messageQueue.Clear();
                _instances[0].gameObject.SetActive(false);
            }
        }
    }
}