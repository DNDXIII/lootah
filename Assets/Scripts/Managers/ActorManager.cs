using Gameplay.Player;
using Shared;

namespace Managers
{
    public class ActorManager : Singleton<ActorManager>
    {
        private PlayerController _player;

        public PlayerController Player
        {
            get
            {
                if (!_player)
                {
                    _player = FindFirstObjectByType<PlayerController>();
                }

                return _player;
            }
        }
    }
}