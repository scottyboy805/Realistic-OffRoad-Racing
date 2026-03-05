using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RGSK
{
    public class GameObjectToggler : MonoBehaviour
    {
        [SerializeField] GameObject target;
        [SerializeField] bool deactivateOnStart = true;

        void Start()
        {
            if(deactivateOnStart)
            {
                target.SetActive(false);
            }
        }
        
        public void Toggle()
        {
            target.SetActive(!target.activeSelf);
        }
    }
}