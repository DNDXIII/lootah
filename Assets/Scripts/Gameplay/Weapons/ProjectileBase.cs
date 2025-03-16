using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Weapons
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public GameObject Owner { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }

        public UnityAction OnShoot;

        protected float damage = 10f;

        public void Shoot(GameObject controller, float projectileDamage)
        {
            Owner = controller;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;


            damage = projectileDamage;

            OnShoot?.Invoke();
        }
    }
}