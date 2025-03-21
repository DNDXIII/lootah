using Gameplay.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class WeaponUiEquippedSlots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Image to display the weapon icon")] [SerializeField]
        private Image image;

        private WeaponItem _weaponItem;


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_weaponItem.IsValid)
                TooltipManager.Instance.ShowTooltip(_weaponItem.GetWeaponStats());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_weaponItem.IsValid)
                TooltipManager.Instance.HideTooltip();
        }
    }
}