using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int id;
        public string itemName;
        public string description;
        public Sprite sprite;
        public GameObject prefab;
    }
}