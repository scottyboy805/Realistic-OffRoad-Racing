using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class SharedButton : MonoBehaviour
    {
        [SerializeField] ButtonType type;
        [SerializeField] Button button;
        [SerializeField] bool showModalWindow = true;

        void Start()
        {
            switch (type)
            {
                case ButtonType.Restart:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.restartModal.header,
                                    message = RGSKCore.Instance.UISettings.restartModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.restartModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.restartModal.declineButtonText,
                                    confirmAction = Restart,
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.restartModal.startSelection,
                                    prefabIndex = RGSKCore.Instance.UISettings.restartModal.prefabIndex
                                });
                            }
                            else
                            {
                                Restart();
                            }
                        });

                        break;
                    }

                case ButtonType.BackToMenu:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.exitModal.header,
                                    message = RGSKCore.Instance.UISettings.exitModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.exitModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.exitModal.declineButtonText,
                                    confirmAction = () => SceneLoadManager.LoadMainScene(),
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.exitModal.startSelection,
                                    prefabIndex = RGSKCore.Instance.UISettings.exitModal.prefabIndex
                                });
                            }
                            else
                            {
                                SceneLoadManager.LoadMainScene();
                            }
                        });

                        break;
                    }

                case ButtonType.QuitApplication:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (showModalWindow)
                            {
                                ModalWindowManager.Instance.Show(new ModalWindowProperties
                                {
                                    header = RGSKCore.Instance.UISettings.quitModal.header,
                                    message = RGSKCore.Instance.UISettings.quitModal.message,
                                    confirmButtonText = RGSKCore.Instance.UISettings.quitModal.confirmButtonText,
                                    declineButtonText = RGSKCore.Instance.UISettings.quitModal.declineButtonText,
                                    confirmAction = () => SceneLoadManager.QuitApplication(),
                                    declineAction = () => { },
                                    startSelection = RGSKCore.Instance.UISettings.quitModal.startSelection,
                                    prefabIndex = RGSKCore.Instance.UISettings.quitModal.prefabIndex
                                });
                            }
                            else
                            {
                                SceneLoadManager.QuitApplication();
                            }
                        });

                        break;
                    }

                case ButtonType.WatchReplay:
                    {
                        button?.onClick.AddListener(() =>
                        {
                            if (RaceManager.Instance.Initialized &&
                                RaceManager.Instance.CurrentState != RaceState.PostRace)
                            {
                                return;
                            }

                            RecorderManager.Instance?.ReplayRecorder?.StartPlayback();
                        });
                        break;
                    }

                case ButtonType.DeleteSaveData:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = RGSKCore.Instance.UISettings.deleteSaveModal.header,
                            message = RGSKCore.Instance.UISettings.deleteSaveModal.message,
                            confirmButtonText = RGSKCore.Instance.UISettings.deleteSaveModal.confirmButtonText,
                            declineButtonText = RGSKCore.Instance.UISettings.deleteSaveModal.declineButtonText,
                            confirmAction = () => SaveManager.Instance?.DeleteSaveFile(),
                            declineAction = () => { },
                            startSelection = RGSKCore.Instance.UISettings.deleteSaveModal.startSelection,
                            prefabIndex = RGSKCore.Instance.UISettings.deleteSaveModal.prefabIndex
                        }));
                        break;
                    }

                case ButtonType.EndRaceSession:
                    {
                        button?.onClick.AddListener(() => ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = RGSKCore.Instance.UISettings.endRaceSessionModal.header,
                            message = RGSKCore.Instance.UISettings.endRaceSessionModal.message,
                            confirmButtonText = RGSKCore.Instance.UISettings.endRaceSessionModal.confirmButtonText,
                            declineButtonText = RGSKCore.Instance.UISettings.endRaceSessionModal.declineButtonText,
                            confirmAction = () =>
                            {
                                PauseManager.Instance?.Unpause();
                                RaceManager.Instance?.ForceFinishRace(false);
                            },
                            declineAction = () => { },
                            startSelection = RGSKCore.Instance.UISettings.endRaceSessionModal.startSelection,
                            prefabIndex = RGSKCore.Instance.UISettings.endRaceSessionModal.prefabIndex
                        }));
                        break;
                    } 
            }
        }

        void Restart()
        {
            if (RaceManager.Instance.Initialized)
            {
                PauseManager.Instance.Unpause();
                RaceManager.Instance.RestartRace();
                return;
            }

            SceneLoadManager.ReloadScene();
        }
    }
}