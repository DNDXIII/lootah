using Gameplay.Shared;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Logic
{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        public void Teleport()
        {
            SceneLoaderManager.Instance.LoadLevel(sceneToLoad);
        }
    }
}