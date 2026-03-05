using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class SceneUnloadHandler : MonoBehaviour
    {
        void OnEnable()
        {
            RGSKEvents.OnBeforeSceneLoad.AddListener(OnBeforeSceneLoad);
            RGSKEvents.OnAfterSceneLoad.AddListener(OnAfterSceneLoad);
        }

        void OnDisable()
        {
            RGSKEvents.OnBeforeSceneLoad.RemoveListener(OnBeforeSceneLoad);
            RGSKEvents.OnAfterSceneLoad.RemoveListener(OnAfterSceneLoad);
        }

        void OnBeforeSceneLoad()
        {
            InputManager.Instance?.SetInputMode(InputMode.Disabled);
            RaceManager.Instance?.DeInitializeRace();
            RecorderManager.Instance?.Clear();
            MusicManager.Instance?.Stop();
            WheelSurfaceManager.Instance?.ClearTyremarks();
        }

        void OnAfterSceneLoad()
        {
            AudioListener.pause = false;
        }
    }
}