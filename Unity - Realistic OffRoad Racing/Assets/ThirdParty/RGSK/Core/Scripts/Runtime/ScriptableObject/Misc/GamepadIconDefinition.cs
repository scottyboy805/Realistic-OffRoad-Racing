using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    [System.Serializable]
    public class ControlPathIconComposite
    {
        public string path;
        public Sprite icon;
    }

    [System.Serializable]
    public class GamepadIcons
    {
        [SerializeField] string name;
        public InputController type;
        public List<ControlPathIconComposite> icons = new List<ControlPathIconComposite>();

        public Sprite GetSprite(string controlPath)
        {
            if (string.IsNullOrWhiteSpace(controlPath))
                return null;

            var icon = icons.FirstOrDefault(x => x.path == controlPath.ToLower().Replace(" ", ""));
            
            if(icon != null)
            {
                return icon.icon;
            }

            return null;
        }
    }

    [CreateAssetMenu(menuName = "RGSK/Misc/Gamepad Icons")]
    public class GamepadIconDefinition : ScriptableObject
    {
        [SerializeField] List<GamepadIcons> gamepadIcons = new List<GamepadIcons>();

        public GamepadIcons GetIconSet(InputController controller) => gamepadIcons.FirstOrDefault(x => x.type == controller);
    }
}