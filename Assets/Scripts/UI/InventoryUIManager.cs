using System.Collections.Generic;
using Gameplay.Inventory;
using UnityEngine;

namespace UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private DraggableInventoryItem inventoryItemPrefab;
        [SerializeField] private GameObject inventoryGrid;
        [SerializeField] private GameObject equippedGrid;
        [SerializeField] private ItemDatabase itemDatabase;

        private readonly List<GameObject> _inventorySlots = new();
        private readonly List<GameObject> _equippedSlots = new();

        private PlayerInventoryManager _playerInventoryManager;

        private void Start()
        {
            _playerInventoryManager = PlayerInventoryManager.Instance;
            _playerInventoryManager.OnInventoryChanged += UpdateUi;
            _playerInventoryManager.OnEquippedChanged += UpdateUi;

            for (int i = 0; i < inventoryGrid.transform.childCount; i++)
            {
                _inventorySlots.Add(inventoryGrid.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < equippedGrid.transform.childCount; i++)
            {
                _equippedSlots.Add(equippedGrid.transform.GetChild(i).gameObject);
            }
        }


        private void OnEnable()
        {
            UpdateUi();
        }

        private void UpdateUi()
        {
            // skip if not enabled
            if (!gameObject.activeInHierarchy || !_playerInventoryManager)
            {
                return;
            }

            UpdateSlots(_playerInventoryManager.GetInventoryItems(), _inventorySlots, false);
            UpdateSlots(_playerInventoryManager.GetEquippedItems(), _equippedSlots, true);
        }

        private void UpdateSlots(InventorySlot[] inventorySlots, List<GameObject> slots, bool isEquipped)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                DestroyAllChildren(slot.transform);

                if (i >= inventorySlots.Length || inventorySlots[i].BaseItem == null)
                {
                    continue;
                }

                var item = inventorySlots[i].BaseItem;
                var itemSprite = itemDatabase.GetItemObject[item.id].sprite;
                var inventoryUISlot = Instantiate(inventoryItemPrefab, slot.transform);

                inventoryUISlot.Initialize(inventorySlots[i], isEquipped, i, itemSprite);
            }
        }

        private static void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}