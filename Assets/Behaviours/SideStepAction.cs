using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SideStep", story: "[Agent] sidesteps", category: "Action",
    id: "990de9e941f22f8893b7cb6b42d859ab")]
public partial class SideStepAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    [SerializeReference] public BlackboardVariable<float> SidestepDistance = new(1.0f);
    [SerializeReference] public BlackboardVariable<float> SidestepDuration = new(1.0f);

    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private float _elapsed;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            return Status.Failure;
        }

        Vector3 direction = UnityEngine.Random.value < 0.5f
            ? Agent.Value.transform.right
            : -Agent.Value.transform.right;

        _startPosition = Agent.Value.transform.position;
        _targetPosition = _startPosition + direction * SidestepDistance;
        _elapsed = 0f;

        return Status.Running;
    }


    protected override Status OnUpdate()
    {
        if (Agent.Value == null)
        {
            return Status.Failure;
        }

        Agent.Value.transform.position = Vector3.Lerp(_startPosition, _targetPosition, _elapsed / SidestepDuration);
        _elapsed += Time.deltaTime;

        if (!(_elapsed >= SidestepDuration)) return Status.Running;

        Agent.Value.transform.position = _targetPosition;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}