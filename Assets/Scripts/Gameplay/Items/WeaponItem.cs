using System;
using DataHandling;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Gameplay.Items
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


        public static WeaponItem FakeWeapon => new()
        {
            Id = -1,
            Damage = 0,
            DelayBetweenShots = 0,
            BulletSpreadAngle = 0,
            ClipSize = 0,
            BulletsPerShot = 0
        };
    }
}