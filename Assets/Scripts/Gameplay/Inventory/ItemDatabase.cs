using System.Collections.Generic;
using Gameplay.Items.Blueprints;
using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        public BaseItemBlueprint[] items;
        public Dictionary<int, BaseItemBlueprint> GetItemObject = new();

        public void OnBeforeSerialize()
        {
            GetItemObject = new Dictionary<int, BaseItemBlueprint>();
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].id = i;
                GetItemObject.Add(i, items[i]);
            }
        }
    }
}