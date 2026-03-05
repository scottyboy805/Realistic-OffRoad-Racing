using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class SetTargetFPS : MonoBehaviour
    {
        [Tooltip("-1 = No FPS cap.")]
        [SerializeField] int target = -1;

        void Awake()
        {
            Application.targetFrameRate = target;
        }
    }
}