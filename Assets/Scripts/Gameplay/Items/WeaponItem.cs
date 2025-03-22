using System;
using System.Text;
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
        public WeaponRarity Rarity;

        private WeaponData Data => ItemDatabaseManager.Instance.GetWeaponData(Id);

        public string Name => Data.itemName;
        public string Description => Data.description;
        public Sprite Sprite => Data.sprite;
        public GameObject Prefab => Data.prefab;

        public bool IsValid => Id != -1;


        public static WeaponItem FakeWeapon => new()
        {
            Id = -1,
            Damage = 0,
            DelayBetweenShots = 0,
            BulletSpreadAngle = 0,
            ClipSize = 0,
            BulletsPerShot = 0,
            Rarity = WeaponRarity.Common
        };

        public string GetWeaponStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Rarity: {Rarity}");
            sb.AppendLine($"Damage: {Damage}");
            sb.AppendLine($"Delay Between Shots: {DelayBetweenShots}");
            sb.AppendLine($"Clip Size: {ClipSize}");

            return sb.ToString();
        }
    }
}