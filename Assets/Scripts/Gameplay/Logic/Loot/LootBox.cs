using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Items;
using Gameplay.Pickups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Logic.Loot
{
    public class LootBox : MonoBehaviour
    {
        [SerializeField] private WeaponPickup weaponPickupPrefab;
        [SerializeField] private List<LootItem> lootTable;


        private bool _isPickedUp;


        public void SpawnLoot()
        {
            if (_isPickedUp) return;

            _isPickedUp = true;
            WeaponItem weaponItem = GenerateLoot();
            WeaponPickup weaponPickup =
                Instantiate(weaponPickupPrefab, transform.position + Vector3.up, Quaternion.identity);
            weaponPickup.SetWeapon(weaponItem);

            // Throw weapon pickup up and a bit in a random direction
            weaponPickup.GetComponent<Rigidbody>().AddForce(Vector3.up * 5 + Random.insideUnitSphere * 2,
                ForceMode.Impulse);
        }

        public WeaponItem GenerateLoot()
        {
            var weaponData = GetRandomWeaponData();
            return weaponData.GenerateWeaponItem();
        }

        private WeaponData GetRandomWeaponData()
        {
            int totalWeight = lootTable.Sum(item => item.weight);

            int randomValue = Random.Range(0, totalWeight);
            int cumulativeWeight = 0;

            foreach (LootItem item in lootTable)
            {
                cumulativeWeight += item.weight;
                if (randomValue < cumulativeWeight)
                {
                    return item.weaponData;
                }
            }

            throw new Exception("No item selected");
        }
    }
}