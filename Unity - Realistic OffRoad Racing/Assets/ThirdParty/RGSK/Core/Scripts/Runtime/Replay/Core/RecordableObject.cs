using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    [System.Serializable]
    public class FrameData
    {
        //Transform
        public Vector3 position;
        public Quaternion rotation;

        //Rigidbody
        public Vector3 velocity;
        public Vector3 angularVelocity;

        //Vehicle
        public float throttleInput;
        public float brakeInput;
        public float steerInput;
        public float handbrakeInput;
        public float nitrousInput;
        public float engineRPM;
        public int gear;
        public float nitrousCapacity;
        public bool engineOn;
        public bool headlightsOn;
        public bool hornOn;
    }

    [System.Serializable]
    public class RecordableObject
    {
        public GameObject gameObject;
        public Rigidbody rigidbody;
        public IVehicle vehicle;

        public ListNonAlloc<FrameData> FrameData { get; private set; } = new ListNonAlloc<FrameData>(3000, 3000);

        public void AllocateFrames(int mins = 60, int expandMins = 1)
        {
            var capacity = (int)((1 / Time.fixedDeltaTime) * 60 * mins);
            var expandSize = (int)((1 / Time.fixedDeltaTime) * 60 * expandMins);
            FrameData = new ListNonAlloc<FrameData>(capacity, expandSize);
        }

        public void CreateFrame()
        {
            var data = new FrameData();

            if (gameObject != null)
            {
                data.position = gameObject.transform.position;
                data.rotation = gameObject.transform.rotation;
            }

            if (rigidbody != null)
            {
                data.velocity = rigidbody.linearVelocity;
                data.angularVelocity = rigidbody.angularVelocity;
            }

            if (vehicle != null)
            {
                data.throttleInput = vehicle.ThrottleInput;
                data.brakeInput = vehicle.BrakeInput;
                data.steerInput = vehicle.SteerInput;
                data.handbrakeInput = vehicle.HandbrakeInput;
                data.nitrousInput = vehicle.NitrousInput;

                data.engineRPM = vehicle.EngineRPM;
                data.gear = vehicle.CurrentGear;
                data.nitrousCapacity = vehicle.NitrousCapacity;
                data.engineOn = vehicle.IsEngineOn;
                data.headlightsOn = vehicle.HeadlightsOn;
                data.hornOn = vehicle.HornOn;
            }

            FrameData.Add(data);
        }

        public void CreateFrame(FrameData frame)
        {
            FrameData.Add(frame);
        }

        public void PlayFrame(int frame)
        {
            if (frame < 0 || frame > FrameData.Count)
                return;

            var data = FrameData[frame];
            PlayFrame(data);
        }

        void PlayFrame(FrameData frame)
        {
            if (gameObject != null)
            {
                gameObject.transform.position = frame.position;
                gameObject.transform.rotation = frame.rotation;
            }

            if (rigidbody != null && !rigidbody.isKinematic)
            {
                rigidbody.linearVelocity = frame.velocity;
                rigidbody.angularVelocity = frame.angularVelocity;
            }

            if (vehicle != null)
            {
                if (!vehicle.HasControl)
                {
                    vehicle.EnableControl();
                }

                if (frame.engineOn && !vehicle.IsEngineOn)
                {
                    vehicle.StartEngine(0);
                }
                else if (!frame.engineOn && vehicle.IsEngineOn)
                {
                    vehicle.StopEngine();
                }

                vehicle.ThrottleInput = frame.throttleInput;
                vehicle.BrakeInput = frame.brakeInput;
                vehicle.SteerInput = frame.steerInput;
                vehicle.HandbrakeInput = frame.handbrakeInput;
                vehicle.NitrousInput = frame.nitrousInput;

                vehicle.EngineRPM = frame.engineRPM;
                vehicle.CurrentGear = frame.gear;
                vehicle.NitrousCapacity = frame.nitrousCapacity;
                vehicle.HeadlightsOn = frame.headlightsOn;
                vehicle.HornOn = frame.hornOn;
            }
        }

        public void DeleteFrameData()
        {
            FrameData.Clear();
        }
    }
}