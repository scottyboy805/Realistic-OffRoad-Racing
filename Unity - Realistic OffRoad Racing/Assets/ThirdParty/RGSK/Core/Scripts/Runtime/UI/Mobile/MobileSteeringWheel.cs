using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.Layouts;
using RGSK.Extensions;

namespace RGSK
{
    public class MobileSteeringWheel : OnScreenControl
    {
        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        [SerializeField] RectTransform rectTransform;
        [SerializeField] float maxAngle = 90;
        [SerializeField] float recenterSpeed = 200f;

        Vector2 _centerPoint;
        float _wheelAngle = 0f;
        float _wheelPrevAngle = 0f;
        bool _wheelBeingHeld = false;

        void Start()
        {
            if (rectTransform == null)
                return;

            var events = rectTransform.gameObject.GetComponent<EventTrigger>();

            if (events == null)
                events = rectTransform.gameObject.AddComponent<EventTrigger>();

            if (events.triggers == null)
                events.triggers = new List<EventTrigger.Entry>();

            var entry = new EventTrigger.Entry();
            var callback = new EventTrigger.TriggerEvent();
            var functionCall = new UnityAction<BaseEventData>(PressEvent);
            
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = new UnityAction<BaseEventData>(DragEvent);
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.Drag;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = new UnityAction<BaseEventData>(ReleaseEvent);
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback = callback;

            events.triggers.Add(entry);

            rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
        }

        void Update()
        {
            if (rectTransform == null)
                return;

            if (!_wheelBeingHeld && !Mathf.Approximately(0f, _wheelAngle))
            {
                float deltaAngle = recenterSpeed * Time.deltaTime;

                if (Mathf.Abs(deltaAngle) > Mathf.Abs(_wheelAngle))
                {
                    _wheelAngle = 0f;
                }
                else if (_wheelAngle > 0f)
                {
                    _wheelAngle -= deltaAngle;
                }
                else
                {
                    _wheelAngle += deltaAngle;
                }
            }

            rectTransform.localEulerAngles = Vector3.back * _wheelAngle;
            SendValueToControl(new Vector2(GetClampedValue(), 0));
        }

        public void PressEvent(BaseEventData eventData)
        {
            var pointerPos = ((PointerEventData)eventData).position;

            _wheelBeingHeld = true;
            _centerPoint = RectTransformUtility.WorldToScreenPoint(((PointerEventData)eventData).pressEventCamera, rectTransform.position);
            _wheelPrevAngle = Vector2.Angle(Vector2.up, pointerPos - _centerPoint);
        }

        public void DragEvent(BaseEventData eventData)
        {
            var pointerPos = ((PointerEventData)eventData).position;
            var wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - _centerPoint);

            if (Vector2.Distance(pointerPos, _centerPoint) > 20f)
            {
                if (pointerPos.x > _centerPoint.x)
                    _wheelAngle += wheelNewAngle - _wheelPrevAngle;
                else
                    _wheelAngle -= wheelNewAngle - _wheelPrevAngle;
            }
            _wheelAngle = Mathf.Clamp(_wheelAngle, -maxAngle, maxAngle);
            _wheelPrevAngle = wheelNewAngle;
        }

        public void ReleaseEvent(BaseEventData eventData)
        {
            DragEvent(eventData);
            _wheelBeingHeld = false;
        }

        public float GetClampedValue() => _wheelAngle / maxAngle;
    }
}