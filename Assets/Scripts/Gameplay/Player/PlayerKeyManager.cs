using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    public enum KeyType
    {
        Red,
        Blue,
    }

    public class PlayerKeyManager : MonoBehaviour
    {
        private readonly HashSet<KeyType> keys = new();

        public void AddKey(KeyType keyType)
        {
            keys.Add(keyType);
        }

        public bool HasKey(KeyType keyType)
        {
            return keys.Contains(keyType);
        }

        public void RemoveKey(KeyType keyType)
        {
            keys.Remove(keyType);
        }
    }
}