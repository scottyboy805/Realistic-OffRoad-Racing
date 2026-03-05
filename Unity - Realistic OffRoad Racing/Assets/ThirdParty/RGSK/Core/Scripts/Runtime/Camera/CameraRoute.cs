using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Extensions;

namespace RGSK
{
    public class CameraRoute : MonoBehaviour
    {
        [Tooltip("The route used to determine the closest camera to activate. Leave as null to use worldspace coordinates.")]
        public Route route;

        [Tooltip("The priority assigned to the active child camera.")]
        [SerializeField] int cameraPriority = 100;

        [Tooltip("Should this be auto assigned to the Camera Manager? Leave to false if this is part of a raceable track.")]
        [SerializeField] bool autoAssign = false;

        public bool Active { get; private set; }

        List<RouteCamera> _cameras = new List<RouteCamera>();
        Transform _target => CameraManager.Instance?.Target;

        void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void Awake()
        {
            GetCameras();
            DisableCameras();
        }

        void Start()
        {
            if (autoAssign)
            {
                CameraManager.Instance?.SetRouteCameras(this);
            }
        }

        public void GetCameras()
        {
            GetComponentsInChildren(_cameras);
            _cameras = _cameras.OrderBy(x => x.routeDistance).ToList();
        }

        void Update()
        {
            if (_target == null || !Active)
                return;

            var index = route != null ? GetClosestCameraOnRoute() : GetClosestCameraInWorld();

            for (int i = 0; i < _cameras.Count; i++)
            {
                _cameras[i].VirtualCamera.Priority = i == index ? cameraPriority : 0;
            }
        }

        public void Toggle(bool active)
        {
            if (!active)
            {
                DisableCameras();
            }

            Active = active;
        }

        void OnCameraTargetChanged(Transform t)
        {
            foreach (var c in _cameras)
            {
                c.VirtualCamera.Follow = c.VirtualCamera.LookAt = _target;
            }
        }

        void DisableCameras()
        {
            foreach (var c in _cameras)
            {
                c.VirtualCamera.Priority = 0;
            }
        }

        int GetClosestCameraOnRoute()
        {
            if (route == null || route.nodes.Count < 2)
                return 0;

            var dist = route.GetDistanceAtPosition(_target.position);

            for (int i = 0; i < _cameras.Count; i++)
            {
                var start = i == 0 ? 0 : _cameras[i].routeDistance;
                var end = i < _cameras.Count - 1 ? _cameras[i + 1].routeDistance : route.Distance;

                if (dist.IsBetween(start, end))
                {
                    return i;
                }
            }

            return 0;
        }

        int GetClosestCameraInWorld()
        {
            var closestDistanceSqr = Mathf.Infinity;
            var result = 0;

            foreach (var c in _cameras)
            {
                var distanceToTarget = (c.transform.position - _target.transform.position).sqrMagnitude;

                if (distanceToTarget < closestDistanceSqr)
                {
                    result = _cameras.IndexOf(c);
                    closestDistanceSqr = distanceToTarget;
                }
            }

            return result;
        }
    }
}