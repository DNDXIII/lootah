using UnityEngine;

namespace Managers
{
    public class SceneSetupManager : MonoBehaviour
    {
        private void Start()
        {
            PositionPlayerAtSpawnPoint();
            Destroy(gameObject);
        }

        private void PositionPlayerAtSpawnPoint()
        {
            Debug.Log("Positioning player at spawn point...");
            // Find the spawn point in the loaded scene
            var player = ActorManager.Instance.Player.transform;
            if (player != null)
            {
                player.position = transform.position;
                player.rotation = transform.rotation;
            }
            else
            {
                Debug.LogWarning(
                    "Spawn point or player not found. Ensure a GameObject named 'SpawnPoint' exists in the scene.");
            }
        }
    }
}