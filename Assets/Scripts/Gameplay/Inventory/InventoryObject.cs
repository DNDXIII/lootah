using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gameplay.Items.Instances;
using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath;
        public Inventory container;

        public bool AddItem(BaseItem newBaseItem, int amount)
        {
            if (!newBaseItem.stackable)
            {
                return SetEmptySlot(newBaseItem, amount);
            }

            // If the item is stackable, check if it already exists in the inventory

            foreach (var t in container.itemSlots)
            {
                if (t.id != newBaseItem.id) continue;
                t.AddAmount(amount);
                return true;
            }
            // if it wasn't found, add a new item

            return SetEmptySlot(newBaseItem, amount);
        }

        private bool SetEmptySlot(BaseItem newBaseItem, int amount)
        {
            foreach (var t in container.itemSlots)
            {
                if (t.id > -1) continue;
                t.UpdateSlot(newBaseItem, amount);
                return true;
            }

            return false;
        }

        public void MoveItem(InventorySlot item1, InventorySlot item2)
        {
            InventorySlot temp = new InventorySlot(item2.BaseItem, item2.amount);
            item2.UpdateSlot(item1.BaseItem, item1.amount);
            item1.UpdateSlot(temp.BaseItem, temp.amount);
        }


        public void RemoveItem(BaseItem baseItem)
        {
            foreach (var t in container.itemSlots)
            {
                if (t.BaseItem == baseItem)
                {
                    t.UpdateSlot(null, 0);
                }
            }
        }


        [ContextMenu("Save")]
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create,
                FileAccess.Write);
            formatter.Serialize(stream, container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (!File.Exists(string.Concat(Application.persistentDataPath, savePath))) return;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open,
                FileAccess.Read);
            Inventory newContainer =
                (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < container.itemSlots.Length; i++)
            {
                container.itemSlots[i].UpdateSlot(newContainer.itemSlots[i].BaseItem,
                    newContainer.itemSlots[i].amount);
            }

            stream.Close();
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container = new Inventory();
        }
    }
}