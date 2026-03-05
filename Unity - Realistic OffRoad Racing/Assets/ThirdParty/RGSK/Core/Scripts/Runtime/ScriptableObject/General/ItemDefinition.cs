using UnityEngine;
using RGSK.Helpers;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Item Definition")]
    public class ItemDefinition : UniqueIDScriptableObject
    {
        public string objectName;
        [TextArea(10, 10)]
        public string description;
        public Sprite icon;
        public Sprite previewPhoto;

        public ItemUnlockMode unlockMode = ItemUnlockMode.None;
        public int unlockPrice;
        public int unlockXPLevel;
        [TextArea(5, 5)]
        public string unlockCondition;

        public ItemOwnedMode ownedMode = ItemOwnedMode.None;
        public int ownedPrice;

        [ContextMenu("Unlock Item")]
        public void Unlock()
        {
            if (!SaveData.Instance.unlockedItems.Contains(ID))
            {
                SaveData.Instance.unlockedItems.Add(ID);
            }
        }

        [ContextMenu("Lock Item")]
        public void Lock()
        {
            if (SaveData.Instance.unlockedItems.Contains(ID))
            {
                SaveData.Instance.unlockedItems.Remove(ID);
            }
        }

        [ContextMenu("Purchase Item")]
        public void Purchase()
        {
            if (!SaveData.Instance.ownedItems.Contains(ID))
            {
                SaveData.Instance.ownedItems.Add(ID);
            }
        }

        [ContextMenu("Sell Item")]
        public void Sell()
        {
            if (SaveData.Instance.ownedItems.Contains(ID))
            {
                SaveData.Instance.ownedItems.Remove(ID);
            }
        }

        public bool IsUnlocked()
        {
            switch (unlockMode)
            {
                case ItemUnlockMode.ByDefault:
                    {
                        Unlock();
                        break;
                    }

                case ItemUnlockMode.XPLevel:
                    {
                        if (GeneralHelper.GetLevelFromXP(SaveData.Instance.playerData.xp) >= unlockXPLevel)
                        {
                            Unlock();
                        }
                        break;
                    }
            }

            return SaveData.Instance.unlockedItems.Contains(ID);
        }

        public bool IsOwned()
        {
            switch (ownedMode)
            {
                case ItemOwnedMode.ByDefault:
                    {
                        Purchase();
                        break;
                    }
            }

            return SaveData.Instance.ownedItems.Contains(ID);
        }
    }
}