using UnityEngine;
using RGSK.Extensions;
using System.Collections.Generic;

namespace RGSK
{
    [System.Serializable]
    public class AIAvoidance : AIComponent
    {
        public Vector3 ObstacleHitPosition { get; private set; }
        public float OverrideSpeed { get; private set; }

        float _lastAvoidAttempt;
        Vector3 _lateralObstaclePosition;
        List<Collider> _childColliders = new List<Collider>();

        public override void Initialize(AIController ai)
        {
            base.Initialize(ai);
            ai.gameObject.GetComponentsInChildren<Collider>(_childColliders);
        }

        public override void Update()
        {
            if (Controller.Entity == null || Controller.Entity.Vehicle == null || Controller.Behaviour == null)
                return;

            if (!Controller.Behaviour.canAvoid || !Controller.Entity.Vehicle.HasControl)
                return;

            OverrideSpeed = -1;

            //front cast
            if (Physics.BoxCast(
                        Controller.transform.position,
                        Controller.Dimensions / 2.5f,
                        Controller.transform.forward,
                        out var fowardHit,
                        Controller.transform.rotation,
                        Controller.Behaviour.commonSettings.forwardSensorRange,
                        Controller.Behaviour.commonSettings.obstacleLayers,
                        QueryTriggerInteraction.Ignore))
            {
                if (!_childColliders.Contains(fowardHit.collider))
                {
                    ObstacleHitPosition = fowardHit.point;

                    var otherRigid = fowardHit.collider.GetComponentInParent<Rigidbody>();
                    var otherSpeed = otherRigid?.SpeedInKPH() ?? 0;
                    var distance = Vector3.Distance(Controller.transform.position, fowardHit.collider.transform.position);
                    var relativeVelocity = Mathf.Abs(Controller.Entity.CurrentSpeed - otherSpeed);
                    var impactTime = distance / relativeVelocity;

                    if (Controller.Entity.CurrentSpeed > otherSpeed && impactTime < Controller.Behaviour.avoidImpactTime)
                    {
                        var localDelta = Controller.transform.InverseTransformPoint(fowardHit.collider.transform.position);
                        var avoidDirection = -Mathf.Sign(Mathf.Atan2(localDelta.x, localDelta.z));
                        var targetOffset = CalculateLateralOffset(avoidDirection);

                        float CalculateLateralOffset(float dir)
                        {
                            var obstacleLateralPos = Controller.GetLateralPositionFromCenter(ObstacleHitPosition);

                            return dir > 0 ?
                                   obstacleLateralPos.x += Controller.Dimensions.x * 2 :
                                   obstacleLateralPos.x -= Controller.Dimensions.x * 2;
                        }

                        if (Controller.Behaviour.keepWithinRouteWidth)
                        {
                            //if there is not enough space to avoid then use the other side
                            if (targetOffset < -Controller.GetRouteWidth().x ||
                                targetOffset > Controller.GetRouteWidth().y)
                            {
                                targetOffset = CalculateLateralOffset(avoidDirection * -1);
                            }
                        }

                        Controller.SetTravelOffset(targetOffset);
                    }

                    if (distance < Controller.Behaviour.obstacleSlowDownDistance)
                    {
                        OverrideSpeed = Mathf.Clamp(OverrideSpeed, 10, otherSpeed);
                    }
                }
            }

            //left cast
            if (Physics.BoxCast(
                        Controller.transform.position,
                        Controller.Dimensions / 2.5f,
                        -Controller.transform.right,
                        out var leftHit,
                        Controller.transform.rotation,
                        Controller.Dimensions.x * 0.5f,
                        Controller.Behaviour.commonSettings.obstacleLayers,
                        QueryTriggerInteraction.Ignore))
            {
                if (!_childColliders.Contains(leftHit.collider))
                {
                    ObstacleHitPosition = leftHit.point;

                    _lateralObstaclePosition = Controller.GetLateralPositionFromCenter(ObstacleHitPosition);

                    if (Controller.Behaviour.keepWithinRouteWidth)
                    {
                        if (_lateralObstaclePosition.x < -Controller.GetRouteWidth().x)
                        {
                            _lateralObstaclePosition.x = -Controller.GetRouteWidth().x;
                            _lateralObstaclePosition.x += Controller.Dimensions.x;
                        }
                    }

                    Controller.SetTravelOffset(_lateralObstaclePosition.x + Controller.Dimensions.x);
                }
            }

            //right cast
            if (Physics.BoxCast(
                        Controller.transform.position,
                        Controller.Dimensions / 2.5f,
                        Controller.transform.right,
                        out var rightHit,
                        Controller.transform.rotation,
                        Controller.Dimensions.x * 0.5f,
                        Controller.Behaviour.commonSettings.obstacleLayers,
                        QueryTriggerInteraction.Ignore))
            {
                if (!_childColliders.Contains(rightHit.collider))
                {
                    ObstacleHitPosition = rightHit.point;

                    _lateralObstaclePosition = Controller.GetLateralPositionFromCenter(ObstacleHitPosition);

                    if (Controller.Behaviour.keepWithinRouteWidth)
                    {
                        if (_lateralObstaclePosition.x > Controller.GetRouteWidth().y)
                        {
                            _lateralObstaclePosition.x = Controller.GetRouteWidth().y;
                            _lateralObstaclePosition.x -= Controller.Dimensions.x;
                        }
                    }

                    Controller.SetTravelOffset(_lateralObstaclePosition.x - Controller.Dimensions.x);
                }
            }
        }

        public override void Reset()
        {

        }
    }
}