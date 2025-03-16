using UnityEngine;

namespace Gameplay.Abilities.Grenades
{
    public class ThrowObjectAbility : MonoBehaviour
    {
        [SerializeField] private float throwForce = 20f;
        [SerializeField] private GameObject throwable;
        [SerializeField] private Transform throwStartPoint;

        private void Update()
        {
            // if we press the right mouse button
            // TODO: Move to input manager
            if (Input.GetMouseButtonDown(1))
            {
                ThrowGrenade();
            }
        }

        private void ThrowGrenade()
        {
            // Create a grenade instance
            GameObject grenade = Instantiate(throwable, throwStartPoint.position,
                throwStartPoint.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            if (!rb)
            {
                Debug.LogError("Rigidbody component not found on the throwable prefab");
            }

            // Add force to the grenade
            // throw direction is the camera's forward direction plus a bit upwards
            Vector3 throwDirection = throwStartPoint.forward + Vector3.up * 0.5f;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
    }
}