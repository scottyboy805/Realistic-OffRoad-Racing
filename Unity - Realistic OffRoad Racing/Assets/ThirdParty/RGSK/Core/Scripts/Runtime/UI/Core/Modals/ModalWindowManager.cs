using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RGSK.Helpers;

namespace RGSK
{
    public class ModalWindowManager : Singleton<ModalWindowManager>
    {
        List<ModalWindow> _modalWindowPrefabs => RGSKCore.Instance.UISettings.modalWindowPrefabs;
        ModalWindow _instance;

        void OnEnable()
        {
            InputManager.MenuBackEvent += Close;
        }

        void OnDisable()
        {
            InputManager.MenuBackEvent -= Close;
        }

        public void Show(ModalWindowProperties properties)
        {
            if (_modalWindowPrefabs.Count == 0 || properties.prefabIndex < 0 || properties.prefabIndex >= _modalWindowPrefabs.Count)
            {
                Logger.LogWarning($"The modal window could not be opened! Please add a modal window prefab for element [{properties.prefabIndex}] under RGSK Menu > UI tab!");
                return;
            }

            _instance = Instantiate(_modalWindowPrefabs[properties.prefabIndex], GeneralHelper.GetDynamicParent());
            _instance.Show(properties);
        }

        public void Close()
        {
            _instance?.Close();
            _instance = null;
        }

        public bool IsOpen() => _instance != null;
    }
}