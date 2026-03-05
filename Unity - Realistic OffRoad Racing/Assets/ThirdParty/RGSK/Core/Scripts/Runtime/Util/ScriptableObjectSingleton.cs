using System.Linq;
using UnityEngine;

namespace RGSK
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.LoadAll<T>("").FirstOrDefault();
                }

                return _instance;
            }
        }
    }
}