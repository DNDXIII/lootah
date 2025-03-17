using System;
using System.Collections.Generic;

namespace InventoryPart3
{
    [Serializable]
    public class SaveData
    {
        public List<WeaponItem> inventory = new();

        // These can't be null so we have to initialize them with this fake weapon
        public WeaponItem[] equippedItems = { WeaponItem.FakeWeapon, WeaponItem.FakeWeapon };
        public PlayerData playerData;
    }
}