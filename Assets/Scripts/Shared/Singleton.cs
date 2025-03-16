using UnityEngine;

namespace Shared
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = (T)FindFirstObjectByType(typeof(T));
                if (!_instance)
                {
                    SetupInstance();
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            RemoveDuplicates();
        }

        private static void SetupInstance()
        {
            _instance = (T)FindFirstObjectByType(typeof(T));
            if (_instance) return;
            GameObject gameObj = new GameObject
            {
                name = typeof(T).Name
            };
            _instance = gameObj.AddComponent<T>();
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}