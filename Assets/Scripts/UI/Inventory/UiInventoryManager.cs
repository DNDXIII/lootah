using System;
using Gameplay.Inventory;
using Gameplay.Items;
using Gameplay.Managers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class UiInventoryManager : MonoBehaviour
    {
        [SerializeField] private WeaponUiInventorySlot inventorySlotPrefab;

        [SerializeField] private WeaponUiEquippedSlot[] equippedWeaponSlots;

        [Tooltip("The parent object that will hold all the inventory slots")] [SerializeField]
        private GameObject contentView;

        private PlayerInventory _playerInventory;

        private void Start()
        {
            _playerInventory = ActorManager.Instance.Player.GetComponent<PlayerInventory>();
            if (_playerInventory == null)
            {
                throw new Exception("PlayerInventory not found in scene");
            }

            if (Application.isEditor)
            {
                Invoke(nameof(UpdateContentView), 1);
            }
            else
            {
                UpdateContentView();
            }
        }

        private void UpdateContentView()
        {
            if (_playerInventory == null) return;
            DestroyAllChildren(contentView.transform);

            foreach (var weapon in _playerInventory.GetInventoryWeapons())
            {
                var inventorySlot = Instantiate(inventorySlotPrefab, contentView.transform);
                inventorySlot.SetEquipWeapon(OnEquipWeapon);
                inventorySlot.SetDropWeapon(OnDropWeapon);
                inventorySlot.SetItem(weapon);
            }

            var equippedWeapons = _playerInventory.GetEquippedWeapons();
            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                if (!equippedWeapons[i].IsValid) continue;
                equippedWeaponSlots[i].SetItem(equippedWeapons[i]);
            }
        }

        private void OnDropWeapon(WeaponItem weaponItem)
        {
            _playerInventory.RemoveItem(weaponItem);
            UpdateContentView();
        }

        private void OnEquipWeapon(WeaponItem weaponItem, int indexToEquip)
        {
            _playerInventory.EquipWeapon(weaponItem, indexToEquip);
            UpdateContentView();
        }

        private static void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }


        private void OnEnable()
        {
            UpdateContentView();
        }
    }
}