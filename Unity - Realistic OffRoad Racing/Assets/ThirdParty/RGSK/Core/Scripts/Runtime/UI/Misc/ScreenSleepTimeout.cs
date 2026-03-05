using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    public class ScreenSleepTimeout : MonoBehaviour 
    {
        [SerializeField] bool neverSleep = true;

        void Awake() 
        {
            if (neverSleep && GeneralHelper.IsMobilePlatform())
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }
    }
}