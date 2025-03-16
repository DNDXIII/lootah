using System;
using System.Collections.Generic;

namespace InventoryPart3
{
    [Serializable]
    public class SaveData
    {
        public List<WeaponItem> inventory = new();
        public WeaponItem[] equippedItems = new WeaponItem[2];
        public PlayerData playerData;
    }
}