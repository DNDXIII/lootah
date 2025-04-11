using Gameplay.Managers;
using Managers;
using Shared;
using UnityEngine;

namespace UI
{
    public class CrosshairManager : MonoBehaviour
    {
        [SerializeField] private GameObject criticalHitCrosshair;
        [SerializeField] private float criticalHitDuration = 0.2f;
        [SerializeField] private AudioClip critSfx;

        private void Start()
        {
            EventManager.AddListener<CriticalHitEvent>(OnCriticalHit);
            criticalHitCrosshair.SetActive(false);
        }


        private void OnDestroy()
        {
            EventManager.RemoveListener<CriticalHitEvent>(OnCriticalHit);
        }

        private void OnCriticalHit(CriticalHitEvent criticalHitEvent)
        {
            criticalHitCrosshair.SetActive(true);
            Invoke(nameof(DisableCrosshair), criticalHitDuration);
            if (critSfx)
            {
                AudioUtility.CreateSfx(critSfx, ActorManager.Instance.Player.transform.position,
                    AudioUtility.AudioGroups.DamageTick);
            }
        }

        private void DisableCrosshair()
        {
            criticalHitCrosshair.SetActive(false);
        }
    }
}