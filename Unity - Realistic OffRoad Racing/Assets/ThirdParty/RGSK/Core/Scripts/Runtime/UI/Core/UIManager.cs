using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class UIManager : Singleton<UIManager>
    {
        public UIScreen ActiveScreen { get; private set; }
        Dictionary<UIScreenID, UIScreen> _screenList = new Dictionary<UIScreenID, UIScreen>();

        void OnEnable()
        {
            InputManager.MenuBackEvent += OpenPreviousScreen;
        }

        void OnDisable()
        {
            InputManager.MenuBackEvent -= OpenPreviousScreen;
        }

        public void RegisterScreen(UIScreen screen)
        {
            if (screen.screenID == null)
            {
                Logger.LogError($"The screen {screen} has no ID!");
                return;
            }

            if (!_screenList.ContainsKey(screen.screenID))
            {
                _screenList.Add(screen.screenID, screen);
                screen.gameObject.SetActive(true);
                screen.Initialize();
                CloseScreen(screen.screenID);
            }
        }

        public void UnregisterScreen(UIScreen screen)
        {
            if (_screenList.ContainsKey(screen.screenID))
            {
                _screenList.Remove(screen.screenID);
            }
        }

        public void OpenScreen(UIScreenID id, bool addToHistory = true)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to open is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                if (screen.IsOpen())
                    return;

                CloseAllScreens();
                screen.Open();

                if (ActiveScreen != null && addToHistory)
                {
                    screen.PreviousScreen = ActiveScreen;
                }

                ActiveScreen = screen;
            }
            else
            {
                CreateScreen(id, true, addToHistory);
            }
        }

        public void CloseScreen(UIScreenID id)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to close is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                if (screen.IsOpen())
                {
                    screen.Close();
                }
            }
        }

        public void CreateScreen(UIScreenID id, bool open = false, bool addToHistory = true)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to create is null!");
                return;
            }

            if (_screenList.ContainsKey(id))
                return;

            if (id.Prefab == null)
            {
                Logger.LogWarning($"The UI screen with ID \"{id}\"could not be created! Please ensure it's prefab is assigned or add it to the scene.");
                return;
            }

            if (id.Prefab.screenID != id)
            {
                Logger.LogWarning($"The ID \"{id}\" does not macth the one in the \"{id.Prefab.name}\" prefab! Please select \"{id}\" in the project tab and use the 'Fix' button(s) to resolve this issue.");
                return;
            }

            var newScreen = Instantiate(id.Prefab);

            if (id.isPersistentScreen)
            {
                newScreen.gameObject.AddComponent<DontDestroyOnLoad>();
            }
            else
            {
                newScreen.transform.SetParent(GeneralHelper.GetDynamicParent());
            }

            RegisterScreen(newScreen);

            if (open)
            {
                OpenScreen(id, addToHistory);
            }
        }

        public void DestroyScreen(UIScreenID id)
        {
            if (id == null)
            {
                Logger.LogWarning("The screen you are trying to destroy is null!");
                return;
            }

            if (_screenList.TryGetValue(id, out var screen))
            {
                Destroy(screen.gameObject);
            }
        }

        public void OpenPreviousScreen()
        {
            if (ModalWindowManager.Instance != null && ModalWindowManager.Instance.IsOpen())
                return;

            ActiveScreen?.Back();
        }

        public void CloseAllScreens()
        {
            foreach (var screen in _screenList.Keys)
            {
                CloseScreen(screen);
            }
        }

        public void ClearScreenHistory()
        {
            foreach (var screen in _screenList.Values)
            {
                screen.PreviousScreen = null;
            }

            ActiveScreen = null;
        }

        public bool HasScreenHistory() => ActiveScreen != null && ActiveScreen.PreviousScreen != null;
    }
}