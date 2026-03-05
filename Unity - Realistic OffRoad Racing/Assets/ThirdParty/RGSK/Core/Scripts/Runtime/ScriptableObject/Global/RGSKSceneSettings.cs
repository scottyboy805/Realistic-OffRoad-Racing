using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/Scene")]
    public class RGSKSceneSettings : ScriptableObject
    {
        public ThreadPriority backgroundLoadingPriority = ThreadPriority.High;
        public float loadCompleteDelay = 3;

        [Header("Main Menu")]
        public SceneReference mainMenuScene;

        [Header("Loading")]
        public SceneReference loadingScene;
    }
}