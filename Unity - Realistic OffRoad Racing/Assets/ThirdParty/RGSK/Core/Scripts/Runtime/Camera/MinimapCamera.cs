using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class MinimapCamera : MonoBehaviour
    {
        MinimapSettings _settings => RGSKCore.Instance.GeneralSettings.minimapSettings;
        Camera _camera;

        Transform _target;
        Rigidbody _targetRigid;

        void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (_target != null)
            {
                if (_settings.followPosition)
                {
                    var pos = _target.position;
                    pos.y += 100;
                    _camera.transform.position = pos;
                }

                if (_settings.followRotation)
                {
                    var rot = _camera.transform.eulerAngles;
                    rot.y = _target.eulerAngles.y;
                    _camera.transform.eulerAngles = rot;
                }
            }

            if (_settings.zoomWithSpeed)
            {
                var speed = _targetRigid != null ? _targetRigid.SpeedInKPH() : 0;
                _camera.orthographicSize = _settings.speedVsZoomCurve.Evaluate(speed);
            }
            else
            {
                _camera.orthographicSize = _settings.orthographicSize;
            }
        }

        void OnCameraTargetChanged(Transform target)
        {
            _target = target;
            _targetRigid = target.GetComponent<Rigidbody>();
        }
    }
}