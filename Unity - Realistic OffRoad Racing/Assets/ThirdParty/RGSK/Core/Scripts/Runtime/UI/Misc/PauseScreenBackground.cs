using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class PauseScreenBackground : MonoBehaviour
    {
        [SerializeField] Canvas canvas;

        void Update() => Toggle(PauseManager.IsPaused);
        
        void Toggle(bool active) 
        {
            if(canvas == null)
                return;

            if(canvas.enabled != active)
            {
                canvas.enabled = active;
            }
        }
    }
}