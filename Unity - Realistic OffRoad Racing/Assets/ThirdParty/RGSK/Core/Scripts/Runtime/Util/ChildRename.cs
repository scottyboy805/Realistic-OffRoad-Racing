using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class ChildRename : MonoBehaviour
    {
        public string prefix;
        public bool includeSiblingIndex = true;

        public void Rename()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                string name = prefix;

                if (includeSiblingIndex) name += (i + 1).ToString(); ;

                transform.GetChild(i).name = name;
            }
        }
    }
}