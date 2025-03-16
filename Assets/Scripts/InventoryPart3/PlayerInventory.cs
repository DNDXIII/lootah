using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryPart3
{
    public class PlayerInventory : MonoBehaviour
    {
        private SaveDataManager _saveDataManager;

        private void Start()
        {
            _saveDataManager = SaveDataManager.Instance;
            if (_saveDataManager == null)
            {
                throw new Exception("SaveDataManager not found in scene");
            }
        }

        // TODO: REMOVE THIS
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                var weaponItem = new WeaponItem
                {
                    Id = 0,
                    Damage = 10,
                    DelayBetweenShots = 0.5f,
                    BulletSpreadAngle = 5f,
                    ClipSize = 30,
                    BulletsPerShot = 1
                };
                AddItem(weaponItem);
            }
        }

        public void AddItem(WeaponItem weaponItem)
        {
            _saveDataManager.SaveData.inventory.Add(weaponItem);
        }

        public void RemoveItem(WeaponItem weaponItem)
        {
            _saveDataManager.SaveData.inventory.Remove(weaponItem);
        }

        public List<WeaponItem> GetInventoryWeapons()
        {
            return _saveDataManager.SaveData.inventory;
        }

        public WeaponItem[] GetEquippedWeapons()
        {
            return _saveDataManager.SaveData.equippedItems;
        }
    }
}