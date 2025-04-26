using System;
using System.Collections.Generic;
using Gameplay.Items;
using Shared;
using UnityEngine.AddressableAssets;

namespace Gameplay.DataHandling
{
    public class ItemDatabaseManager : Singleton<ItemDatabaseManager>
    {
        public AssetLabelReference reference;

        private readonly Dictionary<int, WeaponData> _weaponDataDictionary = new();

        public override void Awake()
        {
            base.Awake();
            Addressables.LoadAssetsAsync<WeaponData>(reference, OnDataLoaded);
        }

        private void OnDataLoaded(WeaponData weaponData)
        {
            _weaponDataDictionary.Add(weaponData.id, weaponData);
        }

        public WeaponData GetWeaponData(int key)
        {
            var success = _weaponDataDictionary.TryGetValue(key, out var weaponData);
            if (!success)
            {
                throw new Exception("Weapon data not found for key: " + key);
            }

            return weaponData;
        }
    }
}