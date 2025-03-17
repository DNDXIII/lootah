using UnityEngine;

namespace InventoryPart3
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "InventoryPart3/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int id;
        public string itemName;
        public string description;
        public Sprite sprite;
        public GameObject prefab;
    }
}