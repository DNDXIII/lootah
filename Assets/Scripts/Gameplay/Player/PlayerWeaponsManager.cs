using System;
using Gameplay.Items;
using Gameplay.Weapons;
using Gameplay.Weapons.WeaponGeneration;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Player
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        private enum WeaponSwitchState
        {
            Up,
            Down,
            PutDownPrevious,
            PutUpNew,
        }

        [Header("References")] [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
        public Camera weaponCamera;

        [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform weaponParentSocket;

        [Tooltip("Position for weapons when active but not actively aiming")]
        public Transform defaultWeaponPosition;

        [Tooltip("Position for innactive weapons")]
        public Transform downWeaponPosition;

        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        public float bobFrequency = 10f;

        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        public float bobSharpness = 10f;

        [Tooltip("Distance the weapon bobs when not aiming")]
        public float defaultBobAmount = 0.05f;

        [Header("Weapon Recoil")]
        [Tooltip("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        public float recoilSharpness = 50f;

        [Tooltip("Maximum distance the recoil can affect the weapon")]
        public float maxRecoilDistance = 0.5f;

        [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
        public float recoilRestitutionSharpness = 10f;

        [Tooltip("Field of view when not aiming")]
        public float defaultFov = 60f;

        [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
        public float weaponSwitchDelay = 1f;

        private int ActiveWeaponIndex { get; set; }

        public UnityAction<WeaponController> OnSwitchedToWeapon;

        private readonly WeaponController[] _weaponSlots = new WeaponController[2];
        private PlayerInputHandler _inputHandler;
        private PlayerController _playerCharacterController;
        private float _weaponBobFactor;
        private Vector3 _lastCharacterPosition;
        private Vector3 _weaponMainLocalPosition;
        private Vector3 _weaponBobLocalPosition;
        private Vector3 _weaponRecoilLocalPosition;
        private Vector3 _accumulatedRecoil;
        private float _timeStartedWeaponSwitch;
        private WeaponSwitchState _weaponSwitchState;
        private int _weaponSwitchNewWeaponIndex;


        private void Start()
        {
            ActiveWeaponIndex = -1;
            _weaponSwitchState = WeaponSwitchState.Down;

            _inputHandler = GetComponent<PlayerInputHandler>();

            _playerCharacterController = GetComponent<PlayerController>();

            SetFov(defaultFov);

            OnSwitchedToWeapon += OnWeaponSwitched;
        }


        private void Update()
        {
            // shoot handling
            WeaponController activeWeapon = GetActiveWeapon();

            if (activeWeapon && activeWeapon.IsReloading)
                return;

            if (activeWeapon && _weaponSwitchState == WeaponSwitchState.Up)
            {
                if (_inputHandler.GetReloadButtonDown() &&
                    activeWeapon.CurrentAmmoRatio < 1.0f)
                {
                    activeWeapon.StartReloadAnimation();
                    return;
                }

                // handle shooting
                bool hasFired = activeWeapon.HandleShootInputs(
                    _inputHandler.GetFireInputDown(),
                    _inputHandler.GetFireInputHeld());

                // Handle accumulating recoil
                if (hasFired)
                {
                    _accumulatedRecoil += Vector3.back * activeWeapon.recoilForce;
                    _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, maxRecoilDistance);
                }
            }

            // weapon switch handling
            if (
                activeWeapon &&
                _weaponSwitchState is WeaponSwitchState.Up or WeaponSwitchState.Down)
            {
                bool switchWeaponInput = _inputHandler.GetSwitchWeaponInput();
                if (switchWeaponInput)
                {
                    SwitchWeapon();
                }
            }
        }

        // Update various animated features in LateUpdate because it needs to override the animated arm position
        private void LateUpdate()
        {
            UpdateWeaponBob();
            UpdateWeaponRecoil();
            UpdateWeaponSwitching();

            // Set final weapon socket position based on all the combined animation influences
            weaponParentSocket.localPosition =
                _weaponMainLocalPosition + _weaponBobLocalPosition + _weaponRecoilLocalPosition;
        }

        // Sets the FOV of the main camera and the weapon camera simultaneously
        private void SetFov(float fov)
        {
            _playerCharacterController.playerCamera.fieldOfView = fov;
        }

        // Note this will only work for 2 weapon slots
        private void SwitchWeapon()
        {
            // if we have no weapons try to switch to the next available weapon slot
            if (ActiveWeaponIndex == -1)
            {
                for (int i = 0; i < _weaponSlots.Length; i++)
                {
                    if (!_weaponSlots[i]) continue;
                    SwitchToWeaponIndex(i);
                    return;
                }

                // if we have no weapons to switch to, do nothing
                return;
            }


            var otherWeaponIndex = ActiveWeaponIndex == 0 ? 1 : 0;

            // if we can switch to the other weapon slot, do so
            if (GetWeaponAtSlotIndex(otherWeaponIndex))
            {
                SwitchToWeaponIndex(otherWeaponIndex, true);
                return;
            }

            // if not, but if the current weapon slot is valid, keep it
            if (GetWeaponAtSlotIndex(ActiveWeaponIndex))
            {
                return;
            }

            // if not, switch to no weapon
            ActiveWeaponIndex = -1;
            OnSwitchedToWeapon?.Invoke(null);
        }

        // Switches to the given weapon index in weapon slots if the new index is a valid weapon that is different from our current one
        private void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
        {
            if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
            {
                // Store data related to weapon switching animation
                _weaponSwitchNewWeaponIndex = newWeaponIndex;
                _timeStartedWeaponSwitch = Time.time;

                // Handle case of switching to a valid weapon for the first time (simply put it up without putting anything down first)
                if (!GetActiveWeapon())
                {
                    _weaponMainLocalPosition = downWeaponPosition.localPosition;
                    _weaponSwitchState = WeaponSwitchState.PutUpNew;
                    ActiveWeaponIndex = _weaponSwitchNewWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(_weaponSwitchNewWeaponIndex);
                    OnSwitchedToWeapon?.Invoke(newWeapon);
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    _weaponSwitchState = WeaponSwitchState.PutDownPrevious;
                }
            }
        }

        // Updates the weapon bob animation based on character speed
        private void UpdateWeaponBob()
        {
            if (Time.deltaTime > 0f)
            {
                Vector3 playerCharacterVelocity =
                    (_playerCharacterController.transform.position - _lastCharacterPosition) / Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0f;
                if (_playerCharacterController.IsGrounded)
                {
                    characterMovementFactor =
                        Mathf.Clamp01(playerCharacterVelocity.magnitude /
                                      (_playerCharacterController.maxSpeedOnGround *
                                       _playerCharacterController.sprintSpeedModifier));
                }

                _weaponBobFactor =
                    Mathf.Lerp(_weaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float frequency = bobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * defaultBobAmount * _weaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * defaultBobAmount *
                                  _weaponBobFactor;

                // Apply weapon bob
                _weaponBobLocalPosition.x = hBobValue;
                _weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

                _lastCharacterPosition = _playerCharacterController.transform.position;
            }
        }

        // Updates the weapon recoil animation
        private void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (_weaponRecoilLocalPosition.z >= _accumulatedRecoil.z * 0.99f)
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, _accumulatedRecoil,
                    recoilSharpness * Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, Vector3.zero,
                    recoilRestitutionSharpness * Time.deltaTime);
                _accumulatedRecoil = _weaponRecoilLocalPosition;
            }
        }

        // Updates the animated transition of switching weapons
        private void UpdateWeaponSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            var switchingTimeFactor = weaponSwitchDelay == 0f
                ? 1f
                : Mathf.Clamp01((Time.time - _timeStartedWeaponSwitch) / weaponSwitchDelay);

            // Handle transiting to new switch state
            if (switchingTimeFactor >= 1f)
            {
                switch (_weaponSwitchState)
                {
                    case WeaponSwitchState.PutDownPrevious:
                    {
                        // Deactivate old weapon
                        WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                        if (oldWeapon)
                        {
                            oldWeapon.ShowWeapon(false);
                        }

                        ActiveWeaponIndex = _weaponSwitchNewWeaponIndex;
                        switchingTimeFactor = 0f;

                        // Activate new weapon
                        WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                        OnSwitchedToWeapon?.Invoke(newWeapon);

                        if (newWeapon)
                        {
                            _timeStartedWeaponSwitch = Time.time;
                            _weaponSwitchState = WeaponSwitchState.PutUpNew;
                        }
                        else
                        {
                            // if new weapon is null, don't follow through with putting weapon back up
                            _weaponSwitchState = WeaponSwitchState.Down;
                        }

                        break;
                    }
                    case WeaponSwitchState.PutUpNew:
                        _weaponSwitchState = WeaponSwitchState.Up;
                        break;
                    case WeaponSwitchState.Up:
                        break;
                    case WeaponSwitchState.Down:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _weaponMainLocalPosition = _weaponSwitchState switch
            {
                // Handle moving the weapon socket position for the animated weapon switching
                WeaponSwitchState.PutDownPrevious => Vector3.Lerp(defaultWeaponPosition.localPosition,
                    downWeaponPosition.localPosition, switchingTimeFactor),
                WeaponSwitchState.PutUpNew => Vector3.Lerp(downWeaponPosition.localPosition,
                    defaultWeaponPosition.localPosition, switchingTimeFactor),
                _ => _weaponMainLocalPosition
            };
        }

        private void RemoveWeapon(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _weaponSlots.Length)
            {
                Debug.LogError("Trying to remove weapon from invalid slot index");
                return;
            }

            if (!_weaponSlots[slotIndex])
            {
                // Debug.LogWarning("Trying to remove weapon from slot index that has no weapon");
                return;
            }

            WeaponController weaponInstance = _weaponSlots[slotIndex];
            _weaponSlots[slotIndex] = null;

            Destroy(weaponInstance.gameObject);

            // Handle case of removing active weapon (switch to next weapon)
            if (slotIndex == ActiveWeaponIndex)
            {
                SwitchWeapon();
            }
        }

        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        private WeaponController GetWeaponAtSlotIndex(int index)
        {
            // find the active weapon in our weapon slots based on our active weapon index
            if (index >= 0 &&
                index < _weaponSlots.Length)
            {
                return _weaponSlots[index];
            }

            // if we didn't find a valid active weapon in our weapon slots, return null
            return null;
        }

        private static void OnWeaponSwitched(WeaponController newWeapon)
        {
            if (newWeapon)
            {
                newWeapon.ShowWeapon(true);
            }
        }

        public void EquipWeapon(WeaponItem weaponItem, int indexToEquip)
        {
            if (indexToEquip < 0 || indexToEquip >= _weaponSlots.Length)
            {
                Debug.LogError("Trying to add weapon to invalid slot index");
                return;
            }

            if (_weaponSlots[indexToEquip])
            {
                RemoveWeapon(indexToEquip);
            }

            var weaponPrefab = weaponItem.Prefab;

            var weaponInstance = Instantiate(weaponPrefab, weaponParentSocket).GetComponent<WeaponController>();
            if (weaponInstance == null)
            {
                throw new Exception("Weapon prefab does not have a WeaponController component");
            }

            weaponInstance.Init(new WeaponStats
            {
                damage = weaponItem.Damage,
                delayBetweenShots = weaponItem.DelayBetweenShots,
                bulletSpreadAngle = weaponItem.BulletSpreadAngle,
                clipSize = weaponItem.ClipSize,
                bulletsPerShot = weaponItem.BulletsPerShot
            });
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;

            // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
            weaponInstance.Owner = gameObject;
            weaponInstance.SourcePrefab = weaponPrefab.gameObject;
            weaponInstance.ShowWeapon(false);

            _weaponSlots[indexToEquip] = weaponInstance;

            // Handle auto-switching to weapon if no weapons currently
            if (!GetActiveWeapon())
            {
                SwitchWeapon();
            }
        }
    }
}