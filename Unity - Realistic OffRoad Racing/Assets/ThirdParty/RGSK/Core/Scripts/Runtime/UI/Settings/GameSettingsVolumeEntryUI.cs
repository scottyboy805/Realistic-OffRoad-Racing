using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;

namespace RGSK
{
    public class GameSettingsVolumeEntryUI : GameSettingsEntryUI
    {
        [SerializeField] AudioGroup audioGroup;
        [SerializeField] Slider slider;

        protected override void Start()
        {
            base.Start();

            if (slider != null)
            {
                slider.minValue = 0;
                slider.maxValue = 1;

                slider.value = SaveData.Instance.gameSettingsData.volumes[audioGroup];

                slider.onValueChanged.AddListener((volume) =>
                {
                    var mixer = AudioManager.Instance.Mixer;
                    mixer?.SetFloat(AudioHelper.GetMixerParameterName(audioGroup), ConversionHelper.LinearToDecibel(volume));
                    SaveData.Instance.gameSettingsData.volumes[audioGroup] = volume;
                });
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                titleText?.SetText(audioGroup.ToString());
            }
        }

        public override void SelectOption(int direction) => slider.value += 0.1f * direction;
    }
}