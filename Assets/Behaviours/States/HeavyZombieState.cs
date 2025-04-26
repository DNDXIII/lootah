using Unity.Behavior;

namespace Behaviours.States
{
    [BlackboardEnum]
    public enum HeavyZombieState
    {
        Idle,
        Chase,
        Attacking
    }
}