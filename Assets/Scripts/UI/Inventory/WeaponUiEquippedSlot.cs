﻿using Gameplay.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class WeaponUiEquippedSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Image to display the weapon icon")] [SerializeField]
        private Image image;

        private WeaponItem _weaponItem = WeaponItem.FakeWeapon;

        public void SetItem(WeaponItem weaponItem)
        {
            _weaponItem = weaponItem;
            image.sprite = weaponItem.Sprite;
        }


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