using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class Slipstream : MonoBehaviour
    {
        SlipstreamSettings _settings => RGSKCore.Instance.GeneralSettings.slipstreamSettings;

        Rigidbody _rigid;
        IVehicle _vehicle;
        Vector3 _size;
        bool _active;

        void Start()
        {
            if (_settings == null)
                return;

            _rigid = GetComponent<Rigidbody>();
            _vehicle = GetComponent<IVehicle>();

            var box = GetComponentInChildren<BoundingBox>();
            if (box != null)
            {
                _size = box.Size;
            }
        }

        void FixedUpdate()
        {
            if (_settings == null || !_settings.enabled || _rigid == null || _vehicle == null)
                return;

            var leaderSpeed = 0f;

            if (Physics.BoxCast(
                        transform.position,
                        _size / 2,
                        transform.forward,
                        out var hit,
                        transform.rotation,
                        _settings.maxRange,
                        _settings.layers,
                        QueryTriggerInteraction.Ignore))
            {
                var rigid = hit.collider.GetComponentInParent<Rigidbody>();

                if (rigid != null)
                {
                    leaderSpeed = rigid.SpeedInKPH();
                }

                _active = _vehicle.CurrentSpeed >= _settings.minSpeed && leaderSpeed > _settings.minSpeed * 0.5f;
            }
            else
            {
                _active = false;
            }

            if (_active)
            {
                _rigid?.AddForce(transform.forward * _settings.strength * _vehicle.ThrottleInput, ForceMode.Acceleration);
            }
        }
    }
}