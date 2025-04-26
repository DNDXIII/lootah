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

        public void SetTrigger(string triggerName)
        {
            SetTrigger(Animator.StringToHash(triggerName));
        }

        public void SetTrigger(int triggerHash)
        {
            animator.SetTrigger(triggerHash);
        }

        public void SetBool(string boolName, bool value)
        {
            SetBool(Animator.StringToHash(boolName), value);
        }

        public void SetBool(int boolHash, bool value)
        {
            animator.SetBool(boolHash, value);
        }

        public void PlayAnimation(string animationName)
        {
            PlayAnimation(Animator.StringToHash(animationName));
        }


        public void PlayAnimation(int animationHash)
        {
            animator.Play(animationHash);
        }
    }
}