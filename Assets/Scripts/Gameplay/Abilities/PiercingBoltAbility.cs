using System.Collections;
using System.Linq;
using Gameplay.Managers;
using Gameplay.Shared;
using Managers;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class PiercingBoltAbility : MonoBehaviour
    {
        [SerializeField] private int maxPierces = 3; // Max number of pierces
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private LayerMask hitLayers; // Layers the grenade can hit
        [SerializeField] private float damage = 20f; // Damage applied to objects

        [Header("Visual Effects")] [SerializeField]
        private LineRenderer lineRenderer; // Assign a LineRenderer in the Inspector

        private Camera _playerCamera;

        private void Start()
        {
            _playerCamera = Camera.main; // Cache the camera for efficiency
        }

        private void Update()
        {
            // if we press the right mouse button
            if (Input.GetMouseButtonDown(1))
            {
                FireBolt();
            }
        }

        private void FireBolt()
        {
            // Raycast from the camera
            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            var hits = Physics.RaycastAll(ray, maxDistance, hitLayers);

            // Sort by distance to ensure correct order
            hits = hits.OrderBy(h => h.distance).ToArray();

            Vector3 startPosition = _playerCamera.transform.position;
            Vector3 endPosition = startPosition + _playerCamera.transform.forward * maxDistance;
            var positions = new Vector3[maxPierces + 1];
            positions[0] = startPosition;

            int pierceCount = 0;
            foreach (RaycastHit hit in hits)
            {
                if (pierceCount >= maxPierces) break; // Stop after hitting maxPierces enemies

                if (hit.transform.TryGetComponent(out Damageable damageable))
                {
                    damageable.TakeDamage(damage, ActorManager.Instance.Player.gameObject, true);
                    pierceCount++;
                }

                // Store the hit position for LineRenderer
                positions[pierceCount] = hit.point;
            }

            // Ensure the final point is max range if no full pierce
            if (pierceCount < maxPierces)
            {
                positions[pierceCount + 1] = endPosition;
            }

            StartCoroutine(ShowBoltEffect(positions));
        }

        private IEnumerator ShowBoltEffect(Vector3[] positions)
        {
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(0.2f); // Bolt duration

            lineRenderer.enabled = false;
        }
    }
}