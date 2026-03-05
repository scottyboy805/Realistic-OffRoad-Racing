using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class MinimapMarker : MonoBehaviour
    {
        [SerializeField] string blipName;

        void Start()
        {
            MinimapManager.Instance?.CreateBlip(blipName, transform);
        }

        void OnDestroy()
        {
            MinimapManager.Instance?.RemoveBlip(transform);
        }
    }
}