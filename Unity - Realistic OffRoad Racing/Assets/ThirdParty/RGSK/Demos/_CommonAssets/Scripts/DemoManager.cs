using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class DemoManager : Singleton<DemoManager>
    {
        [System.Serializable]
        public class RGSKSettingsDefinition
        {
            public RGSKGeneralSettings generalSettings;
            public RGSKSceneSettings sceneSettings;
            public RGSKContentSettings contentSettings;
            public RGSKRaceSettings raceSettings;
            public RGSKAISettings aiSettings;
            public RGSKUISettings uiSettings;
            public RGSKInputSettings inputSettings;
            public RGSKVehicleSettings vehicleSettings;
        }

        [System.Serializable]
        public class DemoDefinition
        {
            public string name;
            public string id;
            public Sprite preview;
            public bool isShowcase;
            public RGSKSettingsDefinition settings;
            public SceneReference scene;
        }

        public List<DemoDefinition> demos = new List<DemoDefinition>();

        RGSKSettingsDefinition _cacheSettings;

        void Start()
        {
            _cacheSettings = new RGSKSettingsDefinition
            {
                generalSettings = RGSKCore.Instance.GeneralSettings,
                sceneSettings = RGSKCore.Instance.SceneSettings,
                contentSettings = RGSKCore.Instance.ContentSettings,
                raceSettings = RGSKCore.Instance.RaceSettings,
                aiSettings = RGSKCore.Instance.AISettings,
                uiSettings = RGSKCore.Instance.UISettings,
                inputSettings = RGSKCore.Instance.InputSettings,
                vehicleSettings = RGSKCore.Instance.VehicleSettings
            };

            gameObject.AddComponent<DontDestroyOnLoad>();
        }

        public void LoadDemo(string id)
        {
            var demo = demos.FirstOrDefault(x => x.id == id);

            if (demo != null)
            {
                if (demo.scene != null)
                {
                    UpdateSettings(demo.settings, "Some settings were modified in RGSK Core!");

                    RGSKCore.runtimeData.SelectedVehicle = null;
                    RGSKCore.runtimeData.SelectedTrack = null;
                    RGSKCore.runtimeData.SelectedSession = null;

                    SceneLoadManager.LoadScene(demo.scene.ScenePath);
                }
            }
            else
            {
                Logger.Log(this, $"Could not find a demo with the ID '{id}'!");
            }
        }

        void UpdateSettings(RGSKSettingsDefinition settings, string log = "")
        {
            if (settings == null)
                return;

            RGSKCore.Instance.GeneralSettings = settings.generalSettings;
            RGSKCore.Instance.SceneSettings = settings.sceneSettings;
            RGSKCore.Instance.ContentSettings = settings.contentSettings;
            RGSKCore.Instance.RaceSettings = settings.raceSettings;
            RGSKCore.Instance.AISettings = settings.aiSettings;
            RGSKCore.Instance.UISettings = settings.uiSettings;
            RGSKCore.Instance.InputSettings = settings.inputSettings;
            RGSKCore.Instance.VehicleSettings = settings.vehicleSettings;

            if (!string.IsNullOrWhiteSpace(log))
            {
                Logger.Log(log);
            }
        }

        void OnDestroy()
        {
            if (Application.isEditor)
            {
                UpdateSettings(_cacheSettings, "All modified settings in RGSK Core were reverted!");
            }
        }
    }
}