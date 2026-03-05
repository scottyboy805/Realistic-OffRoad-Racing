using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class RouteProgressTracker : MonoBehaviour
    {
        public float LapPercentage { get; private set; }
        public float TotalPercentage { get; private set; }

        Route _route;
        RouteNode _previous;
        RouteNode _next;
        float _currentPercentage;
        float _lastPercentage;
        float _lower;
        float _upper;
        bool _useClosestNode;
        bool _inFirstSegment;
        bool _inLastSegment;

        public void Setup(Route route, bool useNextNodeAsClosest = false)
        {
            _route = route;
            _useClosestNode = useNextNodeAsClosest;
        }

        void LateUpdate()
        {
            if (_route == null)
                return;

            _inFirstSegment = _route.loop && _next == _route.GetFirstNode();
            _inLastSegment = _route.loop && _next == _route.GetLastNode();
            _lastPercentage = LapPercentage;
            _currentPercentage = _route.GetPercentageAtPosition(transform.position, _useClosestNode ? _next : null);

            if (_inFirstSegment && _currentPercentage.IsBetween(0.5f, 1.0f))
            {
                _currentPercentage = 0;
            }
            else if (_inLastSegment && _currentPercentage.IsBetween(0.0f, 0.5f))
            {
                _currentPercentage = 1;
            }

            if (float.IsNaN(_currentPercentage))
            {
                _currentPercentage = 0;
            }

            if (_inLastSegment)
            {
                _upper = 1;
            }

            LapPercentage = Mathf.Clamp(_currentPercentage, 0, _upper <= 0.001f ? 1f : _upper);

            if (LapPercentage < _lastPercentage * 0.5f || LapPercentage > _lastPercentage * 2f)
            {
                _lastPercentage = LapPercentage;
            }

            var difference = LapPercentage - _lastPercentage;
            TotalPercentage += difference;
        }

        public void SetNextAndPreviousNode(RouteNode next, RouteNode previous)
        {
            _next = next;
            _previous = previous;
            _lower = _previous?.normalizedDistance ?? 0f;
            _upper = _next?.normalizedDistance ?? 1f;
        }

        public void UpdateTotalPercentage(int laps)
        {
            LapPercentage = _lower;
            TotalPercentage = laps + _lower;
        }

        public void ResetTotalPercentage()
        {
            TotalPercentage = LapPercentage = 0;
        }

        public float GetLapDistance() => _route != null ? LapPercentage * _route.Distance : 0f;

        public float GetTotalDistance() => _route != null ? TotalPercentage * _route.Distance : 0f;

        void OnDrawGizmos()
        {
            if (_route == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _route.GetPositionAtPercentage(LapPercentage));
        }
    }
}