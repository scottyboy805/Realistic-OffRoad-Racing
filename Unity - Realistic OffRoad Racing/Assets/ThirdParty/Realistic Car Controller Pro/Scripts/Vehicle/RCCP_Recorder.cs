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
/// Record / Replay system. Saves player's input, vehicle rigid velocity, position, and rotation on record, and replays it when on playback.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Other Addons/RCCP Recorder")]
public class RCCP_Recorder : RCCP_Component {

    /// <summary>
    /// Recorded clip.
    /// </summary>
    [System.Serializable]
    public class RecordedClip {

        /// <summary>
        /// Record name.
        /// </summary>
        public string recordName = "New Record";

        /// <summary>
        /// All vehicle inputs recorded frame by frame.
        /// </summary>
        [HideInInspector] public VehicleInput[] inputs;

        /// <summary>
        /// All position and rotation recorded frame by frame.
        /// </summary>
        [HideInInspector] public VehicleTransform[] transforms;

        /// <summary>
        /// All velocities recorded frame by frame.
        /// </summary>
        [HideInInspector] public VehicleVelocity[] rigids;

        /// <summary>
        /// Saved clip.
        /// </summary>
        /// <param name="_inputs"></param>
        /// <param name="_transforms"></param>
        /// <param name="_rigids"></param>
        /// <param name="_recordName"></param>
        public RecordedClip(VehicleInput[] _inputs, VehicleTransform[] _transforms, VehicleVelocity[] _rigids, string _recordName) {

            inputs = _inputs;
            transforms = _transforms;
            rigids = _rigids;
            recordName = _recordName;

        }

        public RecordedClip() {



        }

    }

    /// <summary>
    /// Last recorded clip.
    /// </summary>
    public RecordedClip recorded;

    /// <summary>
    /// 
    /// </summary>
    private List<VehicleInput> Inputs;

    /// <summary>
    /// Positions and rotations.
    /// </summary>
    private List<VehicleTransform> Transforms;

    /// <summary>
    /// Velocities.
    /// </summary>
    private List<VehicleVelocity> Rigidbodies;

    /// <summary>
    /// Inputs of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleInput {

        public float throttleInput = 0f;
        public float brakeInput = 0f;
        public float steerInput = 0f;
        public float handbrakeInput = 0f;
        public float clutchInput = 0f;
        public float nosInput = 0f;
        public int direction = 1;
        public int currentGear = 0;
        public RCCP_Gearbox.CurrentGearState.GearState gearState = RCCP_Gearbox.CurrentGearState.GearState.InForwardGear;
        public bool NGear = false;

        public bool lowBeamHeadLightsOn = false;
        public bool highBeamHeadLightsOn = false;
        public bool indicatorsLeft;
        public bool indicatorsRight;
        public bool indicatorsAll;

