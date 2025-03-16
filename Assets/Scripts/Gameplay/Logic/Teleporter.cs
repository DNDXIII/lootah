using Gameplay.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Logic
{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        public void Teleport()
        {
            SceneLoader.Instance.LoadLevel(sceneToLoad);
        }
    }
}