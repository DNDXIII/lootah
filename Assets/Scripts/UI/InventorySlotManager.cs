using Gameplay.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventorySlotManager : MonoBehaviour, IDropHandler
    {
        [SerializeField] private int slotIndex;

        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableInventoryItem draggable = dropped.GetComponent<DraggableInventoryItem>();
            if (draggable == null) return;
            PlayerInventoryManager.Instance.HandleMovingItemIntoInventory(draggable, slotIndex);
            Destroy(draggable.gameObject);
        }
    }
}