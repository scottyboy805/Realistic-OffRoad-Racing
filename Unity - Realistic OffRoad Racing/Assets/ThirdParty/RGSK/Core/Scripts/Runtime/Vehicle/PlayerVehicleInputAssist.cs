using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class PlayerVehicleInputAssist : MonoBehaviour
    {
        public bool autoThrottle => RGSKCore.Instance.VehicleSettings.autoThrottle;
        public bool autoBrake => RGSKCore.Instance.VehicleSettings.autoBrake;

        public float Throttle { get; set; }
        public float Brake { get; set; }

        List<AINode> _aiNodes = new List<AINode>();
        AIRoute _route;
        IVehicle _vehicle;
        AIController _ai;
        float _brakeSensitivity = 1f;
        float _nextRouteCheckTime;

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
        }

        void Start()
        {
            _vehicle = GetComponent<IVehicle>();
            _ai = GetComponent<AIController>();
            GetNodes(RaceManager.Instance.Initialized ? RaceManager.Instance.Track.gameObject : null);
        }

        void OnRaceInitialized()
        {
            GetNodes(RaceManager.Instance.Track.gameObject);
        }

        void GetNodes(GameObject parent)
        {
            _aiNodes.Clear();

            if (parent == null)
            {
                _aiNodes = FindObjectsByType<AINode>(FindObjectsSortMode.None).ToList();
                return;
            }

            _aiNodes = parent.GetComponentsInChildren<AINode>().ToList();
        }

        public void UpdateInputs()
        {
            if (_vehicle == null || _ai == null || _ai.IsActive)
                return;

            if (InputManager.Instance.ActiveInputMode == InputMode.Gameplay)
            {
                UpdateBrakeInput();
                UpdateThrottleInput();
            }
        }

        void UpdateThrottleInput()
        {
            if (!autoThrottle)
                return;

            Throttle = Brake > 0.1f ? 0 : 1;
        }

        void UpdateBrakeInput()
        {
            if (!autoBrake)
                return;

            if (_ai.ActiveRoute != null)
            {
                if (Time.time > _nextRouteCheckTime)
                {
                    _nextRouteCheckTime = Time.time + 1;
                    UpdateRoute();
                }

                _ai.CalculateTargetSpeed();
                _ai.UpdateFollowTargetPosition();

                Brake = GetBrakeInput();
            }

            float GetBrakeInput()
            {
                var sensitivity = 0f;

                if (_vehicle.CurrentSpeed > _ai.TargetSpeed)
                {
                    sensitivity = _brakeSensitivity * 0.1f;
                }

                var value = ((_ai.TargetSpeed - _vehicle.CurrentSpeed) * sensitivity);
                return Mathf.Clamp(value, -1, 0) * -1;
            }
        }

        void UpdateRoute()
        {
            var closestDistanceSqr = Mathf.Infinity;

            foreach (var n in _aiNodes)
            {
                var distance = (n.transform.position - transform.position).sqrMagnitude;

                if (distance < closestDistanceSqr)
                {
                    if (_route != n.route)
                    {
                        _route = (AIRoute)n.route;
                        _ai.SetRoute(_route);
                    }
                    else
                    {
                        _ai.LookForClosestNodes();
                    }

                    closestDistanceSqr = distance;
                }
            }
        }
    }
}