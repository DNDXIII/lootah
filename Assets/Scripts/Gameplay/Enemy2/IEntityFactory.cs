using UnityEngine;

namespace Gameplay.Enemy2
{
    public interface IEntityFactory<out T, in TConfig>
    {
        T Create(TConfig config, Vector3 position);
    }
}