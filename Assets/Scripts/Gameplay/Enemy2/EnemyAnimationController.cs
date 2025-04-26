using Shared.Utils;
using UnityEngine;

namespace Gameplay.Enemy2
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private static readonly int IsDeadAnimationHash = Animator.StringToHash("IsDead");

        [SerializeField] private Animator animator;


        private void Awake()
        {
            if (!animator)
            {
                animator = Preconditions.CheckNotNull(GetComponent<Animator>());
            }
        }


        public void Initialize()
        {
            animator.SetBool(IsDeadAnimationHash, false);
        }


        public void PlayDeathAnimation()
        {
            animator.SetBool(IsDeadAnimationHash, true);
        }
    }
}