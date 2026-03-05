using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RaceStartSignal : MonoBehaviour
    {
        public void Trigger()
        {
            RaceManager.Instance?.StartRace();
        }
    }
}