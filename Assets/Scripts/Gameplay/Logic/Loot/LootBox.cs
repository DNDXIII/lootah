using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Logic.Loot
{
    public class LootBox : MonoBehaviour
    {
        [SerializeField] private List<LootItem> lootTable;

        private bool _isPickedUp;

        public void SpawnLoot()
        {
            if (_isPickedUp) return;

            _isPickedUp = true;
            LootGenerator.Instance.GenerateLoot(lootTable, transform.position + Vector3.up);
        }
    }
}