using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using RGSK.Extensions;

namespace RGSK
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Tab : MonoBehaviour
    {
        public GameObject startSelectable;

        [SerializeField] UnityEvent onOpen;
        [SerializeField] UnityEvent onClose;

        CanvasGroup _group;

        void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        public void Open()
        {
            _group.SetAlpha(1);

            if (startSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(startSelectable);
            }

            onOpen.Invoke();
        }

        public void Close()
        {
            _group.SetAlpha(0);
            onClose.Invoke();
        }
    }
}