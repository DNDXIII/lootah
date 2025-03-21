using Gameplay.Player;
using Managers;
using UnityEngine;

namespace Gameplay.Pickups
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class BasePickup : MonoBehaviour
    {
        [SerializeField] private float verticalBobFrequency = 1f;
        [SerializeField] private float verticalBobAmplitude = 0.5f;
        [SerializeField] private AudioClip onPickupSound;
        [SerializeField] private Sprite sprite;

        private Vector3 _startPosition;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            if (sprite != null)
            {
                GetComponent<SpriteRenderer>().sprite = sprite;
            }

            _startPosition = transform.position;
        }

        private void Update()
        {
            float bobbingAnimationPhase =
                (Mathf.Sin(Time.time * verticalBobFrequency) * 0.5f + 0.5f) * verticalBobAmplitude;
            transform.position = _startPosition + Vector3.up * bobbingAnimationPhase;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController pickingPlayer = other.GetComponent<PlayerController>();

            if (!pickingPlayer) return;

            if (onPickupSound)
            {
                AudioUtility.CreateSfx(onPickupSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
            }

            OnPickup(pickingPlayer);
            Destroy(gameObject);
        }

        protected abstract void OnPickup(PlayerController playerController);
    }
}