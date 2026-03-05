//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Truck trailer has additional wheelcolliders. This script handles center of mass of the trailer, wheelcolliders, ligths, etc...
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Truck Trailer")]
public class RCCP_TruckTrailer : RCCP_GenericComponent {

    /// <summary>
    /// Car controller of this trailer.
    /// </summary>
    private RCCP_CarController carController;

    /// <summary>
    /// Rigidbody.
    /// </summary>
    private Rigidbody rigid;

    /// <summary>
    /// Configurable joint of this trailer.
    /// </summary>
    private ConfigurableJoint joint;

    /// <summary>
    /// Wheel colliders and models.
    /// </summary>
    [System.Serializable]
    public class TrailerWheel {

        public WheelCollider wheelCollider;
        public Transform wheelModel;
        public bool steering = false;
        public float maxSteerAngle = 40f;
        [HideInInspector] public Vector3 localPosition = Vector3.zero;

        public void Torque(float torque) {

            wheelCollider.motorTorque = torque;

        }

        public void Brake(float torque) {

            wheelCollider.brakeTorque = torque;

        }

    }

    /// <summary>
    /// All trailer wheels.
    /// </summary>
    public TrailerWheel[] trailerWheels;

    /// <summary>
    /// Center of mass.
    /// </summary>
    public Transform COM;

    /// <summary>
    /// Legs will be enabled when trailer is detached.
    /// </summary>
    public GameObject legs;

    /// <summary>
    /// Is rigidbody of the trailer is sleeping?
    /// </summary>
    private bool isSleeping = false;

    /// <summary>
    /// //  Timer for attach / detach process.
    /// </summary>
    [Range(0f, 1f)] private float timer = 0f;

    /// <summary>
    /// Attacher.
    /// </summary>
    public RCCP_TrailerAttacher attacher;

    /// <summary>
    /// //  Is this trailer attached now?
    /// </summary>
    public bool attached = false;

    /// <summary>
    /// Joint restrictions of the trailer.
    /// </summary>
    private class JointRestrictions {

        public ConfigurableJointMotion motionX;
        public ConfigurableJointMotion motionY;
        public ConfigurableJointMotion motionZ;

        public ConfigurableJointMotion angularMotionX;
        public ConfigurableJointMotion angularMotionY;
        public ConfigurableJointMotion angularMotionZ;

        public void Get(ConfigurableJoint configurableJoint) {

            motionX = configurableJoint.xMotion;
            motionY = configurableJoint.yMotion;
            motionZ = configurableJoint.zMotion;

            angularMotionX = configurableJoint.angularXMotion;
            angularMotionY = configurableJoint.angularYMotion;
            angularMotionZ = configurableJoint.angularZMotion;

        }

        public void Set(ConfigurableJoint configurableJoint) {

            configurableJoint.xMotion = motionX;
            configurableJoint.yMotion = motionY;
            configurableJoint.zMotion = motionZ;

            configurableJoint.angularXMotion = angularMotionX;
            configurableJoint.angularYMotion = angularMotionY;
            configurableJoint.angularZMotion = angularMotionZ;

        }

        public void Reset(ConfigurableJoint configurableJoint) {

            configurableJoint.xMotion = ConfigurableJointMotion.Free;
            configurableJoint.yMotion = ConfigurableJointMotion.Free;
            configurableJoint.zMotion = ConfigurableJointMotion.Free;

            configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
            configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
            configurableJoint.angularZMotion = ConfigurableJointMotion.Free;

        }

    }
    private JointRestrictions jointRestrictions = new JointRestrictions();

    private void Start() {

        rigid = GetComponent<Rigidbody>();      //	Getting rigidbody.
        joint = GetComponent<ConfigurableJoint>();      //	Getting configurable joint.
        jointRestrictions.Get(joint);       //	Getting current limitations of the joint.

        // Fixing stutering bug of the rigid.
        rigid.interpolation = RigidbodyInterpolation.None;
        rigid.interpolation = RigidbodyInterpolation.Interpolate;
        joint.configuredInWorldSpace = true;

        //	If joint is connected as default, attach the trailer. Otherwise detach.
        if (joint.connectedBody) {

            AttachTrailer(joint.connectedBody.gameObject.GetComponent<RCCP_CarController>());

        } else {

            carController = null;
            joint.connectedBody = null;
            jointRestrictions.Reset(joint);

        }

    }

    private void FixedUpdate() {

        attached = joint.connectedBody;     //	Is trailer attached now?
        rigid.centerOfMass = transform.InverseTransformPoint(COM.transform.position);       //	Setting center of mass.

        // If trailer is not connected, return.
        if (!carController)
            return;

        //	Applying torque to the wheels.
        for (int i = 0; i < trailerWheels.Length; i++) {

            trailerWheels[i].Torque(carController.throttleInput_V * (attached ? 1f : 0f));
            trailerWheels[i].Brake((attached ? 0f : 5000f));

        }

    }

    private void Update() {

        //	If trailer is not moving, enable sleeping mode.
        if (rigid.linearVelocity.magnitude < .01f && Mathf.Abs(rigid.angularVelocity.magnitude) < .01f)
            isSleeping = true;
        else
            isSleeping = false;

        // Timer was used for attach/detach delay.
        if (timer > 0f)
            timer -= Time.deltaTime;

        timer = Mathf.Clamp01(timer);       //	Clamping timer between 0f - 1f.

        WheelAlign();  // Aligning wheel model position and rotation.

    }

