//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2023 Skril Studio__________//
//______________________________________________//
//__________ http://skrilstudio.com/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkrilStudio
{
    public class RCCP_RES2 : MonoBehaviour
    {
        private RealisticEngineSound[] res2;
        private RCCP_CarController rccp;
        private RCCP_Camera rccpCamera;
        private GameObject car;
        private GameObject rccpCarCamera;
        private float gasPedalSensity = 0.01f; // sets the sensity of detecting gas pedal pressing
        private int currentActivePrefab = 2; // 0 = exterior, 1 = interior, 2 = scene start
        public string rccpCameraName = "RCCP_Camera";
        void Start()
        {
            res2 = GetComponentsInChildren<RealisticEngineSound>();
            car = gameObject.GetFirstParentWithComponent<RCCP_CarController>();
            rccp = car.GetComponent<RCCP_CarController>();
            rccpCarCamera = GameObject.Find("" + rccpCameraName);
            if(rccpCarCamera != null) rccpCamera = rccpCarCamera.GetComponent<RCCP_Camera>();
            // prepare res2 prefabs
            for (int i = 0; i < res2.Length; i++)
            {
                res2[i].maxRPMLimit = rccp.Engine.maxEngineRPM;
                res2[i].carMaxSpeed = 250;
            }
            // disable rccp stock engine sound
            if (rccp.Audio != null)
            {
                rccp.Audio.DisableEngineSounds();
                rccp.Audio.engineSounds = null;
            }
        }
        void Update()
        {
            //if (res2 == null || currentActivePrefab < 0 || currentActivePrefab >= res2.Length || res2[currentActivePrefab] == null)
            //    return;

            if (currentActivePrefab != 2) // avoid enabling two prefabs at the same time
            {
                if (res2[currentActivePrefab].carMaxSpeed != rccp.maximumSpeed) // rccp max speed value was 0 at the momment when Start() were running, get it's correct max speed value after Start()
                {
                    res2[currentActivePrefab].carMaxSpeed = rccp.maximumSpeed;
                }
                if (res2[currentActivePrefab].enabled)
                {
                    if (rccp.engineRunning)
                        res2[currentActivePrefab].engineCurrentRPM = rccp.engineRPM; // get rcc car's current rpm
                    if (rccp.speed > 0)
                        res2[currentActivePrefab].carCurrentSpeed = rccp.speed; // get rcc car's current speed
                    else
                        res2[currentActivePrefab].carCurrentSpeed = -rccp.speed; // car is going backwards, turn negative speed value to a positive value
                    res2[currentActivePrefab].isShifting = rccp.Gearbox.shiftingNow; // needed for shifting sounds script
                    if (rccp.throttleInput_P >= gasPedalSensity) // gas pedal is pressed
                    {
                        if (rccp.Gearbox.shiftingNow)
                        {
                            res2[currentActivePrefab].gasPedalPressing = false;
                        }
                        else
                        {
                            res2[currentActivePrefab].gasPedalPressing = true;
                        }
                    }
                    if (rccp.throttleInput_P < gasPedalSensity && rccp.throttleInput_P > -gasPedalSensity) // gas pedal is not pressing
                    {
                        res2[currentActivePrefab].gasPedalPressing = false;
                    }
                    if (rccp.direction == -1) // RCC car is in reverse gear, play reversing sound
                    {
                        if (rccp.throttleInput_P <= -gasPedalSensity) // gas pedal is pressing
                        {
                            res2[currentActivePrefab].gasPedalPressing = true;
                        }
                        if (rccp.Gearbox.shiftingNow)
                        {
                            res2[currentActivePrefab].gasPedalPressing = false;
                        }
                        if (res2[currentActivePrefab].enableReverseGear)
                            res2[currentActivePrefab].isReversing = true;
                    }
                    else
                    {
                        res2[currentActivePrefab].isReversing = false;
                    }
                }
                // turn off prefab when RCC car's engine is not running
                if (!rccp.engineRunning)
                    res2[currentActivePrefab].engineCurrentRPM = 0;
            }
        }
        void LateUpdate()
        {
            //if (res2 == null || currentActivePrefab < 0 || currentActivePrefab >= res2.Length || res2[currentActivePrefab] == null)
            //    return;

            if (currentActivePrefab != 2)
            {
                if (res2[currentActivePrefab].enabled && res2.Length > 1)
                    CameraUpdate();
            }
            else // scene start
            {
                if (res2.Length > 1)
                    CameraUpdate();
                else // only one prefab added
                    currentActivePrefab = 0;
            }
        }
        private void CameraUpdate()
        {
            if (rccpCamera == null)
                return;

            // interior camera
            if (rccpCamera.cameraMode == RCCP_Camera.CameraMode.FPS)
            {
                if (currentActivePrefab != 1)
                {
                    // switch sounds
                    res2[0].gameObject.SetActive(false); // exterior prefab
                    res2[1].gameObject.SetActive(true); // interior prefab
                    currentActivePrefab = 1;
                }
            }
            else // exterior cameras
            {
                if (currentActivePrefab != 0)
                {
                    // switch sounds
                    res2[0].gameObject.SetActive(true); // exterior prefab
                    res2[1].gameObject.SetActive(false); // interior prefab
                    currentActivePrefab = 0;
                }
            }
        }
    }
}
