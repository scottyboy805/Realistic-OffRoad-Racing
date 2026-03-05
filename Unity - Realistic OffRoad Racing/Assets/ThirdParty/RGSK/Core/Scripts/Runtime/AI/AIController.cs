using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RGSK.Helpers;

namespace RGSK
{
    public class AIController : RGSKEntityComponent
    {
        [SerializeField] AIBehaviourSettings behaviour;
        [SerializeField] AIRoute route;

        [Header("Speed Zone Multipliers")]
        [SerializeField][Range(0.1f, 3)] float speedMultiplier = 1;
        [SerializeField][Range(0.1f, 3)] float brakeTimeMultiplier = 1;

        public AIBehaviourSettings Behaviour => behaviour;
        public AIRoute ActiveRoute => route;
        public AIInput InputController { get; private set; } = new AIInput();
        public AIAvoidance AvoidanceController { get; private set; } = new AIAvoidance();
        public AIRecovery RecoverController { get; private set; } = new AIRecovery();
        public RouteProgressTracker RouteFollower { get; private set; }
        public AISpeedZone CurrentSpeedZone { get; private set; }
        public AISpeedZone NextSpeedZone { get; private set; }
        public Vector3 Dimensions { get; private set; }
        public Vector3 TargetPosition { get; private set; }
        public float TargetSpeed { get; private set; }
        public bool IsActive { get; private set; }

        AINode _prevNode;
        AINode _nextNode;
        Transform _followTarget;
        float _travelOffset;
        float _idealOffset;
        float _caution;
        float _lastTravelOffsetTime;

        public override void Initialize(RGSKEntity e)
        {
            base.Initialize(e);

            SetBehaviour(behaviour);
            SetRoute(route, route != null);

            var box = GetComponentInChildren<BoundingBox>();
            if (box != null)
            {
                Dimensions = box.Size;
            }

            InputController.Initialize(this);
            AvoidanceController.Initialize(this);
            RecoverController.Initialize(this);
        }

        public void SetBehaviour(AIBehaviourSettings newBehaviour)
        {
            if (newBehaviour == null)
            {
                behaviour = RGSKCore.Instance.AISettings.defaultBehaviour;
                return;
            }

            behaviour = newBehaviour;
        }

        public void SetRoute(AIRoute newRoute, bool activate = false)
        {
            if (newRoute == null || newRoute.nodes.Count < 2)
                return;

            if (RouteFollower == null)
            {
                RouteFollower = gameObject.AddComponent<RouteProgressTracker>();
                _followTarget = new GameObject("AI_FollowTarget").transform;
                _followTarget.transform.SetParent(GeneralHelper.GetDynamicParent());
            }

            route = newRoute;
            RouteFollower.Setup(route, true);
            LookForClosestNodes();

            if (activate)
            {
                ToggleActive(true);
            }
        }

        public void ToggleActive(bool toggle)
        {
            if (!Initialized)
                return;

            IsActive = toggle;

            if (IsActive)
            {
                LookForClosestNodes();
                UpdateFollowTargetPosition();
                
                SetTravelOffset
                (
                    GetLateralPositionFromCenter(transform.position).x, 
                    Behaviour.commonSettings != null ? Behaviour.commonSettings.travelResetDurationOnActivate : 10
                );
                
                RecoverController?.Reset();
                Entity?.Vehicle?.EnableControl();
            }
            else
            {
                Entity?.Vehicle?.DisableControl();
            }
        }

        void FixedUpdate()
        {
            if (!IsActive ||
                route == null ||
                _followTarget == null ||
                Behaviour.commonSettings == null)
                return;

            CalculateTargetSpeed();
            UpdateFollowTargetPosition();

            InputController.Update();
            AvoidanceController.Update();
            RecoverController.Update();
        }

        public void CalculateTargetSpeed()
        {
            if (Entity.Rigid == null)
                return;

            var result = 0f;
            var target = 0f;
            var currentDistance = RouteFollower.GetLapDistance();

            if (Behaviour.speedOverride < 0)
            {
                var zones = route.GetClosestSpeedZones(currentDistance);
                CurrentSpeedZone = zones.Item1;
                NextSpeedZone = zones.Item2;

                target = CurrentSpeedZone != null ? CurrentSpeedZone.speedLimit * speedMultiplier : route.speedLimit;

                if (NextSpeedZone != null && NextSpeedZone.brakeTime > 0)
                {
                    var distance = (NextSpeedZone.startDistance - currentDistance + route.Distance) % route.Distance;
                    var time = Mathf.Abs(distance / Entity.CurrentSpeed);

                    if (time < NextSpeedZone.brakeTime * brakeTimeMultiplier)
                    {
                        var ratio = Mathf.InverseLerp(0, NextSpeedZone.brakeTime * brakeTimeMultiplier, time);

                        target = Mathf.Lerp(
                            Entity.CurrentSpeed,
                            NextSpeedZone.speedLimit * speedMultiplier,
                            ratio);
                    }
                }
            }
            else
            {
                target = Behaviour.speedOverride;
            }

            if (AvoidanceController.OverrideSpeed > 0)
            {
                target = AvoidanceController.OverrideSpeed;
            }

            if (target < 0)
            {
                target = int.MaxValue;
            }

            var _spinningAngle = Entity.Rigid.angularVelocity.magnitude * (behaviour.spinCautionAmount * 50);
            var _spinCautionFactor = Mathf.InverseLerp(behaviour.commonSettings.minMaxCautionAngle.x,
                                                       behaviour.commonSettings.minMaxCautionAngle.y,
                                                       _spinningAngle);

            switch (behaviour.targetApproachAction)
            {
                case TargetApproachAction.Follow:
                    {
                        _caution = _spinCautionFactor;
                        break;
                    }

                case TargetApproachAction.SlowDown:
                    {
                        var distance = (_followTarget.position - transform.position).magnitude;
                        var _distanceCautionFactor = Mathf.InverseLerp(behaviour.commonSettings.minMaxCautionDistance.x,
                                                                       behaviour.commonSettings.minMaxCautionDistance.y,
                                                                       distance);

                        _caution = Mathf.Max(_spinCautionFactor, _distanceCautionFactor);
                        break;
                    }

                case TargetApproachAction.SlowDownAndStop:
                    {
                        var distance = (_followTarget.position - transform.position).magnitude;

                        if (distance < behaviour.commonSettings.targetReachedDistance)
                        {
                            ToggleActive(false);
                        }
                        break;
                    }
            }

            result = Mathf.Lerp(target, Entity.CurrentSpeed * behaviour.cautiousSpeedFactor, _caution);

            if (result < 2)
            {
                result = 0;
            }

            TargetSpeed = result;
        }

