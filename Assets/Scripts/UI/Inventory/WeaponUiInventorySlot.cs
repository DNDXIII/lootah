using System;
using Gameplay.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class WeaponUiInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Action<WeaponItem, int> _onEquipWeapon;
        private Action<WeaponItem> _onDropWeapon;

        [Header("Background Color Configs")] [SerializeField]
        private Color commonColor;

        [SerializeField] private Color uncommonColor;

        [SerializeField] private Color rareColor;

        [SerializeField] private Color legendaryColor;

        [Header("UI References")]
        [FormerlySerializedAs("image")]
        [Tooltip("Image to display the weapon icon")]
        [SerializeField]
        private Image weaponIcon;

        [SerializeField] private Image backgroundImage;

        [Tooltip("Text to display the weapon name")] [SerializeField]
        private TMPro.TextMeshProUGUI weaponNameText;

        [Header("Dropdown References")] [SerializeField]
        private GameObject dropDownPanel;

        [SerializeField] private Button dropButton;
        [SerializeField] private Button equip1Button, equip2Button, dropWeaponButton;

        private WeaponItem _weaponItem;

        private void Awake()
        {
            dropDownPanel.SetActive(false);
            dropButton.onClick.AddListener(ToggleDropdown);
            equip1Button.onClick.AddListener(() => EquipWeapon(0));
            equip2Button.onClick.AddListener(() => EquipWeapon(1));
            dropWeaponButton.onClick.AddListener(DropWeapon);
        }

        public void SetEquipWeapon(Action<WeaponItem, int> onEquipWeapon)
        {
            _onEquipWeapon = onEquipWeapon;
        }

        public void SetDropWeapon(Action<WeaponItem> onDropWeapon)
        {
            _onDropWeapon = onDropWeapon;
        }

        private void ToggleDropdown()
        {
            dropDownPanel.SetActive(!dropDownPanel.activeSelf);
        }

        private void EquipWeapon(int indexToEquip)
        {
            _onEquipWeapon?.Invoke(_weaponItem, indexToEquip);
            TooltipManager.Instance.HideTooltip();
        }

        private void DropWeapon()
        {
            _onDropWeapon?.Invoke(_weaponItem);
            TooltipManager.Instance.HideTooltip();
        }

        public void SetItem(WeaponItem weaponItem)
        {
            _weaponItem = weaponItem;
            weaponIcon.sprite = _weaponItem.Sprite;
            weaponNameText.text = _weaponItem.Name;
            backgroundImage.color = GetBackgroundColor(_weaponItem);
        }

        private Color GetBackgroundColor(WeaponItem weaponItem)
        {
            return weaponItem.Rarity switch
            {
                WeaponRarity.Common => commonColor,
                WeaponRarity.Uncommon => uncommonColor,
                WeaponRarity.Rare => rareColor,
                WeaponRarity.Legendary => legendaryColor,
                _ => commonColor
            };
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.Instance.ShowTooltip(_weaponItem.GetWeaponStats());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}