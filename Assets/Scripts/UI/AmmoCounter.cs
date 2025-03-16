using System.Globalization;
using Gameplay.Player;
using Gameplay.Weapons;
using UnityEngine;

namespace UI
{
    public class AmmoCounter : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI ammoText;

        private PlayerWeaponsManager _playerWeaponsManager;
        private WeaponController _activeWeapon;


        private void Start()
        {
            _playerWeaponsManager = FindFirstObjectByType<PlayerWeaponsManager>();
            _playerWeaponsManager.OnSwitchedToWeapon += OnSwitchedToWeapon;

            _activeWeapon = _playerWeaponsManager.GetActiveWeapon();
        }

        private void OnSwitchedToWeapon(WeaponController newWeapon)
        {
            _activeWeapon = newWeapon;
        }

        private void Update()
        {
            if (!_activeWeapon)
            {
                ammoText.text = "";
                return;
            }

            ammoText.text = _activeWeapon.CurrentAmmo.ToString(CultureInfo.CurrentCulture) + " / " +
                            _activeWeapon.ClipSize.ToString(CultureInfo.CurrentCulture);
        }
    }
}