using System;
using System.Collections;
using Gameplay.Shared;
using Gameplay.Weapons.WeaponGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Weapons
{
    public enum WeaponShootType
    {
        Manual,
        Automatic,
    }

    public enum WeaponFireMode
    {
        Projectile, // Uses instantiated projectiles
        Hitscan // Uses raycasting
    }

    [Serializable]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponController : MonoBehaviour
    {
        // INTERNAL REFERENCES
        [Header("Internal References")] [Tooltip("Root object for the weapon")]
        public GameObject weaponRoot;

        [Tooltip("Muzzle transform where projectiles are spawned")]
        public Transform weaponMuzzle;

        // SHOOT PARAMETERS
        [Header("Shoot Parameters")] [Tooltip("Defines the type of shooting (automatic, semi-automatic, burst, etc.)")]
        public WeaponShootType shootType;

        [Tooltip("Defines whether the weapon uses projectiles or hitscan")]
        public WeaponFireMode fireMode = WeaponFireMode.Projectile; // Default to projectile

        [Tooltip("Prefab for the projectile fired by this weapon (if using projectile-based shooting)")]
        public ProjectileBase projectilePrefab;

        [Tooltip("Maximum range for hitscan weapons")]
        public float hitscanRange = 100f;

        [Tooltip("Impact effect for hitscan weapons")]
        public GameObject hitEffectPrefab;

        [Tooltip("Blood effect for Hitscan weapons")]
        public GameObject bloodEffectPrefab;

        // RECOIL & SHOOTING EFFECTS
        [Header("Recoil & Effects")] [Tooltip("Recoil force applied to the weapon when shooting")] [Range(0f, 2f)]
        public float recoilForce = 1f;

        [Tooltip("Audio played when shooting")]
        public AudioClip shootSfx;

        [Tooltip("Audio played when reloading")]
        public AudioClip reloadSfx;

        [Tooltip("Audio played when switching weapons")]
        public AudioClip changeWeaponSfx;

        // RELOADING & AMMO
        [Header("Reload & Ammo")] [Tooltip("Time required to reload the weapon")]
        public float reloadTime;

        [Tooltip("Current ammo count as a ratio of the clip size")]
        public float CurrentAmmoRatio => CurrentAmmo / _weaponStats.clipSize;

        [Tooltip("Maximum number of rounds per clip")]
        public float ClipSize => _weaponStats.clipSize;

        [Tooltip("Current ammo count in the clip")]
        public float CurrentAmmo { get; private set; }

        [Tooltip("Whether the weapon is currently active")]
        public bool IsWeaponActive { get; private set; }

        [Tooltip("Whether the weapon is currently reloading")]
        public bool IsReloading { get; private set; }

        // COLLISIONS & HIT DETECTION
        [Header("Collision & Hit Detection")]
        [Tooltip("Layer mask for hitscan weapons, determines what objects are hit")]
        public LayerMask layerMask;

        // EVENTS
        [Header("Events")] [Tooltip("Event triggered when the weapon is fired")]
        public UnityAction OnShoot;

        // PROPERTIES
        [Header("Weapon Properties")]
        [Tooltip("Owner of the weapon (who is currently holding it)")]
        public GameObject Owner { get; set; }

        [Tooltip("Original prefab source for this weapon")]
        public GameObject SourcePrefab { get; set; }

        // PRIVATE VARIABLES
        private float _lastTimeShot = Mathf.NegativeInfinity;
        private AudioSource _shootAudioSource;
        private WeaponStats _weaponStats;
        private Camera _camera;

        private void Awake()
        {
            _shootAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (fireMode == WeaponFireMode.Hitscan)
                _camera = Camera.main;
        }

        public void Init(WeaponStats weaponStats)
        {
            _weaponStats = weaponStats;
            CurrentAmmo = _weaponStats.clipSize;
        }

        private void Reload()
        {
            CurrentAmmo = _weaponStats.clipSize;
            IsReloading = false;
        }

        public void StartReloadAnimation()
        {
            if (IsReloading) return;

            IsReloading = true;
            if (reloadSfx)
            {
                _shootAudioSource.PlayOneShot(reloadSfx);
            }

            StartCoroutine(ReloadAnimation());
        }

        private IEnumerator ReloadAnimation()
        {
            float halfTime = reloadTime / 2f;
            Vector3 downPosition = Vector3.down * 2;
            yield return StartCoroutine(MoveWeapon(Vector3.zero, downPosition, halfTime));
            yield return StartCoroutine(MoveWeapon(downPosition, Vector3.zero, halfTime));
            Reload();
        }

        private IEnumerator MoveWeapon(Vector3 startPosition, Vector3 targetPosition, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = targetPosition;
        }

        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);
            if (show && changeWeaponSfx)
            {
                _shootAudioSource.PlayOneShot(changeWeaponSfx);
            }

            IsWeaponActive = show;
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld)
        {
            return shootType switch
            {
                WeaponShootType.Manual => inputDown && TryShoot(),
                WeaponShootType.Automatic => inputHeld && TryShoot(),
                _ => false
            };
        }

        private bool TryShoot()
        {
            if (CurrentAmmo < 1f)
            {
                StartReloadAnimation();
                return false;
            }

            if (_lastTimeShot + _weaponStats.delayBetweenShots >= Time.time)
            {
                return false;
            }

            HandleShoot();
            CurrentAmmo -= 1f;
            return true;
        }

        private void HandleShoot()
        {
            if (fireMode == WeaponFireMode.Projectile)
            {
                ShootProjectile();
            }
            else
            {
                ShootHitscan();
            }

            _lastTimeShot = Time.time;

            if (shootSfx)
            {
                _shootAudioSource.PlayOneShot(shootSfx);
            }

            OnShoot?.Invoke();
        }

        private void ShootProjectile()
        {
            for (int i = 0; i < _weaponStats.bulletsPerShot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(Owner, _weaponStats.damage);
            }
        }

        private void ShootHitscan()
        {
            for (int i = 0; i < _weaponStats.bulletsPerShot; i++)
            {
                Vector3 shotDirection = _camera.transform.forward;

                if (!Physics.Raycast(_camera.transform.position, shotDirection, out var hit, hitscanRange,
                        layerMask)) continue;

                if (hit.collider.TryGetComponent(out Damageable damageable))
                {
                    damageable.TakeDamage(_weaponStats.damage, Owner);
                    if (bloodEffectPrefab)
                    {
                        Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }


                if (hitEffectPrefab)
                {
                    Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }

        private Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = _weaponStats.bulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);
            return spreadWorldDirection;
        }
    }
}