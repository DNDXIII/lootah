using System;
using Gameplay.Enemy2;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TryAttack", story: "Try Attacking [Target] with [EnemyAttack]", category: "Action",
    id: "4c8ba9c945993cf53744c13eef70321f")]
public partial class TryAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<EnemyAttack> EnemyAttack;

    protected override Status OnStart()
    {
        return EnemyAttack.Value.TryAttack(Target.Value) ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return EnemyAttack.Value.IsAttacking ? Status.Running : Status.Success;
    }

   
}