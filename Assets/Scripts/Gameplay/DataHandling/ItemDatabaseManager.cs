using System;
using System.Collections.Generic;
using Gameplay.Items;
using Shared;
using UnityEngine.AddressableAssets;

namespace Gameplay.DataHandling
{
    // TODO: Not sure if this can be a singleton, it would be nice to not destroy it on scene change.
    //  But maybe that is something i should do for all singletons 
    public class ItemDatabaseManager : Singleton<ItemDatabaseManager>
    {
        public AssetLabelReference reference;

        private readonly Dictionary<int, WeaponData> weaponDataDictionary = new();

        public override void Awake()
        {
            base.Awake();
            Addressables.LoadAssetsAsync<WeaponData>(reference, OnDataLoaded);
        }

        private void OnDataLoaded(WeaponData weaponData)
        {
            weaponDataDictionary.Add(weaponData.id, weaponData);
        }

        public WeaponData GetWeaponData(int key)
        {
            var success = weaponDataDictionary.TryGetValue(key, out var weaponData);
            if (!success)
            {
                throw new Exception("Weapon data not found for key: " + key);
            }

            return weaponData;
        }
    }
}