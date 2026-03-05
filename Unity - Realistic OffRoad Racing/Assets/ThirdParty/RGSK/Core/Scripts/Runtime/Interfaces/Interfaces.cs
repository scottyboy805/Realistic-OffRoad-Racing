using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public interface IGameObject
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }

    public interface IInput
    {
        void EnableControl();
        void DisableControl();
        bool HasControl { get; }
    }

    public interface IVehicle : IGameObject, IInput
    {
        float ThrottleInput { get; set; }
        float BrakeInput { get; set; }
        float SteerInput { get; set; }
        float HandbrakeInput { get; set; }
        float NitrousInput { get; set; }
        VehicleDefinition VehicleDefinition { get; }
        TransmissionType TransmissionType { get; set; }
        VehicleHandlingMode HandlingMode { get; set; }
        float EngineRPM { get; set; }
        float MaxEngineRPM { get; }
        bool IsEngineOn { get; }
        float CurrentSpeed { get; }
        int CurrentGear { get; set; }
        bool HasNitrous { get; }
        float NitrousCapacity { get; set; }
        bool HeadlightsOn { get; set; }
        bool HornOn { get; set; }
        bool Damageable { get; set; }
        bool OdometerEnabled { get; set; }
        int OdometerReading { get; }
        bool IsInitialized { get; }

        void Initialize();
        void StartEngine(float delay);
        void StopEngine();
        void ShiftUp();
        void ShiftDown();
        void OnReset();
        void Repair();
    }
}