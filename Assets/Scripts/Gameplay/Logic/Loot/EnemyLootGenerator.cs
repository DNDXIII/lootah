using System;
using System.Collections.Generic;
using Gameplay.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Logic.Loot
{
    [RequireComponent(typeof(Health))]
    public class EnemyLootGenerator : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float dropChance;
        [SerializeField] private List<LootItem> lootTable;

        private void Start()
        {
            if (TryGetComponent<Health>(out var baseHealth))
            {
                baseHealth.OnDie += GenerateLoot;
            }
            else
            {
                throw new Exception(
                    "EnemyLootGenerator requires BaseHealth component to be present on the same GameObject");
            }
        }

        private void GenerateLoot()
        {
            if (Random.value > dropChance) return;
            LootGenerator.Instance.GenerateLoot(lootTable, transform.position);
        }
    }
}