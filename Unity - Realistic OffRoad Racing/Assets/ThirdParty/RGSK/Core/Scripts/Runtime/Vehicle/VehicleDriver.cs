using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace RGSK
{
    public class VehicleDriver : MonoBehaviour
    {
        [Header("IK Constraints")]
        public TwoBoneIKConstraint leftHand;
        public TwoBoneIKConstraint rightHand;
        public TwoBoneIKConstraint leftFoot;
        public TwoBoneIKConstraint rightFoot;
        public MultiAimConstraint head;
        [SerializeField] float maxHeadRotateAngle = 60;
        [SerializeField] float headRoateSpeed = 5;

        IVehicle _vehicle;

        public void PlaceInVehicle(GameObject vehicle, int seatIndex = 0)
        {
            if (vehicle == null)
                return;

            if (leftHand == null || rightHand == null
             || leftFoot == null || rightFoot == null
             || head == null)
            {
                Logger.LogWarning(this, "Please assign all the IK constraints!");
                return;
            }

            var seats = vehicle.GetComponentsInChildren<VehicleSeat>();
            if (seats.Length == 0)
            {
                Logger.LogWarning(this, "No seats found in the vehicle!");
                return;
            }

            foreach (var s in seats)
            {
                if (s.index == seatIndex)
                {
                    transform.SetParent(s.transform, false);
                    break;
                }
            }

            var targets = vehicle.GetComponentsInChildren<VehicleIKTarget>();
            foreach (var t in targets)
            {
                switch (t.target)
                {
                    case IKTarget.LeftHand:
                        {
                            leftHand.data.target = t.transform;
                            break;
                        }

                    case IKTarget.RightHand:
                        {
                            rightHand.data.target = t.transform;
                            break;
                        }

                    case IKTarget.LeftFoot:
                        {
                            leftFoot.data.target = t.transform;
                            break;
                        }

                    case IKTarget.RightFoot:
                        {
                            rightFoot.data.target = t.transform;
                            break;
                        }

                    case IKTarget.HeadLook:
                        {
                            var wTransform = new WeightedTransform { transform = t.transform, weight = 1f };
                            var sourceObjects = new WeightedTransformArray { wTransform };
                            head.data.sourceObjects = sourceObjects;
                            break;
                        }
                }
            }

            if (gameObject.TryGetComponent<RigBuilder>(out var rig))
            {
                rig.Build();
            }

            _vehicle = vehicle.GetComponent<IVehicle>();
        }

        void Update()
        {
            if (_vehicle != null && head != null)
            {
                var offset = head.data.offset;

                offset.y = Mathf.Lerp(offset.y,
                                    _vehicle.SteerInput * maxHeadRotateAngle,
                                    Time.deltaTime * headRoateSpeed);

                head.data.offset = offset;
            }
        }
    }
}