using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Helpers;

namespace RGSK
{
    public class FreemodeButton : MonoBehaviour
    {
        [SerializeField] SceneReference freemodeScene;

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(GeneralHelper.OpenVehicleSelectScreenWithCallback(() =>
                    {
                        Load();
                    }));
                });
            }
        }

        void Load() => SceneLoadManager.LoadScene(freemodeScene);
    }
}