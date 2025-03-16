using UnityEngine;

namespace Gameplay.Logic
{
    public class Door : MonoBehaviour
    {
        public void OpenDoor()
        {
            Destroy(gameObject);
        }
    }
}