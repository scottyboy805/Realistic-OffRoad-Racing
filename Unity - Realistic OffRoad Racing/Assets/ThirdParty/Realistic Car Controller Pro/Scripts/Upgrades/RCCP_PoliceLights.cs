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
using UnityEngine.Rendering;

/// <summary>
/// Police siren with operated lights.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Police Lights")]
[DisallowMultipleComponent]
public class RCCP_PoliceLights : RCCP_Component {

    /// <summary>
    /// Siren modes. On and Off.
    /// </summary>
    public SirenMode sirenMode = SirenMode.Off;
    public enum SirenMode { Off, On }

    /// <summary>
    /// Red lights.
    /// </summary>
    public Light[] redLights;

    /// <summary>
    /// Blue lights.
    /// </summary>
    public Light[] blueLights;

    private LensFlareComponentSRP[] redLights_Lensflares = new LensFlareComponentSRP[0];

    private LensFlareComponentSRP[] blueLights_Lensflares = new LensFlareComponentSRP[0];

    public float flareBrightness = 1f;

    private Camera mainCam;

    private void Update() {

        mainCam = Camera.main;

        //  If siren mode is set to off, set all intensity of the lights to 0. Otherwise, set to 1 with timer.
        switch (sirenMode) {

            case SirenMode.Off:

                for (int i = 0; i < redLights.Length; i++)
                    if (redLights[i] != null) redLights[i].intensity = Mathf.Lerp(redLights[i].intensity, 0f, Time.deltaTime * 50f);

                for (int i = 0; i < blueLights.Length; i++)
                    if (blueLights[i] != null) blueLights[i].intensity = Mathf.Lerp(blueLights[i].intensity, 0f, Time.deltaTime * 50f);

                break;

            case SirenMode.On:

                if (Mathf.Approximately((int)(Time.time) % 2, 0) && Mathf.Approximately((int)(Time.time * 20) % 3, 0)) {

                    for (int i = 0; i < redLights.Length; i++)
                        if (redLights[i] != null) redLights[i].intensity = Mathf.Lerp(redLights[i].intensity, 1f, Time.deltaTime * 50f);

                } else {

                    for (int i = 0; i < redLights.Length; i++)
                        if (redLights[i] != null) redLights[i].intensity = Mathf.Lerp(redLights[i].intensity, 0f, Time.deltaTime * 50f);

                    if (Mathf.Approximately((int)(Time.time * 20) % 3, 0)) {

                        for (int i = 0; i < blueLights.Length; i++)
                            if (blueLights[i] != null) blueLights[i].intensity = Mathf.Lerp(blueLights[i].intensity, 1f, Time.deltaTime * 50f);

                    } else {

                        for (int i = 0; i < blueLights.Length; i++)
                            if (blueLights[i] != null) blueLights[i].intensity = Mathf.Lerp(blueLights[i].intensity, 0f, Time.deltaTime * 50f);

                    }

                }

                break;

        }

        LensFlares();

    }

    private void LensFlares() {

        if (redLights_Lensflares.Length < 1) {

            redLights_Lensflares = new LensFlareComponentSRP[redLights.Length];

            for (int i = 0; i < redLights.Length; i++) {

                if (redLights[i] != null)
                    redLights_Lensflares[i] = redLights[i].GetComponent<LensFlareComponentSRP>();

            }

        }

        if (blueLights_Lensflares.Length < 1) {

            blueLights_Lensflares = new LensFlareComponentSRP[blueLights.Length];

            for (int i = 0; i < blueLights.Length; i++) {

                if (blueLights[i] != null)
                    blueLights_Lensflares[i] = blueLights[i].GetComponent<LensFlareComponentSRP>();

            }

        }

        if (redLights_Lensflares.Length > 0) {

            for (int i = 0; i < redLights_Lensflares.Length; i++) {

                if (redLights_Lensflares[i] != null)
                    AdjustLensFlare(redLights_Lensflares[i], redLights[i].intensity);

            }

        }

        if (blueLights_Lensflares.Length > 0) {

            for (int i = 0; i < blueLights_Lensflares.Length; i++) {

                if (blueLights_Lensflares[i] != null)
                    AdjustLensFlare(blueLights_Lensflares[i], blueLights[i].intensity);

            }

        }

    }

    /// <summary>
    /// Operating SRP lensflares related to camera angle.
    /// </summary>
    private void AdjustLensFlare(LensFlareComponentSRP targetLensFlare, float intensity) {

        //  If no main camera found, return.
        if (!mainCam)
            return;

        //  Lensflares are not affected by collider of the vehicle. They will ignore it. Below code will calculate the angle of the light-camera, and sets intensity of the lensflare.
        float distanceTocam = Vector3.Distance(transform.position, mainCam.transform.position);
        float finalFlareBrightness;

        finalFlareBrightness = flareBrightness * (8f / distanceTocam) / 3f;

        if (finalFlareBrightness < 0)
            finalFlareBrightness = 0f;

        targetLensFlare.attenuationByLightShape = false;
        targetLensFlare.intensity = finalFlareBrightness * intensity;

    }

    /// <summary>
    /// Sets the siren mode to on or off.
    /// </summary>
    /// <param name="state"></param>
    public void SetSiren(bool state) {

        if (state)
            sirenMode = SirenMode.On;
        else
            sirenMode = SirenMode.Off;

    }

}