    /// <summary>
    /// Aligning wheel model position and rotation.
    /// </summary>
    private void WheelAlign() {

        //	If trailer is sleeping, return.
        if (isSleeping)
            return;

        if (trailerWheels == null)
            return;

        if (trailerWheels.Length < 1)
            return;

        float averageSidewaysSlip = 0f;
        int steeringWheelAmount = 0;

        for (int i = 0; i < trailerWheels.Length; i++) {

            // Return if no wheel model selected.
            if (!trailerWheels[i].wheelModel) {

                Debug.LogError(transform.name + " wheel of the " + transform.name + " is missing wheel model. This wheel is disabled");
                enabled = false;
                return;

            }

            // Locating correct position and rotation for the wheel.
            Vector3 wheelPosition;
            Quaternion wheelRotation;
            trailerWheels[i].wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);

            //	Assigning position and rotation to the wheel model.
            trailerWheels[i].wheelModel.transform.SetPositionAndRotation(wheelPosition, wheelRotation);

            if (trailerWheels[i].steering) {

                WheelHit hit;
                trailerWheels[i].wheelCollider.GetGroundHit(out hit);

                int direction = transform.InverseTransformDirection(rigid.linearVelocity).z > 0 ? 1 : -1;

                averageSidewaysSlip += hit.sidewaysSlip * direction;
                steeringWheelAmount++;

            }

        }

        if (averageSidewaysSlip > 0f && steeringWheelAmount > 0)
            averageSidewaysSlip /= (float)steeringWheelAmount;

        for (int i = 0; i < trailerWheels.Length; i++) {

            if (trailerWheels[i].steering) {

                if (trailerWheels[i].localPosition == Vector3.zero)
                    trailerWheels[i].localPosition = trailerWheels[i].wheelCollider.transform.localPosition;

                trailerWheels[i].wheelCollider.steerAngle += averageSidewaysSlip;
                trailerWheels[i].wheelCollider.steerAngle = Mathf.Clamp(trailerWheels[i].wheelCollider.steerAngle, -trailerWheels[i].maxSteerAngle, trailerWheels[i].maxSteerAngle);

                int side = 1;

                if (trailerWheels[i].localPosition.x < 0)
                    side = -1;

                Vector3 targetPosition = new Vector3(trailerWheels[i].localPosition.x - trailerWheels[i].wheelCollider.steerAngle * .01f * 1f, trailerWheels[i].localPosition.y, trailerWheels[i].localPosition.z - trailerWheels[i].wheelCollider.steerAngle * .015f * side);
                trailerWheels[i].wheelCollider.transform.localPosition = targetPosition;

            }

        }

    }

    /// <summary>
    /// Detach the trailer.
    /// </summary>
    public void DetachTrailer() {

        // Resetting attachedTrailer of car controller.
        carController = null;
        timer = 1f;
        joint.connectedBody = null;
        jointRestrictions.Reset(joint);

        if (legs)
            legs.SetActive(true);

        if (RCCSceneManager.activePlayerCamera && RCCSceneManager.activePlayerCamera.TPSAutoFocus)
            StartCoroutine(RCCSceneManager.activePlayerCamera.AutoFocus());

    }

    /// <summary>
    /// Attach the trailer.
    /// </summary>
    /// <param name="vehicle"></param>
    public void AttachTrailer(RCCP_CarController vehicle) {

        // If delay is short, return.
        if (timer > 0)
            return;

        timer = 1f;     //	Setting timer.

        carController = vehicle;        //	Assigning car controller.
        joint.connectedBody = vehicle.Rigid;        //	Connecting joint.
        jointRestrictions.Set(joint);       //	Enabling limitations of the joint.

        // If trailer has legs, disable on attach.
        if (legs)
            legs.SetActive(false);

        // Assigning attachedTrailer of car controller.
        rigid.isKinematic = false;

        // If autofocus is enabled on RCCP Camera, run it.
        if (RCCSceneManager.activePlayerCamera && RCCSceneManager.activePlayerCamera.TPSAutoFocus)
            StartCoroutine(RCCSceneManager.activePlayerCamera.AutoFocus(transform, carController.transform));

    }

    private void Reset() {

        if (transform.Find("COM") == null) {

            GameObject com = new GameObject("COM");
            com.transform.SetParent(transform, false);
            com.transform.localPosition = Vector3.zero;
            com.transform.localRotation = Quaternion.identity;
            com.transform.localScale = Vector3.one;
            COM = com.transform;

        }

        if (transform.Find("Wheel Models") == null) {

            GameObject com = new GameObject("Wheel Models");
            com.transform.SetParent(transform, false);
            com.transform.localPosition = Vector3.zero;
            com.transform.localRotation = Quaternion.identity;
            com.transform.localScale = Vector3.one;

        }

        if (transform.Find("Wheel Colliders") == null) {

            GameObject com = new GameObject("Wheel Colliders");
            com.transform.SetParent(transform, false);
            com.transform.localPosition = Vector3.zero;
            com.transform.localRotation = Quaternion.identity;
            com.transform.localScale = Vector3.one;

        }

        if (transform.Find("RCCP_TrailerAttacher") == null) {

            attacher = new GameObject("RCCP_TrailerAttacher").AddComponent<RCCP_TrailerAttacher>();
            attacher.transform.SetParent(transform, false);
            attacher.transform.localPosition = new Vector3(0f, 0f, 2.75f);

        }

        GetComponent<Rigidbody>().mass = 500f;

    }

}
