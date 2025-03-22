using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Items;
using Gameplay.Pickups;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Logic.Loot
{
    public class LootGenerator : Singleton<LootGenerator>
    {
        [SerializeField] private WeaponPickup weaponPickupPrefab;

        public void GenerateLoot(List<LootItem> lootTable, Vector3 position)
        {
            var weaponData = GetRandomWeaponData(lootTable);
            var weaponItem = weaponData.GenerateWeaponItem();
            WeaponPickup weaponPickup = Instantiate(weaponPickupPrefab, position, Quaternion.identity);
            weaponPickup.SetWeapon(weaponItem);

            // Throw weapon pickup up and a bit in a random direction
            weaponPickup.GetComponent<Rigidbody>().AddForce(Vector3.up * 5 + Random.insideUnitSphere * 2,
                ForceMode.Impulse);
        }

        private static WeaponData GetRandomWeaponData(List<LootItem> lootTable)
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