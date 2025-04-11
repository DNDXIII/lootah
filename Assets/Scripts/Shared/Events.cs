using Gameplay.Enemy;
using Gameplay.Enemy2;
using Managers;
using UnityEngine;

namespace Shared
{
    // The Game Events used across the Game.
    // Anytime there is a need for a new event, it should be added here.

    public static class Events
    {
        public static GameOverEvent GameOverEvent = new();
        public static PlayerDeathEvent PlayerDeathEvent = new();
        public static PlayerDamageEvent PlayerDamageEvent = new();
        public static PlayerLevelUpEvent PlayerLevelUpEvent = new();
        public static EnemyKillEvent EnemyKillEvent = new();
        public static EnemyDamageEvent EnemyDamageEvent = new();
        public static PickupEvent PickupEvent = new();
        public static CriticalHitEvent CriticalHitEvent = new();
    }

    public class CriticalHitEvent : GameEvent
    {
    }


    public class GameOverEvent : GameEvent
    {
        public bool Win;
    }

    public class PlayerDeathEvent : GameEvent
    {
    }

    public class PlayerDamageEvent : GameEvent
    {
    }

    public class PlayerLevelUpEvent : GameEvent
    {
    }

    public class EnemyKillEvent : GameEvent
    {
        public Enemy Enemy;
    }

    public class PickupEvent : GameEvent
    {
        public GameObject Pickup;
    }

    public class EnemyDamageEvent : GameEvent
    {
        public GameObject Sender;
        public float DamageValue;
    }
}