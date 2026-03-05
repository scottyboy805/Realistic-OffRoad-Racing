using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class GameSettingsGraphicsEntryUI : GameSettingsEntryUI
    {
        public enum GraphicSettingType
        {
            Quality,
            Resolution,
            Fullscreen,
            VSync,
            FPSLimit,
        }

        [SerializeField] GraphicSettingType type;

        [Tooltip("The selectable FPS limit options. Only applies for the 'FPS Limit' type!")]
        [SerializeField] int[] fpsOptions = new int[]
        {
            -1,
            30,
            60,
            120
        };

        List<Resolution> _resolutionsList;
        Resolution _currentResolution;
        int _currentResolutionIndex;
        bool _isFullscreen;

        string[] vsyncOptions = new string[]
        {
            "Off",
            "On",
        };

        protected override void Start()
        {
            _currentResolution = Screen.currentResolution;
            _resolutionsList = GeneralHelper.GetResolutionsList();
            _currentResolutionIndex = GeneralHelper.GetCurrentResolutionIndex();
            _isFullscreen = Screen.fullScreen;

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(type.ToString());
            }

            base.Start();
        }

        public override void SelectOption(int direction)
        {
            switch (type)
            {
                case GraphicSettingType.Quality:
                    {
                        SetQuality(direction);
                        break;
                    }

                case GraphicSettingType.Resolution:
                    {
                        SetResolution(direction);
                        break;
                    }

                case GraphicSettingType.Fullscreen:
                    {
                        SetFullscreen(direction);
                        break;
                    }

                case GraphicSettingType.VSync:
                    {
                        SetVSync(direction);
                        break;
                    }

                case GraphicSettingType.FPSLimit:
                    {
                        SetFPSLimit(direction);
                        break;
                    }
            }
        }

        void SetQuality(int direction)
        {
            var count = QualitySettings.names.Length;
            var index = QualitySettings.GetQualityLevel();
            index = GeneralHelper.ValidateIndex(index + direction, 0, count, loop);
            QualitySettings.SetQualityLevel(index);
            SaveData.Instance.gameSettingsData.qualityIndex = index;
            valueText?.SetText(QualitySettings.names[index]);
        }

        void SetResolution(int direction)
        {
            //Deactivate on mobile and WebGL platforms
            if (GeneralHelper.IsMobilePlatform() || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            var count = _resolutionsList.Count;
            _currentResolutionIndex = GeneralHelper.ValidateIndex(_currentResolutionIndex + direction, 0, count, loop);
            _currentResolution = _resolutionsList[_currentResolutionIndex];
            Screen.SetResolution(_currentResolution.width, _currentResolution.height, _isFullscreen);
            SaveData.Instance.gameSettingsData.resolutionIndex = _currentResolutionIndex;

            var text = _resolutionsList[_currentResolutionIndex].ToString();
            valueText?.SetText(text.Substring(0, text.IndexOf("@")));
        }

        void SetFullscreen(int direction)
        {
            //Deactivate on mobile and WebGL platforms
            if (GeneralHelper.IsMobilePlatform() || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            var index = _isFullscreen ? 0 : 1;
            index = GeneralHelper.ValidateIndex(index + direction, 0, toggleOptions.Length, loop);
            _isFullscreen = index == 0;
            Screen.fullScreen = _isFullscreen;
            SaveData.Instance.gameSettingsData.fullscreen = _isFullscreen;
            valueText?.SetText(toggleOptions[index]);
        }

        void SetVSync(int direction)
        {
            //Deactivate on mobile platforms
            if (GeneralHelper.IsMobilePlatform())
            {
                gameObject.SetActive(false);
                return;
            }

            var index = QualitySettings.vSyncCount;
            index = GeneralHelper.ValidateIndex(index + direction, 0, vsyncOptions.Length, loop);
            QualitySettings.vSyncCount = index;
            SaveData.Instance.gameSettingsData.vSyncCount = index;
            valueText?.SetText(vsyncOptions[index]);
        }

        void SetFPSLimit(int direction)
        {
            var index = fpsOptions.ToList().IndexOf(Application.targetFrameRate);

            if (index == -1)
            {
                index = 0;
            }

            index = GeneralHelper.ValidateIndex(index + direction, 0, fpsOptions.Length, loop);
            Application.targetFrameRate = fpsOptions[index];
            SaveData.Instance.gameSettingsData.targetFPS = fpsOptions[index];

            switch (fpsOptions[index])
            {
                case -1:
                    {
                        valueText?.SetText("Default");
                        break;
                    }

                default:
                    {
                        valueText?.SetText(fpsOptions[index].ToString());
                        break;
                    }
            }
        }
    }
}