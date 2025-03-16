using UnityEngine;

namespace Shared
{
    public class TimedSelfDestruct : MonoBehaviour
    {
        public float lifeTime = 1f;

        private float _spawnTime;

        private void Awake()
        {
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time > _spawnTime + lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}