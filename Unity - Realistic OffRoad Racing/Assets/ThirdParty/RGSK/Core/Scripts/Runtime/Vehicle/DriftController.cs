using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class DriftController : RGSKEntityComponent
    {
        public bool IsDrifting { get; set; }
        public bool HasDriftStarted { get; private set; }
        public int TotalPoints { get; set; }
        public float CurrentPoints { get; set; }
        public int TotalCurrentPoints => (int)(CurrentPoints * CurrentMultiplier);
        public float CurrentAngle { get; set; }
        public float CurrentMultiplier { get; set; }
        public float CurrentDistance { get; set; }
        public float CurrentTimer { get; set; }
        public float MultiplierProgress { get; set; }
        public float CompleteProgress { get; set; }
        public float BestPoints { get; set; }
        public float BestDistance { get; set; }
        public float BestTime { get; set; }
        public float BestAngle { get; set; }
        public float BestMultiplier { get; set; }
        public float FurthestDistance { get; set; }

        DriftSettings _settings => RGSKCore.Instance.GeneralSettings.driftSettings;
        Vector3 _previousPosition;
        int _messageIndex;
        float _lastFailTime;
        float _completeCountdownTimer;
        float _nextMultiplyTimer;
        float _currentBestAngle;

        void Awake()
        {
            ResetCurrentValues();
        }

        void Update()
        {
            if (_settings == null)
                return;

            IsDrifting = Drifting();

            if (IsDrifting)
            {
                if (!HasDriftStarted)
                {
                    HasDriftStarted = true;
                    _previousPosition = transform.position;
                    RGSKEvents.OnDriftStart.Invoke(this);
                }

                _completeCountdownTimer = 0;
                UpdatePointsAndDistance();
                UpdateMultiplier();
                UpdateMessages();
            }
            else
            {
                if (HasDriftStarted)
                {
                    _completeCountdownTimer += Time.deltaTime;

                    if (_nextMultiplyTimer > 0)
                    {
                        _nextMultiplyTimer -= _settings.multiplierDecreaseRate * Time.deltaTime;
                    }

                    if (_completeCountdownTimer >= _settings.completeWaitTime)
                    {
                        CompleteDrift();
                    }

                    if (Entity.CurrentSpeed < _settings.failSpeed)
                    {
                        FailDrift();
                    }
                }
            }

            CompleteProgress = _completeCountdownTimer / _settings.completeWaitTime;
            MultiplierProgress = _nextMultiplyTimer / 1;
        }

        void UpdatePointsAndDistance()
        {
            CurrentPoints += _settings.pointsPerSecond * Time.deltaTime;
            CurrentTimer += Time.deltaTime;
            CurrentDistance += Vector3.Distance(transform.position, _previousPosition);
            _previousPosition = transform.position;
        }

        void UpdateMultiplier()
        {
            if (CurrentMultiplier < _settings.maxMultiplierValue)
            {
                _nextMultiplyTimer += Time.deltaTime * _settings.multiplierIncreaseRate;

                if (_nextMultiplyTimer > 1)
                {
                    _nextMultiplyTimer = 0;
                    CurrentMultiplier += _settings.multiplierIncrementValue;
                    RGSKEvents.OnDriftPointMultiplierReached.Invoke(this);
                }
            }
        }

        void UpdateMessages()
        {
            for (int i = 0; i < _settings.messages.Length; i++)
            {
                if (CurrentPoints >= _settings.messages[i].points)
                {
                    if (_messageIndex < i)
                    {
                        _messageIndex = i;
                        RGSKEvents.OnDriftMessage.Invoke(this, _settings.messages[_messageIndex].message);
                    }
                }
            }
        }

        public void CompleteDrift()
        {
            var points = TotalCurrentPoints;

            if (points == 0)
                return;

            TotalPoints += points;

            if (BestPoints < points)
                BestPoints = points;

            if (BestDistance < CurrentDistance)
                BestDistance = CurrentDistance;

            if (BestTime < CurrentTimer)
                BestTime = CurrentTimer;

            if (BestMultiplier < CurrentMultiplier)
                BestMultiplier = CurrentMultiplier;

            if (BestAngle < _currentBestAngle)
                BestAngle = _currentBestAngle;

            RGSKEvents.OnDriftComplete.Invoke(this, points);
            EndDrift();
        }

        public void FailDrift()
        {
            if (Time.time < _lastFailTime || CurrentPoints == 0)
                return;

            var points = TotalCurrentPoints;

            if (points == 0)
                return;

            _lastFailTime = Time.time + 1;
            RGSKEvents.OnDriftFail.Invoke(this, points);
            EndDrift();
        }

        void EndDrift()
        {
            ResetCurrentValues();
            IsDrifting = false;
            RGSKEvents.OnDriftEnd.Invoke(this);
        }

        void ResetCurrentValues()
        {
            HasDriftStarted = false;
            _completeCountdownTimer = 0;
            _messageIndex = -1;
            _currentBestAngle = 0;
            _nextMultiplyTimer = 0;
            CurrentPoints = 0;
            CurrentMultiplier = 1;
            CurrentTimer = 0;
            CurrentDistance = 0;
        }

        public override void ResetValues()
        {
            ResetCurrentValues();
            TotalPoints = 0;
            BestPoints = 0;
            BestDistance = 0;
            BestTime = 0;
            BestAngle = 0;
            BestMultiplier = 0;
            FurthestDistance = 0;
        }

        void OnCollisionEnter(Collision col)
        {
            var force = col.relativeVelocity.magnitude;

            if (force >= _settings?.failCollisionForce)
            {
                FailDrift();
            }
        }

        bool Drifting()
        {
            if (Entity.Rigid == null || Time.time < _lastFailTime || !IsValidDistance())
                return false;

            if (Entity.CurrentSpeed < _settings.minSpeed)
                return false;

            var velo = Entity.Rigid.transform.InverseTransformDirection(Entity.Rigid.linearVelocity);

            CurrentAngle = Mathf.Round(Mathf.Asin(velo.normalized.x) * Mathf.Rad2Deg);
            CurrentAngle = Mathf.Abs(CurrentAngle);

            if (_currentBestAngle < CurrentAngle)
            {
                _currentBestAngle = CurrentAngle;
            }

            return Entity.Rigid.TravelDirection() > 0 && CurrentAngle >= _settings.minAngle;
        }

        bool IsValidDistance()
        {
            if (Entity.Competitor == null || !Entity.Competitor.IsRacing())
                return true;

            var distance = Entity.Competitor?.LapTracker?.GetTotalDistance() ?? 0f;

            if (distance > FurthestDistance)
            {
                FurthestDistance = distance;
            }

            return distance >= FurthestDistance - 10;
        }
    }
}