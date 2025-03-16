using System;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace InventoryPart3
{
    [Serializable]
    public class WeaponItem
    {
        public int Id;
        public int Damage;
        public float DelayBetweenShots;
        public float BulletSpreadAngle;
        public int ClipSize;
        public int BulletsPerShot;

        private WeaponData Data => ItemDatabaseManager.Instance.GetWeaponData(Id);

        public string Name => Data.itemName;
        public string Description => Data.description;
        public Sprite Sprite => Data.sprite;
        public GameObject Prefab => Data.prefab;
    }
}