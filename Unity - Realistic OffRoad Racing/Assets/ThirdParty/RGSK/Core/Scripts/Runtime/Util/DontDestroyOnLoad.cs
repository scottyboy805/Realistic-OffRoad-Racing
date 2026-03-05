using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            if (transform.parent != null)
            {
                transform.SetParent(null);
                Debug.LogWarning($"The parent of '{gameObject.name}' was set to null to ensure DontDestroyOnLoad works.", gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}