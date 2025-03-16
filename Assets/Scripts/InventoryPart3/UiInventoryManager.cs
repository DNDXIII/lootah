using System;
using UnityEngine;

namespace InventoryPart3
{
    public class UiInventoryManager : MonoBehaviour
    {
        [SerializeField] private WeaponUiInventorySlot inventorySlotPrefab;

        [Tooltip("The parent object that will hold all the inventory slots")] [SerializeField]
        private GameObject contentView;

        private PlayerInventory _playerInventory;

        private void Start()
        {
            // TODO: Probably need to change how to get the PlayerInventory at some point

            _playerInventory = FindFirstObjectByType<PlayerInventory>();
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
                inventorySlot.SetItem(weapon);
            }
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