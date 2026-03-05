using UnityEngine;
using System.Collections;

namespace RGSK
{
    [System.Serializable]
    public class VisualWheelAlignment
    {
        [Range(-0.25f, 0.25f)] public float center = 0;
        [Range(0.5f, 1.5f)] public float width = 1f;
        [Range(-30, 30)] public float camberAngle = 0;
        [Range(-10, 10)] public float toeAngle = 0;
    }

    public class WheelVisual : MonoBehaviour
    {
        public Transform wheelParent;
        public Transform wheelChild;

        float _lastCamberValue;
        float _lastToeValue;
        float _lastWidth;

        public void UpdatePosition(Vector3 position)
        {
            if (wheelParent == null)
                return;

            wheelParent.position = position;
        }

        public void UpdateSteerAngle(float value)
        {
            if (wheelParent == null)
                return;

            wheelParent.localRotation = Quaternion.Euler(0, value, 0);
        }

        public void UpdateRotation(float value)
        {
            if (value == 0 || wheelChild == null)
                return;

            wheelChild.Rotate(value * 6.6f * Time.deltaTime, 0, 0, Space.Self);
        }

        public void UpdateCamber(float value)
        {
            if (_lastCamberValue == value || wheelChild == null)
                return;

            _lastCamberValue = value;

            var rot = wheelChild.localEulerAngles;
            rot.x = 0;
            rot.y = _lastToeValue;
            rot.z = value;

            wheelChild.localEulerAngles = rot;
        }

        public void UpdateToe(float value)
        {
            if (_lastToeValue == value || wheelChild == null)
                return;

            _lastToeValue = value;

            var rot = wheelChild.localEulerAngles;
            rot.x = 0;
            rot.y = value;
            rot.z = _lastCamberValue;

            wheelChild.localEulerAngles = rot;
        }

        public void UpdateWidth(float value)
        {
            if (_lastWidth == value || wheelChild == null)
                return;

            _lastWidth = value;

            var scl = wheelChild.localScale;
            scl.x = value;

            wheelChild.localScale = scl;
        }

        public void ResetLocalPosition() => wheelChild.localPosition = Vector3.zero;
        public float GetWidth() => _lastWidth;
    }
}