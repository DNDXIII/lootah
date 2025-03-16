using Gameplay.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class DraggableInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public InventorySlot InventorySlot { get; private set; }
        public bool IsEquipped { get; private set; }


        private Image _image;
        private Transform _originalParent;
        private int _originalIndex;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;
            transform.SetParent(transform.parent.parent.parent);
            transform.SetAsLastSibling();
            _image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(_originalParent);
            _image.raycastTarget = true;
        }

        public int GetOriginalIndex()
        {
            return _originalIndex;
        }

        public void Initialize(InventorySlot inventorySlot, bool isEquipped, int originalIndex, Sprite sprite)
        {
            InventorySlot = inventorySlot;
            IsEquipped = isEquipped;
            _image.sprite = sprite;
            _originalIndex = originalIndex;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // spawn hover panel
            if (InventorySlot.BaseItem == null) return;

            TooltipManager.Instance.ShowTooltip(InventorySlot.BaseItem.GetDescription());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // destroy hover panel
            TooltipManager.Instance.HideTooltip();
        }
    }
}