using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Extensions;
using System.Linq;
using Cinemachine;

namespace RGSK
{
    public class ProximityArrowUI : MonoBehaviour
    {
        ProximityArrowSettings _settings => RGSKCore.Instance.GeneralSettings.proximityArrowSettings;
        Camera _camera;
        Transform _player;
        List<Collider> _playerColliders = new List<Collider>();
        List<RectTransform> _arrows = new List<RectTransform>();
        List<ProximityArrowTarget> _targets = new List<ProximityArrowTarget>();

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
            if (_settings == null || _settings.arrowPrefab == null)
                return;

            _camera = CameraManager.Instance?.Camera?.OutputCamera;

            for (int i = 0; i < _settings.maxArrows; i++)
            {
                var arrow = Instantiate(_settings.arrowPrefab, transform);
                _arrows.Add(arrow);
            }
        }

        void FixedUpdate()
        {
            _targets.Clear();

            foreach (var a in _arrows)
            {
                a.gameObject.SetActiveSafe(false);
            }

            if (!CanShowArrows())
                return;

            var cols = Physics.OverlapSphere(_player.position, _settings.radius, _settings.layers);

            foreach (var c in cols)
            {
                if (_targets.Count < _settings.maxArrows && !_playerColliders.Contains(c))
                {
                    var target = c.GetComponentInParent<ProximityArrowTarget>();

                    if (target != null && !_targets.Contains(target) && target.transform.IsBehind(_player.transform))
                    {
                        _targets.Add(target);
                    }
                }
            }

            for (int i = 0; i < _targets.Count; i++)
            {
                var direction = _player.position - _targets[i].transform.position;
                direction.y = 0;

                var camRelativeDir = _camera.transform.InverseTransformDirection(direction);

                var angle = Mathf.Atan2(camRelativeDir.x, camRelativeDir.z) * Mathf.Rad2Deg;

                _arrows[i].gameObject.SetActiveSafe(true);
                _arrows[i].localRotation = Quaternion.Euler(0, 0, -angle);
            }
        }

        void OnCameraTargetChanged(Transform t)
        {
            _player = t;
            _playerColliders.Clear();
            _player.GetComponentsInChildren<Collider>(_playerColliders);
        }

        bool CanShowArrows()
        {
            if (!RGSKCore.Instance.UISettings.showProximityArrows)
                return false;
                
            if(CameraManager.Instance != null && CameraManager.Instance.RouteCamerasEnabled)
                return false;
            
            if (_settings != null && _player != null && _camera != null && _camera.enabled)
                return true;
            
            return false;
        }
    }
}