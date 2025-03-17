using System;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryPart3
{
    public class UiInventoryManager : MonoBehaviour
    {
        [SerializeField] private WeaponUiInventorySlot inventorySlotPrefab;

        [SerializeField] private Image[] equippedWeaponSlots;

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
                if (equippedWeapons[i].Id == -1) continue;
                equippedWeaponSlots[i].sprite = equippedWeapons[i].Sprite;
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