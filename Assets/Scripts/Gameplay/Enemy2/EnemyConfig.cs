using UnityEngine;

namespace Gameplay.Enemy2
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy Config")]
    public class EnemyConfig : ScriptableObject

    {
        public GameObject prefab;
        public int health;
    }
}