using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    //[CreateAssetMenu(menuName = "RGSK/SO URL")]
    public class ScriptableObjectURL : ScriptableObject
    {
        public string helpText;
        public string buttonText = "Open";
        public string url;
    }
}