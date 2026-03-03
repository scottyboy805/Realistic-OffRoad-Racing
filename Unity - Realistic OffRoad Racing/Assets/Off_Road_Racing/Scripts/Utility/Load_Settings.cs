//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________
using ALIyerEdon;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace ALIyerEdon
{
    public class Load_Settings : MonoBehaviour
    {
        public bool nightMode;

        public AudioSource music;
        public AudioSource[] trumpetSound;

        public Color gamma;
        public Color debugGamma;

        public bool debugColor;

        void Update()
        {
            if (debugColor)
            {
                Volume volume = FindFirstObjectByType<Volume>();

                LiftGammaGain colorGrading;
                volume.profile.TryGet<LiftGammaGain>(out colorGrading);

                colorGrading.gamma.value = debugGamma;
            }
        }

        void Start()
        {
            foreach (EasyCarController controller in FindObjectsOfType<EasyCarController>())
            {
                controller.Toggle_FrontLights(nightMode);

                if (nightMode)
                    controller.nightLight = true;
                else
                    controller.nightLight = false;
            }

            // Update_SSGI();
            Update_SSR();
            Update_MotionBlur();
            Update_DOF();
            Update_AO();
            Update_MusicVolume();
            Update_LocalPosition_UI();

            if(!debugColor)
               Update_CinematicColor();
        }

        public void Update_MusicVolume()
        {
            if(PlayerPrefs.GetString("MusicVolume") == "Low")
                music.volume = 0.1f;
            if (PlayerPrefs.GetString("MusicVolume") == "Medium")
                music.volume = 0.3f;
            if (PlayerPrefs.GetString("MusicVolume") == "High")
                music.volume = 0.5f;

            if (trumpetSound.Length > 0)
            {
                foreach (AudioSource audio in trumpetSound)
                {
                    if (PlayerPrefs.GetString("MusicVolume") == "Low")
                        audio.volume = 0.1f;
                    if (PlayerPrefs.GetString("MusicVolume") == "Medium")
                        audio.volume = 0.3f;
                    if (PlayerPrefs.GetString("MusicVolume") == "High")
                        audio.volume = 0.5f;
                }
            }
        }
        public void Update_CarSFX()
        {
            foreach(EasyCarAudio carAudio in FindObjectsOfType<EasyCarAudio>())
                    carAudio.Update_VolumeSettings();
        }

        public void Update_AO()
        {
            Volume volume = FindFirstObjectByType<Volume>();

            ScreenSpaceAmbientOcclusion AO;
            volume.profile.TryGet<ScreenSpaceAmbientOcclusion>(out AO);

            if (PlayerPrefs.GetString("AO") == "On")
                AO.active = true;
            else
                AO.active = false;
        }

        public void Update_DOF()
        {
            Volume volume = FindFirstObjectByType<Volume>();

            DepthOfField dof;
            volume.profile.TryGet<DepthOfField>(out dof);

            if (PlayerPrefs.GetString("DOF") == "On")
                dof.active = true;
            else
                dof.active = false;
        }

        public void Update_MotionBlur()
        {
            Volume volume = FindFirstObjectByType<Volume>();

            MotionBlur mb;
            volume.profile.TryGet<MotionBlur>(out mb);

            if (PlayerPrefs.GetString("MotionBlur") == "On")
                mb.active = true;
            else
                mb.active = false;
        }

        public void Update_CinematicColor()
        {
            /*Volume volume = FindFirstObjectByType<Volume>();

            LiftGammaGain colorGrading;
            volume.profile.TryGet<LiftGammaGain>(out colorGrading);

            // gamma.gamma.value = cinematicColor;
            if (PlayerPrefs.GetString("CinematicColor") == "On")
                colorGrading.gamma.value = gamma;
            else
                colorGrading.gamma.value = new Vector4(0, 0, 0, 0);*/

            Volume volume = FindFirstObjectByType<Volume>();

            ScreenSpaceLensFlare lensFlare;
            volume.profile.TryGet<ScreenSpaceLensFlare>(out lensFlare);

            // gamma.gamma.value = cinematicColor;
            if (PlayerPrefs.GetString("CinematicColor") == "On")
                lensFlare.active = true;
            else
                lensFlare.active = false;
        }

        public void Update_SSR()
        {
            Volume volume = FindFirstObjectByType<Volume>();

            ScreenSpaceReflection ssr;
            volume.profile.TryGet<ScreenSpaceReflection>(out ssr);

            if (PlayerPrefs.GetString("SSR") == "On")
                ssr.active = true;
            else
                ssr.active = false;
        }

        public void Update_SSGI()
        {
            Volume volume = FindFirstObjectByType<Volume>();

           /* GlobalIllumination gi;
            volume.profile.TryGet<GlobalIllumination>(out gi);

            if (PlayerPrefs.GetString("SSGI") == "On")
                gi.active = true;
            else
                gi.active = false;*/
        }

        public void Update_DisplaFPS()
        {
            if (FindFirstObjectByType<FPSCounter>())
                FindFirstObjectByType<FPSCounter>().Update_DisplayFPS_UI();
        }
        public void Update_SideUI()
        {
            if (FindFirstObjectByType<Race_Manager>())
                FindFirstObjectByType<Race_Manager>().Update_SideUI();
        }
        public void Update_DynamicCamera()
        {
            if (GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>())
            {
                GameObject.FindGameObjectWithTag("Player")
                    .GetComponent<EasyCarController>().Update_DynamicCamera();
            }
        }

        public void Update_LocalPosition_UI()
        {

            if (FindFirstObjectByType<Race_Manager>())
            {
                if (PlayerPrefs.GetString("Local_Position") == "On")
                    FindFirstObjectByType<Race_Manager>().showLocalPosition = true;
                else
                    FindFirstObjectByType<Race_Manager>().showLocalPosition = false;

                if (PlayerPrefs.GetString("Side_UI") == "On")
                    FindFirstObjectByType<Race_Manager>().positionUI.SetActive(true);
                else
                    FindFirstObjectByType<Race_Manager>().positionUI.SetActive(false);
            }

            // Enable local position display on top of the cars
            foreach (Car_Position carPos in FindObjectsOfType<Car_Position>())
            {
                // Show or hide car position on the top of the car
                carPos.GetComponent<Car_Position>().displayPosition =
                    FindFirstObjectByType<Race_Manager>().showLocalPosition;
            }
        }
    }
}