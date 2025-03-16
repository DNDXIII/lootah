using UnityEngine;
using UnityEngine.UI;

namespace InventoryPart3
{
    public class WeaponUiInventorySlot : MonoBehaviour
    {
        [Tooltip("Image to display the weapon icon")] [SerializeField]
        private Image image;

        [Tooltip("Text to display the weapon name")] [SerializeField]
        private TMPro.TextMeshProUGUI weaponNameText;

        [SerializeField] private GameObject dropDownPanel;
        [SerializeField] private Button dropButton;
        [SerializeField] private Button equip1Button, equip2Button, dropWeaponButton;

        private WeaponItem _weaponItem;

        private void Start()
        {
            dropDownPanel.SetActive(false);
            dropButton.onClick.AddListener(ToggleDropdown);
            equip1Button.onClick.AddListener(() => EquipWeapon(0));
            equip2Button.onClick.AddListener(() => EquipWeapon(1));
            dropWeaponButton.onClick.AddListener(DropWeapon);
        }

        private void ToggleDropdown()
        {
            dropDownPanel.SetActive(!dropDownPanel.activeSelf);
        }

        private void EquipWeapon(int indexToEquip)
        {
            Debug.Log("Equipping weapon " + _weaponItem.Name + " to slot " + indexToEquip);
        }

        private void DropWeapon()
        {
            Debug.Log("Dropping weapon " + _weaponItem.Name);
        }

        public void SetItem(WeaponItem weaponItem)
        {
            _weaponItem = weaponItem;
            image.sprite = _weaponItem.Sprite;
            weaponNameText.text = _weaponItem.Name;
        }
    }
}