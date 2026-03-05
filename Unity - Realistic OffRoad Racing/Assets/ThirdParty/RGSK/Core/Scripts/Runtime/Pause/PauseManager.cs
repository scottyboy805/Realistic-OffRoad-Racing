using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class PauseManager : Singleton<PauseManager>
    {
        public static bool IsPaused { get; private set; }
        UIScreenID pauseScreen => RGSKCore.Instance.UISettings.screens.PauseScreen;

        void OnEnable()
        {
            RGSKEvents.OnBeforeSceneLoad.AddListener(OnBeforeSceneLoad);
            InputManager.PauseEvent += TogglePause;
        }

        void OnDisable()
        {
            RGSKEvents.OnBeforeSceneLoad.RemoveListener(OnBeforeSceneLoad);
            InputManager.PauseEvent -= TogglePause;
        }

        public override void Awake()
        {
            base.Awake();
            IsPaused = false;
        }

        public void TogglePause()
        {
            if (!IsPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }

        public void Pause()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            AdjustTimeScale();
            UIManager.Instance?.OpenScreen(pauseScreen);
            RGSKEvents.OnGamePaused.Invoke();
        }

        public void Unpause()
        {
            if (!IsPaused)
                return;

            IsPaused = false;
            AdjustTimeScale();
            UIManager.Instance?.CloseScreen(pauseScreen);
            RGSKEvents.OnGameUnpaused.Invoke();
        }

        void OnBeforeSceneLoad()
        {
            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1;
            }
        }

        void AdjustTimeScale()
        {
            Time.timeScale = IsPaused ? 0 : 1;
            AudioListener.pause = IsPaused ? true : false;
        }
    }
}