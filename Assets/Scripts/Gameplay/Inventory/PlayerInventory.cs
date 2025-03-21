using System;
using System.Collections.Generic;
using DataHandling;
using Gameplay.Items;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Inventory
{
    [RequireComponent(typeof(PlayerWeaponsManager))]
    public class PlayerInventory : MonoBehaviour
    {
        private SaveDataManager _saveDataManager;
        private PlayerWeaponsManager _playerWeaponsManager;

        [SerializeField] private WeaponItem defaultWeapon;

        private void Start()
        {
            _saveDataManager = SaveDataManager.Instance;
            if (_saveDataManager == null)
            {
                throw new Exception("SaveDataManager not found in scene");
            }

            _playerWeaponsManager = GetComponent<PlayerWeaponsManager>();
            // If we are not in a final build, we can setup the initial weapons
            if (!Application.isEditor)
            {
                SetupInitialWeapons();
            }
            else
            {
                // We need to wait a frame to make sure the player weapons manager is setup
                Invoke(nameof(SetupInitialWeapons), 1);
            }
        }


        // TODO: REMOVE THIS
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
               
                AddWeapon(defaultWeapon);
            }
        }

        private void SetupInitialWeapons()
        {
            // Add the default weapons to the inventory
            for (int i = 0; i < _saveDataManager.SaveData.equippedItems.Length; i++)
            {
                var weapon = _saveDataManager.SaveData.equippedItems[i];
                if (weapon.Id != -1)
                {
                    _playerWeaponsManager.EquipWeapon(weapon, i);
                }
            }
        }

        public void AddWeapon(WeaponItem weaponItem)
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

        public void EquipWeapon(WeaponItem weaponItem, int indexToEquip)
        {
            var previousWeapon = _saveDataManager.SaveData.equippedItems[indexToEquip];
            if (previousWeapon.Id != -1)
            {
                // We had a weapon equipped, so we need to add it back to the inventory
                _saveDataManager.SaveData.inventory.Add(previousWeapon);
            }

            // Set the weapon as equipped
            _saveDataManager.SaveData.equippedItems[indexToEquip] = weaponItem;
            // Update the player weapons manager
            _playerWeaponsManager.EquipWeapon(weaponItem, indexToEquip);

            // Remove the weapon from the inventory
            _saveDataManager.SaveData.inventory.Remove(weaponItem);
        }
    }
}