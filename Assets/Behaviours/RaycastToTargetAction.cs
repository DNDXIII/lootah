using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Raycast to Target", story: "[Agent] has line of sight to [Target]", category: "Action", id: "92067872fdd9a609dd712c2e5d12d260")]
public partial class RaycastToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnStart()
    {
        if (Agent == null || Target == null)
        {
            Debug.LogError("Agent or Target is not set.");
            return Status.Failure;
        }

        return Physics.Raycast(
            Agent.Value.transform.position, Target.Value.transform.position + Vector3.up,
            out _, Mathf.Infinity, LayerMask.GetMask("Player"))
            ? Status.Success
            : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}