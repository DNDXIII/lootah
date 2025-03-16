using UnityEngine;

namespace Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyController))]
    public abstract class EnemyAI : MonoBehaviour
    {
        [Tooltip("Whether the AI is active at the start of the game.")] [SerializeField]
        private bool startsActive = true;

        [Tooltip("The delay after the AI is activated before it starts attacking.")] [SerializeField]
        private float delayAfterActivation;

        protected bool IsActive;
        protected EnemyController EnemyController;


        protected virtual void Start()
        {
            EnemyController = GetComponent<EnemyController>();
            if (startsActive)
            {
                Activate();
            }
        }


        public virtual void Activate()
        {
            Invoke(nameof(SetActive), delayAfterActivation);
        }

        private void SetActive()
        {
            IsActive = true;
        }
    }
}