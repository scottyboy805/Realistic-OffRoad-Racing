using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class WrongwayTracker : MonoBehaviour
    {
        Competitor _competitor;
        bool _isGoingWrongway;
        float _distance;
        float _furthestDistance;
        float _waitTimer;
        float _lastDistanceOverrideTime;

        public void Setup(Competitor c)
        {
            _competitor = c;
        }

        void LateUpdate()
        {
            if (!RGSKCore.Instance.RaceSettings.wrongwayTracking)
                return;

            if (_competitor == null || _competitor.CheckpointRoute == null || Time.time < _lastDistanceOverrideTime)
                return;

            _distance = _competitor.DistanceTravelled % _competitor.CheckpointRoute.Distance;

            //force a reset when crossing start/finish
            if (_competitor.CheckpointRoute.loop && _distance < _competitor.CheckpointRoute.Distance * 0.1f && _furthestDistance > _competitor.CheckpointRoute.Distance * 0.9f)
            {
                ResetFurthestDistance();
            }

            if (!_isGoingWrongway)
            {
                if (_distance > _furthestDistance)
                {
                    _furthestDistance = _distance;
                }

                if (_distance > 10f && _distance < _furthestDistance - 10 && _competitor.IsRacing())
                {
                    _waitTimer += Time.deltaTime;

                    if (_waitTimer > 2f)
                    {
                        _waitTimer = 0f;
                        _isGoingWrongway = true;
                        RGSKEvents.OnWrongwayStart.Invoke(_competitor);
                    }
                }
                else
                {
                    _waitTimer = 0f;
                }
            }
            else
            {
                if (_distance < _furthestDistance)
                {
                    _furthestDistance = _distance;
                }

                if (_distance > _furthestDistance + 0.5f || _distance < 10 || !_competitor.IsRacing())
                {
                    _isGoingWrongway = false;
                    RGSKEvents.OnWrongwayStop.Invoke(_competitor);
                }
            }
        }

        public void SetFurthestDistance(float distance)
        {
            _furthestDistance = distance;
            _lastDistanceOverrideTime = Time.time + 1f;
        }

        public void ResetFurthestDistance()
        {
            _furthestDistance = -1;
        }
    }
}