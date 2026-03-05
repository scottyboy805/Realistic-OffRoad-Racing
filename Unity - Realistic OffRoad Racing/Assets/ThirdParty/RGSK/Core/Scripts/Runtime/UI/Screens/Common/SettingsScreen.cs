using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class SettingsScreen : UIScreen
    {
        [SerializeField] TabButton profileButton;

        public override void Open()
        {
            base.Open();

            //hide the profile tab when in race environments
            if (profileButton != null)
            {
                profileButton.gameObject.SetActive(!RaceManager.Instance.Initialized);
            }
        }

        public override void Close()
        {
            base.Close();
            SaveManager.Instance?.Save();
        }
    }
}