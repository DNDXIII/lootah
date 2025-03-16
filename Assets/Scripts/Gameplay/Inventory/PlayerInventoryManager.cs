using Gameplay.Items;
using Gameplay.Items.Instances;
using Shared;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Inventory
{
    public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
    {
        public UnityAction OnInventoryChanged;
        public UnityAction OnEquippedChanged;

        public InventoryObject inventory;
        public InventoryObject equippedInventory;

        public WeaponItem defaultWeapon;


        public override void Awake()
        {
            base.Awake();

            inventory.Load();
            equippedInventory.Load();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                inventory.Save();
                equippedInventory.Save();
                Debug.Log("Saved inventory");
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                inventory.Load();
                equippedInventory.Load();
                Debug.Log("Loaded inventory");

                OnInventoryChanged?.Invoke();
                OnEquippedChanged?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (defaultWeapon != null)
                {
                    AddItemToInventory(defaultWeapon);
                }
            }
        }

        public bool AddItemToInventory(BaseItem baseItem)
        {
            // If it's a weapon, try to add it to the equipped inventory first, if there is space
            if (baseItem is WeaponItem weaponItem)
            {
                if (equippedInventory.AddItem(weaponItem, 1))
                {
                    OnEquippedChanged?.Invoke();
                    return true;
                }
            }

            // We don't have space in either inventory
            if (!inventory.AddItem(baseItem, 1)) return false;

            // else add it to the regular inventory
            OnInventoryChanged?.Invoke();
            return true;
        }

        private void OnApplicationQuit()
        {
            inventory.container.itemSlots = new InventorySlot[inventory.container.itemSlots.Length];
            equippedInventory.container.itemSlots = new InventorySlot[equippedInventory.container.itemSlots.Length];
        }

        public InventorySlot[] GetInventoryItems()
        {
            return inventory.container.itemSlots;
        }

        public InventorySlot[] GetEquippedItems()
        {
            return equippedInventory.container.itemSlots;
        }

        /**
         * Handle moving an item within into the inventory, either from the equipped inventory or the regular inventory
         */
        public void HandleMovingItemIntoInventory(DraggableInventoryItem draggable, int targetSlotIndex)
        {
            // Get the target slot and the source slot
            var targetSlot = inventory.container.itemSlots[targetSlotIndex];

            // Create a copy of the items to avoid reference issues
            var previousItemInTargetSlot = new InventorySlot(targetSlot.BaseItem, targetSlot.amount);
            var draggedItemSlot =
                new InventorySlot(draggable.InventorySlot.BaseItem, draggable.InventorySlot.amount);

            // If the item being dragged was previously equipped 
            if (draggable.IsEquipped)
            {
                switch (previousItemInTargetSlot.BaseItem)
                {
                    // Check if the place where we are dragging the item to has a weapon, 
                    // if it does, we have to swap the weapons
                    case WeaponItem:
                        equippedInventory.container.itemSlots[draggable.GetOriginalIndex()]
                            .UpdateSlot(previousItemInTargetSlot);
                        inventory.container.itemSlots[targetSlotIndex]
                            .UpdateSlot(draggedItemSlot);
                        break;
                    // If there is no item in the slot we are dragging the weapon to, just add it there,
                    // and remove it from the equipped inventory
                    case null:
                        inventory.container.itemSlots[targetSlotIndex]
                            .UpdateSlot(draggedItemSlot);
                        equippedInventory.container.itemSlots[draggable.GetOriginalIndex()].UpdateSlot(
                            null, 0);
                        break;
                    // Otherwise, we are trying to swap a weapon with something that is not a weapon, which is not possible
                    // so we just return
                    default:
                        return;
                }

                OnInventoryChanged?.Invoke();
                OnEquippedChanged?.Invoke();
            }

            // Otherwise, we are just swapping items in the inventory 
            else
            {
                inventory.container.itemSlots[draggable.GetOriginalIndex()]
                    .UpdateSlot(previousItemInTargetSlot);
                inventory.container.itemSlots[targetSlotIndex]
                    .UpdateSlot(draggedItemSlot);
                OnInventoryChanged?.Invoke();
            }
        }


        /**
         * Handle moving an item into the equipped inventory
         */
        public void HandleMovingItemIntoEquipped(DraggableInventoryItem draggable, int targetSlotIndex)
        {
            // We can only equip weapons, everything else is ignored
            if (draggable.InventorySlot.BaseItem is not WeaponItem) return;

            // If it's just the same weapon being dragged back to the same slot then we don't have to do anything
            if (draggable.IsEquipped && draggable.GetOriginalIndex() == targetSlotIndex)
            {
                return;
            }

            var targetSlot = equippedInventory.container.itemSlots[targetSlotIndex];

            // Create a copy of the items to avoid reference issues
            var previousItemInTargetSlot = new InventorySlot(targetSlot.BaseItem, targetSlot.amount);
            var draggedItemSlot =
                new InventorySlot(draggable.InventorySlot.BaseItem, draggable.InventorySlot.amount);

            // If the item being equipped was also equipped, then we are just swapping the weapons
            if (draggable.IsEquipped)
            {
                equippedInventory.container.itemSlots[draggable.GetOriginalIndex()]
                    .UpdateSlot(previousItemInTargetSlot);
                equippedInventory.container.itemSlots[targetSlotIndex]
                    .UpdateSlot(draggedItemSlot);
            }

            // Otherwise, we are dragging a weapon from the inventory to the equipped inventory
            else
            {
                inventory.container.itemSlots[draggable.GetOriginalIndex()]
                    .UpdateSlot(previousItemInTargetSlot);
                equippedInventory.container.itemSlots[targetSlotIndex].UpdateSlot(
                    draggedItemSlot);

                OnInventoryChanged?.Invoke();
            }

            OnEquippedChanged?.Invoke();
        }
    }
}