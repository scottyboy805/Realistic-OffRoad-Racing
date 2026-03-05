using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class DemoLoaderButton : MonoBehaviour
    {
        public string id;

        void Start()
        {
            if (TryGetComponent<Button>(out var btn))
            {
                btn.onClick.AddListener(() => DemoManager.Instance?.LoadDemo(id));
            }
        }
    }
}
