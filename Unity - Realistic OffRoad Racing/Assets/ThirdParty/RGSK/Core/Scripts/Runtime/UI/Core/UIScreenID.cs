using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/UI/UI Screen ID")]
    public class UIScreenID : ScriptableObject
    {
        public UIScreen screenPrefab;
        public UIScreen screenPrefabMobile;
        public InputMode onOpenInputMode = InputMode.UI;
        public bool isPersistentScreen;

        public UIScreen Prefab
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                if(screenPrefabMobile != null)
                {
                    return screenPrefabMobile;
                }
#endif

                return screenPrefab;
            }
        }

        public void Open()
        {
            UIManager.Instance?.OpenScreen(this);
        }

        public void Close()
        {
            UIManager.Instance?.CloseScreen(this);
        }
    }
}