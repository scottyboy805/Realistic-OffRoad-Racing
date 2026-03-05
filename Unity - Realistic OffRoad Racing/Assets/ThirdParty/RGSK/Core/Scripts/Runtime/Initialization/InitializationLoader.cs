using UnityEngine;

namespace RGSK
{
    public static class InitializationLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Execute()
        {
            if(RGSKCore.Instance.persistentManagers != null)
            {
                CreatePersistentObject(RGSKCore.Instance.persistentManagers);
            }
            else
            {
                Logger.LogWarning("Persistent Managers not found! Please re-import the asset or reach out for support!");
            }

            foreach (var obj in RGSKCore.Instance.GeneralSettings.persistentObjects)
            {
                if (obj != null)
                {
                    if(obj == RGSKCore.Instance.persistentManagers)
                        continue;
                    
                    CreatePersistentObject(obj);
                }
            }

            if (RGSKCore.Instance.GeneralSettings.terminal != null)
            {
                var useTerminal = Application.isEditor ||
                                (!Application.isEditor && RGSKCore.Instance.GeneralSettings.includeTerminalInBuild);

                if (useTerminal)
                {
                    CreatePersistentObject(RGSKCore.Instance.GeneralSettings.terminal);
                }
            }
        }

        static void CreatePersistentObject(GameObject obj)
        {
            var o = Object.Instantiate(obj);
            o.name = o.name.Insert(0, "[RGSK Persistent] ");
            Object.DontDestroyOnLoad(o);
        }
    }
}