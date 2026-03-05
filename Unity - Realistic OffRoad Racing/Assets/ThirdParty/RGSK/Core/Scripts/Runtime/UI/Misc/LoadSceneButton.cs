using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] SceneReference scene;

        [Header("Modal")]
        [SerializeField] bool showModalWindow;
        [SerializeField] ModalWindowProperties modalProperties;

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                if (showModalWindow)
                {
                    btn.onClick.AddListener(() =>
                    {
                        ModalWindowManager.Instance.Show(new ModalWindowProperties
                        {
                            header = modalProperties.header,
                            message = modalProperties.message,
                            confirmButtonText = modalProperties.confirmButtonText,
                            declineButtonText = modalProperties.declineButtonText,
                            confirmAction = () => Load(),
                            declineAction = () => { },
                            startSelection = modalProperties.startSelection,
                            prefabIndex = modalProperties.prefabIndex
                        });
                    });
                }
                else
                {
                    btn.onClick.AddListener(() => Load());
                }
            }
        }

        void Load() => SceneLoadManager.LoadScene(scene);
    }
}