        public VehicleInput(float _gasInput, float _brakeInput, float _steerInput, float _handbrakeInput, float _clutchInput, float _boostInput, int _direction, int _currentGear, RCCP_Gearbox.CurrentGearState.GearState _gearState, bool _NGear, bool _lowBeamHeadLightsOn, bool _highBeamHeadLightsOn, bool _indicatorsLeft, bool _indicatorsRight, bool _indicatorsAll) {

            throttleInput = _gasInput;
            brakeInput = _brakeInput;
            steerInput = _steerInput;
            handbrakeInput = _handbrakeInput;
            clutchInput = _clutchInput;
            nosInput = _boostInput;
            direction = _direction;
            currentGear = _currentGear;
            gearState = _gearState;
            NGear = _NGear;

            lowBeamHeadLightsOn = _lowBeamHeadLightsOn;
            highBeamHeadLightsOn = _highBeamHeadLightsOn;
            indicatorsLeft = _indicatorsLeft;
            indicatorsRight = _indicatorsRight;
            indicatorsAll = _indicatorsAll;

        }

    }

    /// <summary>
    /// Position and rotation of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleTransform {

        public Vector3 position;
        public Quaternion rotation;

        public VehicleTransform(Vector3 _pos, Quaternion _rot) {

            position = _pos;
            rotation = _rot;

        }

    }

    /// <summary>
    /// Linear and angular velocity of the vehicle.
    /// </summary>
    [System.Serializable]
    public class VehicleVelocity {

        public Vector3 velocity;
        public Vector3 angularVelocity;

        public VehicleVelocity(Vector3 _vel, Vector3 _angVel) {

            velocity = _vel;
            angularVelocity = _angVel;

        }

    }

    /// <summary>
    /// Current state.
    /// </summary>
    public enum RecorderMode { Neutral, Play, Record }
    public RecorderMode mode = RecorderMode.Neutral;

    public override void Start() {

        base.Start();

        //  Creating new lists for inputs, transforms, and rigids.
        Inputs = new List<VehicleInput>();
        Transforms = new List<VehicleTransform>();
        Rigidbodies = new List<VehicleVelocity>();

    }

    /// <summary>
    /// Replay.
    /// </summary>
    private void RCC_InputManager_OnReplay() {

        //  Return if canControl is disabled.
        if (!CarController.IsControllableByPlayer())
            return;

        Play();

    }

    /// <summary>
    /// Record
    /// </summary>
    private void RCC_InputManager_OnRecord() {

        //  Return if canControl is disabled.
        if (!CarController.IsControllableByPlayer())
            return;

        Record();

    }

    /// <summary>
    /// Record.
    /// </summary>
    public void Record() {

        //  If current state is not record, set it to record. Otherwise set it to neutral and save the clip.
        if (mode != RecorderMode.Record) {

            mode = RecorderMode.Record;

        } else {

            mode = RecorderMode.Neutral;
            SaveRecord();

        }

        //  If mode is set to record before, clear all lists. That means we've saved the clip.
        if (mode == RecorderMode.Record) {

            Inputs.Clear();
            Transforms.Clear();
            Rigidbodies.Clear();

        }

    }

    /// <summary>
    /// Save record clip.
    /// </summary>
    public void SaveRecord() {

        print("Record saved!");
        recorded = new RecordedClip(Inputs.ToArray(), Transforms.ToArray(), Rigidbodies.ToArray(), RCCP_Records.Instance.records.Count.ToString() + "_" + CarController.transform.name);
        RCCP_Records.Instance.records.Add(recorded);

    }

    /// <summary>
    /// Play.
    /// </summary>
    public void Play() {

        //  If clip not found, return.
        if (recorded == null)
            return;

        //  If current state is not play, set it to play. Otherwise set it to neutral.
        if (mode != RecorderMode.Play)
            mode = RecorderMode.Play;
        else
            mode = RecorderMode.Neutral;

        //  If current state is play, enable external controller of the car controller.
        if (mode == RecorderMode.Play)
            OverrideVehicle(true);
        else
            OverrideVehicle(false);

        if (mode == RecorderMode.Play) {

            StartCoroutine(Replay());

            if (recorded != null && recorded.transforms.Length > 0)
                CarController.transform.SetPositionAndRotation(recorded.transforms[0].position, recorded.transforms[0].rotation);

            StartCoroutine(Revel());

        }

    }

    /// <summary>
    /// Play.
    /// </summary>
    /// <param name="_recorded"></param>
    public void Play(RecordedClip _recorded) {

        recorded = _recorded;

        print("Replaying record " + recorded.recordName);

        if (recorded == null)
            return;

        if (mode != RecorderMode.Play)
            mode = RecorderMode.Play;
        else
            mode = RecorderMode.Neutral;

        if (mode == RecorderMode.Play)
            OverrideVehicle(true);
        else
            OverrideVehicle(false);

        if (mode == RecorderMode.Play) {

            StartCoroutine(Replay());

            if (recorded != null && recorded.transforms.Length > 0)
                CarController.transform.SetPositionAndRotation(recorded.transforms[0].position, recorded.transforms[0].rotation);

            StartCoroutine(Revel());

        }

    }

    /// <summary>
    /// Stop.
    /// </summary>
    public void Stop() {

        mode = RecorderMode.Neutral;
        OverrideVehicle(false);

    }

    /// <summary>
    /// Replay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Replay() {

        for (int i = 0; i < recorded.inputs.Length && mode == RecorderMode.Play; i++) {

            OverrideVehicle(true);

            RCCP_Inputs inputs = new RCCP_Inputs();

            inputs.throttleInput = recorded.inputs[i].throttleInput;
            inputs.brakeInput = recorded.inputs[i].brakeInput;
            inputs.steerInput = recorded.inputs[i].steerInput;
            inputs.handbrakeInput = recorded.inputs[i].handbrakeInput;
            inputs.clutchInput = recorded.inputs[i].clutchInput;
            inputs.nosInput = recorded.inputs[i].nosInput;

            if (CarController.Inputs)
                CarController.Inputs.OverrideInputs(inputs);

            if (CarController.Gearbox) {

                CarController.Gearbox.OverrideGear(recorded.inputs[i].currentGear, recorded.inputs[i].gearState);

            }

            if (CarController.Lights) {

                CarController.Lights.lowBeamHeadlights = recorded.inputs[i].lowBeamHeadLightsOn;
                CarController.Lights.highBeamHeadlights = recorded.inputs[i].highBeamHeadLightsOn;
                CarController.Lights.indicatorsLeft = recorded.inputs[i].indicatorsLeft;
                CarController.Lights.indicatorsRight = recorded.inputs[i].indicatorsRight;
                CarController.Lights.indicatorsAll = recorded.inputs[i].indicatorsAll;

            }

            yield return new WaitForFixedUpdate();

        }

        mode = RecorderMode.Neutral;

        OverrideVehicle(false);

    }

    /// <summary>
    /// Linear and angular velocity.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Revel() {

        for (int i = 0; i < recorded.rigids.Length && mode == RecorderMode.Play; i++) {

            CarController.Rigid.linearVelocity = recorded.rigids[i].velocity;
            CarController.Rigid.angularVelocity = recorded.rigids[i].angularVelocity;

            yield return new WaitForFixedUpdate();

        }

        mode = RecorderMode.Neutral;

        OverrideVehicle(false);

    }

    private void FixedUpdate() {

        switch (mode) {

            case RecorderMode.Neutral:

                break;

            case RecorderMode.Play:

                OverrideVehicle(true);

                break;

            case RecorderMode.Record:

                Inputs.Add(new VehicleInput(CarController.throttleInput_V, CarController.brakeInput_V, CarController.steerInput_V, CarController.handbrakeInput_V, CarController.clutchInput_V, CarController.nosInput_V, CarController.direction, CarController.currentGear, CarController.Gearbox.currentGearState.gearState, CarController.NGearNow, CarController.lowBeamLights, CarController.highBeamLights, CarController.indicatorsLeftLights, CarController.indicatorsRightLights, CarController.indicatorsAllLights));
                Transforms.Add(new VehicleTransform(CarController.transform.position, CarController.transform.rotation));
                Rigidbodies.Add(new VehicleVelocity(CarController.Rigid.linearVelocity, CarController.Rigid.angularVelocity));

                break;

        }

    }

    /// <summary>
    /// Overrides the vehicle.
    /// </summary>
    /// <param name="overrideState"></param>
    private void OverrideVehicle(bool overrideState) {

        if (CarController.Inputs)
            CarController.Inputs.overridePlayerInputs = CarController.Inputs.overrideExternalInputs = overrideState;

        if (CarController.Gearbox)
            CarController.Gearbox.overrideGear = overrideState;

    }

    public void Reload() {

        mode = RecorderMode.Neutral;

    }

    private void Reset() {

        if (recorded == null)
            recorded = new RecordedClip();

        if (recorded != null && recorded.recordName == "New Record")
            recorded.recordName = transform.GetComponentInParent<RCCP_CarController>(true).transform.name;

    }

}
