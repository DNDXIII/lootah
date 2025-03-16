using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Logic
{
    [RequireComponent(typeof(Collider))]
    public class AreaTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onTriggerEnter;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            onTriggerEnter.Invoke();
            Destroy(gameObject);
        }
    }
}