        public void UpdateFollowTargetPosition()
        {
            if (_followTarget == null)
                return;

            var lookAhead = route.lookAheadCurve.Evaluate(Entity.CurrentSpeed) / route.Distance;
            var percent = RouteFollower.LapPercentage + lookAhead;

            TargetPosition = route.GetPositionAtPercentage(percent);
            TargetPosition += _followTarget.right * _travelOffset;

            _followTarget.position = route.GetPositionAtPercentage(RouteFollower.LapPercentage);

            if (_nextNode != null && _prevNode != null)
            {
                var dir = _nextNode.transform.position - _prevNode.transform.position;

                if (dir != Vector3.zero)
                {
                    _followTarget.rotation = Quaternion.LookRotation(dir);
                }
            }

            if (_nextNode != null)
            {
                var nextNodeDistance = Vector3.Distance(transform.position, _nextNode.transform.position);
                if (nextNodeDistance < route.nodeReachedDistance)
                {
                    HandleNodeReached();
                }
            }

            if (Time.time > _lastTravelOffsetTime)
            {
                _travelOffset = Mathf.Lerp(_travelOffset, _idealOffset, Time.deltaTime * 1f);
            }
        }

        public void SetTravelOffset(float value, float duration = -1)
        {
            if (Behaviour.keepWithinRouteWidth)
            {
                value = Mathf.Clamp(value,
                        -GetRouteWidth().x + (Dimensions.x / 2),
                         GetRouteWidth().y - (Dimensions.x / 2));
            }

            _travelOffset = value;

            var resetWait = duration < 0 ? UnityEngine.Random.Range(
                                     Behaviour.commonSettings.minMaxTravelResetDuration.x,
                                     Behaviour.commonSettings.minMaxTravelResetDuration.y) :
                                     duration;

            _lastTravelOffsetTime = Time.time + resetWait;
        }

        public void LookForClosestNodes()
        {
            if (route == null)
                return;

            route.GetClosestPointOnPath(transform.position, out Tuple<RouteNode, RouteNode> nodes);
            _nextNode = (AINode)nodes.Item2;
            _prevNode = (AINode)nodes.Item1;
            RouteFollower?.SetNextAndPreviousNode(_nextNode, _prevNode);
            _idealOffset = _nextNode.RacingLineOffset;
        }

        void HandleNodeReached()
        {
            if (!route.loop && _nextNode == route.GetLastNode())
            {
                if (_nextNode.branchRoutes.Count > 0)
                {
                    ChooseBranch(_nextNode.branchRoutes);
                }
                else
                {
                    ToggleActive(false);
                }
            }
            else
            {
                if (_nextNode.branchRoutes.Count > 0)
                {
                    var branches = new List<AIRoute>();
                    branches.Add(route);
                    _nextNode.branchRoutes.ForEach(x => branches.Add(x));
                    ChooseBranch(branches);
                }
                else
                {
                    GoToNextNode();
                }
            }
        }

        void GoToNextNode()
        {
            _nextNode = (AINode)_nextNode.nextNode;
            _prevNode = (AINode)_nextNode?.previousNode;
            RouteFollower?.SetNextAndPreviousNode(_nextNode, _prevNode);
            _idealOffset = _nextNode.RacingLineOffset;
        }

        void ChooseBranch(List<AIRoute> routes)
        {
            var weights = 0f;

            foreach (var branch in routes)
            {
                weights += branch.branchProbability;
            }

            var value = UnityEngine.Random.Range(0f, weights);

            foreach (var branch in routes)
            {
                value -= branch.branchProbability;

                if (value <= 0)
                {
                    if (route != branch)
                    {
                        SetRoute(branch);
                    }
                    else
                    {
                        GoToNextNode();
                    }

                    break;
                }
            }
        }

        public Vector3 GetLateralPositionFromCenter(Vector3 pos)
        {
            if (_followTarget == null)
                return Vector3.zero;

            return _followTarget.InverseTransformPoint(pos);
        }

        public Vector2 GetRouteWidth()
        {
            if (_nextNode != null)
            {
                return new Vector2(_nextNode.leftWidth, _nextNode.rightWidth);
            }
            else
            {
                if (_prevNode != null)
                {
                    return new Vector2(_prevNode.leftWidth, _prevNode.rightWidth);
                }
            }

            return Vector2.one * 10f;
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying && _followTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(TargetPosition, 0.5f);
                Gizmos.DrawLine(transform.position, TargetPosition);
            }
        }
    }
}